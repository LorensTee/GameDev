using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float launchSpeed = 25f;
    [SerializeField] private float maxAimDistance = 100f;
    [SerializeField] private float fireDelay = 0.25f;

    private float nextShotTime;
    private Collider[] shooterColliders;

    private void Awake()
    {
        if (aimCamera == null)
        {
            aimCamera = GetComponent<Camera>();
        }

        if (transform.parent != null)
        {
            shooterColliders = transform.parent.GetComponentsInChildren<Collider>();
        }
        else
        {
            shooterColliders = GetComponentsInChildren<Collider>();
        }
    }

    private void Update()
    {
        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame ||
            Time.time < nextShotTime)
        {
            return;
        }

        Shoot();
    }

    private void Shoot()
    {
        nextShotTime = Time.time + fireDelay;

        Ray aimRay = aimCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f));

        Vector3 targetPoint = aimRay.GetPoint(maxAimDistance);

        if (Physics.Raycast(
            aimRay,
            out RaycastHit hit,
            maxAimDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
        {
            targetPoint = hit.point;
        }

        Vector3 direction =
            (targetPoint - muzzlePoint.position).normalized;

        GameObject projectile = Instantiate(
            projectilePrefab,
            muzzlePoint.position,
            Quaternion.LookRotation(direction));

        Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();

        projectileBody.linearVelocity = direction * launchSpeed;

        Collider[] projectileColliders = projectile.GetComponentsInChildren<Collider>();

        foreach (Collider playerCollider in shooterColliders)
        {
            if (playerCollider is null)
            {
                continue;
            }

            foreach (Collider projectileCollider in projectileColliders)
            {
                if (projectileCollider is not null)
                {
                    Physics.IgnoreCollision(
                        projectileCollider,
                        playerCollider);
                }
            }
        }
    }
}