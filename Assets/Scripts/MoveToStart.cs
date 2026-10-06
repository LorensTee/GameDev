using UnityEngine;

public class MoveToStart : MonoBehaviour
{
    public float speed = 6f;
    public float destroyZ = -62f;

    private void Update()
    {
        transform.position += Vector3.back * (speed * Time.deltaTime);

        if (transform.position.z <= destroyZ)
            Destroy(gameObject);
    }
}
