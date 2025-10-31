using UnityEngine;
using UnityEngine.SceneManagement;

// ステージセレクト画面からステージへ遷移するスクリプト（中山が編集）
public class StageSelect : MonoBehaviour
{
    // 次のシーン名を指定（中山が編集）
    [SerializeField]
    private string nextSceneName1;
    [SerializeField]
    private string nextSceneName2;
    [SerializeField]
    private string nextSceneName3;

    // ボス戦1へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
    public void PressBoss1Button()
    {
        SceneManager.LoadScene(nextSceneName1);// 次のシーンへ遷移（中山が編集）
    }

    // ボス戦2へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
    public void PressBoss2Button()
    {
        SceneManager.LoadScene(nextSceneName2);// 次のシーンへ遷移（中山が編集）
    }

    // ボス戦3へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
    public void PressBoss3Button()
    {
        SceneManager.LoadScene(nextSceneName3);// 次のシーンへ遷移（中山が編集）
    }
}
