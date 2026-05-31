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

        // オンオフ切り替え用フラグ（中山が編集）
        void Start()
        {
            // 初期設定（中山が編集）
            _isSwitched = false;
            _playerCamera.SetActive(false);

           _playerController.Sleep();// プレイヤーを行動不能（中山が編集）
            _particle.Stop();// パーティクル停止（中山が編集）

            StartAngle();// アクション開始（中山が編集）
        }

        // アクション開始時の処理（中山が編集）
        public void StartAngle()
        {
            StartCoroutine(OnStartAngle());// コルーチン開始（中山が編集）
        }

        // アクション中の処理（中山が編集）
        private IEnumerator OnStartAngle()
        {
            yield return new WaitForSeconds(_waitTime);// 指定時間待機（中山が編集）
            _particle.Play();// パーティクル再生（中山が編集）
            yield return new WaitForSeconds(_switchTime);// 指定時間待機（中山が編集）

            // フラグを立ててカメラを切り替える（中山が編集）
            _isSwitched = true;
            _playerController.WakeUp();
        }

        // 毎フレームの更新処理（中山が編集）
        void Update()
        {
            // フラグが立っていなかったらカメラを切り替える（中山が編集）
            if (_isSwitched)
            {
                _playerCamera.SetActive(!_playerCamera.activeSelf);// プレイヤーカメラを有効にする（中山が編集）
                _bossCamera.SetActive(!_bossCamera.activeSelf);// ボスカメラを無効にする（中山が編集）
                _isSwitched = false;
            }
        }
    }
}