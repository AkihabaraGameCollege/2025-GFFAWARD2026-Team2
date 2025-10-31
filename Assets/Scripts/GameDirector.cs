using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    GameObject hpGauge;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.hpGauge = GameObject.Find("hpGauge");
    }

    // Update is called once per frame
    public void DecreaseHp()
    {
        this.hpGauge.GetComponent<Image>().fillAmount -= 0.34f;
    }
}
