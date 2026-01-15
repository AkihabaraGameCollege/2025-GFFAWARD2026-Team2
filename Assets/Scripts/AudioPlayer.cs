using UnityEngine;

namespace QuickTheFury
{
    /// <summary>
    /// BGMとSEを管理するシングルトンクラス
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        public static AudioPlayer instance;// シングルトンインスタンス

        // オーディオクリップ
        // BGMの配列
        [SerializeField]
        private AudioClip[] bgmClips = null;
        // SEの配列
        [SerializeField]
        private AudioClip[] seClips = null;

        // オーディオソース
        // BGM用オーディオソースの参照
        [SerializeField]
        private AudioSource bgmSource = null;
        // SE用オーディオソースの参照
        [SerializeField]
        private AudioSource seSource = null;

        /// <summary>
        /// 初期の設定を行う関数
        /// </summary>
        private void Awake()
        {
            // シングルトンの設定
            if (instance == null)
            {
                instance = this;// インスタンスを設定
                DontDestroyOnLoad(gameObject);// シーン切り替え時に破棄しない
            }
            // すでにインスタンスが存在する場合は破棄する
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// BGMを再生するための関数（フェード中は再生できない）
        /// </summary>
        /// <param name="bgmIndex">BGMの配列インデックス</param>
        public void PlayBGM(int bgmIndex)
        {
            // インデックスの範囲チェック
            if (bgmIndex < 0 || bgmIndex >= bgmClips.Length)
            {
                Debug.LogError("BGMのインデックスが範囲外です");// エラーログ出力
                return;// この関数を抜ける
            }

            // BGMが再生中なら停止する
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();// BGM停止
            }

            // BGMを再生する
            bgmSource.clip = bgmClips[bgmIndex];
            bgmSource.Play();
        }

        public void PauseBGM()
        {
            bgmSource.Pause();
        }

        public void StopBGM()
        {
            bgmSource.Stop();
        }

        /// <summary>
        /// SEを再生する関数
        /// </summary>
        /// <param name="seIndex">SEの配列インデックス</param>
        public void PlaySE(int seIndex, float volume = 0.2f, float pitch = 1)
        {
            // インデックスの範囲チェック
            if (seIndex < 0 || seIndex >= seClips.Length)
            {
                Debug.LogError("SEのインデックスが範囲外です\n呼び出されたIndex:" + seIndex);// エラーログ出力
                return;// この関数を抜ける
            }

            seSource.pitch = pitch;
            seSource.volume = volume;

            seSource.PlayOneShot(seClips[seIndex]);// インデックスに対応するSEを再生
        }

        public void StopSE()
        {
            seSource.Stop();
        }
    }
}