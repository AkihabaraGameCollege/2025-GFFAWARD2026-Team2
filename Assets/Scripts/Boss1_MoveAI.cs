using System;
using System.Collections;
using UnityEngine;

namespace QuickTheFury
{
    /// <summary>
    /// ボスの移動・攻撃処理クラス
    /// </summary>
    public class Boss1_MoveAI : MonoBehaviour
    {
        /// <summary>
        /// パーティクルシステムの構造体
        /// </summary>
        [Serializable]
        private struct ParticleSystems
        {
            /// <summary>
            /// 踏みつけ攻撃エフェクト配列を参照する変数
            /// </summary>
            public ParticleSystem[] Stump;
            /// <summary>
            /// ダブルスレッジハンマー攻撃エフェクト配列を参照する変数
            /// </summary>
            public ParticleSystem[] BigStump;
        }

        /// <summary>
        /// 攻撃エフェクトをまとめる構造体を参照する変数
        /// </summary>
        [SerializeField]
        private ParticleSystems _particles;

        /// <summary>
        /// 踏みつけ攻撃用コライダーを参照する変数
        /// </summary>
        public Collider StumpCollider;
        /// <summary>
        /// 攻撃判定を参照する変数
        /// </summary>
        public Collider AttackCollider;
        /// <summary>
        /// ボスの体に当たった時の判定を参照する変数
        /// </summary>
        public Collider BodyAttackCollider;
        /// <summary>
        /// 弱点コライダーを参照する変数
        /// </summary>
        public Collider WeakCollider;
        /// <summary>
        /// プレイヤーとのCollisionColliderを参照する変数
        /// </summary>
        public Collider Collider2_Player;

        /// <summary>
        /// プレイヤーオブジェクトを参照する変数
        /// </summary>
        private GameObject _targetObject;
        /// <summary>
        /// ダメージエフェクトを参照する変数
        /// </summary>
        public GameObject DamageEffect;
        /// <summary>
        /// ターゲットエフェクトを参照する変数
        /// </summary>
        public GameObject HaloEffect;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        public Animator Animator;

        /// <summary>
        /// プレイヤースクリプトを参照する変数
        /// </summary>
        private PlayerController player;
        /// <summary>
        /// ステータスマネージャーボスを参照する変数
        /// </summary>
        private StatusManagerBoss statusManager;
        /// <summary>
        /// ボスモデルについてるScriptを参照する変数
        /// </summary>
        public ActionSoundsPlayer ModelScript;

        /// <summary>
        /// Rigidbodyコンポーネントを参照する変数
        /// </summary>
        new private Rigidbody rigidbody;

        /// <summary>
        /// スタンのタイマー用の値を参照する変数
        /// </summary>
        private float _stunTimer = 0;
        /// <summary>
        /// 移動速度を参照する変数
        /// </summary>
        public float MoveP = 3;
        /// <summary>
        /// 回転速度を参照する変数
        /// </summary>
        public float RotateSpeed = 11.1f;
        /// <summary>
        /// ボスの少しだけ待機する時間を参照する変数
        /// </summary>
        public float BossLittleWaitTime = 0.5f;
        /// <summary>
        /// ボスの一瞬だけ待機する時間を参照する変数
        /// </summary>
        public float BossVeryLittleWaitTime = 0.1f;
        /// <summary>
        /// ボスの攻撃する時間を参照する変数
        /// </summary>
        public float BossAttackTime = 1.5f;
        /// <summary>
        /// ボスの弱点が出現する前の時間を参照する変数
        /// </summary>
        public float BossWeakBeforeTime = 6;
        /// <summary>
        /// ボスの弱点が出現する時間を参照する変数
        /// </summary>
        public float BossWeakTime = 12f;
        /// <summary>
        /// ボスの目覚める時間を参照する変数
        /// </summary>
        public float BossWakeUpTime = 5f;
        /// <summary>
        /// ボスの待機する時間を参照する変数
        /// </summary>
        public float BossWaitTime = 3f;
        /// <summary>
        /// ボスの踏みつけ攻撃の時間を参照する変数
        /// </summary>
        public float StumpAttackTime = 0.5f;
        /// <summary>
        /// ボスが復帰してくる時間を参照する変数
        /// </summary>
        public float StandTime = 1.0f;
        /// <summary>
        /// ジャンプ攻撃を仕掛ける距離を参照する変数
        /// </summary>
        public float DistanceNumber = 8.0f;
        /// <summary>
        /// ハンマー攻撃を仕掛ける時間を参照する変数
        /// </summary>
        public float HammerAttackTime = 30.0f;
        /// <summary>
        /// ボスがやられる時間を参照する変数
        /// </summary>
        public float BossDefeatTime = 3.0f;
        /// <summary>
        /// ボス開始待機時間を参照する変数
        /// </summary>
        public float BossStartTime = 3.0f;
        /// <summary>
        /// ハンマー攻撃時間初期値を参照する変数
        /// </summary>
        public float HammerAttackTimeDefault = 30.0f;
        /// <summary>
        /// 近接攻撃アニメーション時間を参照する変数
        /// </summary>
        public float MeleeAttackAnimTime = 1.5f;
        /// <summary>
        /// 踏みつけ攻撃の待機時間を参照する変数
        /// </summary>
        public float StumpWaitTime = 1;
        /// <summary>
        /// 踏みつけ攻撃コライダー出現までのクールタイムを参照する変数
        /// </summary>
        public float StumpColliderArriveCooldown = 1;
        /// <summary>
        /// 死亡時攻撃コライダー無効化までの時間を参照する変数
        /// </summary>
        public float DeathColliderTime;
        /// <summary>
        /// 踏みつけ攻撃エフェクト再生までの待機時間を参照する変数
        /// </summary>
        public float ParticleWaitTime = 0.5f;

        // --- アニメーションID登録 ---
        /// <summary>
        /// 歩行アニメーションのIDを参照する変数
        /// </summary>
        public static readonly int IsWalkingID = Animator.StringToHash("On_IsWalking");
        /// <summary>
        /// 近接攻撃アニメーションのIDを参照する変数
        /// </summary>
        public static readonly int AttackID = Animator.StringToHash("OnAttack");
        /// <summary>
        /// 弱点出現アニメーションのIDを参照する変数
        /// </summary>
        public static readonly int ImmediateryWeakID = Animator.StringToHash("On_ImmediatelyWeak");
        /// <summary>
        /// 復帰アニメーションのIDを参照する変数
        /// </summary>
        public static readonly int WakeUpID = Animator.StringToHash("OnWakeUp");
        /// <summary>
        /// 死亡アニメーションのIDを参照する変数
        /// </summary>
        public static readonly int DieID = Animator.StringToHash("OnDie");
        /// <summary>
        /// 着地アニメーションのIDを参照する変数
        /// </summary>
        public static readonly int LandingID = Animator.StringToHash("OnLanding");
        /// <summary>
        /// 踏みつけ攻撃アニメーションのIDを参照する変数
        /// </summary>
        public static readonly int StumpID = Animator.StringToHash("OnStump");

        // --- 判定用フラグ ---
        /// <summary>
        /// 移動中かどうかの判定を参照する変数
        /// </summary>
        private bool _isMoving = false;
        /// <summary>
        /// 回転中かどうかの判定を参照する変数
        /// </summary>
        private bool _isTurning = false;
        /// <summary>
        /// 踏みつけ攻撃中かどうかの判定を参照する変数
        /// </summary>
        private bool _isStumping = false;
        /// <summary>
        /// 歩行SE再生中かどうかの判定を参照する変数
        /// </summary>
        private bool _isWalkingSE = false;
        /// <summary>
        /// 弱点出現中かどうかの判定を参照する変数
        /// </summary>
        private bool _isAppeardWeak = false;
        /// <summary>
        /// スタン中かどうかの判定を参照する変数
        /// </summary>
        private bool _isStunning = false;

        // ---タグ名の参照---
        /// <summary>
        /// プレイヤーのタグ名を参照する変数
        /// </summary>
        private string _playerTagName = "Player";

        /// <summary>
        /// 初期化処理を行う関数
        /// </summary>
        private void Awake()
        {
            // --- スクリプト参照用変数初期化 ---
            // ステータスマネージャーボス参照用
            statusManager = GetComponent<StatusManagerBoss>();
            // Rigidbodyコンポーネント参照用
            rigidbody = GetComponent<Rigidbody>();
            // プレイヤーオブジェクト参照用
            _targetObject = GameObject.FindWithTag(_playerTagName);
            // プレイヤースクリプト参照用
            player = _targetObject.GetComponent<PlayerController>();

            // --- イベント登録 ---
            // 死亡時実行の関数を入れる
            statusManager.OnDeath += Die;
            // スタン食らったときの関数を入れる
            statusManager.OnStunTaken += Stun;
            // ダメージ食らったときの関数を入れる
            statusManager.OnDamageTaken += Damage;
            // 歩行SE再生関数を入れる
            ModelScript.PlayWalkSE += PlayWalkSE;

            // --- フラグ初期化 ---
            // 無敵解除
            statusManager.isInvincible = false;
            // 歩行SE再生可
            _isWalkingSE = true;
            // 方向不可
            _isTurning = false;
            // 移動停止
            _isMoving = false;
            // 踏みつけ攻撃停止
            _isStumping = false;
            // 攻撃判定無効化
            AttackCollider.enabled = false;
            // ボス本体判定有効化
            BodyAttackCollider.enabled = true;
            // 踏みつけ攻撃用コライダー無効化
            StumpCollider.enabled = false;
            // プレイヤーとのCollisionColliderを無効化
            Collider2_Player.enabled = false;

            // すべての攻撃パーティクルをサーチ
            foreach (ParticleSystem stump in _particles.Stump)
            {
                // パーティクルを停止
                stump.Stop();
            }

            // すべての大型攻撃パーティクルをサーチ
            foreach (ParticleSystem bigStump in _particles.BigStump)
            {
                // パーティクルを停止
                bigStump.Stop();
            }

            // 待機時の関数を呼び出し
            Idle();
        }

        /// <summary>
        /// オブジェクト破棄時の処理を行う関数
        /// </summary>
        private void OnDestroy()
        {
            // もしステータスマネージャーがある場合
            if (statusManager != null)
            {
                // 死亡時実行の関数を消す
                statusManager.OnDeath -= Die;
                // スタン食らったときの関数を消す
                statusManager.OnStunTaken -= Stun;
                // ダメージ食らったときの関数を消す
                statusManager.OnDamageTaken -= Damage;
                // 歩行SE再生関数解除
                ModelScript.PlayWalkSE -= PlayWalkSE;
            }
        }

        /// <summary>
        /// スタート時の処理を行う関数
        /// </summary>
        void Start()
        {
            StartCoroutine(OnPose());// 初動行動開始
        }

        // 初動行動
        IEnumerator OnPose()
        {
            yield return new WaitForSeconds(BossStartTime);// ボス開始時間待機

            // フラグ変更
            _isTurning = true;// 回転可能
            _isMoving = true;// 移動開始

            HammerAttackTime = HammerAttackTimeDefault;// ハンマー攻撃時間リセット
        }

        /// <summary>
        /// ボスの行動処理を行う関数
        /// </summary>
        void FixedUpdate()
        {
            // もし弱点出現中であれば
            if (_isAppeardWeak)
            {
                return;// 弱点出現中は処理終了
            }

            float distance = Vector3.Distance(_targetObject.transform.position, this.transform.position);// プレイヤーの近くにいたらジャンプ攻撃を仕掛ける処理

            // もしハンマー攻撃時間が来ていて、ジャンプ攻撃中でなければ
            if (HammerAttackTime <= 0 && !_isStumping)
            {
                DoubleHammerAttack();// ハンマー攻撃処理
                HammerAttackTime = HammerAttackTimeDefault;// ハンマー攻撃時間リセット
            }
            // そうでなければ
            else
            {
                HammerAttackTime -= Time.fixedDeltaTime;// ハンマー攻撃時間カウントダウン
            }

            // もし回転可能であれば
            if (_isTurning)
            {
                Turn();// 回転処理

                // もし移動中であれば
                if (_isMoving)
                {
                    Walk();

                    // もしプレイヤーが近くにいたら
                    if (distance <= DistanceNumber)
                    {
                        TramplingAttack();
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
            float speed = RotateSpeed;// 補完スピードを決める
            Vector3 relativePos = _targetObject.transform.position - transform.position;// ターゲット方向のベクトルを取得

            relativePos.y = 0;// X軸の回転は禁止する

            // 方向を向く処理
            Quaternion rotation = Quaternion.LookRotation(relativePos);// 方向を、回転情報に変換
            transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);// 現在の回転情報と、ターゲット方向の回転情報を補完する
        }

        /// <summary>
        /// ボスを移動する関数
        /// </summary>
        private void Walk()
        {
            Vector3 forward = transform.forward * MoveP;// 前方向に移動ベクトル設定
            rigidbody.linearVelocity = new Vector3(forward.x, rigidbody.linearVelocity.y, forward.z);// 前方向に移動
            Animator.SetFloat(IsWalkingID, rigidbody.linearVelocity.magnitude);// 歩行アニメーション開始

            // 歩行SE再生処理開始
            if (_isWalkingSE)
            {
                _isWalkingSE = false;// 歩行SE再生判定用
            }
        }

        /// <summary>
        /// ボスが止まる関数
        /// </summary>
        private void Idle()
        {
            rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);// 移動停止
            Animator.SetFloat(IsWalkingID, rigidbody.linearVelocity.magnitude);// 歩行アニメーション停止
        }

        /// <summary>
        /// ジャンプ攻撃処理を行う関数
        /// </summary>
        private void TramplingAttack()
        {
            StartCoroutine(OnTramplingAttack());// ジャンプ攻撃処理開始
        }

        // ジャンプ攻撃処理
        IEnumerator OnTramplingAttack()
        {
            // ジャンプ開始
            _isMoving = false;
            _isWalkingSE = true;
            _isStumping = true;

            _isTurning = false;// 回転停止

            Animator.SetTrigger(StumpID);// ジャンプ開始
            yield return new WaitForSeconds(StumpWaitTime);// ジャンプまでの待機時間
            Animator.SetTrigger(LandingID);// ジャンプ着地アニメーション開始
            yield return new WaitForSeconds(StumpColliderArriveCooldown);// 踏みつけ攻撃コライダー出現までのクールタイム待機
            AudioPlayer.Instance.PlaySE(7);// ジャンプ攻撃SE再生（中山が編集）
            StumpCollider.enabled = true;// 踏みつけ攻撃用コライダー有効化
            yield return new WaitForSeconds(ParticleWaitTime);// 踏みつけ攻撃エフェクト再生までの待機時間

            // 踏みつけ攻撃エフェクト再生
            foreach (ParticleSystem stump in _particles.Stump)
            {
                stump.Play();// 踏みつけ攻撃エフェクト再生
            }

            yield return new WaitForSeconds(StumpAttackTime);// 踏みつけ攻撃時間待機
            StumpCollider.enabled = false;// 踏みつけ攻撃用コライダー無効化
            _isTurning = true;// 回転可能
            yield return new WaitForSeconds(StandTime);// 少し待機

            // ジャンプ終了
            _isMoving = true;
            _isStumping = false;
        }

        /// <summary>
        /// ダブルスレッジハンマー攻撃処理を行う関数
        /// </summary>
        private void DoubleHammerAttack()
        {
            StartCoroutine(OnDoubleHammerAttack());// ハンマー攻撃処理開始
        }

        // ハンマー攻撃処理
        IEnumerator OnDoubleHammerAttack()
        {
            _isMoving = false;// 移動停止
            _isWalkingSE = true;// 歩行SE再生判定用

            _isTurning = false;// 攻撃開始
            Animator.SetTrigger(AttackID);// ジャンプアニメーション開始
            AudioPlayer.Instance.PlaySE(2);// 攻撃SE再生）
            yield return new WaitForSeconds(MeleeAttackAnimTime);// 近接攻撃アニメーション時間待機
            AttackCollider.enabled = true;// 攻撃判定有効化

            // ダブルスレッジハンマー攻撃エフェクト再生
            foreach (ParticleSystem bigStump in _particles.BigStump)
            {
                bigStump.Play();// ダブルスレッジハンマー攻撃エフェクト再生
            }

            yield return new WaitForSeconds(BossAttackTime);// 攻撃する時間
            AttackCollider.enabled = false;// 攻撃判定無効化
            yield return new WaitForSeconds(BossLittleWaitTime);// 少し待機

            StartCoroutine(OnStun(BossWeakTime));// 弱点出現処理開始
        }

        // 弱点出現処理
        private IEnumerator OnStun(float stun)
        {
            // スタン開始
            _stunTimer = stun;
            _isStunning = true;

            Weak();// 弱点出現
            yield return new WaitForSeconds(BossWeakBeforeTime);// 弱点タイム
            BodyAttackCollider.enabled = false;// ボス本体判定無効化
            yield return new WaitForSeconds(BossVeryLittleWaitTime);// 少し待機
            Collider2_Player.enabled = true; // プレイヤーとのCollisionColliderを有効化

            // 倒れる間のタイマー
            while (_stunTimer >= 0)
            {
                _stunTimer -= Time.deltaTime;// スタンタイマー減少
                yield return null;// 1フレーム待機
            }

            WakeUp();// 起き上がり
            yield return new WaitForSeconds(BossWakeUpTime);// 待機
            BodyAttackCollider.enabled = true;// ボス本体判定有効化
            yield return new WaitForSeconds(BossWaitTime);// 少し待機

            _isTurning = true;// 攻撃停止
            _isMoving = true;// 移動開始
            _isAppeardWeak = false;// 弱点非出現化
            _isStunning = false;// スタン解除
            HammerAttackTime = HammerAttackTimeDefault;// ハンマー攻撃時間リセット
        }

        /// <summary>
        /// 弱点出現処理を行う関数
        /// </summary>
        private void Weak()
        {
            _isAppeardWeak = true;// 弱点出現化
        }

        /// <summary>
        /// 起き上がり処理を行う関数
        /// </summary>
        private void WakeUp()
        {
            Animator.SetTrigger(WakeUpID);// 起き上がりアニメーション再生
            Collider2_Player.enabled = false; // プレイヤーとのCollisionColliderを無効化
        }

        /// <summary>
        /// ボス撃退時の処理を行う関数
        /// </summary>
        private void Die()
        {
            StopAllCoroutines();// すべてのコルーチン停止
            StartCoroutine(OnDie());// 死亡処理開始

            // エフェクト無効化
            if (HaloEffect != null)
            {
                HaloEffect.SetActive(false);// ターゲットエフェクト無効化
            }
        }

        IEnumerator OnDie()
        {
            Animator.SetTrigger(DieID);// 死亡アニメーション再生（中山が編集）
            AudioPlayer.Instance.PlaySE(4);
            AttackCollider.enabled = false;
            BodyAttackCollider.enabled = false;
            StumpCollider.enabled = false;
            yield return new WaitForSeconds(DeathColliderTime);

            yield return new WaitForSeconds(BossDefeatTime - DeathColliderTime);// 少し待機（中山が編集）
            MainStageScene.Instance.StageClear();// ステージクリア処理（中山が編集）
            Destroy(gameObject);// ボスオブジェクトを破壊（中山が編集）
        }


        private void Damage()
        {
            // エフェクトをインスタンス化
            GameObject effect = Instantiate(DamageEffect);

            effect.transform.position = WeakCollider.transform.position;// 弱点コライダーの位置にエフェクトを出す（中山が編集）

            Destroy(effect, 5);// エフェクトを5秒後に破壊（中山が編集）
        }

        public void PlayWalkSE()
        {
            if (!_isWalkingSE)
            {
                AudioPlayer.Instance.PlaySE(6);
                AudioPlayer.Instance.PlaySE(5);
            }
        }

        private void Stun()
        {
            if (!_isStunning)
            {
                // これのために、アニメーションをanystate→倒れるにしとかないとダメかも

                StopAllCoroutines();

                Animator.SetTrigger(ImmediateryWeakID); // これ専用の倒れるトランジション
                StartCoroutine(OnStun(player.StunSkillTime));
            }
            else
            {
                // ひるむ時間を5秒にする
                _stunTimer = player.StunSkillTime;
            }
        }
    }
}