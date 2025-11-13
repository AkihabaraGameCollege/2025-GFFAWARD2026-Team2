using System.Collections;
using UnityEngine;

public class BossMove3 : MonoBehaviour
{
    new private Rigidbody rigidbody;//Rigidbodyコンポーネント参照用（中山が編集）

    //移動、ジャンプ力設定（中山が編集）
    [SerializeField]
    private float jumpP = 10;
    //移動速度設定（中山が編集）
    [SerializeField]
    private float moveP = 3;
    //回転速度設定（中山が編集）
    [SerializeField]
    private float speedNumber = 11.1f;

    //コライダー参照用（中山が編集）
    [Header("Collider")]
    //攻撃判定（中山が編集）
    [SerializeField]
    private Collider attackCollider;
    //ボス本体判定（中山が編集）
    [SerializeField]
    private Collider thisCollider;
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

    //ステータス設定（中山が編集）
    [Header("Stats")]
    //体力設定（中山が編集）
    [SerializeField]
    private int maxHealth;
    private int health;

    //プレイヤー参照用（中山が編集）
    [SerializeField]
    private Player player;

    //弱体化時間表示用テキスト（中山が編集）
    [SerializeField]
    private GameObject weakTimeText = null;

    Animator animator;//アニメーター（中山が編集）

    //アニメーションID登録（中山が編集）
    static readonly int IsWalkingID = Animator.StringToHash("isWalking");
    static readonly int jumpID = Animator.StringToHash("jump");
    static readonly int landingID = Animator.StringToHash("landing");
    static readonly int weakID = Animator.StringToHash("weak");
    static readonly int grandID = Animator.StringToHash("grand");
    static readonly int dieID = Animator.StringToHash("die");

    // エフェクト再生用の AudioSource を指定します。（中山が編集）
    [SerializeField]
    private AudioSource effectAudio = null;
    [SerializeField]
    private AudioSource effectAudioLoop;
    // サウンドを指定します。（中山が編集）
    [SerializeField]
    private AudioClip soundOnAttack = null;
    [SerializeField]
    private AudioClip soundOnMove = null;
    [SerializeField]
    private float bossWeakTime = 3f;
    [SerializeField]
    private float bossWaitTime = 0.5f;
    [SerializeField]
    private float bossAttackTime = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();//Rigidbodyコンポーネント取得（中山が編集）
        animator = GetComponent<Animator>();//Animatorコンポーネント取得（中山が編集）
        thisCollider = GetComponent<Collider>();//ボス本体コライダー取得（中山が編集）

        health = maxHealth;//体力初期化（中山が編集）

        weakCollider.enabled = false;//弱点判定無効化（中山が編集）
        attackCollider.enabled = false;//攻撃判定無効化（中山が編集）
        weakTimeText.SetActive(false);//弱体化時間表示無効化（中山が編集）

        effectAudioLoop.Stop();//歩行時サウンド停止（中山が編集）

        StartCoroutine(Move(true));//行動パターン開始（中山が編集）
    }

    //ジャンプ攻撃（中山が編集）
    void JumpAttack()
    {
        animator.SetTrigger(jumpID);//ジャンプアニメーション開始（中山が編集）
        rigidbody.AddForce(Vector3.up * jumpP, ForceMode.Impulse);//上方向に力を加える（中山が編集）
        effectAudio.PlayOneShot(soundOnMove);//ジャンプ攻撃時サウンド再生（中山が編集）
    }

    //歩く（中山が編集）
    private void Walking()
    {
        StartCoroutine(OnWalk());//歩行コルーチン開始（中山が編集）
    }

    //歩行コルーチン（中山が編集）
    IEnumerator OnWalk()
    {
        animator.SetFloat(IsWalkingID, 1);//歩行アニメーション開始（中山が編集）

        Vector3 forward = transform.forward * moveP;//前方向に移動ベクトル設定（中山が編集）
        rigidbody.linearVelocity = new Vector3(forward.x, rigidbody.linearVelocity.y, forward.z);//前方向に移動（中山が編集）

        effectAudioLoop.Play();//歩行時サウンド再生（中山が編集）
        yield return new WaitForSeconds(0.8f);//歩行時間（中山が編集）
        rigidbody.linearVelocity = Vector3.zero;//停止（中山が編集）
        animator.SetFloat(IsWalkingID, 0);//歩行アニメーション終了（中山が編集）
        effectAudioLoop.Stop();//歩行時サウンド停止（中山が編集）
    }

    //回転（）の中に角度を設定（中山が編集）
    private void Turn(float rotate)
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

        effectAudio.PlayOneShot(soundOnMove);//回転時サウンド再生（中山が編集）
    }

    //行動パターン（中山が編集）
    IEnumerator Move(bool loop)
    {
        while (loop == true)
        {
            yield return new WaitForSeconds(bossWaitTime);//待機（中山が編集）
            Turn(90);//右回転（中山が編集）
            yield return new WaitForSeconds(bossWaitTime);//待機（中山が編集）
            Turn(0);//正面向き（中山が編集）
            yield return new WaitForSeconds(bossWaitTime);//待機（中山が編集）
            Walking();//歩行開始（中山が編集）
            yield return new WaitForSeconds(bossWaitTime);//歩行時間（中山が編集）
            Turn(-90);//左回転（中山が編集）
            yield return new WaitForSeconds(bossWaitTime);//待機（中山が編集）
            Turn(0);//正面向き（中山が編集）
            yield return new WaitForSeconds(bossWaitTime);//待機（中山が編集）

            JumpAttack();//ジャンプ攻撃（中山が編集）
            yield return new WaitForSeconds(1);//ジャンプ攻撃中（中山が編集）
            attackCollider.enabled = true;//攻撃判定有効化（中山が編集）
            thisCollider.enabled = false;//当たり判定無効化（中山が編集）
            effectAudio.PlayOneShot(soundOnAttack);//攻撃時サウンド再生（中山が編集）
            yield return new WaitForSeconds(bossAttackTime);//ハマるまでの時間（中山が編集）
            attackCollider.enabled = false;//攻撃判定無効化（中山が編集）

            animator.SetTrigger(weakID);//弱体化アニメーション再生（中山が編集）
            weakTimeText.SetActive(true);//弱体化時間表示有効化（中山が編集）
            weakCollider.enabled = true;//弱点判定有効化（中山が編集）
            effectAudioLoop.Play();//歩行時サウンド再生（中山が編集）
            yield return new WaitForSeconds(bossWeakTime);
            effectAudioLoop.Stop();//歩行時サウンド停止（中山が編集）
            weakTimeText.SetActive(false);//弱体化時間表示無効化（中山が編集）
            weakCollider.enabled = false;//弱点判定無効化（中山が編集）
            animator.SetTrigger(grandID);//地面にハマるアニメーション終了（中山が編集）
            yield return new WaitForSeconds(0.1f);// 少し待機（中山が編集）

            JumpAttack();//ジャンプ攻撃（中山が編集）
            yield return new WaitForSeconds(1);//ジャンプ中（中山が編集）
            thisCollider.enabled = true;//当たり判定有効化（中山が編集）
            animator.SetTrigger(landingID);//着地アニメーション再生（中山が編集）
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