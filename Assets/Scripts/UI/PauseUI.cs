using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// PauseUIのRootObjectにアタッチし、UIの進行管理をする
    /// </summary>
    public class PauseUI : MonoBehaviour
    {
        public UnityEvent OnResumeButtonClick => onResumeButtonClick;
        [SerializeField, HideInInspector]
        private UnityEvent onResumeButtonClick = null;

        public UnityEvent OnRetryButtonClick => onRetryButtonClick;
        [SerializeField, HideInInspector]
        private UnityEvent onRetryButtonClick = null;

        public UnityEvent OnExitButtonClick => onExitButtonClick;
        [SerializeField, HideInInspector]
        private UnityEvent onExitButtonClick = null;

        [SerializeField]
        private Button resumeButton = null;
        [SerializeField]
        private Button retryButton = null;
        [SerializeField]
        private Button tutorialButton = null;
        [SerializeField]
        private Button exitButton = null;

        [SerializeField]
        private Image tutorialImage = null;
        private Button tutorialImageButton = null;

        void Start()
        {
            tutorialImageButton = tutorialImage.GetComponent<Button>();

            // UnityEvent を追加
            resumeButton.onClick.AddListener(() => { onResumeButtonClick.Invoke(); });
            retryButton.onClick.AddListener(() => { onRetryButtonClick.Invoke(); });
            tutorialButton.onClick.AddListener(Tutorial);
            exitButton.onClick.AddListener(() => { onExitButtonClick.Invoke(); });

            tutorialImage.enabled = false;
            tutorialImageButton.enabled = false;
            tutorialImageButton.onClick.AddListener(OnClickBack);

            Hide();
        }

        /// <summary>
        /// ResumeButtonをSelectする
        /// </summary>
        public void Select()
        {
            resumeButton.Select();
        }

        /// <summary>
        /// UIを表示
        /// </summary>
        public void Show()
        {
            // 子オブジェクトをすべてアクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            Select();
        }

        /// <summary>
        /// UI非表示
        /// </summary>
        public void Hide()
        {
            // 子オブジェクトをすべて非アクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// チュートリアル表示
        /// </summary>
        public void Tutorial()
        {
            tutorialImage.enabled = true;
            tutorialImageButton.enabled = true;
            tutorialImageButton.Select();
        }

        /// <summary>
        /// 通常のpauseに戻る
        /// </summary>
        public void OnClickBack()
        {
            tutorialImage.enabled = false;
            tutorialImageButton.enabled = false;
            Select();
        }
    }
}