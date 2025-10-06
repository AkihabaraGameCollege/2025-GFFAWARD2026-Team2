using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StageScene : MonoBehaviour
{
    // 自分自身のインスタンスを取得します。
    public static StageScene Instance { get; private set; } = null;

    // このステージをクリアーしたときに読み込むシーンを指定します。
    [SerializeField]
    private string nextStage = "GameClear";

    // ポーズUIを指定します。
    [SerializeField]
    private pauseUI pause = null;

    // ポーズ状態の場合はtrue、プレイ状態の場合はfalse
    public bool IsPaused { get; private set; } = false;

    // このステージのプレイ時間を取得します。
    public float PlayTime { get; private set; } = 0;
    // このステージの残りプレイ時間を取得します。
    public float RemainPlayTime => PlayTimeout - PlayTime;
    // このステージのタイムアウト時間を取得します。
    public float PlayTimeout => playTimeout;
    // このステージのタイムアウト時間を指定します。
    [SerializeField]
    private float playTimeout = 100;

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
    // 現在の進行状態
    SceneState sceneState = SceneState.Intro;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // ポーズUIの各ボタンが押されたときのイベントを登録
        pause.OnResumeButtonClick.AddListener(Resume);
        pause.OnRetryButtonClick.AddListener(Retry);
        pause.OnExitButtonClick.AddListener(Exit);

        // ゲーム開始時はポーズ状態ではない
        sceneState = SceneState.Play;
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
        StartCoroutine(OnLoadScene(nextStage));
    }

    // 指定したシーンを読み込みます。
    IEnumerator OnLoadScene(string sceneName)
    {
        // ポーズ状態の場合は、コルーチン内で処理が流れなくなるためポーズ解除する
        if (IsPaused)
        {
            Resume();
        }
        // アニメーションが終了するまで1秒待機
        yield return new WaitForSeconds(3);
        // シーンをロードする
        SceneManager.LoadScene(sceneName);
    }
}