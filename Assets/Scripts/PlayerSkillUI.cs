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

    public void ReloadFlag()
    {
        for (int i = 0; i < TitleScene.IsUpgraded.Length; i++)
        {
            images[i].enabled = TitleScene.IsUpgraded[i];
        }
    }
}
