using System.Collections;
using UnityEngine;

// ボス1の移動・攻撃パターン制御クラス（中山が編集）
public class BossMove1 : MonoBehaviour
{
    // 移動速度設定（中山が編集）
    [SerializeField]
    private float moveP = 3;
    // 回転速度設定（中山が編集）
    [SerializeField]
    private float speedNumber = 11.1f;
    // ボス行動時間設定（中山が編集）
    [SerializeField]
    private float bossLittleWaitTime = 0.5f;
    [SerializeField]
    private float bossAttackTime = 1.5f;
    [SerializeField]
    private float bossWeakBeforTime = 6;
    [SerializeField]
    private float bossWeakTime = 12f;
    [SerializeField]
    private float bossWakeUpTime = 5f;
    [SerializeField]
    private float bossWaitTime = 3f;
    [SerializeField]
    private float stumpAttackTime = 0.5f;
    [SerializeField]
    private float standTime = 1.0f;
    // ジャンプ攻撃を仕掛ける距離設定（中山が編集）
    [SerializeField]
    private float distanceNumber = 8.0f;
    // ハンマー攻撃を仕掛ける時間設定（中山が編集）
    [SerializeField]
    private float hammerAttackTime = 30.0f;
    // ボスがやられる時間設定（中山が編集）
    [SerializeField]
    private float bossDieTime = 3.0f;
    // ボス開始待機時間設定（中山が編集）
    [SerializeField]
    private float bossStartTime = 3.0f;
    // ハンマー攻撃時間初期値設定（中山が編集）
    [SerializeField]
    private float hammerAttackTimeDefault = 30.0f;
    // 歩行SE再生間隔時間設定（中山が編集）
    [SerializeField]
    private float moveSoundMTime = 1f;

    [SerializeField]
    [Tooltip("ボス足上げる時間")]
    private float stumpWaitTime = 1;
    [SerializeField]
    [Tooltip("足下げアニメーションの後の攻撃までの時間")]
    private float stumpColliderArriveCooldown = 1;

    [SerializeField]
    [Tooltip("スタンプ攻撃用コライダー")]
    private Collider stumpCollider;

    // 攻撃判定（中山が編集）
    [SerializeField]
    private Collider attackCollider;
    // ボスの体に当たった時の判定（中山が編集）
    [SerializeField]
    private Collider bodyAttackCollider;

    [SerializeField]
    [Tooltip("弱点のコライダー")]
    private Collider weakCollider;

    [SerializeField]
    [Tooltip("ダメージ時のエフェクト")]
    private GameObject damageEffect;

    private GameObject targetObject;

    // プレイヤーとのCollisionCollider参照用 (富里が編集)
    [SerializeField]
    private Collider collider2Player = null;

    new private Rigidbody rigidbody;// Rigidbodyコンポーネント参照用（中山が編集）

    [SerializeField]
    Animator animator;// アニメーター（中山が編集）

    // アニメーションID登録（中山が編集）
    static readonly int isWalkingID = Animator.StringToHash("isWalking");
    static readonly int attackID = Animator.StringToHash("attack");
    static readonly int immediateryWeakID = Animator.StringToHash("ImmediatelyWeak");
    static readonly int wakeUpID = Animator.StringToHash("wakeUp");
    static readonly int dieID = Animator.StringToHash("die");
    static readonly int landingID = Animator.StringToHash("landing");
    static readonly int stumpID = Animator.StringToHash("Stump");

    private bool isMoving = false;// 移動中かどうか判定（中山が編集）
    private bool isTurning = false;// 攻撃中かどうか判定（中山が編集）
    private bool isJumping = false;// 攻撃中かどうか判定（中山が編集）
    private bool isWalking = false;// 歩行SE再生判定用（中山が編集）
    private bool isAppeardWeak = false; // 弱点が露出したかどうか
    private float stunTimer = 0;

    private StatusManagerBoss statusManager;

    // 初期設定・登録等（中山が編集）
    void Awake()
    {
        statusManager = GetComponent<StatusManagerBoss>();
        rigidbody = GetComponent<Rigidbody>();
        targetObject = GameObject.FindWithTag("Player");

        statusManager.OnDeath += Die; // 死亡時実行の関数をいれとく 富里
        statusManager.OnStunTaken += TakeStun; //スタン食らったとき
        statusManager.OnDamageTaken += TakeDamage;

        statusManager.isInvincible = false;
        isWalking = true;// 歩行SE再生判定用（中山が編集）
        isTurning = false;// 方向可能（中山が編集）
        isMoving = false;// 移動停止（中山が編集）
        isJumping = false;// 攻撃停止（中山が編集）
        attackCollider.enabled = false;// 攻撃判定無効化（中山が編集）
        bodyAttackCollider.enabled = true;// ボス本体判定有効化（中山が編集）
        stumpCollider.enabled = false;
        collider2Player.enabled = false; // プレイヤーとのCollisionColliderを無効化 (富里が編集)

        StopBoss();// ボス停止処理（中山が編集）
    }

    private void OnDestroy()
    {
        if (statusManager != null)
        {
            statusManager.OnDeath -= Die;
            statusManager.OnStunTaken -= TakeStun;
            statusManager.OnDamageTaken -= TakeDamage;
        }
    }

    // ボスの開始処理（中山が編集）
    void Start()
    {
        StartCoroutine(OnMove());// 初動行動開始（中山が編集）
    }

    // 初動行動（中山が編集）
    IEnumerator OnMove()
    {

        yield return new WaitForSeconds(bossStartTime);// 待機（中山が編集）

        isTurning = true;// 方向可能（中山が編集）
        isMoving = true;// 移動開始（中山が編集）

        hammerAttackTime = hammerAttackTimeDefault;// ハンマー攻撃時間リセット（中山が編集）
    }

    // ボスの毎フレーム更新処理
    void FixedUpdate()
    {

        if (isAppeardWeak)
        {
            return;
        }

        float distance = Vector3.Distance(targetObject.transform.position, this.transform.position);// プレイヤーの近くにいたらジャンプ攻撃を仕掛ける処理（中山が編集）

        // 30秒たったらハンマー攻撃の関数を呼び出す（中山が編集）
        if (hammerAttackTime <= 0 && !isJumping)
        {
            HammerAttack();// ハンマー攻撃処理（中山が編集）
            hammerAttackTime = hammerAttackTimeDefault;// ハンマー攻撃時間リセット（中山が編集）
        }
        else
        {
            hammerAttackTime -= Time.fixedDeltaTime;// ハンマー攻撃時間カウントダウン（中山が編集）
        }

        // 弱点出現していないときの処理（中山が編集）
        if (isTurning)
        {
            Turn();// 回転処理（中山が編集）

            // 移動処理（中山が編集）
            if (isMoving)
            {
                MoveBoss();// ボス移動処理（中山が編集）

                // プレイヤーとボスの距離を取得（中山が編集）
                if (distance <= distanceNumber)
                {
                    StumpAttack();// ジャンプ攻撃処理（中山が編集）
                }
            }
            // 移動停止処理（中山が編集）
            else if (!isMoving)
            {
                // テストでなくしてみてるけど大丈夫そう
                //StopBoss();// ボス停止処理（中山が編集）
            }
        }

        
    }

    // プレイヤーを追尾する（中山が編集）
    private void Turn()
    {
        float speed = speedNumber;// 補完スピードを決める（中山が編集）
        Vector3 relativePos = targetObject.transform.position - transform.position;// ターゲット方向のベクトルを取得（中山が編集）

        relativePos.y = 0;// X軸の回転は禁止する（中山が編集）

        Quaternion rotation = Quaternion.LookRotation(relativePos);// 方向を、回転情報に変換（中山が編集）
        transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);// 現在の回転情報と、ターゲット方向の回転情報を補完する（中山が編集）
    }

    // ボスが移動する（中山が編集）
    private void MoveBoss()
    {
        Vector3 forward = transform.forward * moveP;// 前方向に移動ベクトル設定（中山が編集）
        rigidbody.linearVelocity = new Vector3(forward.x, rigidbody.linearVelocity.y, forward.z);// 前方向に移動（中山が編集）
        animator.SetFloat(isWalkingID, rigidbody.linearVelocity.magnitude);// 歩行アニメーション開始（中山が編集）

        // 歩行SE再生処理開始（中山が編集）
        if (isWalking)
        {
            StartCoroutine(OnMoveSound());// 歩行SE再生処理開始（中山が編集）
            isWalking = false;// 歩行SE再生判定用（中山が編集）
        }
    }

    // 歩行SE再生処理（中山が編集）
    IEnumerator OnMoveSound()
    {
        // 歩行SE再生ループ（中山が編集）
        while (true && isMoving)
        {
            AudioPlayer.instance.PlaySE(5, false);// 歩行足踏みSE再生（中山が編集）
            AudioPlayer.instance.PlaySE(6);// 歩行動作SE再生（中山が編集）
            yield return new WaitForSeconds(moveSoundMTime);// 少し待機（中山が編集）
        }
    }

    // ボスが止まる（中山が編集）
    private void StopBoss()
    {
        rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);// 移動停止（中山が編集）
        animator.SetFloat(isWalkingID, rigidbody.linearVelocity.magnitude);// 歩行アニメーション停止（中山が編集）
    }

    // ジャンプ攻撃処理（中山が編集）
    private void StumpAttack()
    {
        StartCoroutine(OnStump());// ジャンプ攻撃処理開始（中山が編集）
    }

    // ジャンプ攻撃処理（中山が編集）
    IEnumerator OnStump()
    {
        // ジャンプ開始
        isMoving = false;
        isWalking = true;
        isJumping = true;

        isTurning = false;// 回転停止（中山が編集）

        

        animator.SetTrigger(stumpID);// ジャンプ開始
        yield return new WaitForSeconds(stumpWaitTime);
        animator.SetTrigger(landingID);
        yield return new WaitForSeconds(stumpColliderArriveCooldown);
        AudioPlayer.instance.PlaySE(7, false);// ジャンプ攻撃SE再生（中山が編集）
        stumpCollider.enabled = true;
        yield return new WaitForSeconds(stumpAttackTime);
        stumpCollider.enabled = false;
        isTurning = true;// 回転可能（中山が編集）
        yield return new WaitForSeconds(standTime);// 少し待機（中山が編集）

        // ジャンプ終了
        isMoving = true;
        isJumping = false;
    }

    // ハンマー攻撃（中山が編集）
    private void HammerAttack()
    {
        StartCoroutine(OnHammerAttack());// ハンマー攻撃処理開始（中山が編集）
    }

    // ハンマー攻撃処理（中山が編集）
    IEnumerator OnHammerAttack()
    {
        isMoving = false;// 移動停止（中山が編集）
        isWalking = true;// 歩行SE再生判定用（中山が編集）
        AudioPlayer.instance.PlaySE(2, false);// 攻撃SE再生（中山が編集）
        yield return new WaitForSeconds(bossLittleWaitTime);// 少し待機（中山が編集）
        HammerAttackMove();// ハンマー攻撃（中山が編集）
        yield return new WaitForSeconds(bossAttackTime);// 攻撃する時間（中山が編集）
        attackCollider.enabled = false;// 攻撃判定無効化（中山が編集）
        yield return new WaitForSeconds(bossLittleWaitTime);// 少し待機（中山が編集）

        StartCoroutine(OnWeak());
    }

    private IEnumerator OnWeak()
    {
        stunTimer = bossWeakTime;
        Weak();// 弱点出現（中山が編集）
        yield return new WaitForSeconds(bossWeakBeforTime);// 弱点タイム（中山が編集）

        // 倒れる間のタイマー
        while (stunTimer >= 0)
        {
            stunTimer -= Time.deltaTime;
            yield return null;
        }

        WakeUp();// 起き上がり（中山が編集）
        yield return new WaitForSeconds(bossWakeUpTime);// 待機（中山が編集）
        bodyAttackCollider.enabled = true;// ボス本体判定有効化（中山が編集）
        yield return new WaitForSeconds(bossWaitTime);// 少し待機（中山が編集）

        isTurning = true;// 攻撃停止（中山が編集）
        isMoving = true;// 移動開始（中山が編集）
        isAppeardWeak = false;

        hammerAttackTime = hammerAttackTimeDefault;// ハンマー攻撃時間リセット（中山が編集）
    }

    // ハンマー攻撃処理（中山が編集）
    private void HammerAttackMove()
    {
        isTurning = false;// 攻撃開始（中山が編集）
        animator.SetTrigger(attackID);// ジャンプアニメーション開始（中山が編集）
        attackCollider.enabled = true;// 攻撃判定有効化（中山が編集）
    }

    // 弱点タイム（中山が編集）
    private void Weak()
    {
        collider2Player.enabled = true; // プレイヤーとのCollisionColliderを有効化 (富里が編集)
        isAppeardWeak = true;

        bodyAttackCollider.enabled = false;// ボス本体判定無効化（中山が編集）
    }
    // 起き上がり（中山が編集）
    private void WakeUp()
    {
        animator.SetTrigger(wakeUpID);// 起き上がりアニメーション再生（中山が編集）
        collider2Player.enabled = false; // プレイヤーとのCollisionColliderを無効化 (富里が編集)
    }

    private void Die()
    {
        StopAllCoroutines();
        StartCoroutine(OnDeath());
    }

    IEnumerator OnDeath()
    {
        animator.SetTrigger(dieID);// 死亡アニメーション再生（中山が編集）
        AudioPlayer.instance.PlaySE(4);
        attackCollider.enabled = false;
        bodyAttackCollider.enabled = false;

        yield return new WaitForSeconds(bossDieTime);// 少し待機（中山が編集）
        AudioPlayer.instance.StopLoopSE();// ボス撃破SE再生（中山が編集）
        StageScene.Instance.StageClear();// ステージクリア処理（中山が編集）
        Destroy(gameObject);// ボスオブジェクトを破壊（中山が編集）
    }


    private void TakeDamage()
    {
        // エフェクトをインスタンス化
        GameObject effect = Instantiate(damageEffect);

        effect.transform.position = weakCollider.transform.position;
        Destroy(effect, 5);// エフェクトを5秒後に破壊（中山が編集）
    }

    [ContextMenu("デバッグ用すぐスタン")]
    private void TakeStun()
    {
        if (statusManager.isInvincible)
        {
            // これのために、アニメーションをanystate→倒れるにしとかないとダメかも

            StopAllCoroutines();

            animator.SetTrigger(immediateryWeakID); // これ専用の倒れるトランジション
            StartCoroutine(OnWeak());
        }
        else
        {
            // ひるむ時間を３秒くらいのばす
            stunTimer += 3;
        }
    }
}