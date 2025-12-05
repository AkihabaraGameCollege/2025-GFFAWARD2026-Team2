using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageScene : MonoBehaviour
{
    // 自分自身のインスタンスを取得します。
    public static StageScene Instance { get; private set; } = null;

    // このステージをクリアーしたときに読み込むシーンを指定します。
    [SerializeField]
    private string nextStage = "GameClear";

    [SerializeField]
    private string clearStage = "GameClear";

    [SerializeField]
    private string titleStage ="Title";

    // ポーズUIを指定します。
    [SerializeField]
    private PauseUI pause = null;

    [SerializeField]
    private PlayerUI playerUI;

    public bool IsPaused { get; private set; } = false;// ポーズ状態の場合はtrue、プレイ状態の場合はfalse

    // ゲームオーバー表示用のUIを指定します。(中山が編集)
    [SerializeField]
    private GameOverUI gameOverUI = null;

    //プレイヤーを指定(中山が編集)
    [SerializeField]
    private Player player = null;
  
    // ステージクリアー表示用のUIを指定します。（中山が編集）
    [SerializeField]
    private StageClearUI stageClearUI = null;

    // チュートリアル画像（中山が編集）
    [SerializeField]
    private Image tutorialImage = null;
    private Button tutorialImageButton = null;

    // ステージ名での現在のステージ数検知用
    [SerializeField]
    private string boss1StageName = "Boss1";
    [SerializeField]
    private string boss2StageName = "Boss2";
    [SerializeField]
    private string boss3StageName = "Boss2";

    private bool isFullUpgraded = false;

    [SerializeField]
    Image bossLifeImage = null;

    // イントロ演出の時間を指定（中山が編集）
    [SerializeField]
    private float introTime = 5.0f;
    // 音声再生までの待機時間を指定（中山が編集）
    [SerializeField]
    private float waitTime = 1.0f;
    // SEの音量を指定（中山が編集）
    [SerializeField]
    private float seVolume = 0.5f;

    [SerializeField]
    [Tooltip("プレイヤーに追従するfreelookカメラ")]
    private CinemachineInputAxisController freelookCamera;

    private int bGMID;// BGMのIDを指定する変数（中山が編集）
    private int sEID;// SEのIDを指定する変数（中山が編集）

    // ステージ画面内の進行状態を表します。
    enum SceneState
    {
        // ステージ開始演出中
        Intro,
        // ステージプレイ中
        Play,
        // ゲームオーバーが確定していて演出中
        GameOver,
        // ステージクリアーが確定していて演出中
        StageClear,
    }
    SceneState sceneState = SceneState.Intro;// 現在のステージ画面内の進行状態

    // Awake is called when the script instance is being loaded（中山が編集）
    private void Awake()
    {
        Instance = this;// シングルトンインスタンスを設定(中山が編集)

        // 多分いらなくなった(富里が編集)
        //clearAudio.Stop();// ステージクリアー音声を停止しておく(中山が編集)
        //overAudio.Stop();// ゲームオーバー音声を停止しておく(中山が編集)
    }

    // Start is called before the first frame update（中山が編集）
    private void Start()
    {
        tutorialImageButton = tutorialImage.GetComponent<Button>();

        // ポーズUIの各ボタンが押されたときのイベントを登録
        pause.OnResumeButtonClick.AddListener(Resume);
        pause.OnRetryButtonClick.AddListener(Retry);
        pause.OnTutorialButtonClick.AddListener(Tutorial); // とりあえずタイトルに戻るように設定（中山が編集）
        pause.OnExitButtonClick.AddListener(Title);

        // ゲームオーバーUIの各ボタンが押されたときのイベントを登録(中山が編集)
        gameOverUI.OnRetryButtonClick.AddListener(Retry);
        gameOverUI.OnTitleButtonClick.AddListener(Title);

        stageClearUI.OnNextButtonClick.AddListener(LoadNextStage);// ステージクリアーUIのNEXTボタンにイベントを登録（中山が編集）
        stageClearUI.OnTitleButtonClick.AddListener(Title);

        OnApplicationFocus(true);

        // シーン名を取得
        string activeSceneName = SceneManager.GetActiveScene().name;
        // 各シーンに対応したBGMを再生
        if (activeSceneName == boss1StageName)
        {
            bGMID = 0;// boss1MusicのIDを指定（中山が編集）
            sEID = 4;// introMusicのIDを指定（中山が編集）
            StartCoroutine(OnIntro());// イントロ演出コルーチンを開始（中山が編集）
        }
        else if (activeSceneName == boss2StageName)
        {
            bGMID = 2;// boss2MusicのIDを指定（中山が編集）
            sEID = 15;// introMusicのIDを指定（中山が編集）
            StartCoroutine(OnIntro());// イントロ演出コルーチンを開始（中山が編集）
        }
        else if (activeSceneName == boss3StageName)
        {
            bGMID = 4;// boss3MusicのIDを指定（中山が編集）
            sEID = 10;// introMusicのIDを指定（中山が編集）
            StartCoroutine(OnIntro());// イントロ演出コルーチンを開始（中山が編集）
        }
        else
        {
            Debug.LogError("現在のsceneが、どのstageNameとも一致しません");
        }

        tutorialImage.enabled = false;
        tutorialImageButton.enabled = false;
        tutorialImageButton.onClick.AddListener(OnClickBack);
    }

    // イントロ演出を処理するコルーチン（中山が編集）
    IEnumerator OnIntro()
    {
        yield return new WaitForSeconds(waitTime);// 待機してから音声再生（中山が編集）
        AudioPlayer.instance.PlaySE(sEID, seVolume);// introMusicを再生（中山が編集）
        yield return new WaitForSeconds(introTime);// イントロ演出の時間待機（中山が編集）
        AudioPlayer.instance.StopSE();// SEを停止（中山が編集）
        AudioPlayer.instance.PlayBGM(bGMID);// boss1Musicを再生（中山が編集）
        sceneState = SceneState.Play;// シーン状態をPlayに変更（中山が編集）
    }

    void Update()
    {
        switch (sceneState)
        {
            case SceneState.Intro:
                break;
            case SceneState.Play:
                break;
            case SceneState.GameOver:
                break;
            case SceneState.StageClear:
                break;
            default:
                break;
        }
    }

    // このゲームのポーズ状態をトグルします。
    public void TogglePause()
    {
        if (!IsPaused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    // このゲームを一時停止します。
    public void Pause()
    {
        if (sceneState == SceneState.Play && !IsPaused)
        {
            AudioPlayer.instance.StopSE();// SEを停止（中山が編集）
            player.Sleep();
            IsPaused = true;
            Time.timeScale = 0;
            pause.Show();
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    // このゲームの一時停止状態を解除します。
    public void Resume()
    {
        if (sceneState == SceneState.Play && IsPaused)
        {
            IsPaused = false;
            Time.timeScale = 1;
            pause.Hide();
            OnClickBack(); // チュートリアル画像を閉じる（中山が編集）
            Cursor.lockState = CursorLockMode.Locked;
            player.WakeUp();
        }
    }

    // このステージを再読み込みします。
    public void Retry()
    {
        AudioPlayer.instance.StopBGM(); // BGMを停止(中山が編集)
        AudioPlayer.instance.StopSE();// SEを停止（中山が編集）
        OnLoadScene(SceneManager.GetActiveScene().name);
    }

    // チュートリアルボタンが押されたときの処理（中山が編集）
    public void Tutorial()
    {
        tutorialImage.enabled = true;
        tutorialImageButton.enabled = true;
        tutorialImageButton.Select();
    }

    // チュートリアル画像を閉じるボタンが押されたときの処理（中山が編集）
    public void OnClickBack()
    {
        tutorialImage.enabled = false;// チュートリアル画像を非表示にする（中山が編集）
        tutorialImageButton.enabled = false;// チュートリアル画像のボタンを無効化する（中山が編集）
    }

    // このステージを抜けてタイトル画面を読み込みます。
    public void Title()
    {
        OnLoadScene(titleStage);
    }

    // 次のステージを読み込みます。
    public void LoadNextStage()
    {
        // すべての強化を取得していたらクリアシーンに
        if (isFullUpgraded)
        {
            OnLoadScene(clearStage);
        }
        else
        {
            OnLoadScene(nextStage);
        }
    }


    // 指定したシーンを読み込みます。（中山が編集）
    private void OnLoadScene(string sceneName)
    {
        // ポーズ状態の場合は、コルーチン内で処理が流れなくなるためポーズ解除する
        if (IsPaused)
        {
            Resume();
        }

        
        
        // シーンをロードする
        SceneManager.LoadScene(sceneName);
    }

    // このステージをゲームオーバーとします。(中山が編集)
    public void GameOver()
    {
        // ステージプレイ中のみ(中山が編集)
        if (sceneState == SceneState.Play)
        {
            sceneState = SceneState.GameOver;
            player.Sleep();
            AudioPlayer.instance.PlayBGM(8); // gameoverを再生(富里が編集)
            gameOverUI.Show();// ゲームオーバーUIを表示(中山が編集)
            freelookCamera.enabled = false;
        }
    }

    // このステージをステージクリアーとします。
    public void StageClear()
    {
        // ステージプレイ中のみ
        if (sceneState == SceneState.Play)
        {
            sceneState = SceneState.StageClear;
            AudioPlayer.instance.PlayBGM(12); // stageclearを再生 (富里が編集)
            player.Sleep();// プレイヤー操作を無効化(中山が編集)
            // ステージクリアーUIを表示
            stageClearUI.Show();
            Cursor.lockState = CursorLockMode.Confined;
            freelookCamera.enabled = false;

            // 装備強化フラグに応じて装備強化を行う(富里が編集)
            var thisSceneName = SceneManager.GetActiveScene().name;
            // Stage1ならブリキアーム強化
            if (thisSceneName == boss1StageName)
            {
                PlayerPrefs.SetInt("AttackLevel", 2);
            }
            // Stage2ならもこもこブーツ強化
            else if (thisSceneName == boss2StageName)
            {
                PlayerPrefs.SetInt("JumpLevel", 2);
            }
            // Stage3なら殺戮ダッシュ強化
            else if (thisSceneName == boss3StageName)
            {
                PlayerPrefs.SetInt("SpeedLevel", 2);
            }
            // どこでもない場合はエラー
            else
            {
                Debug.LogError("どこやねんここ");
            }

            isFullUpgraded = PlayerPrefs.GetInt("AttackLevel", 1) == 2 &&
                PlayerPrefs.GetInt("JumpLevel", 1) == 2 &&
                PlayerPrefs.GetInt("SpeedLevel", 1) == 2;
        }
    }

    public void DecreaseHpPlayer(int value, int max)
    {
        playerUI.Life((float)value / max);
    }

    // ボスのHPを減少させるメソッド（中山が編集）
    public void BossBarUpdate(float health, int maxhealth)
    {
        bossLifeImage.fillAmount = health / maxhealth;// 15回攻撃で0になるように調整
    }

    public void ApplySprintGauge(float value,float max)
    {
        playerUI.ApplySprintGauge(value / max);
    }

    private void OnApplicationFocus(bool focus)
    {
        // フォーカスがある場合はカーソルをロックし、ない場合はロックを解除する（中山が編集）
        if (focus && (sceneState == SceneState.Play ||sceneState == SceneState.Intro) && !IsPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;// カーソルをロック（中山が編集）
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;// カーソルのロックを解除（中山が編集）
        }
    }

    public void OnUpdateStrongArmCooldown(float value,float max)
    {
        playerUI.StrongArmCooldown(value / max);
    }
}