using UnityEngine;

namespace QuickTheFury
{
    /// <summary>
    /// ターゲットをスタンさせたときに表示する星エフェクトのクラス
    /// </summary>
    public class StunEffect : MonoBehaviour
    {
        // 星エフェクトの配列
        [SerializeField]
        private GameObject[] stars;

        // 回転速度（度/秒）
        [SerializeField]
        private float rotateSpeed = 180;

        /// <summary>
        /// 回転開始時に星を円形に配置する関数
        /// </summary>
        private void Start()
        {
            // 星を円形に配置
            for (int i = 0; i < stars.Length; i++)
            {
                float radian = Mathf.PI * 2 / stars.Length * i;// ラジアン角度計算

                // 星の位置設定
                stars[i].transform.localPosition = new Vector3
                    (
                        Mathf.Cos(radian),// X座標
                        0,// Y座標
                        Mathf.Sin(radian)// Z座標
                    );
            }
        }

        /// <summary>
        /// 星の回転と星画像がカメラに正面を向き続ける処理の関数
        /// </summary>
        private void Update()
        {
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);// 星全体の回転

            // 各星が常にカメラの方を向くようにする
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].transform.forward = Camera.main.transform.forward;// カメラの方を向く
            }
        }
    }
}