using System.Collections;
using UnityEngine;

public class BossMove2 : MonoBehaviour
{
    new private Rigidbody rigidbody;//Rigidbodyコンポーネント参照用（中山が編集）
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
    [SerializeField]
    [Tooltip("スタン時間")]
    private float defaultStunTime = 15;

    [Header("参照")]
    [SerializeField]
    [Tooltip("着地攻撃コライダー")]
    private Collider attackCollider;
    [SerializeField]
    [Tooltip("弱点コライダー")]
    private Collider weakCollider;
    [SerializeField]
    [Tooltip("ダメージエフェクトのプレハブ")]
    private GameObject damageEffect;
    [SerializeField]
    [Tooltip("モデルについてるアニメーター")]
    private Animator animator;

    [SerializeField]
    [Tooltip("ヘイローエフェクト")]
    private GameObject haloEffect;
    [SerializeField]
    [Tooltip("モデルについてるScript")]
    private ModelScript modelScript;

    [Header("ボス固有の設定")]
    [SerializeField]
    [Tooltip("すたーともーしょん時間")]
    private float startMotionTime = 4;
    [SerializeField]
    [Tooltip("何回ダメージを食らったら雑魚を召喚するか")]
    private int damageCount2ZakoSummon = 5;
    [SerializeField]
    [Tooltip("一度に召喚する雑魚の数")]
    private int zakoSummonCount = 3;
    [SerializeField]
    [Tooltip("次の雑魚が召喚されるまでの待機時間\n(0だと一斉スポーン)")]
    private float zakoSummonWaitTime = 0.1f;
    [SerializeField]
    [Tooltip("雑魚のprefab")]
    private GameObject zakoPrefab;
    [SerializeField]
    [Tooltip("雑魚の拡散スピード最小値")]
    private float zakoMinSpreadSpeed;
    [SerializeField]
    [Tooltip("雑魚の拡散スピード最大値")]
    private float zakoMaxSpreadSpeed;
    [SerializeField]
    [Tooltip("雑魚の拡散時間")]
    private float zakoSpreadTime;
    [SerializeField]
    [Tooltip("雑魚の移動速度")]
    private float zakoMoveSpeed;
    [SerializeField]
    [Tooltip("雑魚のスポーン地点")]
    private Vector3 zakoSpawnOffset;
    [SerializeField]
    [Tooltip("雑魚を吸収可能な範囲")]
    private float zakoAbsorbRadius;
    [SerializeField]
    [Tooltip("歩行継続時間")]
    private float walkTime = 2;
    [SerializeField]
    [Tooltip("ジャンプ目標のY軸オフセット")]
    private float jumpTargetOffsetY = 10;
    [SerializeField]
    [Tooltip("ジャンプ後の静止までの待機時間")]
    private float jump2FreezeWaitTime = 1;
    [SerializeField]
    [Tooltip("空中での静止時間")]
    private float jumpFreezeTime = 1;
    [SerializeField]
    [Tooltip("着地時のスピード")]
    private float dropSpeed = 10;
    [SerializeField]
    [Tooltip("着地時コライダー出現継続時間")]
    private float dropAttackTime = 0.1f;
    [SerializeField]
    [Tooltip("立ち上がりにかかる時間")]
    private float standUpTime = 5;
    [SerializeField]
    [Tooltip("死亡アニメーション時間")]
    private float deathAnimTime = 3.5f;
    [SerializeField]
    [Tooltip("死亡SEを鳴らす回数")]
    private float deathScreamCount = 12;
    [SerializeField]
    [Tooltip("死亡SEを鳴らし続ける時間")]
    private float deathScreamTime = 2.5f;

    private Player player;
    private StatusManagerBoss statusManager;
    [Header("その他")]

    [SerializeField]
    [Tooltip("地面との着地判定線始点")]
    private Vector3 groundCheckStartPoint = new Vector3(0, 0.5f, 0);
    [SerializeField]
    [Tooltip("地面との着地判定線終点")]
    private Vector3 groundCheckEndPoint = new Vector3(0, -0.5f, 0);

    [SerializeField]
    [Tooltip("地面のレイヤー")]
    private LayerMask groundLayer;

    [SerializeField]
    [Tooltip("歩行時のSEを鳴らす間隔")]
    private float walkSECooldown;


    //アニメーションID登録
    static readonly int isWalkingID = Animator.StringToHash("IsWalking");
    static readonly int standUpID = Animator.StringToHash("Stand");
    static readonly int hipDropID = Animator.StringToHash("HipDrop");
    static readonly int dieID = Animator.StringToHash("Die");
    static readonly int immediatelyWeakID = Animator.StringToHash("ImmediatelyWeak");


    // 何回ダメージ食らったかのカウンター
    private int damageCounter = 0;
    private float stunTimer = 0;
    private bool isStunning = false;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();//Rigidbodyコンポーネント取得
        statusManager = GetComponent<StatusManagerBoss>();

        // find with tagってやっていいのかな
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        statusManager.OnDamageTaken += TakeDamage;
        statusManager.OnDeath += Die;
        statusManager.OnStunTaken += TakeStun;
        modelScript.PlayWalkSE += PlayWalkSE;

        statusManager.isInvincible = false;
        attackCollider.enabled = false;//攻撃判定無効化

        // 行動のコルーチンを起動
        StartCoroutine(StartMotion());
    }

    // StatusManagerBossから呼び出される
    public void TakeDamage()
    {
        damageCounter++;
        AudioPlayer.instance.PlaySE(1);
        if (damageCounter >= damageCount2ZakoSummon)
        {
            StartCoroutine(SummonZako(zakoSummonCount));
            damageCounter = 0;
        }
        // エフェクトをインスタンス化
        GameObject effect = Instantiate(damageEffect);

        effect.transform.position = weakCollider.transform.position;
        Destroy(effect, 5);// エフェクトを5秒後に破壊
    }

    public void Die()
    {
        StopAllCoroutines();
        StartCoroutine(OnDeath());
        haloEffect.SetActive(false);

    }

    IEnumerator OnDeath()
    {
        animator.SetTrigger(dieID);
        attackCollider.enabled = false;

        int counter = 0;
        while (counter < deathScreamCount)
        {
            counter++;
            yield return new WaitForSeconds(deathScreamTime/deathScreamCount);
            AudioPlayer.instance.PlaySE(1);
        }

        yield return new WaitForSeconds(deathAnimTime);
        StageScene.Instance.StageClear();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (statusManager != null)
        {
            statusManager.OnStunTaken -= TakeStun;
            statusManager.OnDamageTaken -= TakeDamage;
        }
    }

    IEnumerator MainLoop()
    {
        // 基本のループ
        while (true)
        {
            yield return StartCoroutine(MainMotion());
        }
    }

    IEnumerator StartMotion()
    {
        yield return new WaitForSeconds(startMotionTime);
        StartCoroutine(MainLoop());
    }

    IEnumerator MainMotion()
    {
        // 2秒間の間歩く
        float timer = 0;
        while (timer < walkTime)
        {
            timer += Time.fixedDeltaTime;
            
            Walk();
            yield return new WaitForFixedUpdate();
        }
        // 一連の処理
        yield return HipDrop();
        yield return Stun(defaultStunTime);
        yield return StandUp();
    }

    private void Walk()
    {
        // 移動方向を取得
        Vector3 moveDirection = (player.transform.position - transform.position).normalized;

        // Yをなくす
        moveDirection.y = 0;

        // 移動
        rigidbody.linearVelocity = moveDirection * moveSpeed;

        // 方向転換
        // 補完スピードを決める
        // ターゲット方向のベクトルを取得
        Vector3 relativePos = player.gameObject.transform.position - transform.position;

        relativePos.y = 0; // X軸の回転は禁止する

        // 方向を、回転情報に変換
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        // 現在の回転情報と、ターゲット方向の回転情報を補完する
        rigidbody.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.fixedDeltaTime);
        animator.SetFloat(isWalkingID, rigidbody.linearVelocity.magnitude);// 歩行アニメーション
    }

    IEnumerator HipDrop()
    {
        // 方向を定める
        Vector3 direction = ((player.transform.position + new Vector3(0f, jumpTargetOffsetY, 0f)) - transform.position).normalized;
        // スピードに代入
        rigidbody.linearVelocity = direction * jumpForce;
        // アニメーション
        animator.SetTrigger(hipDropID);
        // ちょっとまつ
        yield return new WaitForSeconds(jump2FreezeWaitTime);

        // フリーズ
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.useGravity = false;
        yield return new WaitForSeconds(jumpFreezeTime);

        // ドロップ
        rigidbody.useGravity = true;
        rigidbody.linearVelocity = Vector3.down * dropSpeed;

        // 地面に着地するまで待つ
        bool isGrounded = false;
        while (!isGrounded)
        {
            isGrounded = Physics.Linecast(transform.position + groundCheckStartPoint, transform.position + groundCheckEndPoint, groundLayer);
            yield return new WaitForFixedUpdate();
        }
        // 着地攻撃判定を出す
        attackCollider.enabled = true;
        // 攻撃時間待つ
        yield return new WaitForSeconds(dropAttackTime);
        // 判定消す
        attackCollider.enabled = false;
    }

    IEnumerator Stun(float weakTime)
    {
        stunTimer = weakTime;
        isStunning = true;

        while (stunTimer >= 0)
        {
            stunTimer -= Time.deltaTime;
            yield return null;
        }
        isStunning = false;
    }

    IEnumerator StandUp()
    {
        // どうするんだ？アニメーション？
        animator.SetTrigger(standUpID);
        // 待つ(アニメーションイベントでもいいかも)
        yield return new WaitForSeconds(standUpTime);
    }

   
    IEnumerator SummonZako(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 召喚
            GameObject go = Instantiate(zakoPrefab, transform.position + zakoSpawnOffset, Quaternion.identity);
            // 召喚したオブジェクトのscriptを持ってくる
            CottonMonster script = go.GetComponent<CottonMonster>();
            // Yはプラス、XZは完全ランダムな方向を取得
            Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            // 拡散スピードを決定
            float spd = Random.Range(zakoMinSpreadSpeed, zakoMaxSpreadSpeed);
            // 初期化
            script.Initialize(this, dir, spd,zakoSpreadTime, zakoMoveSpeed, zakoSpawnOffset,zakoAbsorbRadius);
            // 次までの待機
            yield return new WaitForSeconds(zakoSummonWaitTime);
        }
    }

    public void Heal()
    {
        if (statusManager.health % (statusManager.maxHealth / 3) != 0)
        {
            statusManager.health++;
            StageScene.Instance.BossBarUpdate(statusManager.health, statusManager.maxHealth);
            AudioPlayer.instance.PlaySE(14, 1);
        }
    }

    private void TakeStun()
    {
        if (!isStunning)
        {
            // ここで座り込むアニメーション再生が必要かも

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
        attackCollider.enabled = false;
        animator.SetTrigger(immediatelyWeakID);
        yield return StartCoroutine(Stun(defaultStunTime));
        yield return StandUp();
        StartCoroutine(MainLoop());
    }

    public void PlayWalkSE()
    {
        AudioPlayer.instance.PlaySE(0);
    }

    // 以下テスト
    // Inspector上でBossMove2のComponentの右上にある三点リーダーをクリックするとあるよ
    [ContextMenu("デバッグ用雑魚召喚ボタン")]
    private void TesutoZakoShoukan()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("プレイ中のみ実行可能です");
            return;
        }
        StartCoroutine(SummonZako(1));
    }
}