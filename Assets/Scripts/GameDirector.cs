using UnityEngine;
using UnityEngine.UI;

// ゲーム全体を管理するスクリプト（中山が別プロジェクトから移植）
public class GameDirector : MonoBehaviour
{
    // こちらにプレイヤーとボスのライフイメージをアタッチしてください（中山が編集）
    [SerializeField]
    GameObject playerLifeImage = null;
    [SerializeField]
    GameObject bossLifeImage = null;

    // プレイヤーとボスのHP減少量設定（中山が編集）
    [SerializeField]
    private float playerFillAmountNumberL = 0.34f;
    [SerializeField]
    private float playerFillAmountNumberD = 0.34f;
    [SerializeField]
    private float bossFillAmountNumber = 0.2f;

    // プレイヤーのHPを減少させるメソッド（中山が編集）
    public void DecreaseHpPlayer()
    {
        this.playerLifeImage.GetComponent<Image>().fillAmount -= playerFillAmountNumberL;// 3回攻撃で0になるように調整
        this.playerLifeImage.GetComponent<Image>().fillAmount += playerFillAmountNumberD;// 3回攻撃で0になるように調整
    }

    // ボスのHPを減少させるメソッド（中山が編集）
    public void DecreaseHpBoss()
    {
        this.bossLifeImage.GetComponent<Image>().fillAmount -= bossFillAmountNumber;// 15回攻撃で0になるように調整
    }
}