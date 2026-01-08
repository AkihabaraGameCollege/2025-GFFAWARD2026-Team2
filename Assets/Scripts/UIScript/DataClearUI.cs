using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// DataClearUIのRootObjectにアタッチし、UIの進行管理をする
/// </summary>
public class DataClearUI : MonoBehaviour
{
    [SerializeField]
    private Image AskUI;
    [SerializeField]
    private Button NoButton;
    [SerializeField]
    private Image ConfirmUI;
    [SerializeField]
    private Button BackButton;

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
