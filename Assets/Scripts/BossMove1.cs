using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ボスの移動・攻撃処理クラス
/// </summary>
public class BossMove1 : MonoBehaviour
{
    // 移動速度設定
    [SerializeField]
    private float moveP = 3;
    // 回転速度設定
    [SerializeField]
    private float speedNumber = 11.1f;

    // ボス行動時間設定
    [SerializeField]
    private float bossLittleWaitTime = 0.5f;
    [SerializeField]
    private float bossAttackTime = 1.5f;
    [SerializeField]
    private float bossWeakBeforeTime = 6;
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
    // ジャンプ攻撃を仕掛ける距離設定
    [SerializeField]
    private float distanceNumber = 8.0f;
    // ハンマー攻撃を仕掛ける時間設定
    [SerializeField]
    private float hammerAttackTime = 30.0f;
    // ボスがやられる時間設定
    [SerializeField]
    private float bossDieTime = 3.0f;
    // ボス開始待機時間設定
    [SerializeField]
    private float bossStartTime = 3.0f;
    // ハンマー攻撃時間初期値設定
    [SerializeField]
    private float hammerAttackTimeDefault = 30.0f;
    [SerializeField]
    [Tooltip("ボスのアニメーションしてからコライダー出るまでの時間")]
    private float meleeAttackAnimTime = 1.5f;
    [SerializeField]
    [Tooltip("ボス足上げる時間")]
    private float stumpWaitTime = 1;
    [SerializeField]
    [Tooltip("足下げアニメーションの後の攻撃までの時間")]
    private float stumpColliderArriveCooldown = 1;
    [SerializeField]
    [Tooltip("ボス死亡コライダー出現までの時間")]
    private float deathColliderTime;

    // 踏みつけ攻撃エフェクト再生までの待機時間
    [SerializeField]
    private float particleWaitTime = 0.5f;

    // 踏みつけ攻撃用コライダー
    [SerializeField]
    [Tooltip("踏みつけ攻撃用コライダー")]
    private Collider stumpCollider;
    // 攻撃判定
    [SerializeField]
    private Collider attackCollider;
    // ボスの体に当たった時の判定
    [SerializeField]
    private Collider bodyAttackCollider;
    // 弱点コライダーの参照
    [SerializeField]
    [Tooltip("弱点のコライダー")]
    private Collider weakCollider;
    // プレイヤーとのCollisionCollider参照用
    [SerializeField]
    private Collider collider2Player = null;

    // ダメージエフェクトの参照
    [SerializeField]
    [Tooltip("ダメージ時のエフェクト")]
    private GameObject damageEffect;
    // ターゲットエフェクトの参照
    [SerializeField]
    [Tooltip("ヘイローエフェクト")]
    private GameObject haloEffect;

    // アニメーターの参照
    [SerializeField]
    Animator animator;

    // ボスモデルについてるScriptの参照
    [SerializeField]
    [Tooltip("モデルについてるScript")]
    private ActionSounds modelScript;

    // パーティクルシステムの構造体
    [Serializable]
    private struct ParticleSystems
    {
        public ParticleSystem[] stump;// 踏みつけ攻撃エフェクト配列
        public ParticleSystem[] bigStump;// ダブルスレッジハンマー攻撃エフェクト配列
    }

    // 攻撃エフェクト参照
    [SerializeField]
    private ParticleSystems particles;

    private GameObject targetObject;// プレイヤーオブジェクト参照用
    new private Rigidbody rigidbody;// Rigidbodyコンポーネント参照用
    private float stunTimer = 0;// スタンタイマー

    // スクリプト参照用変数
    private Player player;// プレイヤースクリプト参照用
    private StatusManagerBoss statusManager;// ステータスマネージャーボス参照用

    // アニメーションID登録
    static readonly int isWalkingID = Animator.StringToHash("isWalking");
    static readonly int attackID = Animator.StringToHash("attack");
    static readonly int immediateryWeakID = Animator.StringToHash("ImmediatelyWeak");// スタン食らったとき用トランジション
    static readonly int wakeUpID = Animator.StringToHash("wakeUp");
    static readonly int dieID = Animator.StringToHash("die");
    static readonly int landingID = Animator.StringToHash("landing");// ジャンプ攻撃着地用
    static readonly int stumpID = Animator.StringToHash("Stump");

    // 判定用フラグ
    private bool isMoving = false;// 移動中かどうか判定
    private bool isTurning = false;// 攻撃中かどうか判定
    private bool isJumping = false;// 攻撃中かどうか判定
    private bool isWalking = false;// 歩行SE再生判定用
    private bool isAppeardWeak = false;// 弱点が露出したかどうか
    private bool isStunning = false;// スタン中かどうか判定

    /// <summary>
    /// 初期化処理を行う関数
    /// </summary>
    void Awake()
    {
        // スクリプト参照用変数初期化
        statusManager = GetComponent<StatusManagerBoss>();// ステータスマネージャーボス参照用
        rigidbody = GetComponent<Rigidbody>();// Rigidbodyコンポーネント参照用
        targetObject = GameObject.FindWithTag("Player");// プレイヤーオブジェクト参照用
        player = targetObject.GetComponent<Player>();// プレイヤースクリプト参照用

        // イベント登録
        statusManager.OnDeath += Die; // 死亡時実行の関数をいれとく
        statusManager.OnStunTaken += TakeStun; //スタン食らったとき
        statusManager.OnDamageTaken += TakeDamage;// ダメージ食らったとき
        modelScript.PlayWalkSE += PlayWalkSE;// 歩行SE再生関数登録

        // フラグ初期化
        statusManager.isInvincible = false;// 無敵解除
        isWalking = true;// 歩行SE再生判定用
        isTurning = false;// 方向可能
        isMoving = false;// 移動停止
        isJumping = false;// 攻撃停止
        attackCollider.enabled = false;// 攻撃判定無効化
        bodyAttackCollider.enabled = true;// ボス本体判定有効化
        stumpCollider.enabled = false;// 踏みつけ攻撃用コライダー無効化
        collider2Player.enabled = false; // プレイヤーとのCollisionColliderを無効化

        // すべての攻撃パーティクル停止
        foreach (ParticleSystem stump in particles.stump)
        {
            stump.Stop();
        }
        foreach (ParticleSystem bigStump in particles.bigStump)
        {
            bigStump.Stop();
        }

        StopBoss();
    }

    /// <summary>
    /// オブジェクト破棄時の処理を行う関数
    /// </summary>
    private void OnDestroy()
    {
        // イベント登録解除
        if (statusManager != null)
        {
            statusManager.OnDeath -= Die;// 死亡時実行の関数を消す
            statusManager.OnStunTaken -= TakeStun;// スタン食らったときの関数を消す
            statusManager.OnDamageTaken -= TakeDamage;// ダメージ食らったときの関数を消す
            modelScript.PlayWalkSE -= PlayWalkSE;// 歩行SE再生関数解除
        }
    }

    /// <summary>
    /// スタート時の処理を行う関数
    /// </summary>
    void Start()
    {
        StartCoroutine(OnMove());// 初動行動開始
    }

    // 初動行動
    IEnumerator OnMove()
    {
        yield return new WaitForSeconds(bossStartTime);// ボス開始時間待機

        // フラグ変更
        isTurning = true;// 回転可能
        isMoving = true;// 移動開始

        hammerAttackTime = hammerAttackTimeDefault;// ハンマー攻撃時間リセット
    }

    /// <summary>
    /// ボスの行動処理を行う関数
    /// </summary>
    void FixedUpdate()
    {
        // もし弱点出現中であれば
        if (isAppeardWeak)
        {
            return;// 弱点出現中は処理終了
        }

        float distance = Vector3.Distance(targetObject.transform.position, this.transform.position);// プレイヤーの近くにいたらジャンプ攻撃を仕掛ける処理

        // もしハンマー攻撃時間が来ていて、ジャンプ攻撃中でなければ
        if (hammerAttackTime <= 0 && !isJumping)
        {
            HammerAttack();// ハンマー攻撃処理
            hammerAttackTime = hammerAttackTimeDefault;// ハンマー攻撃時間リセット
        }
        // そうでなければ
        else
        {
            hammerAttackTime -= Time.fixedDeltaTime;// ハンマー攻撃時間カウントダウン
        }

        // もし回転可能であれば
        if (isTurning)
        {
            Turn();// 回転処理

            // もし移動中であれば
            if (isMoving)
            {
                MoveBoss();

                // もしプレイヤーが近くにいたら
                if (distance <= distanceNumber)
                {
                    StumpAttack();
                }
            }
        }
    }

    /// <summary>
    /// 回転処理を行う関数
    /// </summary>
    private void Turn()
    {
        // 回転処理
        float speed = speedNumber;// 補完スピードを決める
        Vector3 relativePos = targetObject.transform.position - transform.position;// ターゲット方向のベクトルを取得

        relativePos.y = 0;// X軸の回転は禁止する

        // 方向を向く処理
        Quaternion rotation = Quaternion.LookRotation(relativePos);// 方向を、回転情報に変換
        transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);// 現在の回転情報と、ターゲット方向の回転情報を補完する
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
            isWalking = false;// 歩行SE再生判定用（中山が編集）
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
        AudioPlayer.instance.PlaySE(7);// ジャンプ攻撃SE再生（中山が編集）
        stumpCollider.enabled = true;
        yield return new WaitForSeconds(particleWaitTime);

        // 踏みつけ攻撃エフェクト再生
        foreach (ParticleSystem stump in particles.stump)
        {
            stump.Play();
        }

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

        isTurning = false;// 攻撃開始（中山が編集）
        animator.SetTrigger(attackID);// ジャンプアニメーション開始（中山が編集）
        AudioPlayer.instance.PlaySE(2);// 攻撃SE再生（中山が編集）
        yield return new WaitForSeconds(meleeAttackAnimTime);
        attackCollider.enabled = true;// 攻撃判定有効化（中山が編集）

        // ダブルスレッジハンマー攻撃エフェクト再生
        foreach (ParticleSystem bigStump in particles.bigStump)
        {
            bigStump.Play();
        }

        yield return new WaitForSeconds(bossAttackTime);// 攻撃する時間（中山が編集）
        attackCollider.enabled = false;// 攻撃判定無効化（中山が編集）
        yield return new WaitForSeconds(bossLittleWaitTime);// 少し待機（中山が編集）

        StartCoroutine(OnWeak(bossWeakTime));
    }

    private IEnumerator OnWeak(float stun)
    {
        stunTimer = stun;
        isStunning = true;
        
        Weak();// 弱点出現（中山が編集）
        yield return new WaitForSeconds(bossWeakBeforeTime);// 弱点タイム（中山が編集）
        bodyAttackCollider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        collider2Player.enabled = true; // プレイヤーとのCollisionColliderを有効化 (富里が編集)

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
        isStunning = false;
        //rigidbody.isKinematic = false;
        hammerAttackTime = hammerAttackTimeDefault;// ハンマー攻撃時間リセット（中山が編集）
    }


    // 弱点タイム（中山が編集）
    private void Weak()
    {
        isAppeardWeak = true;
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
        if (haloEffect != null)
        {
            haloEffect.SetActive(false);
        }
    }

    IEnumerator OnDeath()
    {
        animator.SetTrigger(dieID);// 死亡アニメーション再生（中山が編集）
        AudioPlayer.instance.PlaySE(4);
        attackCollider.enabled = false;
        bodyAttackCollider.enabled = false;
        stumpCollider.enabled = false;
        yield return new WaitForSeconds(deathColliderTime);

        yield return new WaitForSeconds(bossDieTime - deathColliderTime);// 少し待機（中山が編集）
        StageScene.Instance.StageClear();// ステージクリア処理（中山が編集）
        Destroy(gameObject);// ボスオブジェクトを破壊（中山が編集）
    }


    private void TakeDamage()
    {
        // エフェクトをインスタンス化
        GameObject effect = Instantiate(damageEffect);

        effect.transform.position = weakCollider.transform.position;// 弱点コライダーの位置にエフェクトを出す（中山が編集）

        Destroy(effect, 5);// エフェクトを5秒後に破壊（中山が編集）
    }

    public void PlayWalkSE()
    {
        if (!isWalking)
        {
            AudioPlayer.instance.PlaySE(6);
            AudioPlayer.instance.PlaySE(5);
        }
    }

    private void TakeStun()
    {
        if (!isStunning)
        {
            // これのために、アニメーションをanystate→倒れるにしとかないとダメかも

            StopAllCoroutines();

            animator.SetTrigger(immediateryWeakID); // これ専用の倒れるトランジション
            StartCoroutine(OnWeak(player.StunSkillTime));
        }
        else
        {
            // ひるむ時間を5秒にする
            stunTimer = player.StunSkillTime;
        }
    }
}