using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    // 毎秒の移動速度指定
    [SerializeField]
    private float moveSpeed = 1;

    [SerializeField]
    private float jumpForce = 10;

    [SerializeField]
    private float rotationOffset = 0;

    [SerializeField]
    [Tooltip("ジャンプに必要な速度を指定")]
    private float requiredJumpSpeed = 0.1f;


    [Header("GroundChecker")]
    [SerializeField]
    private Vector3 groundCheckStartPoint = new Vector3(0, -0.5f, 0);
    [SerializeField]
    private Vector3 groundCheckEndPoint = new Vector3(0, -1.5f, 0);

    [Header("攻撃関連")]
    [SerializeField]
    [Tooltip("childを指定")]
    private GameObject attackCollider = null;

    // ポーズUIを指定します。
    [SerializeField]
    private PauseUI pause = null;

    private bool IsGrounded => Physics.Linecast(transform.position + groundCheckStartPoint, transform.position + groundCheckEndPoint);

    // 移動ベクトル保持用
    private Vector2 moveInput;

    new private Rigidbody rigidbody;
    public bool IsSleeping { get; private set; }

    private int health;

    [Header("ステータス")]
    [SerializeField]
    private int maxHealth;

    Animator animator;// Animator コンポーネントの参照（中山が編集）

    //アニメーションID登録（中山が編集）
    static readonly int landingID = Animator.StringToHash("landing");
    static readonly int jumpID = Animator.StringToHash("jump");
    static readonly int attackID = Animator.StringToHash("attack");
    static readonly int speedID = Animator.StringToHash("speed");

    //BossMoveScript登録
    [SerializeField]
    private BossMove bossMove = null;

    enum MotionState
    {
        Stopping,
        Walking,
        JumpAnticipation,
        Jumping,
    }

    MotionState motionState = MotionState.Stopping;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        StatusReset();

        attackCollider.SetActive(false);

        animator = GetComponent<Animator>();// Animator コンポーネントを取得（中山が編集）
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

    // PlayerInputからUnityEventで起動
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
                    Move(moveInput);
                }
                break;
            //移動入力がある場合は移動状態へ移行（中山が編集）
            case MotionState.Walking:
                animator.SetFloat(speedID, rigidbody.linearVelocity.magnitude);// Runアニメーションを継続（中山が編集）
                Move(moveInput);
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

    private void Move(Vector2 input)//input: 入力ベクトル（中山が編集）
    {
        // 移動(速度変更)
        Vector3 velocity = rigidbody.linearVelocity;
        velocity.x = input.x * moveSpeed;// X軸方向の速度設定（中山が編集）
        velocity.z = input.y * moveSpeed;// Z軸方向の速度設定（中山が編集）
        rigidbody.linearVelocity = velocity;

        // 回転（中山が編集）
        if (input != Vector2.zero)
        {
            transform.rotation = Quaternion.Euler(0,
                            (Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg) + rotationOffset, 0);
            // 以下、Unityの機能を使った簡単バージョン(AI頼り)
            //transform.rotation = Quaternion.LookRotation(new Vector3(moveInput.x, 0f, moveInput.y));
        }
    }

    private void Jump(float power)
    {
        Vector3 velocity = rigidbody.linearVelocity;
        velocity.y = power;
        rigidbody.linearVelocity = velocity;

        motionState = MotionState.JumpAnticipation;
        animator.SetTrigger(jumpID);// Jumpアニメーションを開始（中山が編集）
    }

    private void Attack()
    {  
        StartCoroutine(AttackTimer()); //攻撃処理開始（中山が編集）
    }

    //攻撃判定の有効時間、攻撃演出を制御するコルーチン（中山が編集）
    IEnumerator AttackTimer()
    {
        attackCollider.SetActive(true);//攻撃判定を有効化（中山が編集）
        animator.SetTrigger(attackID);// Attackアニメーションを開始（中山が編集）
        yield return new WaitForSeconds(1f);//1秒待機（中山が編集）
        attackCollider.SetActive(false);//攻撃判定を無効化（中山が編集）
        bossMove.TakeDamage(); //ボスにダメージを与える（中山が編集）
    }

    public void TakeDamage()
    {
        health--;
        Debug.Log($"Player TakeDamage{health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("ImDead");
    }
}
