using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// StageClearUIのRootObjectにアタッチし、UIの進行管理をする
/// </summary>
public class StageClearUI : MonoBehaviour
{
    // 「NEXT」ボタンが押されたときに発生する UnityEvent を取得または設定します。
    public UnityEvent OnNextButtonClick => onNextButtonClick;
    [SerializeField,HideInInspector]
    private UnityEvent onNextButtonClick = null;

    public UnityEvent OnTitleButtonClick => onTitleButtonClick;
    [SerializeField,HideInInspector]
    private UnityEvent onTitleButtonClick = null;

    [SerializeField]
    [Tooltip("Next")]
    private Button nextButton = null;

    [SerializeField]
    [Tooltip("Title")]
    private Button titleButton = null;

    Animator animator;

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

    /// <summary>
    /// UIの表示
    /// </summary>
    public void Show()
    {
        animator.SetTrigger(showId);
        nextButton.Select();
    }
}