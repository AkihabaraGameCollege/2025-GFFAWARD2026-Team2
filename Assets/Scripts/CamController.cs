using UnityEngine;
using System.Collections;

/// <summary>
/// カメラの切り替え制御を行う
/// </summary>
public class CamController : MonoBehaviour
{
    [Header("オブジェクト参照")]
    [SerializeField]
    [Tooltip("プレイヤーの三人称カメラ")]
    private GameObject playerCam;
    [SerializeField]
    [Tooltip("ボスを見ている演出用カメラ")]
    private GameObject bossCam;

    [SerializeField]
    [Tooltip("ボス登場のParticleEffect")]
    private ParticleSystem particle;

    [Header("各種数値設定")]
    [SerializeField]
    [Tooltip("ボス登場エフェクトまでの待機時間")]
    private float waitTime = 3f;
    [SerializeField]
    [Tooltip("ボス登場エフェクトからプレイヤー操作可能までの遷移時間")]
    private float switchTime = 3f;

    // スクリプト参照用（中山が編集）
    [SerializeField]
    private Player player;

    bool isSwitched;

    // オンオフ切り替え用フラグ（中山が編集）
    void Start()
    {
        // 初期設定（中山が編集）
        isSwitched = false;
        playerCam.SetActive(false);

        player.Sleep();// プレイヤーを行動不能（中山が編集）
        particle.Stop();// パーティクル停止（中山が編集）

        StartAction();// アクション開始（中山が編集）
    }

    // アクション開始時の処理（中山が編集）
    public void StartAction()
    {
        StartCoroutine(OnAction());// コルーチン開始（中山が編集）
    }

    // アクション中の処理（中山が編集）
    private IEnumerator OnAction()
    {
        yield return new WaitForSeconds(waitTime);// 指定時間待機（中山が編集）
        particle.Play();// パーティクル再生（中山が編集）
        yield return new WaitForSeconds(switchTime);// 指定時間待機（中山が編集）

        // フラグを立ててカメラを切り替える（中山が編集）
        isSwitched = true;
        player.WakeUp();
    }

    // 毎フレームの更新処理（中山が編集）
    void Update()
    {
        // フラグが立っていなかったらカメラを切り替える（中山が編集）
        if (isSwitched)
        {
            playerCam.SetActive(!playerCam.activeSelf);// プレイヤーカメラを有効にする（中山が編集）
            bossCam.SetActive(!bossCam.activeSelf);// ボスカメラを無効にする（中山が編集）
            isSwitched = false;
        }
    }
}