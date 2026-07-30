using UnityEngine;
using UnityEngine.Events;

// Se lo ponemos a las cajas, paredes o pisos que el jugador puede romper
public class Destructible : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [Tooltip("¿Cuántos golpes aguanta antes de romperse? (Por defecto 3)")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Animaciones (Opcional)")]
    public Animator anim;
    [Tooltip("Animación cuando pasa a 2 de vida")]
    public string hitAnim1 = "Block_Hit1"; 
    [Tooltip("Animación cuando pasa a 1 de vida")]
    public string hitAnim2 = "Block_Hit2";
    [Tooltip("Animación cuando se destruye por completo")]
    public string destroyAnim = "Block_Destroy";
    [Tooltip("¿Cuánto tiempo tarda en desaparecer para que se alcance a ver la animación de destrucción?")]
    public float destroyDelay = 0.5f;

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

        // Reproducir la animación correspondiente según la vida que nos quede
        if (anim != null)
        {
            if (currentHealth == 2)
            {
                anim.Play(hitAnim1);
            }
            else if (currentHealth == 1)
            {
                anim.Play(hitAnim2);
            }
            else if (currentHealth <= 0)
            {
                anim.Play(destroyAnim);
            }
        }

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
        // Si tenemos animaciones, esperamos un poquito para que se alcance a ver. Si no, lo borramos ya mismo.
        if (anim != null)
        {
            // Apagamos el collider para que el jugador no pueda pegarle al "fantasma" del bloque
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
