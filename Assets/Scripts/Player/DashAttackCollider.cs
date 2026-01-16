using System.Collections;
using UnityEngine;

namespace QuickTheFury.Player
{
    /// <summary>
    /// DashAttackColliderのオンオフなどを管理する
    /// </summary>
    public class DashAttackCollider : MonoBehaviour
    {
        private Collider thiscollider;
        private PlayerController playerScript;
        private bool isCanDashAttack = true;


        private void Start()
        {
            thiscollider = GetComponent<Collider>();
            playerScript = GetComponentInParent<PlayerController>();
            SetActive(false);
        }

        /// <summary>
        /// thisColliderで敵に攻撃したら発動
        /// </summary>
        public void Hit()
        {
            isCanDashAttack = false;
            SetActive(false);
            StartCoroutine(OnHit());
        }

        /// <summary>
        /// 数秒待って再度コライダーをオンに
        /// </summary>
        private IEnumerator OnHit()
        {
            yield return new WaitForSeconds(1);

            isCanDashAttack = true;

            if (playerScript.IsSprinting)
            {
                SetActive(true);
            }
        }

        public void SetActive(bool enabled)
        {
            if (enabled && !isCanDashAttack)
            {
                return;
            }

            thiscollider.enabled = enabled;
        }
    }
}