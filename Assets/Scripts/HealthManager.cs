using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 5;

    [Header("Brief hit protection")]
    public float invulnTime = 0.8f;
    private float lastDamageTime = -99f;

    public int CurrentHealth { get; private set; }

    private void Start()
    {
        ResetHealth();
    }

    public void TakeDamage(int damage)
    {
        if (Time.time < lastDamageTime + invulnTime)
            return;

        lastDamageTime = Time.time;
        CurrentHealth -= damage;

        if (CurrentHealth < 0)
            CurrentHealth = 0;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHealthUI(CurrentHealth);

            if (CurrentHealth <= 0)
                GameManager.Instance.PlayerDied();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;

        if (CurrentHealth > maxHealth)
            CurrentHealth = maxHealth;

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHealthUI(CurrentHealth);
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        lastDamageTime = -99f;

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHealthUI(CurrentHealth);
    }
}
