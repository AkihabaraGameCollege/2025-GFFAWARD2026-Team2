using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// 子オブジェクトにつけたParticleSystemを再生したりする
    /// </summary>
    [Serializable]
    public class Effect
    {
        [SerializeField]
        public GameObject objectParent;
        private ParticleSystem[] particleSystems;
        /// <summary>
        /// 初期化メソッド
        /// </summary>
        public void Init()
        {
            particleSystems = objectParent.GetComponentsInChildren<ParticleSystem>();
            Stop();
        }

        /// <summary>
        /// 子オブジェクトらをまとめて再生
        /// </summary>
        public void Play()
        {
            if (particleSystems == null)
            {
                Debug.LogError("初期化されていません.。Init();してください");
                return;
            }
            foreach (var particle in particleSystems)
            {
                particle.Play();
            }
        }

        /// <summary>
        /// 子オブジェクトらをまとめて停止
        /// </summary>
        public void Stop()
        {
            if (particleSystems == null)
            {
                Debug.LogError("初期化されていません.。Init();してください");
                return;
            }
            foreach (var particle in particleSystems)
            {
                particle.Stop();
            }
        }
    }
}