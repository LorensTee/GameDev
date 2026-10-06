using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HealthManager health = other.GetComponent<HealthManager>();

        if (health == null)
            health = other.GetComponentInParent<HealthManager>();

        if (health != null)
            GameManager.Instance.WinRun();
    }
}
