using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script para cualquier objeto que pueda ser destruido por el jugador (muros, pisos, cajas).
/// </summary>
public class Destructible : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Cantidad de golpes necesarios para destruir el objeto.")]
    public int maxHealth = 1;
    private int currentHealth;

    [Header("Visual & Audio Effects")]
    [Tooltip("Prefab de partículas a instanciar cuando el objeto se destruye (opcional).")]
    public GameObject destructionVFX;

    [Header("Events")]
    [Tooltip("Eventos que se disparan al recibir daño (ej. reproducir un sonido de golpe).")]
    public UnityEvent OnTakeDamage;
    [Tooltip("Eventos que se disparan justo antes de destruirse (ej. dar puntos, sonido de ruptura).")]
    public UnityEvent OnDestroyed;

    private void Start()
    {
        // Inicializar la vida al máximo al empezar
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Reduce la vida del objeto y lo destruye si llega a 0.
    /// </summary>
    public void TakeDamage(int damage = 1)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        OnTakeDamage?.Invoke();

        // Si la vida se agota, procedemos a romper el objeto
        if (currentHealth <= 0)
        {
            Break();
        }
    }

    /// <summary>
    /// Lógica de destrucción del objeto.
    /// </summary>
    private void Break()
    {
        // Disparar eventos configurados en el inspector
        OnDestroyed?.Invoke();

        // Crear efecto visual si hay uno asignado
        if (destructionVFX != null)
        {
            Instantiate(destructionVFX, transform.position, Quaternion.identity);
        }

        // Eliminar el objeto de la escena
        Destroy(gameObject);
    }
}
