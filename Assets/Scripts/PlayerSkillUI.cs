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
    private void ReloadFlag()
    {
        images[0].sprite = (PlayerPrefs.GetInt("AttackLevel",1) == 2) ? gotSprites[0] : notGotSprites[0];
        images[1].sprite = (PlayerPrefs.GetInt("JumpLevel", 1) == 2) ? gotSprites[1] : notGotSprites[1];
        images[2].sprite = (PlayerPrefs.GetInt("SpeedLevel", 1) == 2) ? gotSprites[2] : notGotSprites[2];
    }
}
