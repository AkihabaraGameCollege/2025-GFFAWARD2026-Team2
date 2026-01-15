using UnityEngine;

namespace QuickTheFury
{
    public class CottonMonsterMoveAI : MonoBehaviour
    {
        private Boss2MoveAI bossScript = null;
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

        // ダメージエフェクト（中山が編集）
        [SerializeField]
        private GameObject defeatEffect;

        enum MotionState
        {
            // スポーン直後の拡散している状態
            Spreading,
            // ボスに近づいている状態
            Moving
        }
        private MotionState motionState = MotionState.Spreading;
        public void Initialize(Boss2MoveAI script, Vector3 direction, float spreadSpeed, float spreadTime, float moveSpeed, Vector3 spawnOffset, float absorbRad)
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
                    diff.y = 0;
                    Vector3 direction = diff.normalized;
                    rb.linearVelocity = direction * moveSpeed;
                    transform.rotation = Quaternion.LookRotation(direction);

                    break;
            }
        }

        public void OnDamageTaken(int dummy = 0, bool dummybool = false)
        {
            GameObject effect = Instantiate(defeatEffect);// ダメージエフェクト生成（中山が編集）
            effect.transform.position = transform.position;// エフェクト位置設定（中山が編集）

            hitbox.OnHit -= OnDamageTaken;
            Destroy(gameObject);
        }
        [ContextMenu("Destroy")]
        private void Shine()
        {
            OnDamageTaken();
        }
    }
}
