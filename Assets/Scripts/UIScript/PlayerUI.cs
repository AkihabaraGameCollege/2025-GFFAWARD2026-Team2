using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// PlayerUIのRootObjectにアタッチし、UIの進行管理をする
/// </summary>
public class PlayerUI : MonoBehaviour
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


    [SerializeField]
    [Tooltip("スプリントゲージ")]
    private Image sprintGauge;
    [SerializeField]
    [Tooltip("スプリントゲージの背景")]
    private Image sprintBackGround;
    [SerializeField]
    [Tooltip("スプリントゲージの下にあるshiftってやつ")]
    private Image sprintShiftImage;

    [SerializeField]
    [Tooltip("HPの画像")]
    private Image lifeImage;
    [SerializeField]
    [Tooltip("ダメージを食らったHP画像")]
    private Image damageImage;

    [SerializeField]
    [Tooltip("スタン攻撃のクールダウン表示の基礎画像")]
    private Image strongArmCooldown;
    [SerializeField]
    [Tooltip("スタン攻撃のクールダウン表示基礎画像の上にかぶせる画像")]
    private Image strongArmOverlay;

    private void Start()
    {
        ReloadFlag();

        // Skillの有無に応じて画像表示を切り替え
        bool isGotSpeedSkill = PlayerPrefs.GetInt("SpeedLevel", 1) == 2;
        sprintBackGround.enabled = isGotSpeedSkill;
        sprintGauge.enabled = isGotSpeedSkill;
        sprintShiftImage.enabled = isGotSpeedSkill;

        bool isGotAttackSkill = PlayerPrefs.GetInt("AttackLevel", 1) == 2;
        strongArmCooldown.enabled = isGotAttackSkill;
        strongArmOverlay.enabled = isGotAttackSkill;
    }

    /// <summary>
    /// 能力取得状態画像の更新
    /// </summary>
    private void ReloadFlag()
    {
        images[0].sprite = (PlayerPrefs.GetInt("AttackLevel", 1) == 2) ? gotSprites[0] : notGotSprites[0];
        images[1].sprite = (PlayerPrefs.GetInt("JumpLevel", 1) == 2) ? gotSprites[1] : notGotSprites[1];
        images[2].sprite = (PlayerPrefs.GetInt("SpeedLevel", 1) == 2) ? gotSprites[2] : notGotSprites[2];
    }

    /// <summary>
    /// sprintGaugeUIの更新
    /// </summary>
    /// <param name="value">0-1で設定する表示割合</param>
    public void ApplySprintGauge(float value)
    {
        sprintGauge.fillAmount = value;
    }

    /// <summary>
    /// HP UIの更新
    /// </summary>
    /// <param name="value">0-1で設定する表示割合 1がMaxHP</param>
    public void Life(float value)
    {
        lifeImage.fillAmount = value;
        damageImage.fillAmount = 1 - value;
    }

    /// <summary>
    /// スタン攻撃UIの更新
    /// </summary>
    /// <param name="amount">0-1で設定する表示割合</param>
    public void StrongArmCooldown(float amount)
    {
        strongArmOverlay.fillAmount = amount;
    }
}