
using UnityEngine;

public class FlashlightReveal : MonoBehaviour
{
    [Header("Flashlight Reference")]
    public Light flashlight;

    private Renderer[] objectRenderers;
    private Collider targetCollider;
    private bool isVisible;

    private void Awake()
    {
        objectRenderers = GetComponentsInChildren<Renderer>(true);
        targetCollider = GetComponentInChildren<Collider>(true);
        
        SetVisible(false);

        if (targetCollider == null)
        {
            Debug.LogWarning(
                name + ": A Collider is required for flashlight detection.",
                this
            );
        }
    }

    private void Update()
    {
        bool shouldBeVisible = IsInsideFlashlight();

        if (shouldBeVisible != isVisible)
            SetVisible(shouldBeVisible);
    }

    private bool IsInsideFlashlight()
    {
        if (flashlight == null ||
            !flashlight.enabled ||
            !flashlight.gameObject.activeInHierarchy ||
            flashlight.type != LightType.Spot ||
            targetCollider == null)
        {
            return false;
        }

        Vector3 origin = flashlight.transform.position;
        Vector3 target = targetCollider.bounds.center;
        Vector3 direction = target - origin;

        float distance = direction.magnitude;

        if (distance <= 0.01f || distance > flashlight.range)
            return false;
        
        float angle = Vector3.Angle(
            flashlight.transform.forward,
            direction.normalized
        );

        if (angle > flashlight.spotAngle * 0.5f)
            return false;

        if (Physics.Raycast(
            origin,
            direction.normalized,
            out RaycastHit hit,
            distance + 0.05f,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
        {
            Transform hitTransform = hit.collider.transform;

            return hitTransform == transform ||
                   hitTransform.IsChildOf(transform);
        }

        return false;
    }

    private void SetVisible(bool visible)
    {
        isVisible = visible;

        foreach (Renderer objectRenderer in objectRenderers)
        {
            if (objectRenderer != null)
                objectRenderer.enabled = visible;
        }
    }
}
