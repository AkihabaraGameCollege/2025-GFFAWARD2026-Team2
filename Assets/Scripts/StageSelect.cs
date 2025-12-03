using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField]
    private Sprite defeatedSprite1;
    [SerializeField]
    private Sprite defeatedSprite2;
    [SerializeField]
    private Sprite defeatedSprite3;

    [SerializeField]
    private Button buttonBoss1;
    [SerializeField]
    private Button buttonBoss2;
    [SerializeField]
    private Button buttonBoss3;


    // 登録・音楽再生用（中山が編集）
    void Awake()
    {
        AudioPlayer.instance.PlayBGM(13); // stageSelectMusicを再生(中山が編集)

        if (PlayerPrefs.GetInt("AttackLevel", 1) == 2)
        {
            buttonBoss1.GetComponent<Image>().sprite = defeatedSprite1;
            buttonBoss1.enabled = false;
        }
        else
        {
            buttonBoss1.onClick.AddListener(PressBoss1Button);
        }

        if (PlayerPrefs.GetInt("JumpLevel", 1) == 2)
        {
            buttonBoss2.GetComponent<Image>().sprite = defeatedSprite2;
            buttonBoss2.enabled = false;
        }
        else
        {
            buttonBoss2.onClick.AddListener(PressBoss2Button);
        }

        if (PlayerPrefs.GetInt("SpeedLevel", 1) == 2)
        {
            buttonBoss3.GetComponent<Image>().sprite = defeatedSprite3;
            buttonBoss3.enabled = false;
        }
        else
        {
            buttonBoss3.onClick.AddListener(PressBoss3Button);
        }
    }

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
