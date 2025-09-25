using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private float startPosX = 1.4f;
    private float startPosY = 0.4f;
    private float startPosZ = 0;

    private float movePosY = 0.8f;

    private MeshRenderer meshRenderer;
    new private Collider collider;

    private float totalMoveDegree;
    private float attackRotationSpeed;
    private float rotateCount;

    private bool isEnable = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();

        meshRenderer.enabled = false;
        collider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (isEnable) Move();
    }

    // ‰~’Œ‚Ì•¨‚ðŒX‚¯‚é
    public void Init(float startDeg, float totalDeg, float speed)
    {
        meshRenderer.enabled = true;
        collider.enabled = true;
        isEnable = true;

        transform.localPosition = new Vector3(startPosX, startPosY, startPosZ);

        transform.rotation = Quaternion.Euler(0, 0, startDeg);

        totalMoveDegree = totalDeg;

        attackRotationSpeed = speed;

        rotateCount = 0;

        Move();
    }

    private void Move()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z -= attackRotationSpeed * Time.deltaTime;
        rotateCount += attackRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotation);

        Vector3 pos = transform.position;
        pos.y -= movePosY * Time.deltaTime;
        transform.position = pos;

        Debug.Log(rotation.z);

        Debug.Log(totalMoveDegree);

        if (rotateCount >= totalMoveDegree)
        {
            End();
        }
    }

    private void End()
    {
        meshRenderer.enabled = false;
        collider.enabled = false;
        isEnable = false;
    }
}
