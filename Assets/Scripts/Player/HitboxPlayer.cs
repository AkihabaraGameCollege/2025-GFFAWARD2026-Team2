using QuickTheFury.Player;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// HixBoxColliderに一緒につける、ダメージ検知を送るクラス
    /// </summary>
    public class HitboxPlayer : MonoBehaviour
    {
        private PlayerController playerScript;

        void Start()
        {
            // 自身の親オブジェクトからStatusManagerを取得
            playerScript = GetComponentInParent<PlayerController>();

            if (playerScript == null)
            {
                Debug.LogError("Hitboxの親にPlayerScriptが見つかりません。");
                enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            playerScript.Hit(other.transform.position);
        }
    }
}