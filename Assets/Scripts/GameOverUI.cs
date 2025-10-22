using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

    // ゲームオーバーUIの進行制御を管理します。
    public class GameOverUI : MonoBehaviour
    {
        // Retry Button が押されたときに発生する UnityEvent を取得または設定します。
        public UnityEvent OnRetryButtonClick => onRetryButtonClick;
        [SerializeField]
        private UnityEvent onRetryButtonClick = null;
        // Exit Button が押されたときに発生する UnityEvent を取得または設定します。
        public UnityEvent OnExitButtonClick => onExitButtonClick;
        [SerializeField]
        private UnityEvent onExitButtonClick = null;

        // Retry Button を指定します。
        [SerializeField]
        private Button retryButton = null;
        // Exit Button を指定します。
        [SerializeField]
        private Button exitButton = null;
        // 楽曲再生用の AudioSource を指定します。
        [SerializeField]
        private AudioSource musicAudio = null;

        // コンポーネントを事前に参照しておく変数
        Animator animator;
        // AnimatorパラメーターID
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
            exitButton.onClick.AddListener(() =>
            {
                animator.SetTrigger(outroId);
                OnExitButtonClick.Invoke();
            });
        }

        // このUIを表示します。
        public void Show()
        {
            animator.SetTrigger(showId);
            retryButton.Select();
            musicAudio.Play();
        }
    }