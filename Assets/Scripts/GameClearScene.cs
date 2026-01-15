using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QuickTheFury
{
    public class GameClearScene : MonoBehaviour
    {
        [SerializeField]
        private string nextScene = "Title";

        // 各種待機時間の設定（中山が編集）
        [SerializeField]
        private float loadWaitTime = 1;
        [SerializeField]
        private float musicWaitTime = 24;
        [SerializeField]
        private float outroTime = 2;
        [SerializeField]
        private float skipButtonAppearWaitTime = 2.0f;
        [SerializeField]
        private float stageTransitionDelay = 5.0f;

        // ボタンの参照（中山が編集）
        [SerializeField]
        private Button nextButton = null;
        [SerializeField]
        private Button skipButton;

        private bool isLoadable = false;

        // Animatorコンポーネントの参照
        [SerializeField]
        private Animator animator;

        static readonly int outroId = Animator.StringToHash("outroGamCle");

        private void Start()
        {
            AudioPlayer.instance.StopSE(); // SEを停止(中山が編集)
            AudioPlayer.instance.PlayBGM(7); // Gameclear1を再生（富里が編集）
            Cursor.lockState = CursorLockMode.None;// カーソルのロックを解除（富里が編集）
            StartCoroutine(OnStart());
            nextButton.onClick.AddListener(OnTitleButtonClick);
        }

        IEnumerator OnStart()
        {
            yield return new WaitForSeconds(loadWaitTime);
            isLoadable = true;
            // button select
            nextButton.Select();
            yield return new WaitForSeconds(musicWaitTime - loadWaitTime);
            AudioPlayer.instance.PlayBGM(16);// gameclear2を再生（富里が編集）
        }

        public void OnTitleButtonClick()
        {
            if (isLoadable)
            {
                StopAllCoroutines();// すべてのコルーチンを停止（中山が編集）
                StartCoroutine(LoadNextScene());// コルーチンを開始（中山が編集）
            }
        }

        // 次のシーンを読み込むコルーチン（中山が編集）
        IEnumerator LoadNextScene()
        {
            animator.SetTrigger(outroId);// アウトロアニメーションを再生（中山が編集）
            yield return new WaitForSeconds(outroTime);// アニメーションの再生時間分待機（中山が編集）
            AudioPlayer.instance.StopBGM(); // BGMを停止(中山が編集)
                                            // ウェイト
            yield return new WaitForSeconds(skipButtonAppearWaitTime);
            // スキップボタンをセレクトする
            skipButton.Select();
            // ウェイト
            yield return new WaitForSeconds(stageTransitionDelay - skipButtonAppearWaitTime);
            // 次のシーンへ遷移
            SceneManager.LoadScene(nextScene);
        }

        // スキップボタンが押されたときの処理（中山が編集）
        public void OnSkipButtonClick()
        {
            SceneManager.LoadScene(nextScene);// 次のシーンへ遷移（中山が編集）
        }
    }
}
