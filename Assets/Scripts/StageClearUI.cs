using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// ステージクリアーUIの進行制御を管理します。
public class StageClearUI : MonoBehaviour
{
    // 「NEXT」ボタンが押されたときに発生する UnityEvent を取得または設定します。
    public UnityEvent OnNextButtonClick => onNextButtonClick;
    [SerializeField]
    private UnityEvent onNextButtonClick = null;

    // 「NEXT」ボタンを指定します。
    [SerializeField]
    private Button nextButton = null;


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
    }

    // このUIを表示します。
    public void Show()
    {
        animator.SetTrigger(showId);
        nextButton.Select();
    }
}