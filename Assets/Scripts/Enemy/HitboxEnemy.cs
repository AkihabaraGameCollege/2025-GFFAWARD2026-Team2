using QuickTheFury.Player;
using System;
using UnityEngine;

namespace QuickTheFury.Enemy
{
    /// <summary>
    /// 敵の被ダメージEventを送る
    /// </summary>
    public class HitboxEnemy : MonoBehaviour
    {
        // 注意: このColliderが衝突する条件は、Unityの「Project Settings」->「Physics」の
        //       「Layer Collision Matrix」で設定されています。
        //       例: 「EnemyAttack」レイヤーは「PlayerHitbox」レイヤーのみ衝突が許可されている必要があります。

        public event Action<int, bool> OnHit;

        private void OnTriggerEnter(Collider other)
        {
            // 衝突判定について：
            // 物理エンジンレベルでの事前フィルタリングは、レイヤー設定で行われています。(Enemy同士がぶつからないように、等)
            // HitboxとAttackは、敵対関係のある相手にのみ当たり判定がある状態でこのスクリプトは機能します。
            // もし意図しない衝突判定が起きた場合は最初にレイヤーマスクを確認してください。

            // 被弾側のStatusManagerに通知
            PlayerController playerScript = other.GetComponentInParent<PlayerController>();
            int dam = playerScript.damage;

            // ダッシュアタックかどうかを検知
            DashAttackCollider colliderScript = other.GetComponent<DashAttackCollider>();


            if (colliderScript != null)
            {
                // ダッシュアタックならダッシュアタックコライダー側にヒットを送る
                colliderScript.Hit();
                // かつスタンを強制オフ
                OnHit?.Invoke(dam, false);
            }
            else
            {
                // ダッシュアタックでないなら通常攻撃
                OnHit?.Invoke(dam, playerScript.IsStunable);
            }
        }
    }
}