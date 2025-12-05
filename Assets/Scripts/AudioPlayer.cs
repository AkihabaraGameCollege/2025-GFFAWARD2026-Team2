using UnityEngine;

//BGMとSEの再生、停止を行うクラス
//BGMのフェードイン、フェードアウトができる
public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer instance;

    [Header("AudioClips")]

    [SerializeField]
    [Tooltip("BGMを格納する")]
    private AudioClip[] bgmClips = null;

    [SerializeField]
    [Tooltip("SEを格納する")]
    private AudioClip[] seClips = null;

    [Header("AudioSources\n上はBGM,下はSE")]
    [SerializeField]
    [Tooltip("BGMを再生するAudioSource")]
    private AudioSource bgmSource = null;

    [SerializeField]
    [Tooltip("SEを再生するAudioSource")]
    private AudioSource seSource = null;


    //====================================================================
    //初期化処理
    //====================================================================

    private void Awake()
    {
        //シングルトン
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //====================================================================
    //BGM処理
    //====================================================================

    /// <summary>
    /// BGMを再生する。フェード中は再生できない
    /// </summary>
    /// <param name="bgmIndex">BGMの配列インデックス</param>
    public void PlayBGM(int bgmIndex)
    {

        if (bgmIndex < 0 || bgmIndex >= bgmClips.Length)
        {
            Debug.LogError("BGMのインデックスが範囲外です");
            return;
        }

        //BGMが再生中なら停止する
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }

        //BGMを再生する
        bgmSource.clip = bgmClips[bgmIndex];
        bgmSource.Play();
    }

    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    /// <summary>
    /// BGMを停止する。
    /// </summary>
    public void StopBGM()
    {


        bgmSource.Stop();
    }


    //====================================================================
    //SE処理
    //====================================================================

    /// <summary>
    /// SEを再生する
    /// </summary>
    /// <param name="seIndex">SEの配列インデックス</param>
    /// 音量指定用の引数を追加（中山が編集）
    public void PlaySE(int seIndex, float volume = 0.2f)
    {
        if (seIndex < 0 || seIndex >= seClips.Length)
        {
            Debug.LogError("SEのインデックスが範囲外です\n呼び出されたIndex:" + seIndex);
            return;
        }

        seSource.volume = volume;// 音量設定（中山が編集）

        seSource.PlayOneShot(seClips[seIndex]);
    }

    /// <summary>
    /// SEを停止する
    /// </summary>
    public void StopSE()
    {
        seSource.Stop();
    }

}
