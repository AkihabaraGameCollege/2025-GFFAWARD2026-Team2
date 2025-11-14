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
    private Button nextButton = null;

    private bool isLoadable = false;

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
            SceneManager.LoadScene(nextScene);

        }
    }
}
