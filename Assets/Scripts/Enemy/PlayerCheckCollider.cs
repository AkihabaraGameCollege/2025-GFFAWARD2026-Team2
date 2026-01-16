using System;
using UnityEngine;

namespace QuickTheFury.Enemy
{
    /// <summary>
    /// Playerがコライダー内にいるか検知する
    /// </summary>
    public class PlayerCheckCollider : MonoBehaviour
    {
        public event Action Enter;
        private Collider thisCollider;
        private void Awake()
        {
            thisCollider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Enter?.Invoke();
        }

        /// <summary>
        /// コライダーオンオフ切り替え
        /// </summary>
        /// <param name="enabled">有効化ならtrue無効化ならfalse</param>
        public void SetActive(bool enabled)
        {
            thisCollider.enabled = enabled;
        }
    }
}