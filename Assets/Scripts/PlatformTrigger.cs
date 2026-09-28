using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private PlatformMover targetPlatform;

    private void OnTriggerEnter(Collider other)
    {
        targetPlatform.Activate();
    }
}