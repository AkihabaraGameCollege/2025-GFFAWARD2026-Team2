using System;
using UnityEngine;

namespace QuickTheFury
{
    // ボスのステータスに関するスクリプト（中山が別プロジェクトから移植）
    public class StatusManagerBoss : MonoBehaviour
    {
        //hp現在値
        [SerializeField]
        public int maxHealth = 15;
        public int health;
        //ラストスパートHP（中山が編集）
        private int lastSpurtHP;
        //ラストスパートBGM（中山が編集）
        [SerializeField]
        private int bossLastBGM = 1;

        [SerializeField]
        [Tooltip("HitBox")]
        private HitboxEnemy hitbox;


        private bool isAlreadyPlayed = false;//ラストスパートBGM再生判定用（中山が編集）

        public bool isInvincible = false;

        public event Action OnDamageTaken;
        public event Action OnDeath;
        public event Action OnStunTaken;

        // 登録用（中山が編集）
        void Awake()
        {

            hitbox.HitAction += Hit;

            health = maxHealth;

            lastSpurtHP = maxHealth / 3;
        }

        public void Hit(int damage, bool stun)
        {
            if (!isInvincible)
            {

                // HPを減少させ、ダメージエフェクトを発生させる
                health -= damage;

                MainStageScene.Instance.BossBarUpdate(health, maxHealth);//HPゲージを減少させる（中山が編集）



                // ラストスパートBGM再生判定（中山が編集）
                if (health <= lastSpurtHP && !isAlreadyPlayed)
                {
                    AudioPlayer.Instance.PlayBGM(bossLastBGM);// ラストスパートBGM再生（中山が編集）
                    isAlreadyPlayed = true;// 2回目以降再生されないようにする（Tomisatoが編集）
                }

                OnDamageTaken?.Invoke();

                if (health <= 0)
                {
                    isInvincible = true;
                    OnDeath?.Invoke();
                }
            }

            if (stun)
            {
                OnStunTaken?.Invoke();
            }
        }

        void OnDestroy()
        {
            if (hitbox != null)
            {
                hitbox.HitAction -= Hit;
            }
        }
    }
}