using UnityEngine;

public class SurvivorWhistle : MonoBehaviour
{
    [Header("Componentes")]
    public AudioSource whistleAudioSource;
    public Transform player;

    [Header("Ajustes de Distancia")]
    public float detectionRadius = 5f;

    void Start()
    {
        // Busca al jugador por su Tag

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player == null || whistleAudioSource == null) return;

        // Calcula la distancia entre el sobreviviente y el jugador

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si el jugador esta cerca, empieza el sonido

        if (distanceToPlayer <= detectionRadius)
        {
            if (!whistleAudioSource.isPlaying)
            {
                whistleAudioSource.volume = 0.1f;
                whistleAudioSource.Play();
            }

            // Normaliza el volumen en cuanto el jugador se acerca o se aleja

            float normalizedVolume = 1f - (distanceToPlayer / detectionRadius);
  
             whistleAudioSource.volume = Mathf.Clamp01(normalizedVolume);
        }
        else
        {
            // Si el jugador se aleja, detiene el sonido

            if (whistleAudioSource.isPlaying)
            {
                whistleAudioSource.Stop();
            }
        }
    }

    // Gizmos para calcular el radio de entrada
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}