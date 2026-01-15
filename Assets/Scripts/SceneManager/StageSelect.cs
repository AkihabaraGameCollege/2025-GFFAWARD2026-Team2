using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// StageSelectSceneの管理をする
/// </summary>
public class StageSelect : MonoBehaviour
{
    [Serializable]
    private struct StageSelectSceneData
    {
        public string skillName;
        public string sceneName;
        public Sprite defeatedSprite;
        public Button button;
        public Image hideSkillImage;
        public bool isGotSkill;
    }

    [SerializeField]
    private StageSelectSceneData[] sceneData;

    [SerializeField]
    [Tooltip("操作説明画像")]
    private Image tutorialImage;
    //Start()時にGetComponentされる
    private Button tutorialImageButton;

    [SerializeField]
    [Tooltip("能力説明画像")]
    private Image abilityImage;
    //Start()時にGetComponentされる
    private Button abilityImageButton;

    [SerializeField]
    private Button tutorialButton;
    [SerializeField]
    private Button abilityButton;

    // アウトロアニメーションの再生時間を指定（中山が編集）
    [SerializeField]
    private float outroTime;


    [SerializeField]
    [Tooltip("背景のimage")]
    private Image backGroundImage;
    [SerializeField]
    [Tooltip("通常の背景スプライト")]
    private Sprite normalBackGroundSprite;
    [SerializeField]
    [Tooltip("クリア後の背景スプライト")]
    private Sprite clearedBackGroundSprite;

    private Animator animator;

    [SerializeField]
    private string titleScene = "Title";

    static readonly int[] outroID =
    {
        Animator.StringToHash("outro1"),
        Animator.StringToHash("outro2"),
        Animator.StringToHash("outro3")
    };

    void Start()
    {
        AudioPlayer.instance.PlayBGM(13); // stageSelectMusicを再生

        bool isThereFalse = false;
        animator = GetComponent<Animator>();

        // シーンの初期化
        for (int i = 0; i < sceneData.Length; i++)
        {
            int index = i;
            // そのボスを討伐しているかどうかを判定
            if (PlayerPrefs.GetInt(sceneData[i].skillName, 1) == 2)
            {
                // 討伐済みの画像に
                sceneData[i].button.GetComponent<Image>().sprite = sceneData[i].defeatedSprite;
                sceneData[i].button.enabled = true;
                sceneData[i].isGotSkill = true;
            }
            else
            {
                // 未討伐の場合ボタンにメソッド割り当て
                sceneData[i].isGotSkill = false;
                sceneData[i].button.onClick.AddListener(() =>
                {
                    LoadScene(index);
                });
                isThereFalse = true;
            }
            sceneData[i].hideSkillImage.enabled = false;
        }

        // すべてクリアしていた場合は背景を変更
        if (!isThereFalse)
        {
            backGroundImage.sprite = clearedBackGroundSprite;
        }
        else
        {
            backGroundImage.sprite = normalBackGroundSprite;
        }

        // 説明画像の非表示
        abilityImage.enabled = false;
        tutorialImage.enabled = false;

        // ボタンのComponent取得
        tutorialImageButton = tutorialImage.gameObject.GetComponent<Button>();
        abilityImageButton = abilityImage.gameObject.GetComponent<Button>();

        // 説明画面のボタンのdisable
        tutorialImageButton.enabled = false;
        abilityImageButton.enabled = false;

        // ボタンにメソッド割り当て
        tutorialButton.onClick.AddListener(OnClickTutorialButton);
        abilityButton.onClick.AddListener(OnClickAbilityButton);
        tutorialImageButton.onClick.AddListener(OnClickBack);
        abilityImageButton.onClick.AddListener(OnClickBack);

        // カーソルをロック解除
        CursorUnLockJudge(false);
    }

    /// <summary>
    /// 次のシーンを読み込む
    /// </summary>
    /// <param name="sceneIndex">シーンの番号 最小値0</param>
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadStageScene(sceneIndex));
    }

    /// <summary>
    /// 次のシーンの読み込みコルーチン
    /// </summary>
    /// <param name="stageNumber">シーンの番号 最小値0</param>
    IEnumerator LoadStageScene(int stageNumber)
    {
        // 存在しないシーン番号ならエラー
        if (stageNumber < 0 || stageNumber >= sceneData.Length)
        {
            Debug.LogError("指定された番号のステージが存在しません"+stageNumber);
            yield break;
        }

        AudioPlayer.instance.StopBGM(); // BGMを停止(中山が編集)
        animator.SetTrigger(outroID[stageNumber]);// エフェクトを再生（中山が編集）
        yield return new WaitForSeconds(outroTime);// アニメーションの再生時間分待機（中山が編集）
        SceneManager.LoadScene(sceneData[stageNumber].sceneName);// 次のシーンへ遷移（中山が編集）
    }

    /// <summary>
    /// 操作説明画面に遷移
    /// </summary>
    public void OnClickTutorialButton()
    {
        tutorialImage.enabled = true;
        tutorialImageButton.enabled = true;
        tutorialImageButton.Select();
    }

    /// <summary>
    /// 能力説明画面に遷移
    /// </summary>
    public void OnClickAbilityButton()
    {
        abilityImage.enabled = true;
        for (int i = 0; i < sceneData.Length; i++)
        {
            sceneData[i].hideSkillImage.enabled = sceneData[i].isGotSkill;
        }
        abilityImageButton.enabled = true;
        abilityImageButton.Select();
    }

    /// <summary>
    /// 能力説明画面または操作説明画面から戻る
    /// </summary>
    public void OnClickBack()
    {
        tutorialImage.enabled = false;
        abilityImage.enabled = false;
        tutorialImageButton.enabled = false;
        abilityImageButton.enabled = false;

        for (int i = 0; i < sceneData.Length; i++)
        {
            sceneData[i].hideSkillImage.enabled = false;
        }
        tutorialButton.Select();// 戻った後、チュートリアルボタンを選択状態にする（中山が編集）
    }

    /// <summary>
    /// タイトルへ戻る
    /// </summary>
    public void OnClickTitleButton()
    {
        AudioPlayer.instance.StopBGM(); // BGMを停止(中山が編集)
        SceneManager.LoadScene(titleScene);
    }

    /// <summary>
    /// ゲームパッドに対応した、カーソルをロック解除するメソッド
    /// </summary>
    /// <param name="isConfine">ウィンドウ枠から出ないConfineモードにするかどうか</param>
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
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
