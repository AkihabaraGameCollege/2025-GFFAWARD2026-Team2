using UnityEngine;

// ボスのステータスに関するスクリプト（中山が別プロジェクトから移植）
public class StatusManagerBoss : MonoBehaviour
{
    //このスクリプトをアタッチするオブジェクト
    [SerializeField]
    GameObject MainObject;

    //hp現在値
    [SerializeField]
    int maxHp = 6;
    //ダメージを与える値（中山が編集）
    [SerializeField]
    public static int damage = 1;

    [SerializeField] GameObject destroyEffect;  //撃破エフェクト
    [SerializeField] GameObject damageEffect;   //被弾エフェクト

    //スクリプト参照用（中山が編集）
    [SerializeField]
    private BossMove bossMove = null;

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
        // HPを減少させ、ダメージエフェクトを発生させる
        maxHp -= damage;

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
        Destroy(effect, 5);

        bossMove.Die();
    }
}