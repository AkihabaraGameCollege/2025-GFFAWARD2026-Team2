using UnityEngine;

namespace Assets.Scripts.Enemy
{
    /// <summary>
    /// 敵の強制スタン時のeffectを管理
    /// </summary>
    public class StunEffect : MonoBehaviour
    {
        private Transform[] stars;

        [SerializeField]
        private float rotateSpeed = 180;

        private void Start()
        {
            // 子オブジェクトを配列にぶち込む
            stars = new Transform[transform.childCount];
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = transform.GetChild(i);
            }

            // すべてのstarを円周上に等間隔に並べる
            for (int i = 0; i < stars.Length; i++)
            {
                float radian = Mathf.PI * 2 / stars.Length * i;
                stars[i].localPosition = new Vector3
                    (
                        Mathf.Cos(radian),
                        0,
                        Mathf.Sin(radian)
                    )
                    ;
            }
        }

        private void Update()
        {
            // 回転させる
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].forward = Camera.main.transform.forward;
            }
        }
    }
}