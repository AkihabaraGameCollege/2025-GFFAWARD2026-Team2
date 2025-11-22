using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossMove3 : MonoBehaviour
{
    private Rigidbody rb;//Rigidbodyコンポーネント参照用（中山が編集）
    [Header("ステータス")]
    [SerializeField]
    [Tooltip("ジャンプ力")]
    private float jumpForce = 10;
    [SerializeField]
    [Tooltip("移動速度")]
    private float moveSpeed = 3;
    [SerializeField]
    [Tooltip("回転速度")]
    private float rotateSpeed = 11.1f;

    //コライダー参照用（中山が編集）
    [Header("Collider")]
    [SerializeField]
    [Tooltip("攻撃コライダー")]
    private Collider attackCollider;
    [SerializeField]
    [Tooltip("弱点コライダー")]
    private Collider weakCollider;

    [Header("ボス固有の設定")]
    

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

    Animator animator;//アニメーター（中山が編集）

    //アニメーションID登録（中山が編集）
    static readonly int IsWalkingID = Animator.StringToHash("isWalking");
    static readonly int jumpID = Animator.StringToHash("jump");
    static readonly int landingID = Animator.StringToHash("landing");
    static readonly int weakID = Animator.StringToHash("weak");
    static readonly int grandID = Animator.StringToHash("grand");
    static readonly int dieID = Animator.StringToHash("die");



    // 何回ダメージ食らったかのカウンター
    private int damageCounter = 0;
    private float rushWaitTime = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();//Rigidbodyコンポーネント取得（中山が編集）
        animator = GetComponent<Animator>();//Animatorコンポーネント取得（中山が編集）
        statusManager = GetComponent<StatusManagerBoss>();

        // find with tagってやっていいのかな
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        statusManager.OnDamageTaken += TakeDamage;
        statusManager.OnDeath += Die;

        weakCollider.enabled = false;//弱点判定無効化（中山が編集）
        attackCollider.enabled = false;//攻撃判定無効化（中山が編集）
        StageScene.Instance.HideWeakText();

        // 行動のコルーチンを起動
        StartCoroutine(MainLoop());//行動パターン開始（中山が編集）
    }

    // StatusManagerBossから呼び出される
    public void TakeDamage()
    {
        
    }

    public void Die()
    {
        StageScene.Instance.StageClear();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (statusManager != null)
        {
            statusManager.OnDamageTaken -= TakeDamage;
        }
    }

    IEnumerator MainLoop()
    {
        // スタート時のモーション起動
        yield return StartCoroutine(StartMotion());

        // 基本のループ
        while (true)
        {
            yield return StartCoroutine(MainMotion());
        }
    }

    IEnumerator StartMotion()
    {
        yield return null;
    }

    IEnumerator MainMotion()
    {
        // HP8以下なら7回、でなければ5回繰り返す
        for (int i = 0; i < ((statusManager.health <= 8) ? 7 : 5); i++)
        {
            rushWaitTime = 1;
            yield return StartCoroutine(Aim());
            yield return StartCoroutine(Rush());
        }
        //Weak出現

        yield return new WaitForSeconds(15);
        //weak消滅

    }

    IEnumerator Aim()
    {
        float timer = 0;
        while (timer <= rushWaitTime)
        {
            // ここでプレイヤーの方を向いてる
            // 移動方向を取得
            Vector3 relativePos = player.transform.position - transform.position;
            // Yをなくす
            relativePos.y = 0;
            // 方向を、回転情報に変換
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            // 現在の回転情報と、ターゲット方向の回転情報を補完する
            rb.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed);

            // 敵を検知してドリフトをする

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Rush()
    {
        float timer = 0;
        Vector3 rushDirection = transform.forward;
        bool isCasted = false;
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
    }

    private void Walk()
    {
        // 移動方向を取得
        Vector3 moveDirection = (player.transform.position - transform.position).normalized;

        // Yをなくす
        moveDirection.y = 0;

        // 移動
        rb.linearVelocity = moveDirection * moveSpeed;

        // 方向転換
        // 補完スピードを決める
        // ターゲット方向のベクトルを取得
        Vector3 relativePos = player.gameObject.transform.position - transform.position;

        relativePos.y = 0; // X軸の回転は禁止する（中山が編集）

        // 方向を、回転情報に変換
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        // 現在の回転情報と、ターゲット方向の回転情報を補完する
        rb.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed);
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