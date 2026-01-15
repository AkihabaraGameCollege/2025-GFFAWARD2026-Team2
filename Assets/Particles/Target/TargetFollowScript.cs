using UnityEngine;

/// <summary>
/// ターゲットに追従し、回転・脈動するエフェクトのクラス
/// </summary>
public class TargetFollowScript : MonoBehaviour
{
    // 追従ターゲット
    public Transform target;

    // エフェクトの見た目設定
    public Vector3 localOffset = Vector3.zero;// weakpointに対するローカルオフセット
    public float baseScale = 1f;// 基本スケール
    public bool faceCamera = true;// 3Dならカメラに常に面する（ON/OFF）

    // モーション設定
    public float rotationSpeed = 90f;// 回転速度（度/秒）
    public float pulseAmount = 0.12f;// 拡大率
    public float pulseSpeed = 2f;// 1秒での脈動回数（速さ）

    // 内部参照
    SpriteRenderer sr;

    /// <summary>
    /// 変数srにコンポーネントを取得して格納する関数
    /// </summary>
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();// SpriteRendererコンポーネント取得
    }

    /// <summary>
    /// ターゲットエフェクトの位置追従、回転、脈動処理の関数
    /// </summary>
    void Update()
    {
        // ターゲット追従
        if (target != null)
        {
            transform.position = target.position + target.TransformVector(localOffset);// 位置追従

            // オプション: カメラに面させる（3Dシーンの場合）
            if (faceCamera && Camera.main != null)
            {
                transform.forward = Camera.main.transform.forward; // カメラに対して面する
            }
        }

        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);// 回転

        // 脈動
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) * pulseAmount;
        transform.localScale = baseScale * pulse * Vector3.one;
    }
}