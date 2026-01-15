using System.Collections;
using UnityEngine;

namespace QuickTheFury
{
    public class DashAttackCollider : MonoBehaviour
    {
        Collider thiscollider;
        Player playerScript;

        private void Start()
        {
            thiscollider = GetComponent<Collider>();
            playerScript = GetComponentInParent<Player>();
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
