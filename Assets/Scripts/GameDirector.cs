using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    [SerializeField]
    GameObject playerLifeImage = null;

    // Update is called once per frame
    public void DecreaseHp()
    {
        this.playerLifeImage.GetComponent<Image>().fillAmount -= 0.34f;
    }
}