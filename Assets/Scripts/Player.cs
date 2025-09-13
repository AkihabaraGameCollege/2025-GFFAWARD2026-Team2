using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 毎秒の移動速度指定
    [SerializeField]
    private float moveSpeed = 1;

    [SerializeField]
    private bool isEnableSense = true;

    // 移動ベクトル保持用
    private Vector2 moveInput;

    new private Rigidbody rigidbody;
    public bool IsSleeping { get; private set; }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
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

    private void FixedUpdate()
    {
        if (IsSleeping) return;

        if (moveInput != Vector2.zero)
        {
            Move(moveInput);

            transform.rotation = Quaternion.Euler(0, 
                (Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg), 0);

            // 以下、Unityの機能を使った簡単バージョン(AI頼り)
            //transform.rotation = Quaternion.LookRotation(new Vector3(moveInput.x, 0f, moveInput.y));
        }
        else if (!isEnableSense)
        {
            Move(moveInput);
        }
    }

    private void Move(Vector2 Input)
    {
        Vector3 velocity = rigidbody.linearVelocity;
        velocity.x = Input.x * moveSpeed;
        velocity.z = Input.y * moveSpeed;
        rigidbody.linearVelocity = velocity;
    }
}
