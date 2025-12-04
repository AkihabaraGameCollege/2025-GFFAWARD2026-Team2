using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearScene : MonoBehaviour
{
    [SerializeField]
    private string nextScene = "Title";

    [SerializeField]
    private float loadWaitTime = 1;

    [SerializeField]
    private float musicWaitTime = 24;

    [SerializeField]
    private float outroTime = 2;

    [SerializeField]
    private Button nextButton = null;

    private bool isLoadable = false;

    // Animatorコンポーネントの参照
    [SerializeField]
    private Animator animator;

    static readonly int outroId = Animator.StringToHash("outroGamCle");

    private void Start()
    {
        AudioPlayer.instance.PlayBGM(7); // Gameclear1を再生（富里が編集）
        Cursor.lockState = CursorLockMode.None;// カーソルのロックを解除（富里が編集）
        StartCoroutine(OnStart());
    }

    IEnumerator OnStart()
    {
        yield return new WaitForSeconds(loadWaitTime);
        isLoadable = true;
        // button select
        nextButton.Select();
        yield return new WaitForSeconds(musicWaitTime - loadWaitTime);
        AudioPlayer.instance.PlayBGM(16);// gameclear2を再生（富里が編集）
    }

    public void OnTitleButtonClick()
    {
        if (isLoadable)
        {
            StopCoroutine(OnStart());
            StartCoroutine(LoadNextScene());// コルーチンを開始（中山が編集）
        }
    }

    // 次のシーンを読み込むコルーチン（中山が編集）
    IEnumerator LoadNextScene()
    {
        Debug.Log("Loading Next Scene...");
        animator.SetTrigger(outroId);// アウトロアニメーションを再生（中山が編集）
        yield return new WaitForSeconds(outroTime);// アニメーションの再生時間分待機（中山が編集）
        SceneManager.LoadScene(nextScene);
    }
}
