using UnityEngine;

public class Laser : MonoBehaviour
{
    public int damage = 1;

    private bool hasHit;

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        HealthManager health = other.GetComponent<HealthManager>();

        if (health == null)
            health = other.GetComponentInParent<HealthManager>();
        
        if (health == null && other.CompareTag("Player"))
            health = FindAnyObjectByType<HealthManager>();

        if (health != null)
        {
            hasHit = true;
            health.TakeDamage(damage);
            
            Transform root = transform;
            MoveToStart mover = GetComponentInParent<MoveToStart>();
            if (mover != null)
                root = mover.transform;

            Destroy(root.gameObject);
        }
    }
}
