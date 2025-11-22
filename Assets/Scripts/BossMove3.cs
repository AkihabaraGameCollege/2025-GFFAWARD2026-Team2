using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossMove3 : MonoBehaviour
{
    private Rigidbody rb;//Rigidbodyコンポーネント参照用（中山が編集）
    [Header("ステータス")]
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
    [SerializeField]
    [Tooltip("ドリブル用プレイヤー検知コライダー")]
    private PlayerCheckCollider playerCheckCollider;

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



    private float rushWaitTime = 1;
    private bool isPlayerCheckColliderEntered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();//Rigidbodyコンポーネント取得（中山が編集）
        animator = GetComponent<Animator>();//Animatorコンポーネント取得（中山が編集）
        statusManager = GetComponent<StatusManagerBoss>();

        // find with tagってやっていいのかな
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        statusManager.OnDeath += Die;
        playerCheckCollider.Enter += OnPlayerCheckColliderEnter;

        weakCollider.enabled = false;//弱点判定無効化（中山が編集）
        attackCollider.enabled = false;//攻撃判定無効化（中山が編集）
        playerCheckCollider.Hide();
        StageScene.Instance.HideWeakText();

        // 行動のコルーチンを起動
        StartCoroutine(MainLoop());//行動パターン開始（中山が編集）
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
            playerCheckCollider.Enter -= OnPlayerCheckColliderEnter;
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
            if (isPlayerCheckColliderEntered)
            {
                // Drift
                yield return StartCoroutine(Drift());
                // 突進回数にはカウントしない
                i--;
                // TEST
                Debug.Log("DRIFT");
            }
            else
            {
                yield return StartCoroutine(Rush());
            }
        }
        //Weak出現
        weakCollider.enabled = true;
        yield return new WaitForSeconds(15);
        //weak消滅
        weakCollider.enabled = false;
    }

    IEnumerator Aim()
    {
        float timer = 0;
        isPlayerCheckColliderEntered = false;
        playerCheckCollider.Show();
        while (timer <= rushWaitTime && !isPlayerCheckColliderEntered)
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
        // 仮で1秒くらい待つ
        yield return new WaitForSeconds(1);
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