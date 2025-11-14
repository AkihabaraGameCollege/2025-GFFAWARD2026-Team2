using System.Collections;
using UnityEngine;

public class BossMove1 : MonoBehaviour
{
    //移動速度設定（中山が編集）
    [SerializeField]
    private float moveP = 3;
    //回転速度設定（中山が編集）
    [SerializeField]
    private float speedNumber = 11.1f;

    //ボス本体判定（中山が編集）
    [SerializeField]
    private Collider thisCollider;
    //プレイヤーがボスに触れたらダメージを受ける判定（中山が編集）
    [SerializeField]
    private Collider damageCollider;
    //攻撃判定（中山が編集）
    [SerializeField]
    private Collider attackCollider;
    // 弱点判定（中山が編集）
    [SerializeField]
    private Collider weakCollider;

    // ターゲットオブジェクト（中山が編集）
    [SerializeField]
    private GameObject targetObject;
    // ボスオブジェクト（中山が編集）
    [SerializeField]
    private GameObject bossObject;

    //ステージシーン参照用（中山が編集）
    [SerializeField]
    private StageScene stageScene = null;

    //プレイヤー参照用（中山が編集）
    [SerializeField]
    private Player player;

    //弱体化時間表示用テキスト（中山が編集）
    [SerializeField]
    private GameObject weakTimeText = null;

    new private Rigidbody rigidbody;//Rigidbodyコンポーネント参照用（中山が編集）
    Animator animator;//アニメーター（中山が編集）

    //アニメーションID登録（中山が編集）
    static readonly int IsWalkingID = Animator.StringToHash("isWalking");
    static readonly int attackID = Animator.StringToHash("attack");
    static readonly int weakID = Animator.StringToHash("weak");
    static readonly int wakeUpID = Animator.StringToHash("wakeUp");
    static readonly int dieID = Animator.StringToHash("die");

    // ボス行動時間設定（中山が編集）
    [SerializeField]
    private float bossMoveTime = 10f;
    [SerializeField]
    private float bossLittleWaitTime = 0.5f;
    [SerializeField]
    private float bossAttackTime = 1.5f;
    [SerializeField]
    private float bossWeakTime = 12f;
    [SerializeField]
    private float bossWakeUpTime = 5f;
    [SerializeField]
    private float bossWaitTime = 3f;
    //前進と回転の繰り返し回数設定（中山が編集）
    [SerializeField]
    private int forwardTurn = 10;

    private bool isMoving = false;//移動中かどうか判定（中山が編集）

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();//Rigidbodyコンポーネント取得（中山が編集）
        animator = GetComponent<Animator>();//Animatorコンポーネント取得（中山が編集）

        isMoving = false;//移動停止（中山が編集）
        weakCollider.enabled = false;//弱点判定無効化（中山が編集）
        attackCollider.enabled = false;//攻撃判定無効化（中山が編集）
        weakTimeText.SetActive(false);//弱体化時間表示無効化（中山が編集）

        StartCoroutine(Move(true));//行動パターン開始（中山が編集）
    }

    // ボスの毎フレーム更新処理
    void FixedUpdate()
    {
        Turn();//回転処理（中山が編集）

        //移動処理（中山が編集）
        if (isMoving)
        {
            Vector3 forward = transform.forward * moveP;//前方向に移動ベクトル設定（中山が編集）
            rigidbody.linearVelocity = new Vector3(forward.x, rigidbody.linearVelocity.y, forward.z);//前方向に移動（中山が編集）
            animator.SetFloat(IsWalkingID, rigidbody.linearVelocity.magnitude);//歩行アニメーション開始（中山が編集）
        }
        //移動停止処理（中山が編集）
        else if (!isMoving)
        {
        rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);//移動停止（中山が編集）
        animator.SetFloat(IsWalkingID, rigidbody.linearVelocity.magnitude);//歩行アニメーション停止（中山が編集）
        }
    }

    //回転（）の中に角度を設定（中山が編集）
    private void Turn()
    {
        // 補完スピードを決める
        float speed = speedNumber;
        // ターゲット方向のベクトルを取得
        Vector3 relativePos = targetObject.transform.position - bossObject.transform.position;

        relativePos.y = 0; // X軸の回転は禁止する（中山が編集）

        // 方向を、回転情報に変換
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        // 現在の回転情報と、ターゲット方向の回転情報を補完する
        transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);
    }

    //ジャンプ攻撃（中山が編集）
    void HammerAttack()
    {
        animator.SetTrigger(attackID);//ジャンプアニメーション開始（中山が編集）
        attackCollider.enabled = true;//攻撃判定有効化（中山が編集）
    }

    //弱点タイム（中山が編集）
    private void Weak()
    {
        attackCollider.enabled = false;//攻撃判定無効化（中山が編集）
        animator.SetTrigger(weakID);//弱体化アニメーション再生（中山が編集）
        damageCollider.enabled = false;//ダメージ判定無効化（中山が編集）
        weakCollider.enabled = true;//弱点判定有効化（中山が編集）
        weakTimeText.SetActive(true);//弱体化時間表示有効化（中山が編集）
    }

    // 起き上がり（中山が編集）
    private void WakeUp()
    {
        animator.SetTrigger(wakeUpID);
        damageCollider.enabled = true;//ダメージ判定有効化（中山が編集）
        weakCollider.enabled = false;//弱点判定無効化（中山が編集）
        weakTimeText.SetActive(false);//弱体化時間表示無効化（中山が編集）
    }

    //行動パターン（中山が編集）
    IEnumerator Move(bool loop)
    {
       while(loop == true)
        {
               isMoving = true;//移動開始（中山が編集）
                yield return new WaitForSeconds(bossMoveTime);//待機（中山が編集）
               isMoving = false;//移動停止（中山が編集）

            Turn();//向く（中山が編集）
            yield return new WaitForSeconds(bossLittleWaitTime);
            HammerAttack();//ハンマー攻撃（中山が編集）
            yield return new WaitForSeconds(bossAttackTime);//攻撃する時間（中山が編集）

            yield return new WaitForSeconds(bossLittleWaitTime);//少し待機（中山が編集）
            Weak();//弱点出現（中山が編集）
            yield return new WaitForSeconds(bossWeakTime);// 弱点タイム（中山が編集）

            // ループ終了条件（中山が編集）
            if (!loop)
            {
                       yield break;//コルーチン終了（中山が編集）
            }

            WakeUp();// 起き上がり（中山が編集）
            yield return new WaitForSeconds(bossWakeUpTime);// 待機（中山が編集）

            yield return new WaitForSeconds(bossWaitTime);//待機（中山が編集）
        }
    }

    //撃破処理（中山が編集）
    public void Die()
    {
        StartCoroutine(OnDie());//撃破演出開始（中山が編集）
    }

    //撃破演出（中山が編集）
    IEnumerator OnDie()
    {
        Move(false);
        animator.SetTrigger(dieID);//死亡アニメーション再生（中山が編集）
        yield return new WaitForSeconds(2);//少し待機（中山が編集）
        stageScene.StageClear();//ステージクリア処理（中山が編集）
        Destroy(gameObject);//ボスオブジェクトを破壊（中山が編集）
    }
}