using UnityEngine;

namespace QuickTheFury
{
    /// <summary>
    /// BGMとSEを管理するシングルトンクラス
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        /// <summary>
        /// シングルトンインスタンスを参照する変数
        /// </summary>
        public static AudioPlayer Instance;

        /// <summary>
        /// BGMの配列を参照する変数
        /// </summary>
        public AudioClip[] BGM_Clips;
        /// <summary>
        /// SEの配列を参照する変数
        /// </summary>
        public AudioClip[] SE_Clips;
        /// <summary>
        /// BGM用オーディオソースを参照する変数
        /// </summary>
        public AudioSource BGM_Source;
        /// <summary>
        /// SE用オーディオソースを参照する変数
        /// </summary>
        public AudioSource SE_Source = null;

        /// <summary>
        /// 初期の設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしインスタンスが存在しない場合
            if (Instance == null)
            {
                // インスタンスを設定
                Instance = this;
                // シーン切り替え時に破棄しない
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// BGMを再生するための関数
        /// </summary>
        /// <param name="bgmIndex"></param>
        public void PlayBGM(int bgmIndex)
        {
            // もしインデックスが0未満またはBGMの配列の長さ以上の場合
            if (bgmIndex < 0 || bgmIndex >= BGM_Clips.Length)
            {
                return;
            }

            // もしBGMが再生中の場合
            if (BGM_Source.isPlaying)
            {
                BGM_Source.Stop();
            }

            // ---BGMを再生する---
            // インデックスに対応するBGMを設定
            BGM_Source.clip = BGM_Clips[bgmIndex];
            BGM_Source.Play();
        }

        /// <summary>
        /// BGMを一時停止する関数
        /// </summary>
        public void PauseBGM()
        {
            BGM_Source.Pause();
        }

        /// <summary>
        /// BGMを停止する関数
        /// </summary>
        public void StopBGM()
        {
            BGM_Source.Stop();
        }

        /// <summary>
        /// SEを再生する関数
        /// </summary>
        /// <param name="seIndex"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        public void PlaySE(int seIndex, float volume = 0.2f, float pitch = 1)
        {
            // もしインデックスが0未満またはSEの配列の長さ以上の場合
            if (seIndex < 0 || seIndex >= SE_Clips.Length)
            {
                return;
            }

            // ---SEを再生する---
            // ピッチを設定
            SE_Source.pitch = pitch;
            // ボリュームを設定
            SE_Source.volume = volume;
            // インデックスに対応するSEを再生
            SE_Source.PlayOneShot(SE_Clips[seIndex]);
        }

        /// <summary>
        /// SEを停止する関数
        /// </summary>
        public void StopSE()
        {
            SE_Source.Stop();
        }
    }
}