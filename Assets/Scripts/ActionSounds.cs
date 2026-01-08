using System;
using UnityEngine;

public class ActionSounds : MonoBehaviour
{
    public event Action PlayWalkSE; 

    public void OnPlayWalkSE()
    {
        PlayWalkSE?.Invoke();
    }
}
