using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QuickTheFury
{
    /// <summary>
    /// ゲームクリアシーンの管理クラス
    /// </summary>
    public class GameClearSceneManager : MonoBehaviour
    {
        // --- ボタンの参照 ---
        /// <summary>
        /// NEXTボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _nextButton;
        /// <summary>
        /// スキップボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _skipButton;

        /// <summary>
        /// Animatorコンポーネントの参照する変数
        /// </summary>
        [SerializeField]
        private Animator _animator;

        // --- 各種待機時間の設定 ---
        /// <summary>
        /// シーン開始からNEXTボタンが押せるようになるまでの待機時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _loadWaitTime = 1;
        /// <summary>
        /// アウトロアニメーションの再生開始から次のシーンへ遷移するまでの待機時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _stageTransitionDelay = 5.0f;

        // --- 各種待機時間の設定 ---
        /// <summary>
        /// シーン開始からBGMが切り替わるまでの待機時間を参照する変数
        /// </summary>
        private float _musicWaitTime = 24;
        /// <summary>
        /// アウトロアニメーションの再生時間を参照する変数
        /// </summary>
        private float _outroTime = 2;
        /// <summary>
        /// アウトロアニメーションの再生後からスキップボタンが表示されるまでの待機時間を参照する変数
        /// </summary>
        private float _skipButtonAppearWaitTime = 2.0f;

        /// <summary>
        /// 次のシーンの名前を参照する変数
        /// </summary>
        private string _nextScene = "Title";

        /// <summary>
        /// ボタンが押せるようになっているかを参照する変数
        /// </summary>
        private bool _isLoadable = false;

        /// <summary>
        /// アウトロアニメーションのIDを参照する変数
        /// </summary>
        private static readonly int _outro_ID = Animator.StringToHash("OnClearOutro");

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // ボタンのクリックイベントに関数を登録
            _nextButton.onClick.AddListener(GoNextScene);

            // カーソルのロックを解除
            Cursor.lockState = CursorLockMode.None;

            // --- 音響設定 ---
            // SEを停止
            AudioPlayer.Instance.StopSE();
            // GameClearのBGMを再生
            AudioPlayer.Instance.PlayBGM(7);

            // シーン開始持演出のコルーチンを開始
            StartCoroutine(SceneIntroCoroutine());
        }

        /// <summary>
        /// シーン開始時の演出を管理するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator SceneIntroCoroutine()
        {
            // ボタンは最初は押せないよう指定時間分待機
            yield return new WaitForSeconds(_loadWaitTime);
            // ボタンが押せるようにフラグオン
            _isLoadable = true;
            // NEXTボタンをセレクトする
            _nextButton.Select();
            // BGMの切り替えまで指定時間分待機
            yield return new WaitForSeconds(_musicWaitTime - _loadWaitTime);
            // BGMをもう一つのゲームクリアシーンBGMに切り替え
            AudioPlayer.Instance.PlayBGM(16);
        }

        /// <summary>
        /// 次のシーンへ遷移するためのコルーチンを開始する関数
        /// </summary>
        public void GoNextScene()
        {
            // もしボタンが押せる状態なら
            if (_isLoadable)
            {
                // すべてのコルーチンを停止
                StopAllCoroutines();
                // コルーチンを開始
                StartCoroutine(LoadNextSceneCoroutine());
            }
        }

        /// <summary>
        /// 次のシーンを読み込むコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadNextSceneCoroutine()
        {
            // アウトロアニメーションを再生
            _animator.SetTrigger(_outro_ID);
            // アニメーションの再生時間分待機
            yield return new WaitForSeconds(_outroTime);
            // BGMを停止
            AudioPlayer.Instance.StopBGM();
            // スキップボタンが表示されるまで指定時間分待機
            yield return new WaitForSeconds(_skipButtonAppearWaitTime);
            // スキップボタンをセレクトする
            _skipButton.Select();
            // エンディングムービーの再生開始から次のシーンへ遷移するまで指定時間分待機
            yield return new WaitForSeconds(_stageTransitionDelay - _skipButtonAppearWaitTime);
            // 次のシーンへ遷移
            SceneManager.LoadScene(_nextScene);
        }

        /// <summary>
        /// エンディングムービーをスキップして次のシーンへ遷移する関数
        /// </summary>
        public void SkipEvent()
        {
            // 次のシーンへ遷移
            SceneManager.LoadScene(_nextScene);
        }
    }
}