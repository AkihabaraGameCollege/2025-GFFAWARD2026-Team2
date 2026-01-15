using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QuickTheFury
{
    //タイトルのアニメーション・ステージ画面への遷移・ボタン機能・音響を制御するスクリプト
    public class TitleScene : MonoBehaviour
    {
        // ステージ画面に遷移するまでの時間を指定（中山が編集）
        [SerializeField]
        private float stageTransitionDelay;

        // 次のシーン名を指定（中山が編集）
        [SerializeField]
        private string nextSceneName;

        [SerializeField]
        private Image titleImage;

        [SerializeField]
        private GameObject titleButtons;

        [SerializeField]
        private Button skipButton;

        [SerializeField]
        private Button StartButton;

        [SerializeField]
        private float skipButtonAppearWaitTime;

        // アウトロアニメーションの再生時間を指定（中山が編集）
        [SerializeField]
        private float outroTime;

        [SerializeField]
        private Image AskUI;
        [SerializeField]
        private Button NoButton;
        [SerializeField]
        private Image ConfirmUI;
        [SerializeField]
        private Button BackButton;

        // Animatorコンポーネント
        Animator animator;
        // AnimatorのパラメーターID
        static readonly int outroId = Animator.StringToHash("Outro");

        //スタート時に呼び出されるメソッド
        void Start()
        {
            CursorUnLockJudge(false);
            // Animatorコンポーネントを取得
            animator = GetComponent<Animator>();
            AudioPlayer.instance.PlayBGM(15); // titlemusicを再生(富里が編集)
                                              Hide();
        }

        // スタートボタンが押されたときに呼び出されるメソッド
        public void PressStartButton()
        {
            // エフェクトを再生
            StartCoroutine(OnStart());
        }
        // スタートボタンが押されたときの処理を行うコルーチン
        IEnumerator OnStart()
        {
            // エフェクトを再生
            animator.SetTrigger(outroId);
            yield return new WaitForSeconds(outroTime);// アニメーションの再生時間分待機（中山が編集）
            AudioPlayer.instance.StopBGM(); // BGMを停止(中山が編集)
                                            // ウェイト
            yield return new WaitForSeconds(skipButtonAppearWaitTime);
            // スキップボタンをセレクトする
            skipButton.Select();
            // ウェイト
            yield return new WaitForSeconds(stageTransitionDelay - skipButtonAppearWaitTime);
            // 次のシーンへ遷移
            SceneManager.LoadScene(nextSceneName);
        }

        public void OnSkipButtonClick()
        {
            SceneManager.LoadScene(nextSceneName);
        }

        // クイットボタンが押されたときに呼び出されるメソッド（中山が編集）
        public void QuitGame()
        {
            Debug.Log("ゲームを終了します");// コンソールに終了メッセージを表示（中山が編集）
            Application.Quit();// ゲーム終了（中山が編集）
        }

        public void OnClickBackButton()
        {
           Hide();
            titleImage.enabled = true;
            titleButtons.SetActive(true);
            StartButton.Select();
        }

        public void OnClickYesButton()
        {
            ShowConfirm();
            SaveDataClear();
        }

        public void OnClickDataClearButton()
        {
            ShowAsk();
            titleImage.enabled = false;
            titleButtons.SetActive(false);
        }

        private void SaveDataClear()
        {
            // ステージランクのリセット
            PlayerPrefs.SetInt("AttackLevel", 1);
            PlayerPrefs.SetInt("JumpLevel", 1);
            PlayerPrefs.SetInt("SpeedLevel", 1);
        }

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

        public void Hide()
        {
            AskUI.gameObject.SetActive(false);
            ConfirmUI.gameObject.SetActive(false);
        }

        public void ShowAsk()
        {
            AskUI.gameObject.SetActive(true);
            NoButton.Select();
        }

        public void ShowConfirm()
        {
            AskUI.gameObject.SetActive(false);
            ConfirmUI.gameObject.SetActive(true);
            BackButton.Select();
        }
    }
}