using System;
using UnityEngine;

namespace QuickTheFury
{
    /// <summary>
    /// アクションに応じた効果音を再生するためのイベントを管理するクラス
    /// </summary>
    public class ActionSoundsPlayer : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーの歩行に応じた効果音を再生するイベントを参照する変数
        /// </summary>
        public event Action PlayWalkSE;

        /// <summary>
        /// アクションに応じた歩行効果音を再生するイベントを発火する関数
        /// </summary>
        public void PlaySE()
        {
            PlayWalkSE?.Invoke();// イベント発火
        }
    }
}