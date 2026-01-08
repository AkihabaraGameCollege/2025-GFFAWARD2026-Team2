using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    public UnityEvent OnTutorialButtonClick => onTutorialButtonClick;
    [SerializeField, HideInInspector]
    private UnityEvent onTutorialButtonClick = null;

    public UnityEvent OnExitButtonClick => onExitButtonClick;
    [SerializeField, HideInInspector]
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