using Assets.Scripts.UI;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioPlayer = QuickTheFury.Core.AudioPlayer;
using PlayerController = QuickTheFury.Player.PlayerController;

namespace Assets.Scripts.Scene
{
    /// <summary>
    /// StageSceneの管理をする
    /// </summary>
    public class StageScene : MonoBehaviour
    {
        public static StageScene Instance { get; private set; } = null;

        // シーン名
        private static readonly string StageSelectSceneName = "StageSelect";
        private static readonly string GameClearSceneName = "GameClear";
        private static readonly string TitleSceneName = "Title";

        // ポーズ状態かどうか
        public bool IsPaused { get; private set; } = false;

        [SerializeField]
        private PauseUI pauseUI;

        [SerializeField]
        private PlayerUI playerUI;

        [SerializeField]
        private GameOverUI gameOverUI;

        [SerializeField]
        private StageClearUI stageClearUI;

        [SerializeField]
        private PlayerController player;
        private PlayerInput playerInput;

        /// <summary>
        /// Sceneの番号により異なるデータを管理するクラス
        /// </summary>
        [Serializable]
        private class SceneData
        {
            [SerializeField]
            private int sceneNumber;
            [SerializeField]
            private int soundIndex;
            [SerializeField]
            private int musicIndex;

            public int SceneNumber => sceneNumber;
            public int SoundIndex => soundIndex;
            public int MusicIndex => musicIndex;
        }

        [SerializeField]
        private SceneData sceneData;

        [SerializeField]
        Image bossLifeImage = null;

        [SerializeField]
        [Tooltip("Intro演出の時間")]
        private float introTime = 5.0f;

        [SerializeField]
        [Tooltip("SE再生までの待機時間")]
        private float introWaitTimeToPlaySoundEffect = 1.0f;

        [SerializeField]
        [Tooltip("IntroのSEの音量")]
        private float seVolume = 0.5f;

        [SerializeField]
        [Tooltip("プレイヤーに追従するfreelookカメラ")]
        private CinemachineInputAxisController freelookCamera;

        private bool IsFullUpgraded => PlayerPrefs.GetInt("AttackLevel", 1) == 2 &&
                    PlayerPrefs.GetInt("JumpLevel", 1) == 2 &&
                    PlayerPrefs.GetInt("SpeedLevel", 1) == 2;

        /// <summary>
        /// Sceneの状態
        /// </summary>
        enum SceneState
        {
            /// <summary>
            /// Intro演出中
            /// </summary>
            Intro,
            /// <summary>
            /// ステージプレイ中
            /// </summary>
            Play,
            /// <summary>
            /// ゲームオーバー後
            /// </summary>
            GameOver,
            /// <summary>
            /// クリア後
            /// </summary>
            StageClear,
        }
        SceneState sceneState = SceneState.Intro;

        /// <summary>
        /// プレイヤーがゲームパッドを使っているか判定
        /// </summary>
        /// <returns>GamePadを使っていたらtrue</returns>
        private bool IsUsingGamepad()
        {
            return playerInput.currentControlScheme == "Gamepad";
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // 各ボタンが押されたときのイベントを登録
            pauseUI.OnResumeButtonClick.AddListener(Resume);
            pauseUI.OnRetryButtonClick.AddListener(Retry);
            pauseUI.OnExitButtonClick.AddListener(Title);
            gameOverUI.OnRetryButtonClick.AddListener(Retry);
            gameOverUI.OnTitleButtonClick.AddListener(Title);
            stageClearUI.OnNextButtonClick.AddListener(LoadNextStage);
            stageClearUI.OnTitleButtonClick.AddListener(Title);

            // カーソルをロック
            Cursor.lockState = CursorLockMode.Locked;

            // Intro演出開始
            StartCoroutine(OnIntro(sceneData.MusicIndex,sceneData.SoundIndex));

            playerInput = player.GetComponent<PlayerInput>();
        }

        /// <summary>
        /// Intro演出
        /// </summary>
        /// <param name="musicIndex">このステージで流すBGMの番号</param>
        /// <param name="effectIndex">このステージでIntro演出時に流すSEの番号</param>
        IEnumerator OnIntro(int musicIndex,int effectIndex)
        {
            yield return new WaitForSeconds(introWaitTimeToPlaySoundEffect);

            AudioPlayer.instance.PlaySE(effectIndex, seVolume);

            yield return new WaitForSeconds(introTime);

            AudioPlayer.instance.StopSE();

            AudioPlayer.instance.PlayBGM(musicIndex);

            sceneState = SceneState.Play;
        }

        /// <summary>
        /// ポーズ状態を切り替えます
        /// </summary>
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

        /// <summary>
        /// ポーズ
        /// </summary>
        public void Pause()
        {
            // プレイ中のみポーズ可能
            if (sceneState == SceneState.Play && !IsPaused)
            {
                AudioPlayer.instance.StopSE();
                player.Sleep();
                IsPaused = true;
                Time.timeScale = 0;
                pauseUI.Show();
                CursorUnLockJudge(true);
            }
        }

        /// <summary>
        /// ポーズ解除
        /// </summary>
        public void Resume()
        {
            if (sceneState == SceneState.Play && IsPaused)
            {
                IsPaused = false;
                Time.timeScale = 1;
                pauseUI.Hide();
                pauseUI.OnClickBack();
                Cursor.lockState = CursorLockMode.Locked;
                player.WakeUp();
            }
        }

        /// <summary>
        /// ステージを再読み込みします
        /// </summary>
        public void Retry()
        {
            AudioPlayer.instance.StopBGM();
            AudioPlayer.instance.StopSE();
            OnLoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// タイトルシーンへ移動します
        /// </summary>
        public void Title()
        {
            OnLoadScene(TitleSceneName);
        }

        /// <summary>
        /// 次のシーンを判断し、そのステージへ遷移します
        /// </summary>
        public void LoadNextStage()
        {
            // すべての強化を取得していたらクリアシーンに
            if (IsFullUpgraded)
            {
                OnLoadScene(GameClearSceneName);
            }
            else
            {
                OnLoadScene(StageSelectSceneName);
            }
        }

        /// <summary>
        /// 指定したシーンへ遷移します
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        private void OnLoadScene(string sceneName)
        {
            // ポーズ状態の場合は、コルーチン内で処理が流れなくなるためポーズ解除する
            if (IsPaused)
            {
                Resume();
            }

            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// ゲームオーバーの処理
        /// </summary>
        public void GameOver()
        {
            // プレイ中のみ可能
            if (sceneState == SceneState.Play)
            {
                sceneState = SceneState.GameOver;
                player.Sleep();
                AudioPlayer.instance.PlayBGM(8); // GameOverBGM
                gameOverUI.Show();

                // カメラのプレイヤー追従をオフに
                freelookCamera.enabled = false;

                CursorUnLockJudge(true);
            }
        }

        /// <summary>
        /// ステージクリア処理
        /// </summary>
        public void StageClear()
        {
            // ステージプレイ中のみ
            if (sceneState == SceneState.Play)
            {
                sceneState = SceneState.StageClear;
                AudioPlayer.instance.PlayBGM(12); // stageclearを再生
                player.Sleep();
                stageClearUI.Show();
                CursorUnLockJudge(true);

                // カメラのプレイヤー追従をオフ
                freelookCamera.enabled = false;

                // シーン番号に合わせた強化を実行
                switch (sceneData.SceneNumber)
                {
                    case 1:
                        PlayerPrefs.SetInt("AttackLevel", 2);
                        break;
                    case 2:
                        PlayerPrefs.SetInt("JumpLevel", 2);
                        break;
                    case 3:
                        PlayerPrefs.SetInt("SpeedLevel", 2);
                        break;
                }
            }
        }

        /// <summary>
        /// 体力を表示するUIを更新
        /// </summary>
        /// <param name="value">現在値</param>
        /// <param name="max">最大体力</param>
        public void UpdateLifeImage(int value, int max)
        {
            playerUI.Life((float)value / max);
        }

        /// <summary>
        /// ボスの体力UIを更新
        /// </summary>
        /// <param name="health">現在地</param>
        /// <param name="maxhealth">最大体力</param>
        public void UpdateBossBar(float health, int maxhealth)
        {
            bossLifeImage.fillAmount = health / maxhealth;
        }

        /// <summary>
        /// プレイヤーのダッシュゲージを更新
        /// </summary>
        /// <param name="value">現在値</param>
        /// <param name="max">最大値</param>
        public void UpdateSprintGauge(float value, float max)
        {
            playerUI.ApplySprintGauge(value / max);
        }

        /// <summary>
        /// プレイヤーのスタン攻撃のクールダウンUIを更新
        /// </summary>
        /// <param name="value">現在地</param>
        /// <param name="max">最大値</param>
        public void UpdateStrongArmCooldown(float value, float max)
        {
            playerUI.StrongArmCooldown(value / max);
        }

        /// <summary>
        /// ゲーム画面が開かれているかをチェック
        /// </summary>
        /// <param name="focus">開かれていたらtrue</param>
        private void OnApplicationFocus(bool focus)
        {
            // 開かれていて、特定のシーン中かつポーズでないならカーソルロック
            if (focus && (sceneState == SceneState.Play || sceneState == SceneState.Intro) && !IsPaused)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            // 開かれていないならロック解除
            else
            {
                CursorUnLockJudge(false);
            }
        }

        /// <summary>
        /// gamepadに対応した、カーソルのロックを外す関数
        /// </summary>
        /// <param name="isConfine">trueだとウィンドウ枠から出ないConfineに</param>
        public void CursorUnLockJudge(bool isConfine = false)
        {
            // ゲームパッドを使っていないのであればロック解除
            if (!IsUsingGamepad())
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
        }
    }
}