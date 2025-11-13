using UnityEngine;

public class PlayerAttackColliderN : MonoBehaviour
{
    // プレイヤーの子オブジェクトにアタッチしている場合はtrue、敵の場合はfalse
    [SerializeField]
    private bool isPlayersScript = false;

    // ProjectSettingsで判定をするレイヤーを制限しているため、関数内で絞る必要はない
    private void OnTriggerEnter(Collider collider)
    {
        if (isPlayersScript)
        {
            // プレイヤーの場合は
            // 敵の親スクリプト持ってきて実行
            var victimScript1 = collider.GetComponentInParent<BossMove1>();
            var victimScript2 = collider.GetComponentInParent<BossMove2>();
            var victimScript3 = collider.GetComponentInParent<BossMove3>();
            // victimScript.TakeDamage();
        }
        else
        {
            Debug.Log("EnemyAttackPlpayer");
            // 敵の場合は
            // プレイヤーの親スクリプト持ってきて実行
            var victimScript = collider.GetComponentInParent<Player>();
           // victimScript.TakeDamage();
        }
    }
}