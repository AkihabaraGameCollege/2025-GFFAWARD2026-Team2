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
    private Button nextButton = null;

    private bool isLoadable = false;

    private void Start()
    {
        StartCoroutine(OnStart());
    }

    IEnumerator OnStart()
    {
        yield return new WaitForSeconds(loadWaitTime);
        isLoadable = true;
        // button select
        nextButton.Select();
    }

    public void OnTitleButtonClick()
    {
        if (isLoadable)
        {
            SceneManager.LoadScene(nextScene);

        }
    }
}
