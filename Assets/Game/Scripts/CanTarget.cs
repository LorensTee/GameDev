using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CanTarget : MonoBehaviour
{
    private Rigidbody body;
    private CanResetManager resetManager;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool countedAsFallen;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        resetManager = GetComponentInParent<CanResetManager>();

        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (countedAsFallen ||
            !collision.collider.CompareTag("Floor"))
        {
            return;
        }

        countedAsFallen = true;

        if (resetManager != null)
        {
            resetManager.RegisterCanOnFloor(this);
        }
    }

    public void PrepareForReset()
    {
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;

        transform.localPosition = startPosition;
        transform.localRotation = startRotation;

        countedAsFallen = false;
    }

    public void FinishReset()
    {
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = false;
    }
}