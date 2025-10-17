using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private void FixedUpdate()
    {
        if (IsSleeping) return;

        switch (motionState)
        {
            case MotionState.Stopping:
                if (moveInput != Vector2.zero)
                {
                    motionState = MotionState.Walking;

                    Move(moveInput);
                }
                break;
            case MotionState.Walking:
                if (moveInput != Vector2.zero)
                {
                    Move(moveInput);
                }
                break;
            case MotionState.JumpAnticipation:
                if (moveInput != Vector2.zero)
                {
                    Move(moveInput);
                }
                if (!IsGrounded)
                {
                    motionState = MotionState.Jumping;
                }
                else if (rigidbody.linearVelocity.y < requiredJumpSpeed)
                {
                    if (moveInput != Vector2.zero)
                    {
                        motionState = MotionState.Walking;
                    }
                    else
                    {
                        motionState = MotionState.Stopping;
                    }
                }
                break;
            case MotionState.Jumping:
                if (moveInput != Vector2.zero)
                {
                    Move(moveInput);
                }
                if (IsGrounded)
                {
                    if (moveInput != Vector2.zero)
                    {
                        motionState = MotionState.Walking;
                    }
                    else
                    {
                        motionState = MotionState.Stopping;
                    }
                }
                break;
        }
    }

    private void Move(Vector2 Input)
    {
        // 移動(速度変更)
        Vector3 velocity = rigidbody.linearVelocity;
        velocity.x = Input.x * moveSpeed;
        velocity.z = Input.y * moveSpeed;
        rigidbody.linearVelocity = velocity;

        // 回転
        transform.rotation = Quaternion.Euler(0,
                        (Mathf.Atan2(Input.x, Input.y) * Mathf.Rad2Deg) + rotationOffset, 0);
        // 以下、Unityの機能を使った簡単バージョン(AI頼り)
        //transform.rotation = Quaternion.LookRotation(new Vector3(moveInput.x, 0f, moveInput.y));
    }

    private void Jump(float power)
    {
        Vector3 velocity = rigidbody.linearVelocity;
        velocity.y = power;
        rigidbody.linearVelocity = velocity;

        motionState = MotionState.JumpAnticipation;
    }

    private void Attack()
    {
        attackCollider.SetActive(true);
        StartCoroutine(AttackTimer());

        Debug.Log("Attack");
    }

    IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(0.5f);

        attackCollider.SetActive(false);
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
