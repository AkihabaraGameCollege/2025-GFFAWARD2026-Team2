using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//タイトルのアニメーション・ステージ画面への遷移・ボタン機能・音響を制御するスクリプト
public class TitleScene : MonoBehaviour
{
    // ステージ画面に遷移するまでの時間を指定
    [SerializeField]
    [Tooltip("スタートボタンを押した後、ステージ画面に遷移するまでの時間を指定")]
    private float stageTransitionDelay;

    // 次のシーン名を指定
    [SerializeField]
    [Tooltip("次のシーン名を指定")]
    private string nextSceneName;

    // エフェクト再生用の AudioSource を指定します。
    [SerializeField]
    private AudioSource effectAudio = null;

    // Animatorコンポーネント
    Animator animator;
    // AnimatorのパラメーターID
   static readonly int outroId = Animator.StringToHash("outro");

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
        // エフェクトを再生
        animator.SetTrigger(outroId);
        // ウェイト
        yield return new WaitForSeconds(stageTransitionDelay);
        // 次のシーンへ遷移
        SceneManager.LoadScene(nextSceneName);
    }
}
