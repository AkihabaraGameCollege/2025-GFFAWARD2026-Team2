using UnityEngine;
using UnityEngine.UI;

namespace QuickTheFury
{
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
        private Image lifeImage;
        [SerializeField]
        private Image damageImage;

        [SerializeField]
        private Image strongArmCooldown;
        [SerializeField]
        private Image strongArmOverlay;

        private void Start()
        {
            ReloadFlag();

            bool isGotSpeedSkill = PlayerPrefs.GetInt("SpeedLevel", 1) == 2;
            if (isGotSpeedSkill)
            {
                sprintBackGround.enabled = true;
                sprintGauge.enabled = true;
                sprintShiftImage.enabled = true;
            }
            else
            {
                sprintBackGround.enabled = false;
                sprintGauge.enabled = false;
                sprintShiftImage.enabled = false;
            }

            if (PlayerPrefs.GetInt("AttackLevel", 1) == 2)
            {
                strongArmCooldown.enabled = true;
                strongArmOverlay.enabled = true;
            }
            else
            {
                strongArmCooldown.enabled = false;
                strongArmOverlay.enabled = false;
            }
        }

        // 能力取得状態を更新 (富里が編集)
        private void ReloadFlag()
        {
            images[0].sprite = (PlayerPrefs.GetInt("AttackLevel", 1) == 2) ? gotSprites[0] : notGotSprites[0];
            images[1].sprite = (PlayerPrefs.GetInt("JumpLevel", 1) == 2) ? gotSprites[1] : notGotSprites[1];
            images[2].sprite = (PlayerPrefs.GetInt("SpeedLevel", 1) == 2) ? gotSprites[2] : notGotSprites[2];
        }

        public void ApplySprintGauge(float value)
        {
            sprintGauge.fillAmount = value;
        }

        public void Life(float value)
        {
            lifeImage.fillAmount = value;
            damageImage.fillAmount = 1 - value;
        }

        public void StrongArmCooldown(float amount)
        {
            strongArmOverlay.fillAmount = amount;
        }
    }
}