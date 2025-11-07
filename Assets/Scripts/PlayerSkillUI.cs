using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ブリキアームから順に入れてください")]
    private Image[] images = null;

    private void Start()
    {
        ReloadFlag();
    }

    // 能力取得状態を更新 (富里が編集)
    public void ReloadFlag()
    {
        for (int i = 0; i < TitleScene.IsUpgraded.Length; i++)
        {
            images[i].enabled = TitleScene.IsUpgraded[i];
        }
    }
}
