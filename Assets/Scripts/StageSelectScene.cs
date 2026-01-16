using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

namespace QuickTheFury
{
    // ステージセレクト画面からステージへ遷移するスクリプト（中山が編集）
    public class StageSelectScene : MonoBehaviour
    {
        // 次のシーン名を指定（中山が編集）
        [SerializeField]
        private string nextSceneName1;
        [SerializeField]
        private string nextSceneName2;
        [SerializeField]
        private string nextSceneName3;

        [SerializeField]
        private Sprite defeatedSprite1;
        [SerializeField]
        private Sprite defeatedSprite2;
        [SerializeField]
        private Sprite defeatedSprite3;

        [SerializeField]
        private Button buttonBoss1;
        [SerializeField]
        private Button buttonBoss2;
        [SerializeField]
        private Button buttonBoss3;

        [SerializeField]
        private Image secretAbility1;
        [SerializeField]
        private Image secretAbility2;
        [SerializeField]
        private Image secretAbility3;

        [SerializeField]
        private Image tutorialImage;
        private Button tutorialImageButton;


        [SerializeField]
        private Image abilityImage;
        private Button abilityImageButton;

        [SerializeField]
        private Button tutorialButton;
        [SerializeField]
        private Button abilityButton;

        // アウトロアニメーションの再生時間を指定（中山が編集）
        [SerializeField]
        private float outroTime;

        private bool isDefeatedBoss1;
        private bool isDefeatedBoss2;
        private bool isDefeatedBoss3;
        [SerializeField]
        [Tooltip("背景のimage")]
        private Image backGroundImage;
        [SerializeField]
        [Tooltip("通常の背景スプライト")]
        private Sprite normalBackGroundSprite;
        [SerializeField]
        [Tooltip("クリア後の背景スプライト")]
        private Sprite clearedBackGroundSprite;

        // Animatorコンポーネントの参照（中山が編集）
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private string titleScene = "Title";

        // AnimatorのパラメーターIDの配列
        private struct OutroTriggerIds
        {
            public static readonly int outro1Id = Animator.StringToHash("Outro1");
            public static readonly int outro2Id = Animator.StringToHash("Outro2");
            public static readonly int outro3Id = Animator.StringToHash("Outro3");
        }

        // 登録・音楽再生用（中山が編集）
        void Start()
        {
            AudioPlayer.instance.PlayBGM(13); // stageSelectMusicを再生(中山が編集)

            if (PlayerPrefs.GetInt("AttackLevel", 1) == 2)
            {
                buttonBoss1.GetComponent<Image>().sprite = defeatedSprite1;
                buttonBoss1.enabled = false;
                isDefeatedBoss1 = true;
            }
            else
            {
                buttonBoss1.onClick.AddListener(PressBoss1Button);
            }

            if (PlayerPrefs.GetInt("JumpLevel", 1) == 2)
            {
                buttonBoss2.GetComponent<Image>().sprite = defeatedSprite2;
                buttonBoss2.enabled = false;
                isDefeatedBoss2 = true;
            }
            else
            {
                buttonBoss2.onClick.AddListener(PressBoss2Button);
            }

            if (PlayerPrefs.GetInt("SpeedLevel", 1) == 2)
            {
                buttonBoss3.GetComponent<Image>().sprite = defeatedSprite3;
                buttonBoss3.enabled = false;
                isDefeatedBoss3 = true;
            }
            else
            {
                buttonBoss3.onClick.AddListener(PressBoss3Button);
            }

            if (isDefeatedBoss1 && isDefeatedBoss2 && isDefeatedBoss3)
            {
                backGroundImage.sprite = clearedBackGroundSprite;
            }
            else
            {
                backGroundImage.sprite = normalBackGroundSprite;
            }

            abilityImage.enabled = false;
            tutorialImage.enabled = false;

            secretAbility1.enabled = false;
            secretAbility2.enabled = false;
            secretAbility3.enabled = false;

            tutorialImageButton = tutorialImage.gameObject.GetComponent<Button>();
            abilityImageButton = abilityImage.gameObject.GetComponent<Button>();

            tutorialImageButton.enabled = false;
            abilityImageButton.enabled = false;

            tutorialButton.onClick.AddListener(OnClickTutorialButton);
            abilityButton.onClick.AddListener(OnClickAbilityButton);

            tutorialImageButton.onClick.AddListener(OnClickBack);
            abilityImageButton.onClick.AddListener(OnClickBack);

            CursorUnLockJudge(false);
        }

        // ボス戦1へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
        public void PressBoss1Button()
        {
            StartCoroutine(LoadBoss1Scene());// コルーチンを開始（中山が編集）
        }

        // ボス戦1へ行くコルーチン（中山が編集）
        IEnumerator LoadBoss1Scene()
        {
            AudioPlayer.instance.StopBGM(); // BGMを停止(中山が編集)
            animator.SetTrigger(OutroTriggerIds.outro1Id);// アウトロアニメーションを再生
            yield return new WaitForSeconds(outroTime);// アニメーションの再生時間分待機（中山が編集）
            SceneManager.LoadScene(nextSceneName1);// 次のシーンへ遷移（中山が編集）
        }

        // ボス戦2へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
        public void PressBoss2Button()
        {
            StartCoroutine(LoadBoss2Scene());// コルーチンを開始（中山が編集）
        }

        // ボス戦2へ行くコルーチン（中山が編集）
        IEnumerator LoadBoss2Scene()
        {
            AudioPlayer.instance.StopBGM(); // BGMを停止(中山が編集)
            animator.SetTrigger(OutroTriggerIds.outro2Id);// アウトロアニメーションを再生
            yield return new WaitForSeconds(outroTime);// アニメーションの再生時間分待機（中山が編集）
            SceneManager.LoadScene(nextSceneName2);// 次のシーンへ遷移（中山が編集）
        }

        // ボス戦3へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
        public void PressBoss3Button()
        {
            StartCoroutine(LoadBoss3Scene());// コルーチンを開始（中山が編集）
        }

        // ボス戦3へ行くコルーチン（中山が編集）
        IEnumerator LoadBoss3Scene()
        {
            AudioPlayer.instance.StopBGM(); // BGMを停止(中山が編集)
            animator.SetTrigger(OutroTriggerIds.outro3Id);// アウトロアニメーションを再生
            yield return new WaitForSeconds(outroTime);// アニメーションの再生時間分待機（中山が編集）
            SceneManager.LoadScene(nextSceneName3);// 次のシーンへ遷移（中山が編集）
        }

        public void OnClickTutorialButton()
        {
            tutorialImage.enabled = true;
            tutorialImageButton.enabled = true;
            tutorialImageButton.Select();
        }

        public void OnClickAbilityButton()
        {
            abilityImage.enabled = true;
            secretAbility1.enabled = !isDefeatedBoss1;
            secretAbility2.enabled = !isDefeatedBoss2;
            secretAbility3.enabled = !isDefeatedBoss3;
            abilityImageButton.enabled = true;
            abilityImageButton.Select();
        }

        public void OnClickBack()
        {
            tutorialImage.enabled = false;
            abilityImage.enabled = false;
            tutorialImageButton.enabled = false;
            abilityImageButton.enabled = false;
            secretAbility1.enabled = false;
            secretAbility2.enabled = false;
            secretAbility3.enabled = false;
            tutorialButton.Select();// 戻った後、チュートリアルボタンを選択状態にする（中山が編集）
        }

        public void OnClickTitleButton()
        {
            AudioPlayer.instance.StopBGM(); // BGMを停止(中山が編集)
            SceneManager.LoadScene(titleScene);
        }

        public void CursorUnLockJudge(bool isConfine = false)
        {
            // ゲームパッドを使っていないのであればロック解除
            if (Gamepad.current == null)
            {
                // ウィンドウ枠から出ないようにするか設定可能
                if (isConfine)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                Debug.Log("YEAH");
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
