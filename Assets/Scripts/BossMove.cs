using UnityEngine;
using System.Collections;

public class BossMove : MonoBehaviour
{
    new private Rigidbody rigidbody;

    [SerializeField]
    private float jumpP = 10;
    [SerializeField]
    private float moveP = 3;

    [Header("Collider")]
    [SerializeField]
    private Collider attackCollider;
    [SerializeField]
    private Collider damageArea;

    [Header("Stats")]
    
    [SerializeField]
    private int maxHealth;
    private int health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //行動パターン開始
        StartCoroutine(Move());

        health = maxHealth;
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

    public void TakeDamage()
    {
        health--;
        Debug.Log($"Enemy TakeDamage{health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
