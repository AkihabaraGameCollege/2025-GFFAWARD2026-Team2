using System;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private bool isEscapePressing = false;
    private float escapePressTime = 0f;

    [SerializeField]
    private float reqEscapePressTime = 3f;

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

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isEscapePressing = true;
        }
        if (context.canceled)
        {
            isEscapePressing = false;
            if (escapePressTime > reqEscapePressTime)
            {
                SaveDataClear();
            }
            escapePressTime = 0f;
        }
    }
    private void SaveDataClear()
    {
        // ステージランクのリセット
        PlayerPrefs.SetInt("AttackLevel", 1);
        PlayerPrefs.SetInt("JumpLevel", 1);
        PlayerPrefs.SetInt("SpeedLevel", 1);
    }

    void Update()
    {
        if (isEscapePressing) escapePressTime += Time.deltaTime;
    }
}
