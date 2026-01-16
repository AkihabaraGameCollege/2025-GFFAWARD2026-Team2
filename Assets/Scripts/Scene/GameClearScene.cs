using AudioPlayer = QuickTheFury.Core.AudioPlayer;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Scene
{
    /// <summary>
    /// GameClearSceneの管理をする
    /// </summary>
    public class GameClearScene : MonoBehaviour
    {
        private static readonly string TitleSceneName = "Title";

        [SerializeField]
        [Tooltip("ボタンを押せるようになるまでの待機時間")]
        private float loadWaitTime = 1;
        [SerializeField]
        [Tooltip("一つ目のJingleが終わるまでの待機時間")]
        private float musicWaitTime = 24;
        [SerializeField]
        [Tooltip("ホワイトアウトのアニメーション待機時間")]
        private float outroTime = 2;
        [SerializeField]
        [Tooltip("スキップボタン選択可能になるまでの時間")]
        private float skipButtonAppearWaitTime = 2.0f;
        [SerializeField]
        [Tooltip("自動的に次のシーンへ進んでしまう時間")]
        private float stageTransitionDelay = 5.0f;

        [Header("オブジェクト参照")]
        [SerializeField]
        [Tooltip("エンディングへ進むボタン")]
        private Button nextButton;
        [SerializeField]
        [Tooltip("エンディングをスキップするボタン")]
        private Button skipButton;

        // 次のシーンへ遷移可能かのフラグ
        private bool isLoadable = false;

        private Animator animator;

        static readonly int outroId = Animator.StringToHash("outroGamCle");

        private void Start()
        {
            animator = GetComponent<Animator>();

            AudioPlayer.instance.StopSE();
            AudioPlayer.instance.PlayBGM(7); // Gameclear1を再生
            Cursor.lockState = CursorLockMode.None;

            // シーン開始時ルーティン実行
            StartCoroutine(OnStart());

            // ボタンに関数をいれる
            nextButton.onClick.AddListener(OnButtonClick);
            skipButton.onClick.AddListener(OnSkipButtonClick);
        }

        /// <summary>
        /// シーン開始時の演出を起こすIEnumerator
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnStart()
        {
            yield return new WaitForSeconds(loadWaitTime);

            // 次のシーンへ遷移可能に
            isLoadable = true;
            nextButton.Select();

            yield return new WaitForSeconds(musicWaitTime - loadWaitTime);
            AudioPlayer.instance.PlayBGM(16);// gameclear2を再生
        }

        /// <summary>
        /// ボタンを押したときの処理
        /// </summary>
        public void OnButtonClick()
        {
            // 遷移可能なら実行
            if (isLoadable)
            {
                // OnStartの途中ならそれを停止
                StopAllCoroutines();

                StartCoroutine(LoadNextScene());
            }
        }

        /// <summary>
        /// 次のシーンへ遷移する
        /// </summary>
        private IEnumerator LoadNextScene()
        {
            // アニメーション
            animator.SetTrigger(outroId);

            yield return new WaitForSeconds(outroTime);
            AudioPlayer.instance.StopBGM();

            yield return new WaitForSeconds(skipButtonAppearWaitTime);
            skipButton.Select();

            yield return new WaitForSeconds(stageTransitionDelay - skipButtonAppearWaitTime);

            BackToTitleScene();
        }

        /// <summary>
        /// スキップボタンを押したときの処理
        /// </summary>
        public void OnSkipButtonClick()
        {
            BackToTitleScene();
        }

        /// <summary>
        /// Titleシーンへ戻る処理
        /// </summary>
        private void BackToTitleScene()
        {
            SceneManager.LoadScene(TitleSceneName);
        }
    }
}