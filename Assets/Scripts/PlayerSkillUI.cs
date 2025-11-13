using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ブリキアームから順に入れてください")]
    private Image[] images = null;

    [SerializeField]
    [Tooltip("ブリキアームから順に入れてください")]
    private Sprite[] gotSprites = null;

    [SerializeField]
    [Tooltip("ブリキアームから順に入れてください")]
    private Sprite[] notGotSprites = null;

    private void Start()
    {
        ReloadFlag();
    }

    // 能力取得状態を更新 (富里が編集)
    public void ReloadFlag()
    {
        for (int i = 0; i < TitleScene.IsUpgraded.Length; i++)
        {
            images[i].sprite = (TitleScene.IsUpgraded[i]) ? gotSprites[i] : notGotSprites[i];
        }
    }
}
