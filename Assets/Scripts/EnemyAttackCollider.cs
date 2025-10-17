using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    // ProjectSettingsで判定をするレイヤーを制限しているため、関数内で絞る必要はない
    private void OnTriggerEnter(Collider collider)
    {
        // 親のスクリプト持ってくる
        var victimScript = collider.GetComponentInParent<Player>();
        victimScript.TakeDamage();
    }
}