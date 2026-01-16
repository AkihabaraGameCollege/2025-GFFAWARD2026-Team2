using Assets.Scripts.Scene;
using System;
using UnityEngine;

namespace QuickTheFury.Enemy
{
    /// <summary>
    /// Boss用のステータス管理Class
    /// </summary>
    public class StatusManagerBoss : MonoBehaviour
    {
        [SerializeField]
        private int maxHealth = 15;
        public int MaxHealth => maxHealth;

        private int health;
        public int Health => health;

        [SerializeField]
        [Tooltip("HitBox")]
        private HitboxEnemy hitbox;

        // LowHPLineを過ぎたかどうかのフラグ
        private bool isAlreadyOveredLowHPLine = false;

        // 無敵時間中かどうかのフラグ
        private bool isInvincible = false;

        public event Action OnDamageTaken;
        public event Action OnDeath;
        public event Action OnStunTaken;

        void Awake()
        {
            hitbox.OnHit += Hit;

            health = maxHealth;

            isInvincible = false;
        }

        /// <summary>
        /// 被弾の処理
        /// </summary>
        /// <param name="damage">ダメージ量</param>
        /// <param name="stun">スタン攻撃かどうか</param>
        public void Hit(int damage, bool stun)
        {
            if (!isInvincible)
            {
                // Music変更のラインを下回ったら実行
                if (!isAlreadyOveredLowHPLine && health <= maxHealth / 3)
                {
                    StageScene.Instance.PlayLowHealthMusic();

                    // 2回目以降再生されないようにする
                    isAlreadyOveredLowHPLine = true;
                }

                Damage(damage);
            }

            if (stun)
            {
                OnStunTaken?.Invoke();
            }
        }

        /// <summary>
        /// ダメージをHPに適応させる
        /// </summary>
        /// <param name="value">ダメージ量</param>
        public void Damage(int value)
        {
            health -= value;
            StageScene.Instance.UpdateBossBar(health, maxHealth);
            OnDamageTaken?.Invoke();

            // 死亡してたら実行
            if (health <= 0)
            {
                isInvincible = true;
                OnDeath?.Invoke();
            }
        }

        /// <summary>
        /// 回復をHPに適応
        /// </summary>
        /// <param name="value">回復量</param>
        public void Heal(int value)
        {
            health += value;

            // 最大量超えたらclamp
            if (health > maxHealth)
            {
                 health = maxHealth;
            }
        }

        private void OnDestroy()
        {
            if (hitbox != null)
            {
                hitbox.OnHit -= Hit;
            }
        }
    }
}