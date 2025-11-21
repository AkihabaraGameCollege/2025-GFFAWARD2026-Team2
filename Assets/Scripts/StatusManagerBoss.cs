using System;
using UnityEngine;

// ボスのステータスに関するスクリプト（中山が別プロジェクトから移植）
public class StatusManagerBoss : MonoBehaviour
{
    //撃破エフェクト
    [SerializeField] 
    GameObject destroyEffect;
    //被弾エフェクト
    [SerializeField] 
    GameObject damageEffect;

    //hp現在値
    [SerializeField]
    private int maxHealth = 15;
    public int health;
    //ラストスパートHP（中山が編集）
    [SerializeField]
    private int lastSpurtHP = 5;
    //ラストスパートBGM（中山が編集）
    [SerializeField]
    private int bossLastBGM = 1;

    [SerializeField]
    [Tooltip("HitBox")]
    private HitboxBoss hitbox;


    private bool isAlreadyPlayed = false;//ラストスパートBGM再生判定用（中山が編集）

    public event Action OnDamageTaken;
    public event Action OnDeath;

    // 登録用（中山が編集）
    void Awake()
    {
        destroyEffect.SetActive(false);// 撃破エフェクト非表示（中山が編集）
        damageEffect.SetActive(false);// 被弾エフェクト非表示（中山が編集）

        hitbox.OnHit += Damage;

        health = maxHealth;
    }

    public void Damage(int damage)
    {
        damageEffect.SetActive(true);// 被弾エフェクト表示（中山が編集）

        // HPを減少させ、ダメージエフェクトを発生させる
        health -= damage;

        StageScene.Instance.DecreaseHpBoss(health,maxHealth);//HPゲージを減少させる（中山が編集）

        // エフェクトをインスタンス化
        GameObject effect = Instantiate(damageEffect);

        effect.transform.parent = transform;
        Destroy(effect, 5);// エフェクトを5秒後に破壊（中山が編集）

        // ラストスパートBGM再生判定（中山が編集）
        if (health <= lastSpurtHP && !isAlreadyPlayed)
        {
            AudioPlayer.instance.PlayBGM(bossLastBGM);// ラストスパートBGM再生（中山が編集）
            isAlreadyPlayed = true;// 2回目以降再生されないようにする（Tomisatoが編集）
        }

        OnDamageTaken?.Invoke();

        if (health <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}