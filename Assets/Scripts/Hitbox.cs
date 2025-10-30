using UnityEngine;

public class Hitbox : MonoBehaviour
{
    // 注意: このColliderが衝突する条件は、Unityの「Project Settings」->「Physics」の
    //       「Layer Collision Matrix」で設定されています。
    //       例: 「EnemyAttack」レイヤーは「PlayerHitbox」レイヤーのみ衝突が許可されている必要があります。

    private StatusManager receiverStatus; // ダメージを受ける側のStatusManager

    void Start()
    {
        // 自身の親オブジェクトからStatusManagerを取得
        receiverStatus = GetComponentInParent<StatusManager>();

        if (receiverStatus == null)
        {
            Debug.LogError("Hitboxの親にStatusManagerが見つかりません。");
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 衝突判定について：
        // 物理エンジンレベルでの事前フィルタリングは、レイヤー設定で行われています。(Enemy同士がぶつからないように、等)
        // HitboxとAttackは、敵対関係のある相手にのみ当たり判定がある状態でこのスクリプトは機能します。
        // もし意図しない衝突判定が起きた場合は最初にレイヤーマスクを確認してください。

        // 被弾側のStatusManagerに通知
        receiverStatus.Damage();

    }
}
