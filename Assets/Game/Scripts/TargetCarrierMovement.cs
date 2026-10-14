using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetCarrierMovement : MonoBehaviour
{
    [SerializeField] private float travelDistance = 9f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private bool moveNegativeXFirst = true;

    private Rigidbody body;
    private Vector3 startPosition;
    private float travelProgress;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;

        startPosition = body.position;
    }

    private void FixedUpdate()
    {
        travelProgress += Time.fixedDeltaTime * moveSpeed;

        float offset = Mathf.PingPong(travelProgress, travelDistance);

        float direction = moveNegativeXFirst ? -1f : 1f;

        Vector3 targetPosition = startPosition + Vector3.right * (offset * direction);

        body.MovePosition(targetPosition);
    }

    public void ResetMovement()
    {
        travelProgress = 0f;
        body.position = startPosition;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }
}