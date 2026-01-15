using System;
using UnityEngine;

namespace QuickTheFury
{
    /// <summary>
    /// アクションに応じた効果音を再生するためのイベントを管理するクラス
    /// </summary>
    public class ActionSounds : MonoBehaviour
    {
        public event Action PlayWalkSE;// 歩行効果音再生イベントの定義

        /// <summary>
        /// アクションに応じた歩行効果音を再生するイベントを発火する関数
        /// </summary>
        public void OnPlayWalkSE()
        {
            PlayWalkSE?.Invoke();// イベント発火
        }
    }
}