using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// GameOverUIのRootObjectにアタッチし、UIの進行管理をする
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        public UnityEvent OnRetryButtonClick => onRetryButtonClick;
        [SerializeField, HideInInspector]
        private UnityEvent onRetryButtonClick = null;

        public UnityEvent OnTitleButtonClick => onTitleButtonClick;
        [SerializeField, HideInInspector]
        private UnityEvent onTitleButtonClick = null;

        [SerializeField]
        private Button retryButton = null;

        [SerializeField]
        private Button titleButton = null;

        Animator animator;

        static readonly int showId = Animator.StringToHash("Show");
        static readonly int outroId = Animator.StringToHash("Outro");

        void Awake()
        {
            // コンポーネントを参照しておく
            animator = GetComponent<Animator>();

            // UnityEvent を追加
            retryButton.onClick.AddListener(() =>
            {
                animator.SetTrigger(outroId);
                OnRetryButtonClick.Invoke();
            });
            titleButton.onClick.AddListener(() =>
            {
                animator.SetTrigger(outroId);
                OnTitleButtonClick.Invoke();
            });
        }

        /// <summary>
        /// UI表示
        /// </summary>
        public void Show()
        {
            animator.SetTrigger(showId);
            retryButton.Select();
        }
    }
}