using UnityEngine;

public class CottonMonster : MonoBehaviour
{
    private BossMove2 bossScript = null;
    private Rigidbody rb = null;

    [SerializeField]
    [Tooltip("移動速度")]
    private float moveSpeed = 1;
    enum MotionState
    {
        // スポーン直後の拡散している状態
        Spreading,
        // ボスに近づいている状態
        Moving
    }
    private MotionState motionState = MotionState.Spreading;
    public void Initialize(BossMove2 script)
    {
        bossScript = script;
        rb = GetComponent<Rigidbody>();
        motionState = MotionState.Spreading;
    }

    private void FixedUpdate()
    {
        switch (motionState)
        {
            case MotionState.Spreading:
                break;
            case MotionState.Moving:
                break;
        }
    }
}
