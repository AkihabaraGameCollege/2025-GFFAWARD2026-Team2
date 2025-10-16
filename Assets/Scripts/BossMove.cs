using UnityEngine;
using System.Collections;

public class BossMove : MonoBehaviour
{
    new private Rigidbody rigidbody;

    [SerializeField]
    private float jumpP = 10;
    [SerializeField]
    private float moveP = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //行動パターン開始
        StartCoroutine(Move());
    }

    //ジャンプ
    void Jump()
        {
        //上方向に力を加える
        rigidbody.AddForce(Vector3.up * jumpP, ForceMode.Impulse);
    }

    //行動パターン
    IEnumerator Move()
    {
        //無限ループ
        while (true)
        {
            // 移動(速度変更)
            Vector3 velocity = rigidbody.linearVelocity;
            velocity.x = moveP;
            rigidbody.linearVelocity = velocity;
            //3秒待つ
            yield return new WaitForSeconds(3);
            //ジャンプ
            Jump();
            //3秒待つ
            yield return new WaitForSeconds(3);
            // 移動(速度変更)
            velocity = rigidbody.linearVelocity;
            velocity.x = -moveP;
            rigidbody.linearVelocity = velocity;
            //3秒待つ
            yield return new WaitForSeconds(3);
        }
    }
}
