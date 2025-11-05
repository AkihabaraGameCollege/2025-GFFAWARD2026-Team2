using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//タイトルのアニメーション・ステージ画面への遷移・ボタン機能・音響を制御するスクリプト
public class TitleScene : MonoBehaviour
{
    // ステージ画面に遷移するまでの時間を指定（中山が編集）
    [SerializeField]
    private float stageTransitionDelay;

    // 次のシーン名を指定（中山が編集）
    [SerializeField]
    private string nextSceneName;

    // エフェクト再生用の AudioSource を指定します。
    [SerializeField]
    private AudioSource effectAudio = null;

    public static bool setUpgrade = false;// 装備強化フラグ（中山が編集）

    // Animatorコンポーネント
    Animator animator;
    // AnimatorのパラメーターID
   static readonly int outroId = Animator.StringToHash("Outro");

    //スタート時に呼び出されるメソッド
    void Start()
    {
        // Animatorコンポーネントを取得
        animator = GetComponent<Animator>();
        // 効果音を再生
        effectAudio.Play();
    }

    // スタートボタンが押されたときに呼び出されるメソッド
    public void PressStartButton()
    {
        // 効果音を停止
        effectAudio.Stop();
        // エフェクトを再生
        StartCoroutine(OnStart());
    }
    // スタートボタンが押されたときの処理を行うコルーチン
    IEnumerator OnStart()
    {
        setUpgrade= true;// 装備強化フラグを立てる（中山が編集）
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
        StartCoroutine(OnQuit());// クイット処理開始（中山が編集）
    }

    // クイットボタンが押されたときの処理を行うコルーチン（中山が編集）
    IEnumerator OnQuit()
    {
        animator.SetTrigger(outroId);// エフェクトを再生（中山が編集）
        yield return new WaitForSeconds(stageTransitionDelay);// ウェイト（中山が編集）
        Application.Quit();// ゲーム終了（中山が編集）
    }
}
