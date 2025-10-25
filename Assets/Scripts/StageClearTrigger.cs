using UnityEngine;
using UnityEngine.Events;

    // ステージクリアー判定のためのゴールを表します。
    public class StageClearTrigger : MonoBehaviour
    {
        // トリガー内に侵入した際に発生する UnityEvent を取得または設定します。
        public UnityEvent OnEnter { get => onEnter; set => onEnter = value; }
        [SerializeField]
        [Tooltip("トリガー内に侵入した際に発生する UnityEvent")]
        private UnityEvent onEnter = null;

        // トリガー内に他のオブジェクトが侵入してきた際に呼び出されます。
        private void OnTriggerEnter(Collider collision)
        {
            // ステージクリアー判定
            if (collision.CompareTag("Boss"))
            {
                OnEnter.Invoke();
            }
        }
    }