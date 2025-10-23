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

    Animator animator;// Animator �R���|�[�l���g�̎Q�Ɓi���R���ҏW�j

    static readonly int IsWalkingID = Animator.StringToHash("isWalking"); // Animator�p�����[�^�̃n�b�V���l�i���R���ҏW�j
    static readonly int jumpID = Animator.StringToHash("jump"); // Animator�p�����[�^�̃n�b�V���l�i���R���ҏW�j
    static readonly int grandID = Animator.StringToHash("grand"); // Animator�p�����[�^�̃n�b�V���l�i���R���ҏW�j

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        
        animator = GetComponent<Animator>();// �R���|�[�l���g���Q�Ƃ��Ă���(���R���ҏW)
        
        //�s���p�^�[���J�n
        StartCoroutine(Move());

        health = maxHealth;
    }

    //�W�����v
    void Jump()
        {
        animator.SetTrigger(jumpID);// Jump�A�j���[�V�������J�n�i���R���ҏW�j
        //������ɗ͂�������
        rigidbody.AddForce(Vector3.up * jumpP, ForceMode.Impulse);
    }

    private void Walking()
        {
        animator.SetFloat(IsWalkingID, rigidbody.linearVelocity.magnitude);// Walk�A�j���[�V�������J�n�i���R���ҏW�j
      
        rigidbody.AddForce(transform.forward * moveP, ForceMode.Impulse);//�O�����ɗ͂�������
    }

    //�s���p�^�[��
    IEnumerator Move()
    {
        //�������[�v
        while (true)
        {
            animator.SetFloat(IsWalkingID, rigidbody.linearVelocity.magnitude);// Walk�A�j���[�V�������J�n�i���R���ҏW�j
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90, 0), 1f);// �E��]�i���R���ҏW�j
            yield return new WaitForSeconds(3);// 3�b�ҋ@
            Walking();//����
            yield return new WaitForSeconds(5);// 5�b�ҋ@
            rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);//�O�����ւ̗͂�0�ɂ���
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -90, 0), 1f);// ����]�i���R���ҏW�j
            yield return new WaitForSeconds(3);// 3�b�ҋ@
            Jump();//�W�����v
            yield return new WaitForSeconds(5);// 5�b�ҋ@
            animator.SetTrigger(grandID);// Grand�A�j���[�V�������J�n�i���R���ҏW�j
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -90, 0), 1f);// ����]�i���R���ҏW�j
            yield return new WaitForSeconds(3);// 3�b�ҋ@
            Walking();//����
            yield return new WaitForSeconds(5);// 5�b�ҋ@
            rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);//�O�����ւ̗͂�0�ɂ���
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90, 0), 1f);// �E��]�i���R���ҏW�j
            yield return new WaitForSeconds(3);// 5�b�ҋ@
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
