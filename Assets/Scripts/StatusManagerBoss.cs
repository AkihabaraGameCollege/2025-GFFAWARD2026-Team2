using UnityEngine;

// ボスのステータスに関するスクリプト（中山が別プロジェクトから移植）
public class StatusManagerBoss : MonoBehaviour
{
    //このスクリプトをアタッチするオブジェクト
    [SerializeField]
    GameObject MainObject;
    //撃破エフェクト
    [SerializeField] 
    GameObject destroyEffect;
    //被弾エフェクト
    [SerializeField] 
    GameObject damageEffect;

    //hp現在値
    [SerializeField]
    public int maxHp = 6;
    //ダメージを与える値（中山が編集）
    [SerializeField]
    public static int damage = 1;
    //ラストスパートHP（中山が編集）
    [SerializeField]
    public int lastSpurtHP = 5;
    //ラストスパートBGM（中山が編集）
    [SerializeField]
    private int bossLastBGM = 1;

    //スクリプト参照用（中山が編集）
    [SerializeField]
    private GameDirector gameDirector = null;

    private bool oncePlay;//ラストスパートBGM再生判定用（中山が編集）

    // 登録用（中山が編集）
    void Awake()
    {
        oncePlay = true;// ラストスパートBGM再生判定用（中山が編集）
        destroyEffect.SetActive(false);// 撃破エフェクト非表示（中山が編集）
        damageEffect.SetActive(false);// 被弾エフェクト非表示（中山が編集）
    }

    // Update is called once per frame
    void Update()
    {
        //hpが0以下なら、撃破エフェクトを生成してMainを破壊
        if (maxHp <= 0)
        {
            DestoryMainObject();
        }
    }

    public void Damage()
    {
        damageEffect.SetActive(true);// 被弾エフェクト表示（中山が編集）

        // HPを減少させ、ダメージエフェクトを発生させる
        maxHp -= damage;

        gameDirector.DecreaseHpBoss();//HPゲージを減少させる（中山が編集）

        // エフェクトをインスタンス化
        GameObject effect = Instantiate(damageEffect);

        effect.transform.position = damageEffect.transform.position;// effect変数のエフェクトの位置をdamageEffectの位置と同期させる（中山が編集）
        Destroy(effect, 5);// エフェクトを5秒後に破壊（中山が編集）

        // ラストスパートBGM再生判定（中山が編集）
        if (maxHp <= lastSpurtHP&&oncePlay)
        {
            AudioPlayer.instance.PlayBGM(bossLastBGM);// ラストスパートBGM再生（中山が編集）
            oncePlay = false;// 2回目以降再生されないようにする（中山が編集）
        }
    }

    // ボス撃破処理（中山が編集）
    private void DestoryMainObject()
    {
        destroyEffect.SetActive(true);// 撃破エフェクト表示（中山が編集）

        // 破壊エフェクトを発生させてから、MainObjectに設定したもの（自分自身や部位破壊対象）を破壊
        maxHp = 0;

        // エフェクトをインスタンス化
        GameObject effect = Instantiate(destroyEffect);

        effect.transform.position = destroyEffect.transform.position;// effect変数のエフェクトの位置をdestroyEffectの位置と同期させる（中山が編集）
        Destroy(effect, 30);// エフェクトを30秒後に破壊（中山が編集）
    }
}