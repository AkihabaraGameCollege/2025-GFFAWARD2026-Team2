using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ステージセレクト画面からステージへ遷移するスクリプト（中山が編集）
public class StageSelect : MonoBehaviour
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
    private Button ruleButton;
    [SerializeField]
    private Button abilityButton;

    private bool isDefeatedBoss1;
    private bool isDefeatedBoss2;
    private bool isDefeatedBoss3;


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

        abilityImage.enabled = false;
        tutorialImage.enabled = false;

        secretAbility1.enabled = false;
        secretAbility2.enabled = false;
        secretAbility3.enabled = false;

        tutorialImageButton = tutorialImage.gameObject.GetComponent<Button>();
        abilityImageButton = abilityImage.gameObject.GetComponent<Button>();

        tutorialImageButton.enabled = false;
        abilityImageButton.enabled = false;

        ruleButton.onClick.AddListener(OnClickTutorialButton);
        abilityButton.onClick.AddListener(OnClickAbilityButton);

        tutorialImageButton.onClick.AddListener(OnClickBack);
        abilityImageButton.onClick.AddListener (OnClickBack);
    }

    // ボス戦1へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
    public void PressBoss1Button()
    {
        SceneManager.LoadScene(nextSceneName1);// 次のシーンへ遷移（中山が編集）
    }

    // ボス戦2へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
    public void PressBoss2Button()
    {
        SceneManager.LoadScene(nextSceneName2);// 次のシーンへ遷移（中山が編集）
    }

    // ボス戦3へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
    public void PressBoss3Button()
    {
        SceneManager.LoadScene(nextSceneName3);// 次のシーンへ遷移（中山が編集）
    }

    public void OnClickTutorialButton()
    {
        tutorialImage.enabled = true;
        tutorialImageButton.enabled=true;
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
    }
}
