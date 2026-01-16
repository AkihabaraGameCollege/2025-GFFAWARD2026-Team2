using UnityEngine;
using System.Collections;

namespace QuickTheFury
{
    // カメラのコントロールを行うスクリプト（中山が編集）
    public class CameraController : MonoBehaviour
    {
        // オブジェクト参照用（中山が編集）
        [SerializeField]
        private GameObject playerCam;
        [SerializeField]
        private GameObject bossCam;

        // パーティクル参照用（中山が編集）
        [SerializeField]
        private ParticleSystem particle;

        // 時間指定用変数（中山が編集）
        [SerializeField]
        private float waitTime = 3f;
        [SerializeField]
        private float switchTime = 3f;

        // スクリプト参照用（中山が編集）
        [SerializeField]
        private PlayerController player;

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
}