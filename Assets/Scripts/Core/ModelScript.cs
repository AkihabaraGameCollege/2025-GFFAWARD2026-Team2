using System;
using UnityEngine;

namespace QuickTheFury.Core
{
    /// <summary>
    /// AnimatorからMoveScriptにEventを送る
    /// </summary>
    public class ModelScript : MonoBehaviour
    {
        /// <summary>
        /// AnimationEventで呼び出される足音再生のEvent
        /// </summary>
        public event Action PlayWalkSE;

        /// <summary>
        /// AnimationEventで呼び出される足音再生のメソッド
        /// </summary>
        public void OnPlayWalkSE()
        {
            PlayWalkSE?.Invoke();
        }
    }
}