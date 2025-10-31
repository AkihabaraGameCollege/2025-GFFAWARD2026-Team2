using NUnit.Framework.Interfaces;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// プレイヤーのステータスに関するスクリプト（中山が別プロジェクトから移植）
public class StatusManagerPlayer : MonoBehaviour
{
    // このスクリプトをアタッチするオブジェクト
    [SerializeField]
    GameObject MainObject;
    // プレイヤーのアタックコリダーの参照（中山が編集）
    [SerializeField]
    GameObject playerAttackCollider;

    // hp現在値
    [SerializeField] 
    int hp = 3;
    // いずれmaxHp利用する際に使用
    [SerializeField] 
    int maxHp = 3;
    // 攻撃リーチの倍率（中山が編集）
    [SerializeField]
    private float magnificationReachX = 2.0f;
    [SerializeField]
    private float magnificationReachY = 2.0f;
    [SerializeField]
    private float magnificationReachZ = 2.0f;
    // ダメージの倍率（中山が編集）
    [SerializeField]
    private int magnificationDamage = 2;

    [SerializeField] GameObject destroyEffect;  //撃破エフェクト
    [SerializeField] GameObject damageEffect;   //被弾エフェクト

    //スクリプト参照用（中山が編集）
    [SerializeField]
 private Player player = null;
    [SerializeField]
    private GameDirector gameDirector = null;
    [SerializeField]
    private StatusManagerBoss statusManagerBoss = null;

    // Update is called once per frame
    void Update()
    {
        //hpが0以下なら、撃破エフェクトを生成してMainを破壊
        if (hp <= 0)
        {
            DestoryMainObject();
        }
    }

    public void Damage()
    {
        // HPを減少させ、ダメージエフェクトを発生させる
        hp--;

        gameDirector.DecreaseHp();//HPゲージを減少させる（中山が編集）

        // エフェクトをインスタンス化
        GameObject effect = Instantiate(damageEffect);

        // 現在の位置を取得し、Vector3型の変数に格納
        Vector3 effectPos = transform.position;

        // エフェクトの位置を少し上に調整
        effectPos.y += 1.0f;

        // エフェクトの位置を設定
        effect.transform.position = effectPos;
    }

    private void DestoryMainObject()
    {
        // 破壊エフェクトを発生させてから、MainObjectに設定したもの（自分自身や部位破壊対象）を破壊
        hp = 0;
        // エフェクトをインスタンス化
        GameObject effect = Instantiate(destroyEffect);

        // 現在の位置を取得し、Vector3型の変数に格納
        Vector3 effectPos = transform.position;

        // エフェクトの位置を少し上に調整
        effectPos.y += 1.0f;

        // エフェクトの位置を設定
        effect.transform.position = effectPos;
        Destroy(effect, 5);
        Destroy(MainObject);

        player.Die();
    }

    // プレイヤーの攻撃面を強化する関数（中山が編集）
    public void BurikiArm()
    {
        playerAttackCollider.transform.localScale = new Vector3(magnificationReachX,magnificationReachY,magnificationReachZ);// 当たり判定を指定の倍率に拡大
        statusManagerBoss.damage *= magnificationDamage; // ボスに与えるダメージを指定の倍率に変更
    }
}
