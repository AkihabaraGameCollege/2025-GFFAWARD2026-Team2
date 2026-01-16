using Assets.Scripts.Scene;
using QuickTheFury.Core;
using System.Collections;
using UnityEngine;
using PlayerController = QuickTheFury.Player.PlayerController;

namespace QuickTheFury.Enemy
{
    /// <summary>
    /// Boss3の行動制御
    /// </summary>
    public class BossMove3 : MonoBehaviour
    {
        private Rigidbody rb;
        [Header("ステータス")]
        [SerializeField]
        [Tooltip("移動速度")]
        private float moveSpeed = 3;
        [SerializeField]
        [Tooltip("回転速度")]
        private float rotateSpeed = 1;
        [SerializeField]
        [Tooltip("スタン時間")]
        private float defaultStunTime = 15;

        //コライダー参照用（中山が編集）
        [Header("Collider")]
        [SerializeField]
        [Tooltip("攻撃コライダー")]
        private Collider attackCollider;
        [SerializeField]
        [Tooltip("ドリブル用プレイヤー検知コライダー")]
        private PlayerCheckCollider playerCheckCollider;

        [Header("ボス固有の設定")]
        [SerializeField]
        private float rushWaitTime = 1;
        [SerializeField]
        private float startMotionTime = 3;
        [SerializeField]
        [Tooltip("スタン終了後のアニメーションタイム")]
        private float stunEndAnimTime = 2;

        // 死亡関連設定（中山が編集）
        [SerializeField]
        private float deathTime = 3;
        [SerializeField]
        private float deathLittleTime = 2;

        [SerializeField]
        [Tooltip("スタン終了時のジャンプ力")]
        private float stunEndJumpForce = 10;

        private PlayerController player;
        private StatusManagerBoss statusManager;
        [Header("その他")]
        [SerializeField]
        [Tooltip("Offset")]
        private Vector3[] wallCheckerPos = null;
        [SerializeField]
        [Tooltip("Distance")]
        private float wallCheckerDistance = 0;


        [SerializeField]
        [Tooltip("地面のレイヤー")]
        private LayerMask groundLayer;

        // ダメージエフェクト（中山が編集）
        [SerializeField]
        private GameObject damageEffect;

        // Animator参照用（中山が編集）
        [SerializeField]
        private Animator animator;

        [SerializeField]
        GameObject stunEffect;

        [SerializeField]
        Vector3 stunEffectPos;

        [SerializeField]
        Vector3 stunEffectScale;

        // 攻撃エフェクトオブジェクト参照用（中山が編集）
        [SerializeField]
        private ParticleSystem driftParticle1, driftParticle2, driftParticle3, driftParticle4;
        [SerializeField]
        private ParticleSystem rushParticle1, rushParticle2, rushParticle3, rushParticle4, rushParticle5, rushParticle6, rushParticle7, rushParticle8;

        private GameObject stunEffectObject;

        static readonly int defeatId = Animator.StringToHash("defeat");//死亡モーション用パラメーターID（中山が編集）

        private bool isPlayerCheckColliderEntered = false;
        private float stunTimer = 0;
        private bool isStunning;
        void Start()
        {
            rb = GetComponent<Rigidbody>();


            statusManager = GetComponent<StatusManagerBoss>();


            // find with tagってやっていいのかな
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();


            statusManager.OnDeath += Die;
            statusManager.OnStunTaken += TakeStun;
            playerCheckCollider.Enter += OnPlayerCheckColliderEnter;
            statusManager.OnDamageTaken += TakeDamage;//ダメージエフェクト再生用（中山が編集）

            attackCollider.enabled = false;//攻撃判定無効化（中山が編集）
            playerCheckCollider.Hide();

            //ドリブルエフェクト停止（中山が編集）
            driftParticle1.Stop();
            driftParticle2.Stop();
            driftParticle3.Stop();
            driftParticle4.Stop();

            //突進エフェクト停止（中山が編集）
            rushParticle1.Stop();
            rushParticle2.Stop();
            rushParticle3.Stop();
            rushParticle4.Stop();
            rushParticle5.Stop();
            rushParticle6.Stop();
            rushParticle7.Stop();
            rushParticle8.Stop();

            // 行動のコルーチンを起動
            StartCoroutine(StartMotion());
        }

        public void Die()
        {
            AudioPlayer.instance.StopSE(); // BossSE停止（中山が編集）
            animator.SetTrigger(defeatId);//死亡モーション再生（中山が編集）
            AudioPlayer.instance.PlaySE(12, 1);
            attackCollider.enabled = false;
            StopAllCoroutines();
            StartCoroutine(DeathTimer());
        }
        IEnumerator DeathTimer()
        {
            AudioPlayer.instance.PlaySE(10, 0.5f); // BossDefeatを再生（中山が編集）
            yield return new WaitForSeconds(deathTime);
            AudioPlayer.instance.StopSE();
            yield return new WaitForSeconds(deathLittleTime);
            StageScene.Instance.StageClear();
            AudioPlayer.instance.PlaySE(13, 0.5f); // BossDestroyを再生（富里が編集）
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            if (statusManager != null)
            {
                statusManager.OnDeath -= Die;// 死亡時の処理解除（中山が編集）
                statusManager.OnDamageTaken -= TakeDamage;// ダメージエフェクト用（中山が編集）
                playerCheckCollider.Enter -= OnPlayerCheckColliderEnter;
                statusManager.OnStunTaken -= TakeStun;
            }
        }

        IEnumerator MainLoop()
        {

            // 基本のループ
            while (true)
            {
                yield return StartCoroutine(MainMotion());

                yield return StartCoroutine(Stun(defaultStunTime));
            }
        }

        IEnumerator StartMotion()
        {
            yield return new WaitForSeconds(startMotionTime);
            StartCoroutine(MainLoop());
        }

        IEnumerator MainMotion()
        {
            // HP8以下なら7回、でなければ5回繰り返す
            for (int i = 0; i < (statusManager.Health <= 8 ? 7 : 5); i++)
            {
                rushWaitTime = 1;
                yield return StartCoroutine(Aim());
                if (isPlayerCheckColliderEntered)
                {
                    // Drift
                    yield return StartCoroutine(Drift());
                    // 突進回数にはカウントしない
                    i--;
                }
                else
                {
                    yield return StartCoroutine(Rush());
                }
            }
        }

        IEnumerator Aim()
        {
            float timer = 0;
            isPlayerCheckColliderEntered = false;
            playerCheckCollider.Show();
            while (timer <= rushWaitTime)
            {
                // ここでプレイヤーの方を向いてる
                // 移動方向を取得
                Vector3 relativePos = player.transform.position - transform.position;
                // Yをなくす
                relativePos.y = 0;
                // 方向を、回転情報に変換
                Quaternion rotation = Quaternion.LookRotation(relativePos);
                // 現在の回転情報と、ターゲット方向の回転情報を補完する
                rb.rotation = Quaternion.Slerp(rb.rotation, rotation, rotateSpeed * Time.fixedDeltaTime);

                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            playerCheckCollider.Hide();
        }

        public void OnPlayerCheckColliderEnter()
        {
            isPlayerCheckColliderEntered = true;
        }

        IEnumerator Drift()
        {
            AudioPlayer.instance.PlaySE(9); // BossDriftを再生（中山が編集）
            attackCollider.enabled = true;
            float rotatedDegree = 0;
            Quaternion startRot = rb.rotation;

            // ドリブルエフェクト再生（中山が編集）
            driftParticle1.Play();
            driftParticle2.Play();
            driftParticle3.Play();
            driftParticle4.Play();

            while (rotatedDegree <= 360)
            {
                float delta = 180f * Time.fixedDeltaTime;
                rotatedDegree += delta;

                Quaternion rotation = Quaternion.Euler(0f, rotatedDegree, 0f);

                rb.MoveRotation(startRot * rotation);

                yield return new WaitForFixedUpdate();
            }
            attackCollider.enabled = false;
        }

        IEnumerator Rush()
        {
            AudioPlayer.instance.PlaySE(11); // BossRushを再生（中山が編集）
            float timer = 0;
            Vector3 rushDirection = transform.forward;
            bool isCasted = false;
            attackCollider.enabled = true;

            // 突進エフェクト再生（中山が編集）
            rushParticle1.Play();
            rushParticle2.Play();
            rushParticle3.Play();
            rushParticle4.Play();
            rushParticle5.Play();
            rushParticle6.Play();
            rushParticle7.Play();
            rushParticle8.Play();

            while (timer <= 2 && !isCasted)
            {
                // ここで力を加える
                rb.linearVelocity = rushDirection * moveSpeed;
                // 壁にぶつかったらbreakさせよう
                // すべてのoffsetで繰り返す
                for (int i = 0; i < wallCheckerPos.Length; i++)
                {
                    Vector3 offset = transform.forward * wallCheckerPos[i].x + transform.right * wallCheckerPos[i].z;
                    offset.y = wallCheckerPos[i].y;

                    isCasted = Physics.Raycast(transform.position + offset, rushDirection, wallCheckerDistance, groundLayer);
                    // 一個でもtrueがあったらbreakして
                    if (isCasted) break;
                }

                if (isCasted)
                {
                    // trueならwaittimeを短くする上にwhileを抜ける
                    rushWaitTime = 0.5f;
                }
                else
                {
                    rushWaitTime = 1;
                }
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            attackCollider.enabled = false;
        }

        IEnumerator Stun(float stunTime)
        {
            stunTimer = stunTime;
            isStunning = true;
            while (stunTimer >= 0)
            {
                stunTimer -= Time.deltaTime;
                yield return null;
            }
            if (stunEffectObject != null)
            {
                Destroy(stunEffectObject);
            }
            rb.linearVelocity = new Vector3(0, stunEndJumpForce, 0);
            yield return new WaitForSeconds(stunEndAnimTime);
            isStunning = false;
        }

        [ContextMenu("デバッグ用すぐすたーん")]
        private void TakeStun()
        {
            if (!isStunning)
            {
                // 現在のコルーチンを止めてひるむ
                StopAllCoroutines();
                StartCoroutine(OnStunTaken());
            }
            else
            {
                // ひるむ時間を３秒くらいのばす
                stunTimer = player.StunSkillTime;
            }
        }

        IEnumerator OnStunTaken()
        {
            playerCheckCollider.Hide();
            attackCollider.enabled = false;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            stunEffectObject = Instantiate(stunEffect, transform.localPosition + stunEffectPos, Quaternion.identity);
            stunEffectObject.transform.localScale = stunEffectScale;
            yield return StartCoroutine(Stun(player.StunSkillTime));
            StartCoroutine(MainLoop());
        }


        private void OnDrawGizmos()
        {
            Vector3 rushDirection = transform.forward;
            for (int i = 0; i < wallCheckerPos.Length; i++)
            {
                Vector3 offset = transform.forward * wallCheckerPos[i].x + transform.right * wallCheckerPos[i].z;
                offset.y = wallCheckerPos[i].y;

                Gizmos.color = Color.red;

                Gizmos.DrawLine(transform.position + offset, transform.position + offset + rushDirection * wallCheckerDistance);
            }
        }

        private void TakeDamage()
        {
            // エフェクトをインスタンス化
            GameObject effect = Instantiate(damageEffect);

            effect.transform.position = player.PlayerHand.transform.position;// 攻撃コライダーの位置にエフェクトを出す（中山が編集）

            Destroy(effect, 5);// エフェクトを5秒後に破壊（中山が編集）
        }
    }
}