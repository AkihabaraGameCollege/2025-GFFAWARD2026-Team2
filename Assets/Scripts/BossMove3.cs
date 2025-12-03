using System.Collections;
using UnityEngine;

public class BossMove3 : MonoBehaviour
{
    private Rigidbody rb;
    [Header("ステータス")]
    [SerializeField]
    [Tooltip("移動速度")]
    private float moveSpeed = 3;
    [SerializeField]
    [Tooltip("回転速度")]
    private float rotateSpeed = 1;
    [SerializeField]
    [Tooltip("スタン時間")]
    private float defaultStunTime = 15;

    //コライダー参照用（中山が編集）
    [Header("Collider")]
    [SerializeField]
    [Tooltip("攻撃コライダー")]
    private Collider attackCollider;
    [SerializeField]
    [Tooltip("ドリブル用プレイヤー検知コライダー")]
    private PlayerCheckCollider playerCheckCollider;

    [Header("ボス固有の設定")]
    [SerializeField]
    private float rushWaitTime = 1;
    [SerializeField]
    private float startMotionTime = 3;

    private Player player;
    private StatusManagerBoss statusManager;
    [Header("その他")]
    [SerializeField]
    [Tooltip("Offset")]
    private Vector3[] wallCheckerPos = null;
    [SerializeField]
    [Tooltip("Distance")]
    private float wallCheckerDistance = 0;


    [SerializeField]
    [Tooltip("地面のレイヤー")]
    private LayerMask groundLayer;

    // Animator参照用（中山が編集）
    [SerializeField]
    private Animator animator;
    
    static readonly int defeatId = Animator.StringToHash("defeat");//死亡モーション用パラメーターID（中山が編集）

    private bool isPlayerCheckColliderEntered = false;
    private float stunTimer = 0;
    private IEnumerator mainMotionRoutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        statusManager = GetComponent<StatusManagerBoss>();

        // find with tagってやっていいのかな
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        statusManager.OnDeath += Die;
        statusManager.OnStunTaken += TakeStun;
        statusManager.isInvincible = true;
        playerCheckCollider.Enter += OnPlayerCheckColliderEnter;

        attackCollider.enabled = false;//攻撃判定無効化（中山が編集）
        playerCheckCollider.Hide();
        StageScene.Instance.HideWeakText();

        // 行動のコルーチンを起動
        StartCoroutine(StartMotion());
    }

    public void Die()
    {
        animator.SetTrigger(defeatId);//死亡モーション再生（中山が編集）
        AudioPlayer.instance.PlaySE(12); 
        StartCoroutine(DeathTimer());
    }
    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(2);
        AudioPlayer.instance.PlaySE(13); // BossDestroyを再生（富里が編集）
        StageScene.Instance.StageClear();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (statusManager != null)
        {
            playerCheckCollider.Enter -= OnPlayerCheckColliderEnter;
            statusManager.OnStunTaken -= TakeStun;
        }
    }

    IEnumerator MainLoop()
    {

        // 基本のループ
        while (true)
        {
            mainMotionRoutine = MainMotion();
            yield return mainMotionRoutine;

            yield return StartCoroutine(Stun(defaultStunTime));
        }
    }

    IEnumerator StartMotion()
    {
        yield return new WaitForSeconds(startMotionTime);
        StartCoroutine(MainLoop());
    }

    IEnumerator MainMotion()
    {
        // HP8以下なら7回、でなければ5回繰り返す
        for (int i = 0; i < ((statusManager.health <= 8) ? 7 : 5); i++)
        {
            rushWaitTime = 1;
            yield return StartCoroutine(Aim());
            if (isPlayerCheckColliderEntered)
            {
                // Drift
                yield return StartCoroutine(Drift());
                // 突進回数にはカウントしない
                i--;
            }
            else
            {
                yield return StartCoroutine(Rush());
            }
        }
    }

    IEnumerator Aim()
    {
        float timer = 0;
        isPlayerCheckColliderEntered = false;
        playerCheckCollider.Show();
        while (timer <= rushWaitTime/* && !isPlayerCheckColliderEntered*/)
        {
            // ここでプレイヤーの方を向いてる
            // 移動方向を取得
            Vector3 relativePos = player.transform.position - transform.position;
            // Yをなくす
            relativePos.y = 0;
            // 方向を、回転情報に変換
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            // 現在の回転情報と、ターゲット方向の回転情報を補完する
            rb.rotation = Quaternion.Slerp(rb.rotation, rotation, rotateSpeed * Time.fixedDeltaTime);

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        playerCheckCollider.Hide();
    }

    public void OnPlayerCheckColliderEnter()
    {
        isPlayerCheckColliderEntered = true;
    }

    IEnumerator Drift()
    {
        AudioPlayer.instance.PlaySE(9); // BossDriftを再生（中山が編集）
        attackCollider.enabled = true;
        float rotatedDegree = 0;
        Quaternion startRot = rb.rotation;
        while (rotatedDegree <= 360)
        {
            float delta = 180f * Time.fixedDeltaTime;
            rotatedDegree += delta;

            Quaternion rotation = Quaternion.Euler( 0f,rotatedDegree,0f );

            rb.MoveRotation(startRot * rotation);

            yield return new WaitForFixedUpdate();
        }
        attackCollider.enabled = false;
    }

    IEnumerator Rush()
    {
        AudioPlayer.instance.PlaySE(11,true); // BossRushを再生（中山が編集）
        float timer = 0;
        Vector3 rushDirection = transform.forward;
        bool isCasted = false;
        attackCollider.enabled = true;
        while (timer <= 2 && !isCasted)
        {
            // ここで力を加える
            rb.linearVelocity = rushDirection * moveSpeed;
            // 壁にぶつかったらbreakさせよう
            // すべてのoffsetで繰り返す
            for (int i = 0; i < wallCheckerPos.Length; i++)
            {
                Vector3 offset = transform.forward * wallCheckerPos[i].x + transform.right * wallCheckerPos[i].z;
                offset.y = wallCheckerPos[i].y;

                isCasted = Physics.Raycast(transform.position + (offset), rushDirection, wallCheckerDistance, groundLayer);
                // 一個でもtrueがあったらbreakして
                if (isCasted) break;
            }

            if (isCasted)
            {
                // trueならwaittimeを短くする上にwhileを抜ける
                rushWaitTime = 0.5f;
                Debug.Log("CAST");
            }
            else
            {
                rushWaitTime = 1;
            }
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        attackCollider.enabled = false;
        AudioPlayer.instance.StopLoopSE();
    }

    IEnumerator Stun(float stunTime)
    {
        //Weak出現
        statusManager.isInvincible = false;
        stunTimer = stunTime;

        while (stunTimer >= 0)
        {
            stunTimer -= Time.deltaTime;
            yield return null;
        }
        //weak消滅
        statusManager.isInvincible = true;
    }

    private void TakeStun()
    {
        if (statusManager.isInvincible)
        {
            // 現在のコルーチンを止めてひるむ
            StopAllCoroutines();
            StartCoroutine(OnStunTaken());
        }
        else
        {
            // ひるむ時間を３秒くらいのばす
            stunTimer += 3;
        }
    }

    IEnumerator OnStunTaken()
    {
        playerCheckCollider.Hide();
        attackCollider.enabled = false;
        rb.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        yield return StartCoroutine(Stun(defaultStunTime));
        StartCoroutine(MainLoop());
    }


    private void OnDrawGizmos()
    {
        Vector3 rushDirection = transform.forward;
        for (int i = 0; i < wallCheckerPos.Length; i++)
        {
            Vector3 offset = transform.forward * wallCheckerPos[i].x + transform.right * wallCheckerPos[i].z;
            offset.y = wallCheckerPos[i].y;

            Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.position + offset, transform.position + offset + rushDirection * wallCheckerDistance);
        }
    }
}