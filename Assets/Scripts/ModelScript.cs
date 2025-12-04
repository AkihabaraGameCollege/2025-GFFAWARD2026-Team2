using System;
using UnityEngine;

public class ModelScript : MonoBehaviour
{
    public event Action PlayWalkSE; 

    public void OnPlayWalkSE()
    {
        PlayWalkSE?.Invoke();
    }
}
