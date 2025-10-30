using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    GameObject gameOver;
    GameObject hpGauge;
    GameObject BGM;
    GameObject gameDirector;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.hpGauge = GameObject.Find("hpGauge");
        this.gameOver = GameObject.Find("gameOver");
        this.BGM = GameObject.Find("BGM");
         this. gameDirector= GameObject.Find("gameDirector");
        this.gameOver.SetActive(false);
    }

    // Update is called once per frame
    public void DecreaseHp()
    {
        this.hpGauge.GetComponent<Image>().fillAmount -= 0.1f;
        if (this.hpGauge.GetComponent<Image>().fillAmount <= 0) 
        {
            this.gameOver.SetActive(true);
            this.BGM.SetActive(false);
            this.gameDirector.SetActive(false);
            StartCoroutine(OnStart());

        }

        IEnumerator OnStart()
        {
            yield return new WaitForSeconds(3);
            if (Input.GetButtonDown("Submit"))
                SceneManager.LoadScene("SampleScene");
            
        }
    }
}
