using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [SerializeField]
    BossMove enemyParentScript;


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("YEAH");
        enemyParentScript.TakeDamage();
    }
}
