using UnityEngine;
using UnityEngine.Events;

// Se lo ponemos a las cajas, paredes o pisos que el jugador puede romper
public class Destructible : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [Tooltip("¿Cuántos golpes aguanta antes de romperse?")]
    public int maxHealth = 1;
    private int currentHealth;

    [Header("Efectos")]
    [Tooltip("Aquí ponemos las partículas o explosión cuando se rompe")]
    public GameObject destructionVFX;

    [Header("Eventos especiales")]
    [Tooltip("Cosas que pasan cuando le pegan (como reproducir un sonido)")]
    public UnityEvent OnTakeDamage;
    [Tooltip("Cosas que pasan justo antes de desaparecer")]
    public UnityEvent OnDestroyed;

    private void Start()
    {
        // Al inicio, le damos toda la vida
        currentHealth = maxHealth;
    }

    // Esta función la llamamos desde el script del jugador cuando le damos un golpe
    public void TakeDamage(int damage = 1)
    {
        // Si ya está muerto, no hacemos nada
        if (currentHealth <= 0) return;

        // Le quitamos vida y avisamos que recibió daño
        currentHealth -= damage;
        OnTakeDamage?.Invoke();

        // Si se quedó sin vida, lo rompemos
        if (currentHealth <= 0)
        {
            Break();
        }
    }

    // Lo que pasa cuando se rompe por completo
    private void Break()
    {
        // Disparamos los eventos (sonidos, puntos, etc.)
        OnDestroyed?.Invoke();

        // Si le pusimos un efecto de partículas, lo creamos justo donde está el objeto
        if (destructionVFX != null)
        {
            Instantiate(destructionVFX, transform.position, Quaternion.identity);
        }

        // Borramos el objeto del juego
        Destroy(gameObject);
    }
}
