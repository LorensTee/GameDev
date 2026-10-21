using UnityEngine;

public class MovingHazard : MonoBehaviour
{
    public enum DetectionShape
    {
        Sphere,
        Box,
        Capsule
    }

    [Header("Movement")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 0.25f;

    [Header("Detection Shape")]
    public DetectionShape shape = DetectionShape.Sphere;
    public LayerMask playerLayer;
    public Vector3 detectionOffset = Vector3.zero;

    public float sphereRadius = 2f;
    public Vector3 boxHalfExtents = new Vector3(1.5f, 1.5f, 1.5f);
    public float capsuleRadius = 0.8f;
    public float capsuleHeight = 3.5f;

    [Header("Raycast")]
    public float raycastDistance = 8f;

    [Header("Detection Cooldown")]
    public float resetCooldown = 2.5f;

    private Transform player;
    private static float nextDetectionAllowedTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        nextDetectionAllowedTime = 0f;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            player = GameManager.Instance.player;

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player == null)
        {
            Debug.LogError(
                name + ": Could not find the player!",
                this
            );
            return;
        }

        Debug.Log(
            name + ": Found player " + player.name,
            this
        );
    }

    private bool wasInsideZone;
    private bool raycastWarningShown;

    private void Update()
    {
        MoveHazard();

        if (player == null)
            return;

        if (!IsPlayerInDetectionZone())
        {
            wasInsideZone = false;
            raycastWarningShown = false;
            return;
        }

        if (!wasInsideZone)
        {
            Debug.Log(
                name + ": Player entered the detection zone.",
                this
            );

            wasInsideZone = true;
        }

        if (!HasLineOfSight())
        {
            if (!raycastWarningShown)
            {
                Debug.LogWarning(
                    name + ": Player is in the detection zone, " +
                    "but the raycast did not reach the player.",
                    this
                );

                raycastWarningShown = true;
            }

            return;
        }

        raycastWarningShown = false;

        if (Time.time < nextDetectionAllowedTime)
            return;

        nextDetectionAllowedTime = Time.time + resetCooldown;

        Debug.LogWarning(
            name + " detected the player! Resetting.",
            this
        );

        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
    }

    private void MoveHazard()
    {
        if (pointA == null || pointB == null)
            return;

        float t = Mathf.PingPong(Time.time * moveSpeed, 1f);

        transform.position = Vector3.Lerp(
            pointA.position,
            pointB.position,
            t
        );
    }

    private bool IsPlayerInDetectionZone()
    {
        Vector3 center = transform.TransformPoint(detectionOffset);

        switch (shape)
        {
            case DetectionShape.Sphere:
                return Physics.CheckSphere(
                    center,
                    sphereRadius,
                    playerLayer,
                    QueryTriggerInteraction.Ignore
                );

            case DetectionShape.Box:
                return Physics.CheckBox(
                    center,
                    boxHalfExtents,
                    transform.rotation,
                    playerLayer,
                    QueryTriggerInteraction.Ignore
                );

            case DetectionShape.Capsule:
                float halfSegment = Mathf.Max(
                    0f,
                    capsuleHeight * 0.5f - capsuleRadius
                );

                Vector3 top = center + transform.up * halfSegment;
                Vector3 bottom = center - transform.up * halfSegment;

                return Physics.CheckCapsule(
                    top,
                    bottom,
                    capsuleRadius,
                    playerLayer,
                    QueryTriggerInteraction.Ignore
                );
        }

        return false;
    }

    private bool HasLineOfSight()
    {
        Vector3 origin = transform.TransformPoint(detectionOffset);
        Vector3 target = player.position + Vector3.up * 0.8f;
        Vector3 direction = target - origin;

        float distance = direction.magnitude;

        if (distance <= 0.01f || distance > raycastDistance)
            return false;

        if (Physics.Raycast(
            origin,
            direction.normalized,
            out RaycastHit hit,
            distance + 0.05f,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
        {
            return hit.transform == player ||
                   hit.transform.IsChildOf(player);
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.TransformPoint(detectionOffset);

        Gizmos.color = Color.yellow;

        if (shape == DetectionShape.Sphere)
        {
            Gizmos.DrawWireSphere(center, sphereRadius);
        }
        else if (shape == DetectionShape.Box)
        {
            Matrix4x4 previousMatrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(
                center,
                transform.rotation,
                Vector3.one
            );

            Gizmos.DrawWireCube(
                Vector3.zero,
                boxHalfExtents * 2f
            );

            Gizmos.matrix = previousMatrix;
        }
        else if (shape == DetectionShape.Capsule)
        {
            float halfSegment = Mathf.Max(
                0f,
                capsuleHeight * 0.5f - capsuleRadius
            );

            Vector3 top = center + transform.up * halfSegment;
            Vector3 bottom = center - transform.up * halfSegment;

            Gizmos.DrawWireSphere(top, capsuleRadius);
            Gizmos.DrawWireSphere(bottom, capsuleRadius);

            Gizmos.DrawLine(
                top + transform.right * capsuleRadius,
                bottom + transform.right * capsuleRadius
            );

            Gizmos.DrawLine(
                top - transform.right * capsuleRadius,
                bottom - transform.right * capsuleRadius
            );
        }

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}