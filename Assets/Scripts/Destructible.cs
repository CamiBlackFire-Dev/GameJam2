using UnityEngine;
using UnityEngine.Events;

public class Destructible : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 1;
    private int currentHealth;

    [Header("Visual & Audio Effects")]
    public GameObject destructionVFX;

    [Header("Events")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDestroyed;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage = 1)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        OnTakeDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        OnDestroyed?.Invoke();

        if (destructionVFX != null)
        {
            Instantiate(destructionVFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
