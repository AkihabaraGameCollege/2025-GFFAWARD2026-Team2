using UnityEngine;
using UnityEngine.Events;

public class AttackCollider : MonoBehaviour
{
    public UnityEvent OnEnter { get => onEnter; set => onEnter = value; }
    [SerializeField]
    [Tooltip("アタックエリアに入ったときのイベント")]
    private UnityEvent onEnter = null;

    // 判定対象のタグを指定します（中山が編集）
    [SerializeField]
    private string targetTag = "Player";

    // ProjectSettingsで判定をするレイヤーを制限しているため、関数内で絞る必要はない（中山が編集）
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(targetTag))
        {
            Debug.Log("AttackCollider: OnTriggerEnter");
            onEnter.Invoke();
        }
    }
}