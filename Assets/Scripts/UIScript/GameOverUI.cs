using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// GameOverUIのRootObjectにアタッチし、UIの進行管理をする
/// </summary>
public class GameOverUI : MonoBehaviour
{
    // Retry Button が押されたときに発生する UnityEvent を取得または設定します。
    public UnityEvent OnRetryButtonClick=> onRetryButtonClick;
    [SerializeField]
    private UnityEvent onRetryButtonClick = null;
    // Exit Button が押されたときに発生する UnityEvent を取得または設定します。
    public UnityEvent OnTitleButtonClick => onTitleButtonClick;
    [SerializeField]
    private UnityEvent onTitleButtonClick = null;

    // Retry Button を指定します。
    [SerializeField]
    private Button retryButton = null;
    // Exit Button を指定します。
    [SerializeField]
    private Button titleButton = null;

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
        retryButton.Select();
    }
}