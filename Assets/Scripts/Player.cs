using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    //移動速度
    [SerializeField]
    private float moveSpeed = 5f;
    // ジャンプ力指定
    [SerializeField]
    private float jumpForce = 10;
    // 回転オフセット指定
    [SerializeField]
    private float rotationOffset = 0;
    // ジャンプに必要な速度指定
    [SerializeField]
    [Tooltip("ジャンプに必要な速度を指定")]
    private float requiredJumpSpeed = 0.1f;

    // 地面判定用の線分の始点と終点を指定
    [Header("GroundChecker")]
    [SerializeField]
    private Vector3 groundCheckStartPoint = new Vector3(0, -0.5f, 0);
    [SerializeField]
    private Vector3 groundCheckEndPoint = new Vector3(0, -1.5f, 0);

    // 攻撃判定用のコライダーを指定
    [Header("攻撃関連")]
    [SerializeField]
    [Tooltip("childを指定")]
    private GameObject attackCollider = null;

    // ポーズUIを指定します。
    [SerializeField]
    private PauseUI pause = null;

    private bool IsGrounded => Physics.Linecast(transform.position + groundCheckStartPoint, transform.position + groundCheckEndPoint);// 地面接地判定

    private Vector2 moveInput;// 移動入力ベクトル

    new private Rigidbody rigidbody;// Rigidbody コンポーネントの参照

    public bool IsSleeping { get; private set; }// 眠っているかどうか

    private int health;// プレイヤーの体力

    //ステータス設定
    [Header("ステータス")]
    [SerializeField]
    private int maxHealth;

    Animator animator;// Animator コンポーネントの参照（中山が編集）

    //アニメーションID登録（中山が編集）
    static readonly int landingID = Animator.StringToHash("landing");
    static readonly int jumpID = Animator.StringToHash("jump");
    static readonly int attackID = Animator.StringToHash("attack");
    static readonly int speedID = Animator.StringToHash("speed");
    static readonly int hitID = Animator.StringToHash("hit");
    static readonly int dieID = Animator.StringToHash("die");

    //BossMoveScript登録
    [SerializeField]
    private BossMove bossMove = null;
    // ゲームオーバーUI登録
    [SerializeField]
    private GameOverUI gameOverUI = null;

    // エフェクト再生用の AudioSource を指定します。（中山が編集）
    [SerializeField]
    private AudioSource effectAudio = null;
    // ジャンプ時のサウンドを指定します。（中山が編集）
    [SerializeField]
    private AudioClip soundOnAttack = null;

    /*
    //ダメージコライダー登録をいったん消去（中山が編集）
    [SerializeField]
    private Collider damageCollision = null;
    */

    // モーション状態定義（中山が編集）
    enum MotionState
    {
        Stopping,
        Walking,
        JumpAnticipation,
        Jumping,
    }
    MotionState motionState = MotionState.Stopping;// 現在のモーション状態（中山が編集）

    private void Start()
    {
        //damageCollision = GetComponent<Collider>();// ダメージコライダーを取得をいったん消去（中山が編集）
        rigidbody = GetComponent<Rigidbody>();// Rigidbody コンポーネントを取得
        animator = GetComponent<Animator>();// Animator コンポーネントを取得（中山が編集）

        attackCollider.SetActive(false);// 攻撃判定を無効化（中山が編集）
        StatusReset();// ステータス初期化（中山が編集）
    }

    private void StatusReset()
    {
        health = maxHealth;
    }

    public void Sleep()
    {
        IsSleeping = true;
    }
    public void WakeUp()
    {
        IsSleeping = false;
    }

    // Move アクションによって呼び出されます。
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsSleeping && context.started && (motionState == MotionState.Stopping || motionState == MotionState.Walking)) Jump(jumpForce);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!IsSleeping && context.started) Attack();
    }

    // Pause アクションが発生した際に呼び出されます。
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StageScene.Instance.TogglePause();
            pause.Show();
        }
    }

    void Update() //モーション状態に応じた処理（中山が編集）
    {
        if (IsSleeping) return;

        switch (motionState)
        {
            case MotionState.Stopping:
                //移動入力がある場合は移動状態へ移行（中山が編集）
                if (moveInput != Vector2.zero)
                {
                    motionState = MotionState.Walking;
                    animator.SetFloat(speedID, rigidbody.linearVelocity.magnitude);// Runアニメーションを開始（中山が編集）
                    //Move(moveInput);//カメラに準じた移動ができないため削除（中山が編集）
                }
                break;
            //移動入力がある場合は移動状態へ移行（中山が編集）
            case MotionState.Walking:
                animator.SetFloat(speedID, rigidbody.linearVelocity.magnitude);// Runアニメーションを継続（中山が編集）
                //Move(moveInput);//カメラに準じた移動ができないため削除（中山が編集）
                break;
            //移動入力がなくなったら停止状態へ移行（中山が編集）
            case MotionState.JumpAnticipation:
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
                    animator.SetTrigger(landingID);// Jumpアニメーションを終了（中山が編集）
                }
                break;
            case MotionState.Jumping:
                //地面に着地した判定（中山が編集）
                if (IsGrounded)
                {
                    motionState = MotionState.Stopping;
                    animator.SetTrigger(landingID);// Jumpアニメーションを終了（中山が編集）
                }
                break;
        }
    }

    // 固定フレームレートで呼び出される更新処理（中山が編集）
    void FixedUpdate()
    {
        Move();// 移動処理呼び出し（中山が編集）
    }

    // 指定した速度で、このキャラクターを移動させます。
    public void Move()
    {
        // メインカメラが存在する場合のみ処理を行う
        if (Camera.main != null)
        {
            // メインカメラの前方と右方向を取得（カメラローカル座標でいうところのz軸とx軸）
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            // カメラのy軸方向を無視して、地面に沿った移動にする
            cameraForward.y = 0;
            cameraRight.y = 0;

            Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;//正規化して移動方向ベクトルを計算
            rigidbody.linearVelocity = moveDirection * moveSpeed + new Vector3(0, rigidbody.linearVelocity.y, 0);//移動ベクトルを速度に設定


            // キャラクターを移動する方向に向かせるための処理
            if (moveDirection != Vector3.zero)  // 何かしら移動が発生している場合のみ回転させる
            {
                // Quaternion.LookRotationは、指定された方向（moveDirection）を向くための回転を計算します。
                // moveDirectionはカメラの向きに基づいた移動方向です。
                // つまり、キャラクターが進む方向に合わせてキャラクターの向きを変えるための回転を求めています。
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                // transform.rotationはキャラクターの現在の回転を表します。
                // Quaternion.Slerpは、現在の回転（transform.rotation）から目標の回転（targetRotation）までを滑らかに補間します。
                // Time.deltaTime * 10fは、補間の速度を決めるためのものです。値が大きいほど速く回転し、小さいほどゆっくり回転します。
                // この補間処理によって、キャラクターは急に向きを変えるのではなく、自然な速度で回転します。
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }

    //カメラに準じた移動ができないため削除（中山が編集）
    /*
     private void Move(Vector2 input)//input: 入力ベクトル（中山が編集）
    {
        Vector3 velocity = rigidbody.linearVelocity;// 現在の速度取得（中山が編集）
        velocity.x = input.x * moveSpeed;// X軸方向の速度設定（中山が編集）
        velocity.z = input.y * moveSpeed;// Z軸方向の速度設定（中山が編集）
        rigidbody.linearVelocity = velocity;// 速度変更（中山が編集）

        // 回転（中山が編集）
        if (input != Vector2.zero)
        {
            transform.rotation = Quaternion.Euler(0,(Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg) + rotationOffset, 0);// Y軸回転（中山が編集）
            //transform.rotation = Quaternion.LookRotation(new Vector3(moveInput.x, 0f, moveInput.y));// 以下、Unityの機能を使った簡単バージョン(AI頼り)
        }
    }
    */

    // ジャンプ処理（中山が編集）
    private void Jump(float power)
    {
        // ジャンプ(速度変更)
        Vector3 velocity = rigidbody.linearVelocity;
        velocity.y = power;
        rigidbody.linearVelocity = velocity;

        motionState = MotionState.JumpAnticipation;// ジャンプ予備動作状態へ移行（中山が編集）
        animator.SetTrigger(jumpID);// Jumpアニメーションを開始（中山が編集）
    }

    //攻撃処理（中山が編集）
    private void Attack()
    {  
        StartCoroutine(AttackTimer()); //攻撃処理開始（中山が編集）
    }

    //攻撃判定の有効時間、攻撃演出を制御するコルーチン（中山が編集）
    IEnumerator AttackTimer()
    {
        effectAudio.PlayOneShot(soundOnAttack);// 攻撃音再生（中山が編集）
        attackCollider.SetActive(true);//攻撃判定を有効化（中山が編集）
        animator.SetTrigger(attackID);// Attackアニメーションを開始（中山が編集）
        yield return new WaitForSeconds(1f);//1秒待機（中山が編集）
        attackCollider.SetActive(false);//攻撃判定を無効化（中山が編集）
    }

    //ダメージ処理（中山が編集）
    public void TakeDamage()
    {
        animator.SetTrigger(hitID);// Hitアニメーションを開始（中山が編集）

        health--;//体力を1減らす（中山が編集）

        //体力が0以下になったら死亡処理を呼び出す（中山が編集）
        if (health <= 0)
        {
            Die();//死亡処理を呼び出す（中山が編集）
        }
    }

    //死亡処理（中山が編集）
    private void Die()
    {
        animator.SetTrigger(dieID);// Dieアニメーションを開始（中山が編集）
        this.enabled = false; // Player スクリプトを無効化
        gameOverUI.Show(); // ゲームオーバーUIを表示
    }
}