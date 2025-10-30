using UnityEngine;

// 指定した判定対象のコライダーと交差しているかを判定する機能を移植（中山が編集）
public class BoxCaster3D : MonoBehaviour
{
    // ボックスの中心座標を指定します。
    [SerializeField]
    [Tooltip("ボックスの中心座標を指定します。")]
    private Vector3 offset = new(0, 0, 0);
    // ボックスのサイズを指定します。
    [SerializeField]
    [Tooltip("ボックスのサイズを指定します。")]
    private Vector3 size = new(1, 1, 1);
    // 判定対象のレイヤーを指定します。
    [SerializeField]
    [Tooltip("判定対象のレイヤーを指定します。")]
    private LayerMask targetLayers = default;

    // 判定対象と交差している場合はtrue、交差していない場合はfalse
    public bool IsCasted { get; private set; } = false;

    // 固定フレームレートで呼び出される更新処理です。
    void FixedUpdate()
    {
        // 交差判定用のポイントを設定
        var point = transform.TransformPoint(offset);
        var size = this.size;
        size.x *= transform.lossyScale.x;
        size.y *= transform.lossyScale.y;
        size.z *= transform.lossyScale.z;
        // 交差を判定
        IsCasted = Physics.CheckBox(point, size / 2, transform.rotation, targetLayers);
    }

    // Unityエディター上で常時描画するギズモを記述します。
    private void OnDrawGizmos()
    {
        // 交差判定用のポイントを設定
        var halfSize = size / 2;
        var controlPoints = new Vector3[]{
            offset + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            offset + new Vector3( halfSize.x, -halfSize.y, -halfSize.z),
            offset + new Vector3( halfSize.x, -halfSize.y,  halfSize.z),
            offset + new Vector3(-halfSize.x, -halfSize.y,  halfSize.z),
            offset + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            offset + new Vector3(-halfSize.x,  halfSize.y, -halfSize.z),
            offset + new Vector3( halfSize.x,  halfSize.y, -halfSize.z),
            offset + new Vector3( halfSize.x,  halfSize.y,  halfSize.z),
            offset + new Vector3(-halfSize.x,  halfSize.y,  halfSize.z),
            offset + new Vector3(-halfSize.x,  halfSize.y, -halfSize.z),
            offset + new Vector3( halfSize.x,  halfSize.y, -halfSize.z),
            offset + new Vector3( halfSize.x, -halfSize.y, -halfSize.z),
            offset + new Vector3( halfSize.x,  halfSize.y,  halfSize.z),
            offset + new Vector3( halfSize.x, -halfSize.y,  halfSize.z),
            offset + new Vector3(-halfSize.x,  halfSize.y,  halfSize.z),
            offset + new Vector3(-halfSize.x, -halfSize.y,  halfSize.z),
            offset + new Vector3(-halfSize.x,  halfSize.y, -halfSize.z),
            offset + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),};
        transform.TransformPoints(controlPoints);
        // 判定ラインを描画
        Gizmos.color = Color.yellow;
        Gizmos.DrawLineStrip(controlPoints, true);
    }
}
