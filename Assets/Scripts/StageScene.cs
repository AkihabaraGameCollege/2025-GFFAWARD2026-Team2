using System.Collections;
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

    // ポーズUIを指定します。
    [SerializeField]
    private PauseUI pause = null;

    public bool IsPaused { get; private set; } = false;// ポーズ状態の場合はtrue、プレイ状態の場合はfalse

    // このステージのプレイ時間を取得します。
    public float PlayTime { get; private set; } = 0;
    // このステージの残りプレイ時間を取得します。
    public float RemainPlayTime => PlayTimeout - PlayTime;
    // このステージのタイムアウト時間を取得します。
    public float PlayTimeout => playTimeout;
    // このステージのタイムアウト時間を指定します。
    [SerializeField]
    private float playTimeout = 100;

    // ゲームオーバー表示用のUIを指定します。(中山が編集)
    [SerializeField]
    private GameOverUI gameOverUI = null;

    //プレイヤーを指定(中山が編集)
    [SerializeField]
    private Player player = null;

    Animator animator;// コンポーネントを事前に参照しておく変数(中山が編集)

    static readonly int outroId = Animator.StringToHash("Outro");// AnimatorパラメーターID(中山が編集)

    // ステージクリアー表示用のUIを指定します。（中山が編集）
    [SerializeField]
    private StageClearUI stageClearUI = null;

    // ステージ名での現在のステージ数検知用
    [SerializeField]
    private string boss1StageName = "Boss1";
    [SerializeField]
    private string boss2StageName = "Boss2";
    [SerializeField]
    private string boss3StageName = "Boss2";

    private bool isFullUpgraded = false;

    // こちらにプレイヤーとボスのライフイメージをアタッチしてください（中山が編集）
    [SerializeField]
    Image playerLifeImage = null;
    [SerializeField]
    Image playerDamageImage = null;
    [SerializeField]
    Image bossLifeImage = null;

    // プレイヤーとボスのHP減少量設定（中山が編集）
    [SerializeField]
    private float playerFillAmountNumberLife = 0.34f;
    [SerializeField]
    private float playerFillAmountNumberDamage = 0.34f;

    [SerializeField]
    private Image weakText = null;
    [SerializeField]
    private Image defeatBossText = null;

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
        // ポーズUIの各ボタンが押されたときのイベントを登録
        pause.OnResumeButtonClick.AddListener(Resume);
        pause.OnRetryButtonClick.AddListener(Retry);
        pause.OnExitButtonClick.AddListener(Exit);

        // ゲームオーバーUIの各ボタンが押されたときのイベントを登録(中山が編集)
        gameOverUI.OnRetryButtonClick.AddListener(Retry);
        gameOverUI.OnExitButtonClick.AddListener(Exit);

        stageClearUI.OnNextButtonClick.AddListener(LoadNextStage);// ステージクリアーUIのNEXTボタンにイベントを登録（中山が編集）
        animator = GetComponent<Animator>();// コンポーネントを参照しておく(中山が編集)
        player.enabled = true;// プレイヤーを無効化しておく(中山が編集)
        sceneState = SceneState.Play;// ステージプレイ中に変更(中山が編集)

        // シーン名を取得
        string activeSceneName = SceneManager.GetActiveScene().name;
        // 各シーンに対応したBGMを再生
        if (activeSceneName == boss1StageName)
        {
            //BossMove1のBGM再生処理と被るためコメントアウト（中山が編集）
            //AudioPlayer.instance.PlayBGM(0);//boss1Musicを再生(富里が編集)
        }
        else if (activeSceneName == boss2StageName)
        {
            AudioPlayer.instance.PlayBGM(2);//boss2Musicを再生(富里が編集)
        }
        else if (activeSceneName == boss3StageName)
        {
            AudioPlayer.instance.PlayBGM(4);//boss3Musicを再生(富里が編集)
        }
        else
        {
            Debug.LogError("現在のsceneが、どのstageNameとも一致しません");
        }
    }

    void Update()
    {
        switch (sceneState)
        {
            case SceneState.Intro:
                break;
            case SceneState.Play:
                // プレイ時間を計測する
                PlayTime += Time.deltaTime;
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
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // このステージを再読み込みします。
    public void Retry()
    {
        StartCoroutine(OnLoadScene(SceneManager.GetActiveScene().name));
    }

    // このステージを抜けてタイトル画面を読み込みます。
    public void Exit()
    {
        StartCoroutine(OnLoadScene("Title"));
    }

    // 次のステージを読み込みます。
    public void LoadNextStage()
    {
        // すべての強化を取得していたらクリアシーンに
        if (isFullUpgraded)
        {
            StartCoroutine(OnLoadScene(clearStage));
        }
        else
        {
            StartCoroutine(OnLoadScene(nextStage));
        }
    }

    // 指定したシーンを読み込みます。（中山が編集）
    IEnumerator OnLoadScene(string sceneName)
    {
        // ポーズ状態の場合は、コルーチン内で処理が流れなくなるためポーズ解除する
        if (IsPaused)
        {
            Resume();
        }



        animator.SetTrigger(outroId);// アウトロアニメーションを開始(中山が編集)
        // アニメーションが終了するまで1秒待機
        yield return new WaitForSeconds(1);
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
            player.enabled = false;// プレイヤー操作を無効化(中山が編集)
            AudioPlayer.instance.PlayBGM(8); // gameoverを再生(富里が編集)
            gameOverUI.Show();// ゲームオーバーUIを表示(中山が編集)
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
            player.enabled = false;// プレイヤー操作を無効化(中山が編集)
            // ステージクリアーUIを表示
            stageClearUI.Show();
            Cursor.lockState = CursorLockMode.Confined;

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

    public void DecreaseHpPlayer()
    {
        playerLifeImage.fillAmount -= playerFillAmountNumberLife;// 3回攻撃で0になるように調整
        playerDamageImage.fillAmount += playerFillAmountNumberDamage;// 3回攻撃で0になるように調整
    }

    // ボスのHPを減少させるメソッド（中山が編集）
    public void BossBarUpdate(float health, int maxhealth)
    {
        bossLifeImage.fillAmount = health / maxhealth;// 15回攻撃で0になるように調整
    }

    public void ShowWeakText()
    {
        if (weakText == null) return;
        weakText.gameObject.SetActive(true);
        if (defeatBossText == null) return;
        defeatBossText.gameObject.SetActive(false);
    }

    public void HideWeakText()
    {
        if (weakText == null) return;
        weakText.gameObject.SetActive(false);
        if (defeatBossText == null) return;
        defeatBossText.gameObject.SetActive(true);
    }
}