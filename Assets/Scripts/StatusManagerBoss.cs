using UnityEngine;
using System.Collections;

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

    //スクリプト参照用（中山が編集）
    [SerializeField]
    private GameDirector gameDirector = null;

    Animator animator;// ボスのアニメーター（中山が編集）

    static readonly int dieID = Animator.StringToHash("Die");// アニメーションID登録（中山が編集）

    // 登録用（中山が編集）
    void Awake()
    {
        animator = GetComponent<Animator>();// アニメーターコンポーネント取得（中山が編集）

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
        destroyEffect.SetActive(true);// 撃破エフェクト表示（中山が編集）
        damageEffect.SetActive(true);// 被弾エフェクト表示（中山が編集）

        // HPを減少させ、ダメージエフェクトを発生させる
        maxHp -= damage;

        gameDirector.DecreaseHpBoss();//HPゲージを減少させる（中山が編集）

        // エフェクトをインスタンス化
        GameObject effect = Instantiate(damageEffect);

        // 現在の位置を取得し、Vector3型の変数に格納
        Vector3 effectPos = transform.position;

        // エフェクトの位置を少し上に調整
        effectPos.y += 5.0f;

        // エフェクトの位置を設定
        effect.transform.position = effectPos;

        // エフェクトのサイズを少し大きくする（中山が編集）
        effect.transform.localScale *= 5f;

        // エフェクトを5秒後に破壊（中山が編集）
        Destroy(effect, 5);
    }

    private void DestoryMainObject()
    {
        // 破壊エフェクトを発生させてから、MainObjectに設定したもの（自分自身や部位破壊対象）を破壊
        maxHp = 0;
        // エフェクトをインスタンス化
        GameObject effect = Instantiate(destroyEffect);

        // 現在の位置を取得し、Vector3型の変数に格納
        Vector3 effectPos = transform.position;

        // エフェクトの位置を少し上に調整
        effectPos.y += 1.0f;

        // エフェクトの位置を設定
        effect.transform.position = effectPos;
        Destroy(effect, 10);
    }
}