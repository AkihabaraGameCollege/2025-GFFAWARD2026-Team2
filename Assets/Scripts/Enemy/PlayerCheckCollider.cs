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

        public void Show()
        {
            thisCollider.enabled = true;
        }

        public void Hide()
        {
            thisCollider.enabled = false;
        }
    }
}