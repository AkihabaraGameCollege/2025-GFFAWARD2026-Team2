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

    private bool isDefeatedBoss1 = false;
    private bool isDefeatedBoss2 = false;
    private bool isDefeatedBoss3 = false;

    // 登録・音楽再生用（中山が編集）
    void Awake()
    {
        AudioPlayer.instance.PlayBGM(13); // stageSelectMusicを再生(中山が編集)

        if (PlayerPrefs.GetInt("AttackLevel",1) == 2)
        {
            isDefeatedBoss1 = true;
        }

        if (PlayerPrefs.GetInt("JumpLevel",1) == 2)
        {
            isDefeatedBoss2 = true;
        }

        if (PlayerPrefs.GetInt("SpeedLevel",1) == 2)
        {
            isDefeatedBoss3 = true;
        }
    }

    // ボス戦1へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
    public void PressBoss1Button()
    {
        if (!isDefeatedBoss1)
        {
            SceneManager.LoadScene(nextSceneName1);// 次のシーンへ遷移（中山が編集）
        }
    }

    // ボス戦2へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
    public void PressBoss2Button()
    {
        if (!isDefeatedBoss2)
        {
            SceneManager.LoadScene(nextSceneName2);// 次のシーンへ遷移（中山が編集）
        }
    }

    // ボス戦3へ行くボタンが押されたときに呼び出されるメソッド（中山が編集）
    public void PressBoss3Button()
    {
        if (!isDefeatedBoss3)
        {
            SceneManager.LoadScene(nextSceneName3);// 次のシーンへ遷移（中山が編集）
        }
    }
}
