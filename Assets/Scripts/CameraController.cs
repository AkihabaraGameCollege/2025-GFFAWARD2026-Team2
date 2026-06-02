using UnityEngine;
using System.Collections;

namespace QuickTheFury
{
    /// <summary>
    /// カメラのコントロールを行うクラス
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーカメラを参照する変数
        /// </summary>
        [SerializeField]
        private GameObject _playerCamera;
        /// <summary>
        /// ボスカメラを参照する変数
        /// </summary>
        [SerializeField]
        private GameObject _bossCamera;

        /// <summary>
        /// パーティクルエフェクトを参照する変数
        /// </summary>
        [SerializeField]
        private ParticleSystem _particle;

        /// <summary>
        /// プレイヤー操作を管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private PlayerController _playerController;

        /// <summary>
        /// カメラ操作までの待機時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _waitTime = 3f;
        /// <summary>
        /// カメラを入れ替える時間を参照する変数
        /// </summary>
        [SerializeField]
        private float _switchTime = 3f;

        /// <summary>
        /// 入れ替えるかを判別するフラグを参照する変数
        /// </summary>
        private bool _isSwitched;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // --- 各変数のオンオフ準備 ---
            _isSwitched = false;
            _playerCamera.SetActive(false);

            // プレイヤーを行動不能
            _playerController.Sleep();
            // パーティクル停止
            _particle.Stop();

            // アクション開始
            StartAngle();
        }

        /// <summary>
        /// アクション開始時の処理を呼び出す関数
        /// </summary>
        public void StartAngle()
        {
            // コルーチン開始
            StartCoroutine(StartAngleCoroutine());
        }

        /// <summary>
        /// アクション中の処理を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartAngleCoroutine()
        {
            // 指定時間待機
            yield return new WaitForSeconds(_waitTime);
            // パーティクル再生
            _particle.Play();
            // 指定時間待機
            yield return new WaitForSeconds(_switchTime);

            // --- フラグを立ててカメラを切り替える ---
            // カメラを入れ替えるフラグを立てる
            _isSwitched = true;
            // プレイヤーを行動可能にする
            _playerController.WakeUp();
        }

        /// <summary>
        /// 毎フレームの更新処理を行う関数
        /// </summary>
        private void Update()
        {
            // もし入れ替えるフラグが立っていた場合
            if (_isSwitched)
            {
                // プレイヤーカメラを有効にする
                _playerCamera.SetActive(!_playerCamera.activeSelf);
                // ボスカメラを無効にする
                _bossCamera.SetActive(!_bossCamera.activeSelf);
                // 入れ替えフラグを下ろす
                _isSwitched = false;
            }
        }
    }
}