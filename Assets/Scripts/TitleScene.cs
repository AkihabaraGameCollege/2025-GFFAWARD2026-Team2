using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    [SerializeField]
    [Tooltip("スタートボタンを押した後、ステージ画面に遷移するまでの時間を指定")]
    private float stageTransitionDelay;

    [SerializeField]
    [Tooltip("次のシーン名を指定")]
    private string nextSceneName;
    public void PressStartButton()
    {
        StartCoroutine(OnStart());
    }
    IEnumerator OnStart()
    {
        yield return new WaitForSeconds(stageTransitionDelay);

        SceneManager.LoadScene(nextSceneName);
    }
}
