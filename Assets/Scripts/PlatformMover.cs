using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlatformMover : MonoBehaviour
{
    public enum MovementType
    {
        Rotate,
        Vertical,
        Horizontal
    }

    [SerializeField] private MovementType movementType = MovementType.Rotate;
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float moveDistance = 3f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float elapsedTime;
    private bool activated;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void Activate()
    {
        activated = true;
    }

    private void FixedUpdate()
    {
        if (!activated)
        {
            return;
        }

        elapsedTime += Time.fixedDeltaTime;

        if (movementType == MovementType.Rotate)
        {
            Quaternion rotation = startRotation * Quaternion.Euler(0f, rotationSpeed * elapsedTime, 0f);

            rb.MoveRotation(rotation);
            return;
        }

        float offset = Mathf.Sin(elapsedTime * moveSpeed) * moveDistance;

        Vector3 direction = movementType == MovementType.Vertical ? Vector3.up : Vector3.right;

        rb.MovePosition(startPosition + direction * offset);
    }
}