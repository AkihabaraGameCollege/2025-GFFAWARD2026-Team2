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
            foreach (ParticleSystem _stump in _particles.Stump)
            {
                // パーティクルを停止
                _stump.Stop();
            }

            // すべての大型攻撃パーティクルをサーチ
            foreach (ParticleSystem _bigStump in _particles.BigStump)
            {
                // パーティクルを停止
                _bigStump.Stop();
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
        private void Start()
        {
            // 初動行動を開始
            StartCoroutine(OnPose());
        }

        /// <summary>
        /// 初動の行動を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnPose()
        {
            // ボス開始時間待機
            yield return new WaitForSeconds(BossStartTime);

            // --- フラグ変更 ---
            // 回転可能
            _isTurning = true;
            // 移動開始
            _isMoving = true;

            // ハンマー攻撃時間リセット
            HammerAttackTime = HammerAttackTimeDefault;
        }

        /// <summary>
        /// ボスの行動処理を毎フレームで管理する関数
        /// </summary>
        private void FixedUpdate()
        {
            // もし弱点出現中な場合
            if (_isAppeardWeak)
            {
                return;
            }

            // プレイヤーとの距離を参照する変数を定義
            float _distance = Vector3.Distance(_targetObject.transform.position, this.transform.position);

            // もしハンマー攻撃時間が来ていて、ジャンプ攻撃中の場合
            if (HammerAttackTime <= 0 && !_isStumping)
            {
                // ハンマー攻撃処理
                DoubleHammerAttack();
                // ハンマー攻撃時間リセット
                HammerAttackTime = HammerAttackTimeDefault;
            }
            else
            {
                // ハンマー攻撃時間カウントダウン
                HammerAttackTime -= Time.fixedDeltaTime;
            }

            // もし回転可能の場合
            if (_isTurning)
            {
                // 回転処理
                Turn();

                // もし移動中の場合
                if (_isMoving)
                {
                    // 移動処理
                    Walk();

                    // もしプレイヤーが近くにいた場合
                    if (_distance <= DistanceNumber)
                    {
                        // 踏みつけ攻撃
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
            // --- 回転処理 ---
            // 補完スピードを決める
            float _speed = RotateSpeed;
            // ターゲット方向のベクトルを取得
            Vector3 _relativePos = _targetObject.transform.position - transform.position;

            // Y軸座標のサーチは禁止する
            _relativePos.y = 0;

            // --- 方向を向く処理 ---
            // 方向を、回転情報に変換
            Quaternion _rotation = Quaternion.LookRotation(_relativePos);
            // 現在の回転情報と、ターゲット方向の回転情報を補完する
            transform.rotation = Quaternion.Slerp(this.transform.rotation, _rotation, _speed);
        }

        /// <summary>
        /// ボスが移動する関数
        /// </summary>
        private void Walk()
        {
            // 前方向に移動ベクトル設定
            Vector3 _forward = transform.forward * MoveP;
            // 前方向に移動
            rigidbody.linearVelocity = new Vector3(_forward.x, rigidbody.linearVelocity.y, _forward.z);
            // 歩行アニメーション開始
            Animator.SetFloat(IsWalkingID, rigidbody.linearVelocity.magnitude);

            // もし歩行SEが流れていた場合
            if (_isWalkingSE)
            {
                // 歩行SE再生のフラグをリセット
                _isWalkingSE = false;
            }
        }

        /// <summary>
        /// ボスが待機状態になる関数
        /// </summary>
        private void Idle()
        {
            // 移動停止
            rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);
            // 歩行アニメーション停止
            Animator.SetFloat(IsWalkingID, rigidbody.linearVelocity.magnitude);
        }

        /// <summary>
        /// 踏みつけ攻撃処理を呼び出す関数
        /// </summary>
        private void StumpAttack()
        {
            // 踏みつけ攻撃処理開始
            StartCoroutine(StumpAttackCoroutine());
        }

        /// <summary>
        /// 踏みつけ攻撃処理を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator StumpAttackCoroutine()
        {
            // --- フラグをリセット ---
            // 移動停止
            _isMoving = false;
            // 歩行SE停止
            _isWalkingSE = true;
            // 踏みつけ攻撃開始
            _isStumping = true;
            // 視点移動停止
            _isTurning = false;

            // ジャンプ開始
            Animator.SetTrigger(StumpID);
            // ジャンプまでの待機
            yield return new WaitForSeconds(StumpWaitTime);
            // ジャンプ着地アニメーション開始
            Animator.SetTrigger(LandingID);
            // 踏みつけ攻撃コライダー出現までのクールタイム待機
            yield return new WaitForSeconds(StumpColliderArriveCooldown);
            // ジャンプ攻撃SE再生
            AudioPlayer.Instance.PlaySE(7);
            // 踏みつけ攻撃用コライダー有効化
            StumpCollider.enabled = true;
            // 踏みつけ攻撃エフェクト再生までの待機
            yield return new WaitForSeconds(ParticleWaitTime);

            // 踏みつけ攻撃エフェクトをすべてサーチ
            foreach (ParticleSystem _stump in _particles.Stump)
            {
                // 踏みつけ攻撃エフェクト再生
                _stump.Play();
            }

            // 踏みつけ攻撃時間待機
            yield return new WaitForSeconds(StumpAttackTime);
            // 踏みつけ攻撃用コライダー無効化
            StumpCollider.enabled = false;
            // 回転可能
            _isTurning = true;
            // 少し待機
            yield return new WaitForSeconds(StandTime);

            // --- フラグを変更 ---
            // 移動開始
            _isMoving = true;
            // 踏みつけ攻撃停止
            _isStumping = false;
        }

        /// <summary>
        /// ダブルスレッジハンマー攻撃処理を呼び出す関数
        /// </summary>
        private void DoubleHammerAttack()
        {
            // ハンマー攻撃処理開始
            StartCoroutine(DoubleHammerAttackCoroutine());
        }

        /// <summary>
        /// ハンマー攻撃処理を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator DoubleHammerAttackCoroutine()
        {
            // --- フラグをリセット ---
            // 移動停止
            _isMoving = false;
            // 歩行SE停止
            _isWalkingSE = true;
            // 視点移動停止
            _isTurning = false;

            // ジャンプアニメーション開始
            Animator.SetTrigger(AttackID);
            // 攻撃SE再生）
            AudioPlayer.Instance.PlaySE(2);
            // 近接攻撃アニメーション時間待機
            yield return new WaitForSeconds(MeleeAttackAnimTime);
            // 攻撃判定有効化
            AttackCollider.enabled = true;

            // ダブルスレッジハンマー攻撃エフェクトをすべてサーチ
            foreach (ParticleSystem _bigStump in _particles.BigStump)
            {
                // ダブルスレッジハンマー攻撃エフェクト再生
                _bigStump.Play();
            }

            // 攻撃する時間分待機
            yield return new WaitForSeconds(BossAttackTime);
            // 攻撃判定無効化
            AttackCollider.enabled = false;
            // 少し待機
            yield return new WaitForSeconds(BossLittleWaitTime);

            // 弱点出現処理開始
            StartCoroutine(StunCoroutine(BossWeakTime));
        }

        /// <summary>
        /// 弱点出現処理を行うコルーチン
        /// </summary>
        /// <param name="_stun"></param>
        /// <returns></returns>
        private IEnumerator StunCoroutine(float _stunTime)
        {
            // --- スタン処理の準備 ---
            _stunTimer = _stunTime;
            _isStunning = true;

            // 弱点ポイント出現
            Weak();
            // ボスがスタンする前に少し待機
            yield return new WaitForSeconds(BossWeakBeforeTime);
            // ボス本体判定無効化
            BodyAttackCollider.enabled = false;
            // 少し待機
            yield return new WaitForSeconds(BossVeryLittleWaitTime);
            // プレイヤーとのCollisionColliderを有効化
            Collider2_Player.enabled = true;

            // スタンタイマーが0以上の間ループ
            while (_stunTimer >= 0)
            {
                _stunTimer -= Time.deltaTime;// スタンタイマー減少
                yield return null;// 1フレーム待機
            }

            // 起き上がり
            WakeUp();
            // 待機
            yield return new WaitForSeconds(BossWakeUpTime);
            // ボス本体判定有効化
            BodyAttackCollider.enabled = true;
            // 少し待機
            yield return new WaitForSeconds(BossWaitTime);

            // --- スタン終了後の後処理 ---
            // 攻撃停止
            _isTurning = true;
            // 移動開始
            _isMoving = true;
            // 弱点非出現化
            _isAppeardWeak = false;
            // スタン解除
            _isStunning = false;
            // ハンマー攻撃時間リセット
            HammerAttackTime = HammerAttackTimeDefault;
        }

        /// <summary>
        /// 弱点出現処理を行う関数
        /// </summary>
        private void Weak()
        {
            // 弱点出現化
            _isAppeardWeak = true;
        }

        /// <summary>
        /// 起き上がり処理を行う関数
        /// </summary>
        private void WakeUp()
        {
            // 起き上がりアニメーション再生
            Animator.SetTrigger(WakeUpID);
            // プレイヤーとのCollisionColliderを無効化
            Collider2_Player.enabled = false;
        }

        /// <summary>
        /// ボス撃退時の処理を呼び出す関数
        /// </summary>
        private void Die()
        {
            // すべてのコルーチン停止
            StopAllCoroutines();
            // 死亡処理開始
            StartCoroutine(DieCoroutine());

            // もし弱点強調エフェクトがある場合
            if (HaloEffect != null)
            {
                // 弱点強調エフェクト無効化
                HaloEffect.SetActive(false);
            }
        }

        /// <summary>
        /// ボス撃退時の処理を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator DieCoroutine()
        {
            // 死亡アニメーション再生
            Animator.SetTrigger(DieID);
            // 死亡時の咆哮SE再生
            AudioPlayer.Instance.PlaySE(4);

            // ---各フラグの変更---
            AttackCollider.enabled = false;
            BodyAttackCollider.enabled = false;
            StumpCollider.enabled = false;

            // ボスが死亡する時間分待機
            yield return new WaitForSeconds(DeathColliderTime);

            // 少し待機
            yield return new WaitForSeconds(BossDefeatTime - DeathColliderTime);
            // ステージクリア処理
            MainStageScene.Instance.StageClear();
            // ボスオブジェクトを破壊
            Destroy(gameObject);
        }

        /// <summary>
        /// ダメージ処理を行う関数
        /// </summary>
        private void Damage()
        {
            // エフェクトをインスタンス化
            GameObject effect = Instantiate(DamageEffect);

            // 弱点コライダーの位置にエフェクトを出す
            effect.transform.position = WeakCollider.transform.position;

            // エフェクトを5秒後に破壊
            Destroy(effect, 5);
        }

        /// <summary>
        /// 歩行SEを再生する関数
        /// </summary>
        public void PlayWalkSE()
        {
            // もし歩行SE再生のフラグがオフの場合
            if (_isWalkingSE)
            {
                AudioPlayer.Instance.PlaySE(6);
                AudioPlayer.Instance.PlaySE(5);
            }
        }

        /// <summary>
        /// スタン処理を呼び出す関数
        /// </summary>
        private void Stun()
        {
            // スタンしていない場合
            if (!_isStunning)
            {
                StopAllCoroutines();

                // 倒れるアニメーションを再生
                Animator.SetTrigger(ImmediateryWeakID);
                // スタン処理開始
                StartCoroutine(StunCoroutine(player.StunSkillTime));
            }
            else
            {
                // ひるむ時間を調整する
                _stunTimer = player.StunSkillTime;
            }
        }
    }
}