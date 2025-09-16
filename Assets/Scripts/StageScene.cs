using UnityEngine;

public class StageScene : MonoBehaviour
{
    // 自分自身のインスタンスを取得します。
    public static StageScene Instance { get; private set; } = null;

    private void Awake()
    {
        Instance = this;
    }
}
