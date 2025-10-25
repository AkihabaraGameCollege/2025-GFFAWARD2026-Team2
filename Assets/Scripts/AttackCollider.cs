using UnityEngine;
using UnityEngine.Events;

public class AttackCollider : MonoBehaviour
{
    // プレイヤーの子オブジェクトにアタッチしている場合はtrue、敵の場合はfalseをいったん消去（中山が編集）
    /*
    [SerializeField]
    private bool isPlayersScript = false;
    */

    // ゲームオーバーエリアに入ったときのイベントを取得または設定します（中山が編集）
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

        // ダメージを与える処理をいったん消去（中山が編集）
        /*
        if (isPlayersScript)
        {
            // プレイヤーの場合は
            // 敵の親スクリプト持ってきて実行
            var victimScript = collider.GetComponentInParent<BossMove>();
            victimScript.TakeDamage();
        }
        else
        {
            // 敵の場合は
            // プレイヤーの親スクリプト持ってきて実行
            var victimScript = collider.GetComponentInParent<Player>();
            victimScript.TakeDamage();
        }
        */
    }
}