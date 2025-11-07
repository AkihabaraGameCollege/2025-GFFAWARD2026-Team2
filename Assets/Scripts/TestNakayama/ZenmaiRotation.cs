using UnityEngine;
using System.Collections;

// ゼンマイを回転させるスクリプト（中山が編集）
public class ZenmaiRotation : MonoBehaviour
{
    // 回転速度を調整する変数（中山が編集）
    [SerializeField]
    private float rotationSpeed = 0.5f;
    // 待機時間を調整する変数（中山が編集）
    [SerializeField]
    private float waitTime = 0.001f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       StartCoroutine(Rotation());// ゼンマイを回転させる（中山が編集）
    }

    // ゼンマイをずっと回転させるコルーチン（中山が編集）
    IEnumerator Rotation()
    {
        // 無限ループで回転させ続ける（中山が編集）
        while (true)
        {
            transform.Rotate(rotationSpeed, 0, 0); // X軸回りに？度回転させる（中山が編集）
            yield return new WaitForSeconds(waitTime); // ？秒待つ（中山が編集）
        }
    }
}