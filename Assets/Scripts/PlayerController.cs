using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QuickTheFury
{
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
        private float stunCooldownTime;

        // 地面判定用の線分の始点と終点を指定（中山が編集）
        [SerializeField]
        private Vector3 groundCheckStartPoint = new Vector3(0, 0.5f, 0);
        [SerializeField]
        private Vector3 groundCheckEndPoint = new Vector3(0, -0.5f, 0);
        // ジャンプに必要な速度指定（中山が編集）
        [SerializeField]
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
        Animator animator;// Animator コンポーネントの参照（中山が編集）

        // 地面判定に使用するレイヤーを指定（中山が編集）
        [SerializeField]
        LayerMask groundLayer;

        // 攻撃判定用のコライダーを指定（中山が編集）
        [SerializeField]
        private Collider attackCollider = null;

        // プレイヤーの手のオブジェクトを指定（中山が編集）
        [SerializeField]
        private GameObject playerHand;

        public GameObject PlayerHand { get { return playerHand; } }// プレイヤーの手のオブジェクト取得用プロパティ（中山が編集）

        // ダッシュアタック用のコライダーを指定 (富里が編集)
        [SerializeField]
        private Collider dashAttackCollider = null;

        [SerializeField]
        [Tooltip("攻撃エフェクト")]
        GameObject destroyEffect;

        [SerializeField]
        [Tooltip("被弾エフェクト")]
        GameObject damageEffect;

        // パーティクルの配列
        [Serializable]
        private struct Particles
        {
            public ParticleSystem[] attack;// 攻撃エフェクトの配列
            public ParticleSystem[] jump;// ジャンプエフェクトの配列
            public ParticleSystem[] dashAttack;// ダッシュエフェクトの配列
        }

        // 攻撃エフェクト参照用
        [SerializeField]
        private Particles particles;

        [Header("その他")]
        [SerializeField]
        [Tooltip("Offset")]
        private Vector3[] wallCheckerPos = null;
        [SerializeField]
        [Tooltip("Distance")]
        private float wallCheckerDistance = 0;

        private Vector2 moveInput;// 移動入力ベクトルを移植（中山が編集）

        new private Rigidbody rigidbody;// Rigidbody コンポーネントの参照

        private bool IsGrounded = false;// 地面に接地しているかどうか

        public bool IsSleeping { get; private set; }// 眠っているかどうか

        private bool attackOK = false;// 攻撃制限変数（中山が編集）

        private int health;// プレイヤーの体力

        private bool isInvincible = false; //無敵状態かどうか(富里が編集)

        public bool IsSprinting { get; private set; } = false;

        private float sprintTimer;

        private float stunSkillTimer = 0;
        public bool IsStunable { get; private set; } = false;

        private bool isGotAttackSkill = false;
        private bool isGotSpeedSkill = false;

        private Vector3 knockBackVelocity = Vector3.zero;
        private bool isKnockBacking = false;

        [SerializeField]
        private float stunSkillTime = 5;

        public float StunSkillTime { get { return stunSkillTime; } }

        Collider thiscollider;
        PlayerController playerScript;

        //アニメーションID登録（中山が編集）
        static readonly int jumpID = Animator.StringToHash("jump");
        static readonly int attackID = Animator.StringToHash("attack");
        static readonly int speedID = Animator.StringToHash("speed");
        static readonly int hitID = Animator.StringToHash("hit");
        static readonly int dieID = Animator.StringToHash("die");



        // モーション状態定義（中山が編集）
        enum MotionState
        {
            Stopping,
            Walking,
            JumpAnticipation,
            Jumping,
            Sprinting
        }
        MotionState motionState = MotionState.Stopping;// 現在のモーション状態（中山が編集）

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();// Rigidbody コンポーネントを取得
            thiscollider = GetComponent<Collider>();
            playerScript = GetComponentInParent<PlayerController>();

            attackOK = true;// 攻撃制限変数初期化（中山が編集）
            attackCollider.enabled = false;// 攻撃判定を無効化（中山が編集）
            dashAttackCollider.enabled = false; // ダッシュアタック判定を無効化 (富里が編集)
            StatusReset();// ステータス初期化（中山が編集）

            // 全てのエフェクトを停止
            foreach (var attack in particles.attack)
            {
                attack.Stop();
            }
            foreach (var jump in particles.jump)
            {
                jump.Stop();
            }
            foreach (var dash in particles.dashAttack)
            {
                dash.Stop();
            }
        }

        private void StatusReset()
        {

            if (PlayerPrefs.GetInt("AttackLevel", 1) == 2)
            {
                attackReach *= attackReachMagnification;
                var pos = attackCollider.transform.position;
                pos.z += attackReachMagnification / 4;
                attackCollider.transform.position = pos;

                damage *= damageMagnicifation;

                isGotAttackSkill = true;
                IsStunable = true;
            }

            if (PlayerPrefs.GetInt("JumpLevel", 1) == 2)
            {
                jumpForce *= jumpForceMagnification;
            }

            if (PlayerPrefs.GetInt("SpeedLevel", 1) == 2)
            {
                isGotSpeedSkill = true;
            }

            health = maxHealth;
            attackCollider.transform.localScale = attackReach;
            sprintTimer = sprintSecond;
        }

        private void Start()
        {
            MainStageScene.Instance.OnUpdateStrongArmCooldown(stunSkillTimer, stunCooldownTime);
        }

        public void Sleep()
        {
            IsSleeping = true;
        }
        public void WakeUp()
        {
            IsSleeping = false;
        }

        // Move アクションによって呼び出されるプログラムを移植（中山が編集）
        public void OnMove(InputAction.CallbackContext context)
        {
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

        // Pause アクションが発生した際に呼び出されます。
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                MainStageScene.Instance.TogglePause();
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Sprint();
            }
            else if (context.canceled)
            {
                ExitSprint();
            }
        }
        private void Update()
        {
            if (!IsStunable && isGotAttackSkill)
            {
                stunSkillTimer -= Time.deltaTime;
                if (stunSkillTimer <= 0)
                {
                    stunSkillTimer = 0;
                    IsStunable = true;
                }
                MainStageScene.Instance.OnUpdateStrongArmCooldown(stunSkillTimer, stunCooldownTime);
            }
        }

        // 固定フレームレートで呼び出される更新処理を移植（中山が編集）
        void FixedUpdate()
        {
            IsGrounded = Physics.Linecast(rigidbody.position + groundCheckStartPoint, rigidbody.position + groundCheckEndPoint, groundLayer);// 地面接地判定を更新

            if (IsSleeping) return;

            switch (motionState)
            {
                case MotionState.Stopping:
                    animator.SetFloat(speedID, rigidbody.linearVelocity.magnitude);// Runアニメーションを継続（中山が編集）
                                                                                   //移動入力がある場合は移動状態へ移行（中山が編集）
                    if (moveInput != Vector2.zero)
                    {
                        if (IsSprinting)
                        {
                            motionState = MotionState.Sprinting;
                            // カエルかも
                            animator.SetFloat(speedID, rigidbody.linearVelocity.magnitude);// Runアニメーションを開始（中山が編集）
                            Move(IsSprinting, true);// カメラに準じた移動を呼び出し（富里が編集）
                        }
                        else
                        {
                            motionState = MotionState.Walking;
                            animator.SetFloat(speedID, rigidbody.linearVelocity.magnitude);// Runアニメーションを開始（中山が編集）
                            Move(IsSprinting, true);// カメラに準じた移動を呼び出し（富里が編集）
                        }
                    }
                    else
                    {
                        Move(IsSprinting, false);
                    }
                    break;
                //移動入力がある場合は移動状態へ移行（中山が編集）
                case MotionState.Walking:
                    Move(IsSprinting, true);// カメラに準じた移動を呼び出し（富里が編集）
                    animator.SetFloat(speedID, rigidbody.linearVelocity.magnitude);// Runアニメーションを継続（中山が編集）
                    break;
                //移動入力がなくなったら停止状態へ移行（中山が編集）
                case MotionState.JumpAnticipation:
                    Move(IsSprinting, true);// カメラに準じた移動を呼び出し（富里が編集）
                                            //地面から離れたらジャンピング状態へ移行（中山が編集）
                    if (!IsGrounded)
                    {
                        motionState = MotionState.Jumping;
                        animator.SetTrigger(jumpID);// Jumpアニメーションを開始（中山が編集）
                    }
                    //ジャンプ予備動作から進行しなくなったら待機状態へ戻る（中山が編集）
                    else if (rigidbody.linearVelocity.y < requiredJumpSpeed)
                    {
                        motionState = MotionState.Stopping;
                    }
                    break;
                case MotionState.Jumping:
                    //地面に着地した判定（中山が編集）
                    Move(IsSprinting, true);// カメラに準じた移動を呼び出し（富里が編集）
                    if (IsGrounded)
                    {
                        motionState = MotionState.Stopping;
                    }
                    break;
                case MotionState.Sprinting:
                    Move(IsSprinting, true);// カメラに準じた移動を呼び出し（富里が編集）
                    animator.SetFloat(speedID, rigidbody.linearVelocity.magnitude);// Runアニメーションを継続（中山が編集）
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
        /// 指定した速度で、このキャラクターを移動させるクラス
        /// </summary>
        /// <param name="isSprint"></param>
        /// <param name="isEnableInputMove"></param>
        public void Move(bool isSprint, bool isEnableInputMove)
        {
            // もしメインカメラが存在する場合
            if (Camera.main != null)
            {
                // 方向に関する情報の変数を追加する
                Vector3 moveDirection = Vector3.zero;// 移動方向を初期化
                Vector3 rotateDirection = Vector3.zero;// 視点方向を初期化

                // もし移動可能の場合
                if (isEnableInputMove)
                {
                    // メインカメラの前方と右方向を取得
                    Vector3 cameraForward = Camera.main.transform.forward;// カメラローカル座標でいうＺ軸を取得
                    Vector3 cameraRight = Camera.main.transform.right;// カメラローカル座標でいうＸ軸を取得

                    // カメラのy軸方向を無視して、地面に沿った移動にする
                    cameraForward.y = 0;// Ｚ軸におけるY軸の方向は０にしておく
                    cameraRight.y = 0;// Ｘ軸におけるY軸の方向は０にしておく

                    moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;// 正規化して移動方向ベクトルを計算

                    rotateDirection = moveDirection;// 移動方向と視点方向を同期する

                    // Wall Check
                    bool isCasted = false;

                    // すべてのoffsetで繰り返す
                    for (int i = 0; i < wallCheckerPos.Length; i++)
                    {
                        Vector3 offset = transform.forward * wallCheckerPos[i].x + transform.right * wallCheckerPos[i].z;
                        offset.y = wallCheckerPos[i].y;

                        isCasted = Physics.Raycast(transform.position + offset, moveDirection, out RaycastHit hit, wallCheckerDistance, groundLayer);


                        // 一個でもtrueがあったらbreakして
                        if (isCasted)
                        {
                            moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
                            break;
                        }
                    }
                }

                // もし視点方向が０ではない場合
                if (rotateDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(rotateDirection);// キャラクターが進む方向に合わせてキャラクターの向きを変えるための回転を求める

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);// この補間処理によって、キャラクターは急に向きを変えるのではなく、自然な速度で回転します。
                }

                if (isKnockBacking)
                {
                    knockBackVelocity = Vector3.Lerp(knockBackVelocity, Vector3.zero, knockBackDecay * Time.fixedDeltaTime);
                    if (knockBackVelocity == Vector3.zero)
                    {
                        isKnockBacking = false;
                    }
                }

                // 移動実行！
                rigidbody.linearVelocity = moveDirection * ((isSprint) ? sprintSpeed : moveSpeed) + new Vector3(0, rigidbody.linearVelocity.y, 0) + knockBackVelocity;//移動ベクトルを速度に設定

                // Y軸移動は上書きではなく加算なため
                // 一度ノックバックさせたらそれ以降はY軸のノックバック速度をなくす
                if (knockBackVelocity.y != 0)
                {
                    knockBackVelocity.y = 0;
                }
            }

            if (IsSprinting)
            {
                MainStageScene.Instance.ApplySprintGauge(sprintTimer, sprintSecond);
            }
        }

        // ジャンプ処理（中山が編集）
        private void Jump(float power)
        {
            if (IsSleeping) return;

            if (motionState == MotionState.Walking || motionState == MotionState.Stopping || motionState == MotionState.Sprinting)
            {
                // ジャンプ(速度変更)
                Vector3 velocity = rigidbody.linearVelocity;
                velocity.y = power;
                rigidbody.linearVelocity = velocity;

                motionState = MotionState.JumpAnticipation;// ジャンプ予備動作状態へ移行（中山が編集）

                // PlayerPrefsのJumpLevelが2の場合、ジャンプエフェクトを再生（中山が編集）
                if (PlayerPrefs.GetInt("JumpLevel", 1) == 2)
                {
                    // ジャンプエフェクト再生
                    foreach (var jump in particles.jump)
                    {
                        jump.Play();
                    }
                }
            }
        }

        //攻撃処理（中山が編集）
        private void Attack()
        {
            if (IsSleeping) return;
            //攻撃制限変数判定（中山が編集）
            if (attackOK)
            {
                StartCoroutine(AttackTimer()); //攻撃処理開始（中山が編集）
            }
            return;//攻撃制限変数判定終了（中山が編集）
        }

        //攻撃判定の有効時間、攻撃演出を制御するコルーチン（中山が編集）
        IEnumerator AttackTimer()
        {
            attackOK = false;//攻撃制限変数をfalseに設定（中山が編集）
            animator.SetTrigger(attackID);// Attackアニメーションを開始（中山が編集）
            AudioPlayer.Instance.PlaySE(3);// PlayerClawAttackを再生 (富里が編集)

            // PlayerPrefsのAttackLevelが2かつスタン可能の場合、攻撃エフェクトを再生（中山が編集）
            if (PlayerPrefs.GetInt("AttackLevel", 1) == 2 && IsStunable)
            {
                // 攻撃エフェクト再生
                foreach (var attack in particles.attack)
                {
                    attack.Play();
                }
            }

            yield return new WaitForSeconds(playerLittleWaitTime);//playerLittleWaitTime秒待機（中山が編集）
            attackCollider.enabled = true;//攻撃判定を有効化（中山が編集）
            yield return new WaitForSeconds(playerAttackTime);//1秒待機（中山が編集）
            attackCollider.enabled = false;//攻撃判定を無効化（中山が編集）
            yield return new WaitForSeconds(playerWaitTime);//playerWaitTime秒待機（中山が編集）
            attackOK = true;//攻撃制限変数をtrueに設定（中山が編集）
            if (IsStunable)
            {
                IsStunable = false;
                stunSkillTimer = stunCooldownTime;
            }
        }


        private void Sprint()
        {
            if (!isGotSpeedSkill) return;
            if (IsSleeping) return;

            if (sprintTimer > 0)
            {
                IsSprinting = true;
                if (motionState == MotionState.Walking)
                {
                    motionState = MotionState.Sprinting;
                }
                dashAttackCollider.enabled = true;

                // ダッシュエフェクト再生
                foreach (var dash in particles.dashAttack)
                {
                    dash.Play();
                }
            }

            //if ((motionState == MotionState.Stopping || motionState == MotionState.Walking) &&
            //    PlayerPrefs.GetInt("AttackLevel", 1) == 2)
            //{
            //    motionState = MotionState.Sprinting; // MotionState更新 (富里が編集)
            //    // DashAttack用なためコメントアウト
            //    //animator.SetTrigger(dashAttackID); // アニメーター起動 (富里が編集)
            //}
        }

        private void ExitSprint()
        {
            if (!isGotSpeedSkill) return;


            IsSprinting = false;
            if (motionState == MotionState.Sprinting)
            {
                motionState = MotionState.Walking;
            }
            dashAttackCollider.enabled = false;

            // ダッシュエフェクト停止
            foreach (var dash in particles.dashAttack)
            {
                dash.Stop();
            }
        }

        public void Hit(Vector3 enemyPos)
        {
            if (IsSleeping) return;

            if (!isInvincible)
            {
                StartCoroutine(EnterInvinsicle());
                Damage();

                // KnockBack
                Vector3 diff = transform.position - enemyPos;

                KnockBack(diff, knockBackForce);

                // anim
                animator.SetTrigger(hitID);
            }
        }

        private IEnumerator EnterInvinsicle()
        {
            isInvincible = true;
            yield return new WaitForSeconds(invincibleTime);
            isInvincible = false;
        }

        private void Damage()
        {
            // HPを減少させ、ダメージエフェクトを発生させる
            health--;

            MainStageScene.Instance.DecreaseHpPlayer(health, maxHealth);//HPゲージを減少させる（中山が編集）

            // エフェクトをインスタンス化
            GameObject effect = Instantiate(damageEffect);

            // 現在の位置を取得し、Vector3型の変数に格納
            Vector3 effectPos = transform.position;

            // エフェクトの位置を少し上に調整
            effectPos.y += 1.0f;

            // エフェクトの位置を設定
            effect.transform.position = effectPos;

            // エフェクトのサイズを少し大きくする（中山が編集）
            effect.transform.localScale *= 2f;


            if (health <= 0)
            {
                DestoryMainObject();
            }
        }

        private void DestoryMainObject()
        {
            // 破壊エフェクトを発生させてから、MainObjectに設定したもの（自分自身や部位破壊対象）を破壊
            health = 0;
            // エフェクトをインスタンス化
            GameObject effect = Instantiate(destroyEffect);

            // 現在の位置を取得し、Vector3型の変数に格納
            Vector3 effectPos = transform.position;

            // エフェクトの位置を少し上に調整
            effectPos.y += 1.0f;

            // エフェクトの位置を設定
            effect.transform.position = effectPos;
            Destroy(effect, 5);

            Die();
        }

        private void Die()
        {
            animator.SetTrigger(dieID);// Dieアニメーションを開始（中山が編集）
            MainStageScene.Instance.GameOver(); // ゲームオーバー処理を呼び出す
        }

        private void KnockBack(Vector3 diff, float force)
        {
            knockBackVelocity = diff.normalized * force;
            if (knockBackVelocity.y < 0)
            {
                knockBackVelocity.y = -knockBackVelocity.y * knockBackInvertMultiplyNumber;
            }
            isKnockBacking = true;
        }

        private void OnDrawGizmos()
        {
            Vector3 rushDirection = transform.forward;
            for (int i = 0; i < wallCheckerPos.Length; i++)
            {
                Vector3 offset = transform.forward * wallCheckerPos[i].x + transform.right * wallCheckerPos[i].z;
                offset.y = wallCheckerPos[i].y;

                Gizmos.color = Color.cyan;

                Gizmos.DrawLine(transform.position + offset, transform.position + offset + rushDirection * wallCheckerDistance);
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

        public void Hit()
        {
            thiscollider.enabled = false;
            StartCoroutine(OnHit());
        }

        private IEnumerator OnHit()
        {
            yield return new WaitForSeconds(1);

            if (playerScript.IsSprinting)
            {
                thiscollider.enabled = true;
            }
        }
    }
}