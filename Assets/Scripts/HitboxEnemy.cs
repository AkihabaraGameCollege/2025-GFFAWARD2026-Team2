using System;
using UnityEngine;

namespace QuickTheFury
{
    /// <summary>
    /// ボスの当たり判定を管理するクラス
    /// </summary>
    public class HitboxEnemy : MonoBehaviour
    {
        /// <summary>
        /// ヒットしたときのアクションを参照する変数
        /// </summary>
        public event Action<int, bool> HitAction;

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
            PlayerController colliderScript = other.GetComponent<PlayerController>();


            if (colliderScript != null)
            {
                // ダッシュアタックならダッシュアタックコライダー側にヒットを送る
                colliderScript.Hit();
                // かつスタンを強制オフ
                HitAction?.Invoke(dam, false);
            }
            else
            {
                // ダッシュアタックでないなら通常攻撃
                HitAction?.Invoke(dam, playerScript.IsStunable);
            }
        }
    }
}
