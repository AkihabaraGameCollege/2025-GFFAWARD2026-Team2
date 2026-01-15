using System.Collections;
using UnityEngine;

namespace QuickTheFury.Player
{
    /// <summary>
    /// DashAttackCollider‚ÌƒIƒ“ƒIƒt‚È‚Ç‚ğŠÇ—‚·‚é
    /// </summary>
    public class DashAttackCollider : MonoBehaviour
    {
        Collider thiscollider;
        PlayerController playerScript;

        private void Start()
        {
            thiscollider = GetComponent<Collider>();
            playerScript = GetComponentInParent<PlayerController>();
        }

        public void Hit()
        {
            thiscollider.enabled = false;
            StartCoroutine(OnHit());
        }

        private IEnumerator OnHit()
        {
            yield return new WaitForSeconds(1);

            if (playerScript.IsSprinting)
            {
                thiscollider.enabled = true;
            }
        }
    }
}