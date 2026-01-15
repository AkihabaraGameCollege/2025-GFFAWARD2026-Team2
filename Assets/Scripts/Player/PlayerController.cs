using Assets.Scripts.Player;
using Assets.Scripts.Scene;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using AudioPlayer = QuickTheFury.Core.AudioPlayer;

namespace QuickTheFury.Player
{
    /// <summary>
    /// Playerの操作やその後の移動、ステータスなどを管理する
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Default Status")]
        [SerializeField]
        [Tooltip("移動速度")]
        private float moveSpeed;

        [SerializeField]
        [Tooltip("スプリント時のスピード")]
        private float sprintSpeed;

        [SerializeField]
        [Tooltip("スプリント可能秒数")]
        private float sprintSecond;

        [SerializeField]
        [Tooltip("ジャンプ力")]
        private float jumpForce;

        [SerializeField]
        [Tooltip("スタン攻撃のクールタイム")]
        private float stunCooldownTime;

        [SerializeField]
        [Tooltip("地面設置判定LineCastの始点")]
        private Vector3 groundCheckStartPoint = new Vector3(0, 0.5f, 0);
        [SerializeField]
        [Tooltip("地面設置判定LineCastの終点")]
        private Vector3 groundCheckEndPoint = new Vector3(0, -0.5f, 0);
        [SerializeField]
        [Tooltip("ジャンプ不能と判断される最大Y軸速度")]
        private float requiredJumpSpeed = 0.1f;

        [SerializeField]
        [Tooltip("攻撃後の膠着を指定")]
        private float playerWaitTime;

        [SerializeField]
        [Tooltip("攻撃前のモーション時間")]
        private float playerLittleWaitTime = 0.2f;

        [SerializeField]
        [Tooltip("攻撃コライダー出現時間を指定")]
        private float playerAttackTime = 0.5f;

        [SerializeField]
        [Tooltip("攻撃コライダーのサイズ")]
        private Vector3 attackReach;

        [SerializeField]
        [Tooltip("最大HP")]
        private int maxHealth;

        [SerializeField]
        [Tooltip("無敵時間")]
        private float invincibleTime = 1;

        [Tooltip("攻撃力")]
        public int damage;

        [SerializeField]
        [Tooltip("ノックバックの力")]
        private float knockBackForce;

        [SerializeField]
        [Tooltip("ノックバック減衰")]
        private float knockBackDecay;

        [SerializeField]
        [Tooltip("ノックバックの時の数値反転時の倍数\n説明：" +
            "\n敵のコライダーオブジェクトには、縦長でY軸の中心が上にある場合があります" +
            "\nその場合、下方向へ強い力がかかってしまうため、数値を反転させて対応しています" +
            "\nその時、逆に上方向への力が強すぎてしまうため、この値を掛けて力を弱めています")]
        private float knockBackInvertMultiplyNumber;


        [Header("強化倍率")]
        [SerializeField]
        [Tooltip("攻撃コライダーサイズ")]
        private float attackReachMagnification;

        [SerializeField]
        [Tooltip("攻撃力")]
        private int damageMagnicifation;

        [SerializeField]
        [Tooltip("ジャンプ力")]
        private float jumpForceMagnification;


        [Header("参照関連")]

        [SerializeField]
        private Animator animator;

        [SerializeField]
        [Tooltip("地面として判定するレイヤー")]
        private LayerMask groundLayer;

        [SerializeField]
        [Tooltip("基本攻撃のコライダー")]
        private Collider attackCollider = null;

        [SerializeField]
        [Tooltip("エフェクト再生用の手の場所取得")]
        private GameObject playerHand;
        public GameObject PlayerHand { get { return playerHand; } }

        [SerializeField]
        [Tooltip("ダッシュ攻撃のコライダー")]
        private Collider dashAttackCollider = null;

        [SerializeField]
        [Tooltip("攻撃エフェクト")]
        private GameObject destroyEffect;

        [SerializeField]
        [Tooltip("被弾エフェクト")]
        private GameObject damageEffect;

        [SerializeField]
        [Tooltip("ダッシュアタック時のエフェクト")]
        Effect dashEffect;

        [SerializeField]
        [Tooltip("強化されたジャンプをした時のエフェクト")]
        Effect jumpEffect;

        [SerializeField]
        [Tooltip("スタン攻撃をした時のエフェクト")]
        Effect attackEffect;


        [Header("その他")]
        [SerializeField]
        [Tooltip("Offset")]
        private Vector3[] wallCheckerPos = null;
        [SerializeField]
        [Tooltip("Distance")]
        private float wallCheckerDistance = 0;

        // 移動入力を保持
        private Vector2 moveInput;

        private Rigidbody rb;

        // 着地しているかどうかのフラグ
        private bool IsGrounded = false;

        /// <summary>
        /// 機能停止をしているかどうか
        /// </summary>
        public bool IsSleeping { get; private set; }

        // 攻撃可能かどうか
        private bool isAttacking = true;

        // 現在の体力
        private int health;

        // 無敵状態かどうか
        private bool isInvincible = false;

        /// <summary>
        /// ダッシュ中かどうか
        /// </summary>
        public bool IsSprinting { get; private set; } = false;

        // 残りのダッシュ可能時間
        private float sprintTimer;

        // 残りのスタン攻撃不能時間
        private float stunSkillTimer = 0;

        /// <summary>
        /// スタン攻撃が可能かどうか
        /// </summary>
        public bool IsStunable { get; private set; } = false;

        // スキルを入手しているかどうか
        private bool isGotAttackSkill = false;
        private bool isGotSpeedSkill = false;
        private bool isGotJumpSkill = false;

        // ノックバックのvelocity
        private Vector3 knockBackVelocity = Vector3.zero;
        // ノックバック中かどうか
        private bool isKnockBacking = false;

        [SerializeField]
        [Tooltip("スタン攻撃でスタンする時間")]
        private float stunSkillTime = 5;

        /// <summary>
        /// スタン攻撃でスタンする時間
        /// </summary>
        public float StunSkillTime { get { return stunSkillTime; } }


        //アニメーションID登録
        static readonly int jumpID = Animator.StringToHash("jump");
        static readonly int attackID = Animator.StringToHash("attack");
        static readonly int speedID = Animator.StringToHash("speed");
        static readonly int hitID = Animator.StringToHash("hit");
        static readonly int dieID = Animator.StringToHash("die");



        /// <summary>
        /// プレイヤーの状態enum
        /// </summary>
        enum MotionState
        {
            Stopping, // プレイヤーからの入力がない停止状態
            Walking, // 通常の歩行状態
            JumpAnticipation, // ジャンプしてから足が離れるまでの様子見時間
            Jumping, // ジャンプ状態
            Sprinting // ダッシュ状態
        }
        private MotionState motionState = MotionState.Stopping;

        private void Awake()
        {
            // コンポーネント取得
            rb = GetComponent<Rigidbody>();

            // 初期化
            isAttacking = false;
            attackCollider.enabled = false;
            dashAttackCollider.enabled = false;
            StatusReset();
            dashEffect.Init();
            jumpEffect.Init();
            attackEffect.Init();
        }

        /// <summary>
        /// ステータスの初期化とスキル入手状態の更新
        /// </summary>
        private void StatusReset()
        {
            // スキル入手状態の更新
            if (PlayerPrefs.GetInt("AttackLevel", 1) == 2)
            {
                // 基本攻撃コライダーをでかく
                attackReach *= attackReachMagnification;
                var pos = attackCollider.transform.position;
                pos.z += attackReachMagnification / 4;
                attackCollider.transform.position = pos;

                // 攻撃力を強く
                damage *= damageMagnicifation;

                // フラグ更新
                isGotAttackSkill = true;
                IsStunable = true;
            }

            if (PlayerPrefs.GetInt("JumpLevel", 1) == 2)
            {
                // ジャンプ力の強化
                jumpForce *= jumpForceMagnification;

                // フラグ更新
                isGotJumpSkill = true;
            }

            if (PlayerPrefs.GetInt("SpeedLevel", 1) == 2)
            {
                // フラグ更新
                isGotSpeedSkill = true;
            }

            // ステータスの初期化
            health = maxHealth;
            attackCollider.transform.localScale = attackReach;
            sprintTimer = sprintSecond;
        }

        private void Start()
        {
            // スタン攻撃クールダウンの初期化
            StageScene.Instance.OnUpdateStrongArmCooldown(stunSkillTimer, stunCooldownTime);
        }

        /// <summary>
        /// 操作不能にする
        /// </summary>
        public void Sleep()
        {
            IsSleeping = true;
        }

        /// <summary>
        /// 操作不能状態から戻す
        /// </summary>
        public void WakeUp()
        {
            IsSleeping = false;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            // 入力を保持
            moveInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started) Jump(jumpForce);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started) Attack();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                StageScene.Instance.TogglePause();
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            // 入力開始時にダッシュ
            if (context.started)
            {
                Sprint();
            }
            // 入力終了時にダッシュ解除
            else if (context.canceled)
            {
                ExitSprint();
            }
        }

        private void Update()
        {
            // スタン攻撃クールダウンの更新と見た目への適応
            if (!IsStunable && isGotAttackSkill)
            {
                stunSkillTimer -= Time.deltaTime;
                if (stunSkillTimer <= 0)
                {
                    stunSkillTimer = 0;
                    IsStunable = true;
                }
                StageScene.Instance.OnUpdateStrongArmCooldown(stunSkillTimer, stunCooldownTime);
            }
        }

        void FixedUpdate()
        {
            // 地面接触判定の更新
            // ここ別scriptに分離したい
            IsGrounded = Physics.Linecast(rb.position + groundCheckStartPoint, rb.position + groundCheckEndPoint, groundLayer);// 地面接地判定を更新

            // 操作不能なら帰れ！
            if (IsSleeping)
            {
                return;
            }

            switch (motionState)
            {
                case MotionState.Stopping:
                    // アニメーション
                    animator.SetFloat(speedID, rb.linearVelocity.magnitude);

                    //移動入力がある場合はステートを変更
                    if (moveInput != Vector2.zero)
                    {
                        // ダッシュ中ならSprinting
                        if (IsSprinting)
                        {
                            motionState = MotionState.Sprinting;

                            // アニメーション
                            animator.SetFloat(speedID, rb.linearVelocity.magnitude);

                            // 移動関数呼び出し
                            Move(moveInput, IsSprinting);
                        }
                        // そうでないならWalking
                        else
                        {
                            motionState = MotionState.Walking;

                            // アニメーション
                            animator.SetFloat(speedID, rb.linearVelocity.magnitude);

                            // 移動関数呼び出し
                            Move(moveInput, IsSprinting);
                        }
                    }
                    // 移動入力がないならこれ
                    else
                    {
                        // ノックバックの適応のためにMoveは呼び出す
                        Move(moveInput, IsSprinting);
                    }
                    break;

                case MotionState.Walking:
                    Move(moveInput, IsSprinting);

                    // アニメーション
                    animator.SetFloat(speedID, rb.linearVelocity.magnitude);

                    break;

                case MotionState.JumpAnticipation:
                    Move(moveInput, IsSprinting);

                    // 地面から離れたらジャンピング状態へ移行
                    if (!IsGrounded)
                    {
                        motionState = MotionState.Jumping;

                        // アニメーション
                        animator.SetTrigger(jumpID);
                    }

                    // ジャンプ予備動作から進行しなくなったら待機状態へ戻る
                    else if (rb.linearVelocity.y < requiredJumpSpeed)
                    {
                        motionState = MotionState.Stopping;
                    }
                    break;

                case MotionState.Jumping:
                    Move(moveInput, IsSprinting);

                    // 地面に着地したら待機状態へ
                    if (IsGrounded)
                    {
                        motionState = MotionState.Stopping;
                    }
                    break;

                case MotionState.Sprinting:
                    Move(moveInput, IsSprinting);

                    // アニメーション
                    animator.SetFloat(speedID, rb.linearVelocity.magnitude);

                    // ダッシュ可能時間を減らす
                    sprintTimer -= Time.fixedDeltaTime;
                    if (sprintTimer <= 0)
                    {
                        sprintTimer = 0;
                        ExitSprint();
                    }
                    break;
            }
        }

        /// <summary>
        /// 入力方向へ移動したりノックバックを適応したり
        /// </summary>
        /// <param name="direction">移動の方向</param>>
        /// <param name="sprint">ダッシュの移動速度かどうか</param>
        public void Move(Vector3 direction, bool sprint)
        {
            Vector3 moveDirection;
            Vector3 rotateDirection;

            // メインカメラの前方と右方向を取得（カメラローカル座標でいうところのz軸とx軸）
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            // カメラのy軸方向を無視して、地面に沿った移動にする
            cameraForward.y = 0;
            cameraRight.y = 0;

            // カメラの向きを考慮したうえで計算
            moveDirection = (cameraForward * direction.y + cameraRight * direction.x).normalized;

            // 回転方向は壁との衝突計算を入れてほしくないので避難
            rotateDirection = moveDirection;

            // Wall Check
            bool isCasted;

            // すべてのoffsetで繰り返す
            // wallCheckは別scriptにして、それを呼び出して一つのtrue or falseを返すようにした方がキレイ
            // 子オブジェクト対応にするともっと良い
            // (親のメソッド呼び出すと自動的に子の物も呼び出されて、子側で一つでもtrueあったらtrueにするように)
            for (int i = 0; i < wallCheckerPos.Length; i++)
            {
                Vector3 offset = transform.forward * wallCheckerPos[i].x + transform.right * wallCheckerPos[i].z;
                offset.y = wallCheckerPos[i].y;

                isCasted = Physics.Raycast(transform.position + offset, moveDirection, out RaycastHit hit, wallCheckerDistance, groundLayer);


                // 一個でもtrueがあったらbreakして
                if (isCasted)
                {
                    // 壁の方向には力を加えないように
                    moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
                    break;
                }
            }

            // ノックバック
            if (isKnockBacking)
            {
                // ノックバックの減衰
                knockBackVelocity = Vector3.Lerp(knockBackVelocity, Vector3.zero, knockBackDecay * Time.fixedDeltaTime);

                // フラグ更新
                if (knockBackVelocity == Vector3.zero)
                {
                    isKnockBacking = false;
                }
            }

            // 移動実行！
            rb.linearVelocity = moveDirection * (sprint ? sprintSpeed : moveSpeed) + new Vector3(0, rb.linearVelocity.y, 0) + knockBackVelocity;


            // Y軸移動は上書きではなく加算なため
            // 一度ノックバックさせたらそれ以降はY軸のノックバック速度をなくす
            if (knockBackVelocity.y != 0)
            {
                knockBackVelocity.y = 0;
            }

            // キャラクターを移動する方向に向かせるための処理
            // 何かしら移動が発生している場合のみ回転させる
            if (rotateDirection != Vector3.zero)
            {
                // 以下お手本スクリプトからの引用

                // Quaternion.LookRotationは、指定された方向（moveDirection）を向くための回転を計算します。
                // moveDirectionはカメラの向きに基づいた移動方向です。
                // つまり、キャラクターが進む方向に合わせてキャラクターの向きを変えるための回転を求めています。
                Quaternion targetRotation = Quaternion.LookRotation(rotateDirection);

                // transform.rotationはキャラクターの現在の回転を表します。
                // Quaternion.Slerpは、現在の回転（transform.rotation）から目標の回転（targetRotation）までを滑らかに補間します。
                // Time.deltaTime * 10fは、補間の速度を決めるためのものです。値が大きいほど速く回転し、小さいほどゆっくり回転します。
                // この補間処理によって、キャラクターは急に向きを変えるのではなく、自然な速度で回転します。
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                // ここまで引用
            }

            // ダッシュ中ならUI更新
            if (IsSprinting)
            {
                StageScene.Instance.ApplySprintGauge(sprintTimer, sprintSecond);
            }
        }

        /// <summary>
        /// ジャンプ
        /// </summary>
        /// <param name="power">ジャンプの強さ</param>
        private void Jump(float power)
        {
            // Move()と違って、入力の関数から直で呼び出されるためIsSleepingのチェックが必要
            if (IsSleeping) return;

            // ジャンプのできるステートなら実行
            if (motionState == MotionState.Walking || motionState == MotionState.Stopping || motionState == MotionState.Sprinting)
            {
                // ジャンプ(速度変更)
                Vector3 velocity = rb.linearVelocity;
                velocity.y = power;
                rb.linearVelocity = velocity;

                // 天井に突っかかって地面から離れなかった場合に起こる問題を回避するため準備ステートに移行
                motionState = MotionState.JumpAnticipation;

                // ジャンプが強化されていたらエフェクト再生
                if (isGotJumpSkill)
                {
                    jumpEffect.Play();
                }
            }
        }

        /// <summary>
        /// 通常の攻撃
        /// </summary>
        private void Attack()
        {
            // Move()と違って、入力の関数から直で呼び出されるためIsSleepingのチェックが必要
            if (IsSleeping) return;

            // 攻撃中なら実行しない
            if (isAttacking) return;

            StartCoroutine(AttackCoroutine());
        }

        /// <summary>
        /// 攻撃のルーティーン
        /// </summary>
        private IEnumerator AttackCoroutine()
        {
            // 攻撃中フラグを立てる
            isAttacking = true;

            // アニメーション
            animator.SetTrigger(attackID);

            AudioPlayer.instance.PlaySE(3);// PlayerClawAttackを再生

            yield return new WaitForSeconds(playerLittleWaitTime);

            attackCollider.enabled = true;

            yield return new WaitForSeconds(playerAttackTime);

            attackCollider.enabled = false;

            yield return new WaitForSeconds(playerWaitTime);

            // フラグオフ
            isAttacking = false;

            // スタン可能だったらオフに
            if (IsStunable)
            {
                IsStunable = false;
                stunSkillTimer = stunCooldownTime;
                attackEffect.Play();
            }
        }

        /// <summary>
        /// ダッシュ開始時に呼び出し
        /// </summary>
        private void Sprint()
        {
            if (!isGotSpeedSkill) return;
            if (IsSleeping) return;

            if (sprintTimer > 0)
            {
                IsSprinting = true;

                // ステート更新
                if (motionState == MotionState.Walking)
                {
                    motionState = MotionState.Sprinting;
                }

                // ダッシュ攻撃コライダーオン
                dashAttackCollider.enabled = true;

                // エフェクト再生
                dashEffect.Play();
            }
        }

        /// <summary>
        /// ダッシュ終了時に呼び出し
        /// </summary>
        private void ExitSprint()
        {
            if (!isGotSpeedSkill) return;

            IsSprinting = false;

            // ステート更新
            if (motionState == MotionState.Sprinting)
            {
                motionState = MotionState.Walking;
            }

            // ダッシュ攻撃コライダーオフ
            dashAttackCollider.enabled = false;

            // エフェクト停止
            dashEffect.Stop();
        }

        /// <summary>
        /// 敵の攻撃が当たったときの処理
        /// </summary>
        /// <param name="enemyPos">攻撃してきた敵の座標</param>
        public void Hit(Vector3 enemyPos)
        {
            if (IsSleeping) return;

            if (isInvincible) return;

            // 無敵時間に突入
            StartCoroutine(EnterInvinsicle());
            // ダメージを食らう
            Damage();

            // ノックバックの方向を計算
            Vector3 diff = transform.position - enemyPos;

            KnockBack(diff, knockBackForce);

            // 被弾アニメーション
            animator.SetTrigger(hitID);
        }

        /// <summary>
        /// 無敵時間突入と解除
        /// </summary>
        private IEnumerator EnterInvinsicle()
        {
            isInvincible = true;
            yield return new WaitForSeconds(invincibleTime);
            isInvincible = false;
        }

        /// <summary>
        /// ダメージを受ける処理
        /// </summary>
        private void Damage()
        {
            health--;

            // UIを更新
            StageScene.Instance.DecreaseHpPlayer(health, maxHealth);

            // エフェクト出現
            GameObject effect = Instantiate(damageEffect);

            // エフェクトのposを指定
            Vector3 effectPos = transform.position;
            effectPos.y += 1.0f;
            effect.transform.position = effectPos;

            // プレイヤーの時はサイズを少し大きく
            effect.transform.localScale *= 2f;

            // healthがなかったら死亡
            if (health <= 0)
            {
                Death();
            }
        }

        /// <summary>
        /// 死亡処理
        /// </summary>
        private void Death()
        {
            health = 0;

            // エフェクトを出現
            GameObject effect = Instantiate(destroyEffect);
            Vector3 effectPos = transform.position;
            effectPos.y += 1.0f;
            effect.transform.position = effectPos;
            Destroy(effect, 5);

            // アニメーション
            animator.SetTrigger(dieID);

            // StageSceneに知らせる
            StageScene.Instance.GameOver();
        }

        /// <summary>
        /// ノックバックを適応
        /// </summary>
        /// <param name="diff">敵と自分のposの差分</param>
        /// <param name="force">ノックバック力</param>
        private void KnockBack(Vector3 diff, float force)
        {
            isKnockBacking = true;

            // 正規化しforceを適応
            knockBackVelocity = diff.normalized * force;

            // Y軸マイナス方向へのノックバックの場合、上へのノックバックへ変更
            // その時すこしノックバック力に変更を加える(想定は少し弱める)
            if (knockBackVelocity.y < 0)
            {
                knockBackVelocity.y = -knockBackVelocity.y * knockBackInvertMultiplyNumber;
            }
        }

        private void OnDrawGizmos()
        {
            // WallCheckerの視覚化
            for (int i = 0; i < wallCheckerPos.Length; i++)
            {
                // 自身の向きを考慮してwallCheckの始点を計算
                Vector3 offset = transform.forward * wallCheckerPos[i].x + transform.right * wallCheckerPos[i].z;

                // Y軸は向きを考慮しなくてよい
                offset.y = wallCheckerPos[i].y;

                Gizmos.color = Color.cyan;

                Gizmos.DrawLine(transform.position + offset, transform.position + offset + transform.forward * wallCheckerDistance);
            }
        }

        [ContextMenu("アタックスキル取得")]
        private void GetAttackSkill()
        {
            PlayerPrefs.SetInt("AttackLevel", 2);
        }

        [ContextMenu("ジャンプスキル取得")]
        private void GetJumpSkill()
        {
            PlayerPrefs.SetInt("JumpLevel", 2);
        }

        [ContextMenu("スピードスキル取得")]
        private void GetSpeedSkill()
        {
            PlayerPrefs.SetInt("SpeedLevel", 2);
        }
    }
}