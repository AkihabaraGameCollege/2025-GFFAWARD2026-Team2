using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QuickTheFury
{
    // ポーズUIの進行制御を管理します。
    public class PauseUI : MonoBehaviour
    {
        // Resume Button が押されたときに発生する UnityEvent を取得または設定します。
        public UnityEvent OnResumeButtonClick => onResumeButtonClick;
        [SerializeField]
        private UnityEvent onResumeButtonClick = null;
        // Restart Button が押されたときに発生する UnityEvent を取得または設定します。
        public UnityEvent OnRetryButtonClick => onRetryButtonClick;
        [SerializeField]
        private UnityEvent onRetryButtonClick = null;
        // Tutorial Button が押されたときに発生する UnityEvent を取得または設定します。（中山が編集）
        public UnityEvent OnTutorialButtonClick => onTutorialButtonClick;
        [SerializeField]
        private UnityEvent onTutorialButtonClick = null;
        // Exit Button が押されたときに発生する UnityEvent を取得または設定します。
        public UnityEvent OnExitButtonClick => onExitButtonClick;
        [SerializeField]
        private UnityEvent onExitButtonClick = null;

        // Resume Button を指定します。
        [SerializeField]
        private Button resumeButton = null;
        // Retry Button を指定します。
        [SerializeField]
        private Button retryButton = null;
        // Tutorial Button を指定します。（中山が編集）
        [SerializeField]
        private Button tutorialButton = null;
        // Exit Button を指定します。
        [SerializeField]
        private Button exitButton = null;

        void Start()
        {
            // UnityEvent を追加
            resumeButton.onClick.AddListener(() => { onResumeButtonClick.Invoke(); });
            retryButton.onClick.AddListener(() => { onRetryButtonClick.Invoke(); });
            tutorialButton.onClick.AddListener(() => { onTutorialButtonClick.Invoke(); });
            exitButton.onClick.AddListener(() => { onExitButtonClick.Invoke(); });

            Hide();
        }

        public void Select()
        {
            resumeButton.Select();
        }

        // このUIを表示します。
        public void Show()
        {
            // 子オブジェクトをすべてアクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            resumeButton.Select();
        }

        // このUIを非表示に設定します。
        public void Hide()
        {
            // 子オブジェクトをすべて非アクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}