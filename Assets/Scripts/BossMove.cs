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
    [SerializeField]
    private Collider thisCollider;

    [SerializeField]
    private StageScene stageScene = null;

    [Header("Stats")]
    [SerializeField]
    private int maxHealth;
    private int health;

    Animator animator;//アニメーター（中山が編集）

    static readonly int IsWalkingID = Animator.StringToHash("isWalking");
    static readonly int jumpID = Animator.StringToHash("jump");
    static readonly int grandID = Animator.StringToHash("grand");
    static readonly int dieID = Animator.StringToHash("die");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();

        //行動パターン開始（中山が編集）
        StartCoroutine(Move());

        health = maxHealth;
    }

    //ジャンプ攻撃（中山が編集）
    void JumpAttack()
    {
        animator.SetTrigger(jumpID);
        //上に力を加える（中山が編集）
        rigidbody.AddForce(Vector3.up * jumpP, ForceMode.Impulse);
    }

    //歩く（中山が編集）
    private void Walking()
    {
        animator.SetFloat(IsWalkingID, rigidbody.linearVelocity.magnitude);

        rigidbody.AddForce(transform.forward * moveP, ForceMode.Impulse);
    }

    //回転、（）の中に角度を設定（中山が編集）
    private void Turn(float rotate)
    {
        transform.Rotate(0, rotate * Time.deltaTime, 0);
    }

    //行動パターン（中山が編集）
    IEnumerator Move()
    {
        //無限ループ（中山が編集）
        while (true)
        {
            yield return new WaitForSeconds(3);
            Turn(90);
            yield return new WaitForSeconds(3);
            Walking();
            yield return new WaitForSeconds(2);
            rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);//移動停止（中山が編集）
            Turn(-90);
            yield return new WaitForSeconds(6);
            JumpAttack();
           thisCollider.enabled = false;//当たり判定無効化（中山が編集）
            yield return new WaitForSeconds(20);
            //ダメージ処理（中山が編集）
            if (health <= 0)
            {
                Die();
                break;
            }
            else
            {
                animator.SetTrigger(grandID);//地面にハマるアニメーション終了（中山が編集）
                JumpAttack();
                yield return new WaitForSeconds(2);
                thisCollider.enabled = true;//当たり判定有効化（中山が編集）
            }
        }
    }

    public void TakeDamage()
    {
        //ダメージ処理（中山が編集）
        health--;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StartCoroutine(OnDie());//撃破演出開始（中山が編集）
    }

    //撃破演出（中山が編集）
    IEnumerator OnDie()
    {
        animator.SetTrigger(dieID);//死亡アニメーション再生（中山が編集）
        yield return new WaitForSeconds(5);
        rigidbody.isKinematic = true;//物理演算無効化（中山が編集）
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
        yield return new WaitForSeconds(3);
        stageScene.StageClear();//ステージクリア処理（中山が編集）
    }
}
