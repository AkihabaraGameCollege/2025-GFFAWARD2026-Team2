using System;
using UnityEngine;

/// <summary>
/// Animator‚©‚çMoveScript‚ÉEvent‚ð‘—‚é
/// </summary>
public class ModelScript : MonoBehaviour
{
    public event Action PlayWalkSE; 

    public void OnPlayWalkSE()
    {
        PlayWalkSE?.Invoke();
    }
}
