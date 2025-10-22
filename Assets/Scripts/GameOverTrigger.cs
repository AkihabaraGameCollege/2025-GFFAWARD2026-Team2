using UnityEngine;
using UnityEngine.Events;

public class GameOverTrigger : MonoBehaviour
{
    public UnityEvent OnEnter { get => onEnter; set => onEnter = value; }
    [SerializeField]
    [Tooltip("ゲームオーバーエリアに入ったときのイベント")]
    private UnityEvent onEnter = null;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            onEnter.Invoke();
        }
    }
}
