using UnityEngine;
using UnityEngine.UI;

namespace QuickTheFury.UI
{
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

        /// <summary>
        /// UI非表示
        /// </summary>
        public void Hide()
        {
            AskUI.gameObject.SetActive(false);
            ConfirmUI.gameObject.SetActive(false);
        }

        /// <summary>
        /// 削除確認画面を表示
        /// </summary>
        public void ShowAsk()
        {
            AskUI.gameObject.SetActive(true);
            NoButton.Select();
        }

        /// <summary>
        /// 削除済み画面を表示
        /// </summary>
        public void ShowConfirm()
        {
            AskUI.gameObject.SetActive(false);
            ConfirmUI.gameObject.SetActive(true);
            BackButton.Select();
        }
    }
}