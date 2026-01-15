using UnityEngine;

namespace Assets.Scripts.Enemy
{
    /// <summary>
    /// “G‚Ì‹­§ƒXƒ^ƒ“‚Ìeffect‚ğŠÇ—
    /// </summary>
    public class StunEffect : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] stars;

        [SerializeField]
        private float rotateSpeed = 180;

        private void Start()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                float radian = Mathf.PI * 2 / stars.Length * i;
                stars[i].transform.localPosition = new Vector3
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
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].transform.forward = Camera.main.transform.forward;
            }
        }
    }
}