using QuickTheFury;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioPlayer = QuickTheFury.Core.AudioPlayer;
using DataClearUI = QuickTheFury.UI.DataClearUI;

namespace Assets.Scripts.Scene
{
    /// <summary>
    /// TitleSceneの管理をする
    /// </summary>
    public class TitleScene : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("次のシーンへ遷移する時間")]
        private float stageTransitionDelay;

        [SerializeField]
        [Tooltip("次のシーンの名前")]
        private string nextSceneName;

        [SerializeField]
        private DataClearUI dataClearUI;

        [SerializeField]
        private Image titleImage;

        [SerializeField]
        private GameObject titleButtons;

        [SerializeField]
        private Button skipButton;

        [SerializeField]
        private Button StartButton;

        [SerializeField]
        [Tooltip("Startが押されてからSkipButtonが出てくるまでの時間")]
        private float skipButtonAppearWaitTime;

        [SerializeField]
        [Tooltip("暗転のアニメーション時間")]
        private float outroTime;

        Animator animator;
        static readonly int outroId = Animator.StringToHash("Outro");

        void Start()
        {
            //カーソルの表示
            CursorUnLockJudge(false);

            animator = GetComponent<Animator>();
            AudioPlayer.instance.PlayBGM(15); // titlemusicを再生

            // UIの非表示
            dataClearUI.Hide();
        }

        /// <summary>
        /// StartのCoroutineを実行 ボタンから設定
        /// </summary>
        public void PressStartButton()
        {
            StartCoroutine(OnStart());
        }
        /// <summary>
        /// スタート時に実行される処理
        /// </summary>
        IEnumerator OnStart()
        {
            // 処理の合間で待機時間を挟みながら次のシーンへ遷移する
            animator.SetTrigger(outroId);
            yield return new WaitForSeconds(outroTime);
            AudioPlayer.instance.StopBGM();

            yield return new WaitForSeconds(skipButtonAppearWaitTime);

            skipButton.Select();

            yield return new WaitForSeconds(stageTransitionDelay - skipButtonAppearWaitTime);

            SceneManager.LoadScene(nextSceneName);
        }

        /// <summary>
        /// オープニング中のスキップボタンの関数 ボタンから設定
        /// </summary>
        public void OnSkipButtonClick()
        {
            SceneManager.LoadScene(nextSceneName);
        }

        /// <summary>
        /// 即座にゲームを終了
        /// ボタンから設定
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("ゲームを終了します");// コンソールに終了メッセージを表示（中山が編集）
            Application.Quit();// ゲーム終了（中山が編集）
        }

        /// <summary>
        /// データ削除画面から通常のTitle画面へ遷移 ボタンから設定
        /// </summary>
        public void OnClickBackButton()
        {
            dataClearUI.Hide();
            titleImage.enabled = true;
            titleButtons.SetActive(true);
            StartButton.Select();
        }

        /// <summary>
        /// データ削除の関数を実行 ボタンから設定
        /// </summary>
        public void OnClickYesButton()
        {
            dataClearUI.ShowConfirm();
            ClearSaveData();
        }

        /// <summary>
        /// データ削除の確認画像表示 ボタンから設定
        /// </summary>
        public void OnClickDataClearButton()
        {
            dataClearUI.ShowAsk();
            titleImage.enabled = false;
            titleButtons.SetActive(false);
        }

        /// <summary>
        /// セーブデータの削除
        /// </summary>
        private void ClearSaveData()
        {
            // ステージランクのリセット
            PlayerPrefs.SetInt("AttackLevel", 1);
            PlayerPrefs.SetInt("JumpLevel", 1);
            PlayerPrefs.SetInt("SpeedLevel", 1);
        }

        /// <summary>
        /// ゲームパッド対応版のカーソル出現関数
        /// </summary>
        /// <param name="isConfine">ウィンドウ枠から出ないようにするかどうか</param>
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
}