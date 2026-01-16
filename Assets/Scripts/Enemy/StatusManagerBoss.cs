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


        private bool isAlreadyOveredLowHPLine = false;//ラストスパートBGM再生判定用（中山が編集）

        public bool isInvincible = false;

        public event Action OnDamageTaken;
        public event Action OnDeath;
        public event Action OnStunTaken;

        // 登録用（中山が編集）
        void Awake()
        {
            hitbox.OnHit += Hit;

            health = maxHealth;
        }

        public void Hit(int damage, bool stun)
        {
            if (!isInvincible)
            {
                if (!isAlreadyOveredLowHPLine && health <= maxHealth / 3)
                {
                    StageScene.Instance.PlayLowHealthMusic();

                    // 2回目以降再生されないようにする
                    isAlreadyOveredLowHPLine = true;
                }
            }

            Damage(damage);

            if (stun)
            {
                OnStunTaken?.Invoke();
            }
        }

        public void Damage(int value)
        {
            health -= value;
            StageScene.Instance.UpdateBossBar(health, maxHealth);
            OnDamageTaken?.Invoke();

            if (health <= 0)
            {
                isInvincible = true;
                OnDeath?.Invoke();
            }
        }

        public void Heal(int value)
        {
            health += value;

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