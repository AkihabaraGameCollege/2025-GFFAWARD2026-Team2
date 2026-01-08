using UnityEngine;

/// <summary>
/// 敵のスタン中に出てくるヘイローエフェクトを管理
/// </summary>

public class HaloFollow : MonoBehaviour
{
    [Header("Target (set to weakpoint Transform)")]
    public Transform target;

    [Header("Appearance")]
    public Vector3 localOffset = Vector3.zero; // weakpointに対するローカルオフセット
    public float baseScale = 1f;
    public bool faceCamera = true; // 3Dならカメラに常に面する（billboard）

    [Header("Motion")]
    public float rotationSpeed = 90f; // deg/sec
    public float pulseAmount = 0.12f; // 拡大率
    public float pulseSpeed = 2f; // 1秒での脈動回数（速さ）

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (target != null)
        {
            // 位置追従（ターゲットのローカル座標系にオフセットで追従）
            transform.position = target.position + target.TransformVector(localOffset);

            // オプション: カメラに面させる（3Dシーンの場合）
            if (faceCamera && Camera.main != null)
            {
                transform.forward = Camera.main.transform.forward; // カメラに対して面する
            }
        }

        // 回転
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // 脈動（sin波）
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) * pulseAmount;
        transform.localScale = baseScale * pulse * Vector3.one;
    }
}
