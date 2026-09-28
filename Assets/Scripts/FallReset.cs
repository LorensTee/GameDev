using UnityEngine;

public class FallReset : MonoBehaviour
{
    private Vector3 startPosition;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        startPosition = player.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        rb.position = startPosition;
        rb.linearVelocity = Vector3.zero;
    }
}