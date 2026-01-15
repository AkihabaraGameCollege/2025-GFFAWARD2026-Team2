using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QuickTheFury
{
    // ステージクリアーUIの進行制御を管理します。
    public class StageClearUI : MonoBehaviour
    {
        // 「NEXT」ボタンが押されたときに発生する UnityEvent を取得または設定します。
        public UnityEvent OnNextButtonClick => onNextButtonClick;
        [SerializeField, HideInInspector]
        private UnityEvent onNextButtonClick = null;

        public UnityEvent OnTitleButtonClick => onTitleButtonClick;
        [SerializeField, HideInInspector]
        private UnityEvent onTitleButtonClick = null;

        // 「NEXT」ボタンを指定します。
        [SerializeField]
        private Button nextButton = null;

        [SerializeField]
        private Button titleButton;


        // コンポーネントを事前に参照しておく変数
        Animator animator;
        // AnimatorパラメーターID
        static readonly int showId = Animator.StringToHash("ShowC");
        static readonly int outroId = Animator.StringToHash("OutroC");

        void Awake()
        {
            // コンポーネントを参照しておく
            animator = GetComponent<Animator>();
            // UnityEvent を追加
            nextButton.onClick.AddListener(() =>
            {
                animator.SetTrigger(outroId);
                OnNextButtonClick.Invoke();
            });

            titleButton.onClick.AddListener(() =>
            {
                animator.SetTrigger(outroId);
                OnTitleButtonClick.Invoke();
            });
        }

        // このUIを表示します。
        public void Show()
        {
            animator.SetTrigger(showId);
            nextButton.Select();
        }
    }
}