using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QuickTheFury
{
    /// <summary>
    /// 第二ボスの移動・攻撃処理クラス
    /// </summary>
    public class Boss2_MoveAI : MonoBehaviour
    {
        /// <summary>
        /// ヒップドロップ攻撃エフェクトの構造体
        /// </summary>
        [Serializable]
        private struct ParticleSystems
        {
            /// <summary>
            /// スタンプエフェクトの配列を参照する変数
            /// </summary>
            public ParticleSystem[] Stump;
        }

        /// <summary>
        /// 着地攻撃コライダーを参照する変数
        /// </summary>
        [SerializeField]
        private Collider _attackCollider;
        /// <summary>
        /// 弱点コライダーを参照する変数
        /// </summary>
        [SerializeField]
        private Collider _weakCollider;

        /// <summary>
        /// ダメージエフェクトのプレハブを参照する変数
        /// </summary>
        [SerializeField]
        private GameObject _damageEffect;
        /// <summary>
        /// 弱点強調エフェクト
        /// </summary>
        [SerializeField]
        private GameObject _haloEffect;
        /// <summary>
        /// 雑魚のprefabを参照する変数
        /// </summary>
        [SerializeField]
        private GameObject _zakoPrefab;
        /// <summary>
        /// スタン時のエフェクトを参照する変数
        /// </summary>
        [SerializeField]
        private GameObject _stunEffect;

        /// <summary>
        /// モデルについてるアニメーターを参照する変数
        /// </summary>
        [SerializeField]
        private Animator _animator;

        /// <summary>
        /// モデルについてるScriptを参照する変数
        /// </summary>
        [SerializeField]
        private ActionSoundsPlayer _model_Script;

        /// <summary>
        /// 雑魚のスポーン地点を参照する変数
        /// </summary>
        [SerializeField]
        private Vector3 _zakoSpawnOffset;
        /// <summary>
        /// 地面との着地判定線始点を参照する変数
        /// </summary>
        [SerializeField]
        private Vector3 _groundCheckStartPoint = new Vector3(0, 0.5f, 0);
        /// <summary>
        /// 地面との着地判定線終点を参照する変数
        /// </summary>
        [SerializeField]
        private Vector3 _groundCheckEndPoint = new Vector3(0, -0.5f, 0);
        /// <summary>
        /// スタンエフェクトの位置を参照する変数
        /// </summary>
        [SerializeField]
        private Vector3 _stunEffectPos;
        /// <summary>
        /// スタンエフェクトのサイズを参照する変数
        /// </summary>
        [SerializeField]
        private Vector3 _stunEffectScale;

        /// <summary>
        /// 地面のレイヤーを参照する変数
        /// </summary>
        [SerializeField]
        private LayerMask _groundLayer;

        /// <summary>
        /// パーティクルエフェクトを参照する変数
        /// </summary>
        [SerializeField]
        private ParticleSystems _particles;

        /// <summary>
        /// ジャンプ力を参照する変数
        /// </summary>
        [SerializeField]
        private float _jumpForce = 10;
        /// <summary>
        /// 移動速度を参照する変数
        /// </summary>
        [SerializeField]
        private float _moveSpeed = 3;
        /// <summary>
        /// 回転速度を参照する変数
        /// </summary>
        [SerializeField]
        private float _rotateSpeed = 11.1f;
        /// <summary>
        /// スタン時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _defaultStunTime = 15;
        /// <summary>
        /// スタートモーション時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _startMotionTime = 4;
        /// <summary>
        /// 次の雑魚が召喚されるまでの待機時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _zakoSummonWaitTime = 0.1f;
        /// <summary>
        /// 雑魚の拡散スピード最小値を参照する変数
        /// </summary>
        [SerializeField]
        private float _zakoMinSpreadSpeed;
        /// <summary>
        /// 雑魚の拡散スピード最大値を参照する変数
        /// </summary>
        [SerializeField]
        private float _zakoMaxSpreadSpeed;
        /// <summary>
        /// 雑魚の拡散時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _zakoSpreadTime;
        /// <summary>
        /// 雑魚の移動速度を参照する変数
        /// </summary>
        [SerializeField]
        private float _zakoMoveSpeed;
        /// <summary>
        /// 雑魚を吸収可能な範囲を参照する変数
        /// </summary>
        [SerializeField]
        private float _zakoAbsorbRadius;
        /// <summary>
        /// 歩行継続時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _walkTime = 2;
        /// <summary>
        /// ジャンプ目標のY軸オフセットを参照する変数
        /// </summary>
        [SerializeField]
        private float _jumpTargetOffsetY = 10;
        /// <summary>
        /// ヒップドロップ時の中に固定される時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _jump2_FreezeWaitTime = 1;
        /// <summary>
        /// 空中での静止時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _jumpFreezeTime = 1;
        /// <summary>
        /// 着地時のスピードを参照する変数
        /// </summary>
        [SerializeField]
        private float _dropSpeed = 10;
        /// <summary>
        /// 着地時コライダー出現継続時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _dropAttackTime = 0.1f;
        /// <summary>
        /// 立ち上がりにかかる時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _standUpTime = 5;
        /// <summary>
        /// 死亡アニメーション時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _deathAnimTime = 3.5f;
        /// <summary>
        /// 死亡SEを鳴らす回数を参照する変数
        /// </summary>
        [SerializeField]
        private float _deathScreamCount = 12;
        /// <summary>
        /// 死亡SEを鳴らし続ける時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _deathScreamTime = 2.5f;
        /// <summary>
        /// 歩行時のSEを鳴らす間隔を参照する変数
        /// </summary>
        [SerializeField]
        private float _walkSE_Cooldown;

        /// <summary>
        /// 何回ダメージを食らったら雑魚を召喚するかの回数を参照する変数
        /// </summary>
        [SerializeField]
        private int _damageCount2_ZakoSummon = 5;
        /// <summary>
        /// 一度に召喚する雑魚の数を参照する変数
        /// </summary>
        [SerializeField]
        private int _zakoSummonCount = 3;

        /// <summary>
        /// プレイヤー操作管理クラスを参照する変数
        /// </summary>
        private PlayerController _playerController;
        /// <summary>
        /// ボスのステータスを管理するクラスを参照する変数
        /// </summary>
        private StatusManagerBoss _statusManagerBoss;

        /// <summary>
        /// Rigidbodyコンポーネントを参照する変数
        /// </summary>
        private Rigidbody _rigidbody;

        /// <summary>
        /// スタンエフェクトを発生させるオブジェクトを参照する変数
        /// </summary>
        private GameObject _stunEffectObject;

        /// <summary>
        /// 何回ダメージ食らったかのカウンターを参照する変数
        /// </summary>
        private int _damageCounter = 0;

        // --- アニメーションデータ ---
        /// <summary>
        /// 歩きモーションのIDを参照する変数
        /// </summary>
        private static readonly int _isWalkingID = Animator.StringToHash("IsWalking");
        /// <summary>
        /// 立ち上がりモーションのIDを参照する変数
        /// </summary>
        private static readonly int _standUpID = Animator.StringToHash("Stand");
        /// <summary>
        /// ヒップドロップモーションのIDを参照する変数
        /// </summary>
        private static readonly int _hipDropID = Animator.StringToHash("HipDrop");
        /// <summary>
        /// 死亡モーションのIDを参照する変数
        /// </summary>
        private static readonly int _dieID = Animator.StringToHash("Die");
        /// <summary>
        /// 弱点発生モーションのIDを参照する変数
        /// </summary>
        private static readonly int _immediatelyWeakID = Animator.StringToHash("ImmediatelyWeak");

        /// <summary>
        /// スタン時間を参照する変数
        /// </summary>
        private float _stunTimer = 0;

        /// <summary>
        /// スタン中かを判別するフラグを参照する変数
        /// </summary>
        private bool _isStunning = false;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();//Rigidbodyコンポーネント取得
            _statusManagerBoss = GetComponent<StatusManagerBoss>();

            // find with tagってやっていいのかな
            _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

            _statusManagerBoss.OnDamageTaken += Damage;
            _statusManagerBoss.OnDeath += Die;
            _statusManagerBoss.OnStunTaken += Stun;
            _model_Script.PlayWalkSE += PlayWalkSE;

            _statusManagerBoss.isInvincible = false;
            _attackCollider.enabled = false;//攻撃判定無効化

            // ヒップドロップ攻撃エフェクト停止
            foreach (ParticleSystem _stump in _particles.Stump)
            {
                _stump.Stop();
            }

            StartCoroutine(OnPose());// スタートモーション開始
        }

        // StatusManagerBossから呼び出される
        public void Damage()
        {
            _damageCounter++;
            AudioPlayer.Instance.PlaySE(1);
            if (_damageCounter >= _damageCount2_ZakoSummon)
            {
                StartCoroutine(OnCottonPopsOut(_zakoSummonCount));
                _damageCounter = 0;
            }
            // エフェクトをインスタンス化
            GameObject effect = Instantiate(_damageEffect);

            effect.transform.position = _weakCollider.transform.position;// 弱点コライダーの位置にエフェクトを出す（中山が編集）

            Destroy(effect, 5);// エフェクトを5秒後に破壊
        }

        public void Die()
        {
            StopAllCoroutines();
            StartCoroutine(OnDie());
            _haloEffect.SetActive(false);

        }

        IEnumerator OnDie()
        {
            _animator.SetTrigger(_dieID);
            _attackCollider.enabled = false;

            int counter = 0;
            while (counter < _deathScreamCount)
            {
                counter++;
                yield return new WaitForSeconds(_deathScreamTime / _deathScreamCount);
                AudioPlayer.Instance.PlaySE(1);
            }

            yield return new WaitForSeconds(_deathAnimTime);
            MainStageScene.Instance.StageClear();
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            if (_statusManagerBoss != null)
            {
                _statusManagerBoss.OnStunTaken -= Stun;
                _statusManagerBoss.OnDamageTaken -= Damage;
            }
        }

        IEnumerator OnMainThinking()
        {
            // 基本のループ
            while (true)
            {
                yield return StartCoroutine(OnMainMoving());
            }
        }

        IEnumerator OnPose()
        {
            yield return new WaitForSeconds(_startMotionTime);
            StartCoroutine(OnMainThinking());
        }

        IEnumerator OnMainMoving()
        {
            // 2秒間の間歩く
            float timer = 0;
            while (timer < _walkTime)
            {
                timer += Time.fixedDeltaTime;

                Walk();
                yield return new WaitForFixedUpdate();
            }
            // 一連の処理
            yield return OnHipDropAttack();
            yield return OnStun(_defaultStunTime);
            yield return OnStandUp();
        }

        private void Walk()
        {
            // 移動方向を取得
            Vector3 moveDirection = (_playerController.transform.position - transform.position).normalized;

            // Yをなくす
            moveDirection.y = 0;

            // 移動
            _rigidbody.linearVelocity = moveDirection * _moveSpeed;

            // 方向転換
            // 補完スピードを決める
            // ターゲット方向のベクトルを取得
            Vector3 relativePos = _playerController.gameObject.transform.position - transform.position;

            relativePos.y = 0; // X軸の回転は禁止する

            // 方向を、回転情報に変換
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            // 現在の回転情報と、ターゲット方向の回転情報を補完する
            _rigidbody.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotateSpeed * Time.fixedDeltaTime);
            _animator.SetFloat(_isWalkingID, _rigidbody.linearVelocity.magnitude);// 歩行アニメーション
        }

        IEnumerator OnHipDropAttack()
        {
            // 方向を定める
            Vector3 direction = ((_playerController.transform.position + new Vector3(0f, _jumpTargetOffsetY, 0f)) - transform.position).normalized;
            // スピードに代入
            _rigidbody.linearVelocity = direction * _jumpForce;
            // アニメーション
            _animator.SetTrigger(_hipDropID);
            // ちょっとまつ
            yield return new WaitForSeconds(_jump2_FreezeWaitTime);

            // フリーズ
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.useGravity = false;
            yield return new WaitForSeconds(_jumpFreezeTime / 2);

            // ピッチ下げてる影響で、着地後に鳴らすと遅すぎる為ここで鳴らす
            AudioPlayer.Instance.PlaySE(0, 1f, 0.2f);

            yield return new WaitForSeconds(_jumpFreezeTime / 2);

            // ドロップ
            _rigidbody.useGravity = true;
            _rigidbody.linearVelocity = Vector3.down * _dropSpeed;

            // 地面に着地するまで待つ
            bool isGrounded = false;
            while (!isGrounded)
            {
                isGrounded = Physics.Linecast(transform.position + _groundCheckStartPoint, transform.position + _groundCheckEndPoint, _groundLayer);
                yield return new WaitForFixedUpdate();
            }

            // ヒップドロップ攻撃エフェクト再生
            foreach (ParticleSystem stump in _particles.Stump)
            {
                stump.Play();
            }

            // 着地攻撃判定を出す
            _attackCollider.enabled = true;
            // 攻撃時間待つ
            yield return new WaitForSeconds(_dropAttackTime);
            // 判定消す
            _attackCollider.enabled = false;
        }

        IEnumerator OnStun(float weakTime)
        {
            _stunTimer = weakTime;
            _isStunning = true;

            while (_stunTimer >= 0)
            {
                _stunTimer -= Time.deltaTime;
                yield return null;
            }
            _isStunning = false;
        }

        IEnumerator OnStandUp()
        {
            // どうするんだ？アニメーション？
            _animator.SetTrigger(_standUpID);
            // 待つ(アニメーションイベントでもいいかも)
            yield return new WaitForSeconds(_standUpTime);
        }


        IEnumerator OnCottonPopsOut(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // 召喚
                GameObject go = Instantiate(_zakoPrefab, transform.position + _zakoSpawnOffset, Quaternion.identity);
                // 召喚したオブジェクトのscriptを持ってくる
                CottonMonsterMoveAI script = go.GetComponent<CottonMonsterMoveAI>();
                // Yはプラス、XZは完全ランダムな方向を取得
                Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                // 拡散スピードを決定
                float spd = Random.Range(_zakoMinSpreadSpeed, _zakoMaxSpreadSpeed);
                // 初期化
                script.Initialize(this, dir, spd, _zakoSpreadTime, _zakoMoveSpeed, _zakoSpawnOffset, _zakoAbsorbRadius);
                // 次までの待機
                yield return new WaitForSeconds(_zakoSummonWaitTime);
            }
        }

        public void Heal()
        {
            if (_statusManagerBoss.health % (_statusManagerBoss.maxHealth / 3) != 0)
            {
                _statusManagerBoss.health++;
                MainStageScene.Instance.BossBarUpdate(_statusManagerBoss.health, _statusManagerBoss.maxHealth);
                AudioPlayer.Instance.PlaySE(14, 1);
            }
        }

        [ContextMenu("STUN")]
        private void Stun()
        {
            if (!_isStunning)
            {
                // ここで座り込むアニメーション再生が必要かも

                StopAllCoroutines();
                StartCoroutine(OnTakenStun());
            }
            else
            {
                // ひるむ時間を３秒くらいのばす
                _stunTimer = _playerController.StunSkillTime;
            }
        }
        IEnumerator OnTakenStun()
        {
            _attackCollider.enabled = false;
            _animator.SetTrigger(_immediatelyWeakID);
            _stunEffectObject = Instantiate(_stunEffect, this.transform.localPosition + _stunEffectPos, Quaternion.identity);
            _stunEffectObject.transform.localScale = _stunEffectScale;
            yield return StartCoroutine(OnStun(_playerController.StunSkillTime));
            Destroy(_stunEffectObject);
            yield return OnStandUp();
            StartCoroutine(OnMainThinking());
        }

        public void PlayWalkSE()
        {
            AudioPlayer.Instance.PlaySE(0);
        }
    }
}