using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//タイトルのアニメーション・ステージ画面への遷移・ボタン機能・音響を制御するスクリプト
public class TitleScene : MonoBehaviour
{
    // ステージ画面に遷移するまでの時間を指定（中山が編集）
    [SerializeField]
    private float stageTransitionDelay;

    // 次のシーン名を指定（中山が編集）
    [SerializeField]
    private string nextSceneName;

    [SerializeField]
    private DataClearUI dataClearUI;

    [SerializeField]
    private Image titleImage;

    [SerializeField]
    private GameObject titleButtons;


    // Animatorコンポーネント
    Animator animator;
    // AnimatorのパラメーターID
    static readonly int outroId = Animator.StringToHash("Outro");

    //スタート時に呼び出されるメソッド
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;// カーソルのロックを解除（富里が編集）
        // Animatorコンポーネントを取得
        animator = GetComponent<Animator>();
        AudioPlayer.instance.PlayBGM(15); // titlemusicを再生(富里が編集)
        dataClearUI.Hide();
    }

    // スタートボタンが押されたときに呼び出されるメソッド
    public void PressStartButton()
    {
        // エフェクトを再生
        StartCoroutine(OnStart());
    }
    // スタートボタンが押されたときの処理を行うコルーチン
    IEnumerator OnStart()
    {
        // エフェクトを再生
        animator.SetTrigger(outroId);
        // ウェイト
        yield return new WaitForSeconds(stageTransitionDelay);
        // 次のシーンへ遷移
        SceneManager.LoadScene(nextSceneName);
    }

    // クイットボタンが押されたときに呼び出されるメソッド（中山が編集）
    public void QuitGame()
    {
        Debug.Log("ゲームを終了します");// コンソールに終了メッセージを表示（中山が編集）
        Application.Quit();// ゲーム終了（中山が編集）
    }

    public void OnClickBackButton()
    {
        dataClearUI.Hide();
        titleImage.enabled = true;
        titleButtons.SetActive(true);
    }

    public void OnClickYesButton()
    {
        dataClearUI.ShowConfirm();
        SaveDataClear();
    }

    public void OnClickDataClearButton()
    {
        dataClearUI.ShowAsk();
        titleImage.enabled = false;
        titleButtons.SetActive(false);
    }

    private void SaveDataClear()
    {
        // ステージランクのリセット
        PlayerPrefs.SetInt("AttackLevel", 1);
        PlayerPrefs.SetInt("JumpLevel", 1);
        PlayerPrefs.SetInt("SpeedLevel", 1);
    }
}
