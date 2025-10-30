using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    [SerializeField] GameObject MainObject; //このスクリプトをアタッチするオブジェクト
    [SerializeField] int hp = 3;             //hp現在値
    [SerializeField] int maxHp = 3;          //いずれmaxHp利用する際に使用

    [SerializeField] GameObject destroyEffect;  //撃破エフェクト
    [SerializeField] GameObject damageEffect;   //被弾エフェクト
    
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
    }
}
