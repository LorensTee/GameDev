using UnityEngine;
using StarterAssets;

public class BonusPanel : MonoBehaviour
{
    public enum BonusType
    {
        Heal,
        SpeedBoost
    }

    [Header("Bonus")]
    public BonusType bonusType;

    [Header("Heal")]
    public int healAmount = 1;

    [Header("Speed Boost")]
    public float speedMultiplier = 2f;
    public float speedDuration = 3f;

    private bool used;
    private Collider panelCollider;

    private void Awake()
    {
        panelCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used)
            return;

        HealthManager health = other.GetComponent<HealthManager>();

        if (health == null)
            health = other.GetComponentInParent<HealthManager>();

        if (health == null)
            return;

        if (bonusType == BonusType.Heal)
        {
            health.Heal(healAmount);
            used = true;
            if (panelCollider != null)
                panelCollider.enabled = false;
        }
        else if (bonusType == BonusType.SpeedBoost)
        {
            ThirdPersonController controller = other.GetComponent<ThirdPersonController>();

            if (controller == null)
                controller = other.GetComponentInParent<ThirdPersonController>();

            if (controller != null && GameManager.Instance != null)
            {
                GameManager.Instance.GiveSpeedBoost(
                    controller,
                    speedMultiplier,
                    speedDuration
                );

                used = true;
                if (panelCollider != null)
                    panelCollider.enabled = false;
            }
        }
    }
    
    public void ResetPanel()
    {
        used = false;
        if (panelCollider == null)
            panelCollider = GetComponent<Collider>();
        if (panelCollider != null)
            panelCollider.enabled = true;
    }
}
