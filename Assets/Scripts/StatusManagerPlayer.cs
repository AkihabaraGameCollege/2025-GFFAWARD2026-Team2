using UnityEngine;

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
    int maxHp = 3;
    // 攻撃リーチの倍率（中山が編集）
    [SerializeField]
    private static float magnificationReachX = 2.0f;
    [SerializeField]
    private static float magnificationReachY = 2.0f;
    [SerializeField]
    private static float magnificationReachZ = 2.0f;
    // ダメージの倍率（中山が編集）
    [SerializeField]
    private static int magnificationDamage = 2;

    // ジャンプ力倍率
    [SerializeField]
    private static float magnificationJumpForce = 3;

    [SerializeField] GameObject destroyEffect;  //撃破エフェクト
    [SerializeField] GameObject damageEffect;   //被弾エフェクト

    //スクリプト参照用（中山が編集）
    [SerializeField]
    private Player player = null;
    [SerializeField]
    private GameDirector gameDirector = null;

    // シングルトンインスタンス（中山が編集）
    [SerializeField]
    public static StatusManagerPlayer instance;

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
        maxHp--;

        gameDirector.DecreaseHp();//HPゲージを減少させる（中山が編集）

        // エフェクトをインスタンス化
        GameObject effect = Instantiate(damageEffect);

        // 現在の位置を取得し、Vector3型の変数に格納
        Vector3 effectPos = transform.position;

        // エフェクトの位置を少し上に調整
        effectPos.y += 1.0f;

        // エフェクトの位置を設定
        effect.transform.position = effectPos;

        // エフェクトのサイズを少し大きくする（中山が編集）
        effect.transform.localScale *= 2f;

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

        player.Die();
    }

    // プレイヤーの攻撃面を強化する関数(富里が編集)
    public void BurikiArm()
    {
        playerAttackCollider.transform.localScale = new Vector3(Player.ReachX * magnificationReachX, Player.ReachY * magnificationReachY, Player.ReachZ * magnificationReachZ);// 当たり判定を指定の倍率に拡大（中山が編集）
        StatusManagerBoss.damage *= magnificationDamage; // ボスに与えるダメージを指定の倍率に変更（中山が編集）
        TitleScene.setUpgrade[0] = true; // 取得状態をtrueに
    }

    public void MokoMokoBoots()
    {
        player.jumpForce *= magnificationJumpForce; // ジャンプ倍率を変更
        TitleScene.setUpgrade[1] = true; // 取得状態をtrueに
    }
}
