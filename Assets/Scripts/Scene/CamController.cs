using PlayerController = QuickTheFury.Player.PlayerController;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Scene;

namespace QuickTheFury.Scene
{
    /// <summary>
    /// カメラの切り替え制御を行う
    /// </summary>
    public class CamController : MonoBehaviour
    {
        // プレイヤーを見ているいつものカメラ
        private GameObject freelookCamera;
        // 開始演出用のボスを見てるカメラ
        private GameObject bossCamera;

        // 演出用のパーティクル
        private ParticleSystem particle;

        [Header("各種数値設定")]
        [SerializeField]
        [Tooltip("ボス登場エフェクトまでの待機時間")]
        private float firstWaitTime = 1.5f;
        [SerializeField]
        [Tooltip("ボス登場エフェクトからプレイヤー操作可能までの遷移時間")]
        private float secoundWaitTime = 3f;

        private PlayerController player;

        private void Start()
        {
            player = StageScene.Instance.Player;
            freelookCamera = StageScene.Instance.FreeLookCamera;
            bossCamera = StageScene.Instance.BossCamera;
            particle = StageScene.Instance.BossEnterParticle;

            freelookCamera.SetActive(false);

            player.Sleep();
            particle.Stop();

            StartCoroutine(OnAction());
        }

        /// <summary>
        /// シーン開始時演出
        /// </summary>
        private IEnumerator OnAction()
        {
            yield return new WaitForSeconds(firstWaitTime);
            particle.Play();

            yield return new WaitForSeconds(secoundWaitTime);

            // カメラを切り替え
            freelookCamera.SetActive(true);
            bossCamera.SetActive(false);

            player.WakeUp();
        }
    }
}