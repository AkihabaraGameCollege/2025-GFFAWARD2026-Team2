using UnityEngine;
using UnityEngine.EventSystems;

public class CottonMonster : MonoBehaviour
{
    private BossMove2 bossScript = null;
    private Rigidbody rb = null;

    private Vector3 spreadVelocity = Vector3.zero;
    private float moveSpeed = 0;
    private float absorbRadius = 0;

    private float spreadTimer = 0;
    private float spreadTime = 0;
    private Vector3 targetOffset = Vector3.zero;


    [SerializeField]
    [Tooltip("HitBox")]
    private HitboxEnemy hitbox;

    enum MotionState
    {
        // スポーン直後の拡散している状態
        Spreading,
        // ボスに近づいている状態
        Moving
    }
    private MotionState motionState = MotionState.Spreading;
    public void Initialize(BossMove2 script, Vector3 direction, float spreadSpeed, float spreadTime, float moveSpeed, Vector3 spawnOffset, float absorbRad)
    {
        rb = GetComponent<Rigidbody>();
        hitbox.OnHit += OnDamageTaken;


        bossScript = script;
        motionState = MotionState.Spreading;
        spreadVelocity = direction * spreadSpeed;
        this.spreadTime = spreadTime;
        this.moveSpeed = moveSpeed;
        targetOffset = spawnOffset;
        absorbRadius = absorbRad * absorbRad;
    }

    private void FixedUpdate()
    {
        switch (motionState)
        {
            case MotionState.Spreading:
                rb.linearVelocity = spreadVelocity;
                spreadTimer += Time.fixedDeltaTime;
                if (spreadTimer >= spreadTime)
                {
                    motionState = MotionState.Moving;
                }
                break;
            case MotionState.Moving:
                Vector3 diff = bossScript.transform.position + targetOffset - transform.position;

                if (diff.sqrMagnitude <= absorbRadius)
                {
                    bossScript.Heal();
                    OnDamageTaken();
                }

                rb.linearVelocity = diff.normalized * moveSpeed;
                break;
        }
    }
    
    public void OnDamageTaken(int dummy = 0)
    {
        hitbox.OnHit -= OnDamageTaken;
        Destroy(gameObject);
    }
    [ContextMenu("Destroy")]
    private void Shine()
    {
        OnDamageTaken();
    }
}
