using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HealthManager health = other.GetComponent<HealthManager>();
        if (health == null)
            health = other.GetComponentInParent<HealthManager>();
        if (health == null)
            return;

        if (GameManager.Instance != null && !GameManager.Instance.IsRunActive)
            GameManager.Instance.StartTimer();
    }
}
