using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario para gestionar el cambio de escenas

public class UIManager : MonoBehaviour
{
    [Header("Configuración de Música")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuMusic;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.5f;

    [Header("Efectos de Sonido (SFX)")]
    [SerializeField] private AudioSource sfxSource; // AudioSource dedicado a los efectos de sonido
    [SerializeField] private AudioClip hoverSound;   // Sonido al pasar el mouse por encima
    [SerializeField] private AudioClip clickSound;   // Sonido al hacer clic
    

    private void Start()
    {
        // Configurar y reproducir la música de fondo al iniciar el menú
        if (audioSource != null && menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.volume = musicVolume;
            audioSource.loop = true; // Para que la canción se repita en bucle
            audioSource.Play();
        }
    }
   public void PlayGame()
    {
        PlayClickSound(); // Reproduce el clic
        StartCoroutine(LoadSceneWithDelay("Level1", 0.9f)); // Espera 0.15s y cambia de escena
    }

    public void OpenTutorial()
    {
        PlayClickSound(); // Reproduce el clic
        StartCoroutine(LoadSceneWithDelay("Tutorial", 0.9f)); // Espera 0.15s y cambia de escena
    }

    // 🔄 NUEVO: Método para el botón RETRY (Reiniciar Nivel)
    public void RetryGame()
    {
        PlayClickSound(); // Reproduce el sonido de clic
        // Opción A: Cargar directamente la escena del nivel (Level1)
        StartCoroutine(LoadSceneWithDelay("Level1", 0.9f)); 

        // Opción B (Alternativa dinámica): Recargar la escena en la que se encuentra actualmente
        // string currentScene = SceneManager.GetActiveScene().name;
        // StartCoroutine(LoadSceneWithDelay(currentScene, 0.9f));
    }

    // 🏠 NUEVO (Opcional): Método para volver al Menú Principal si agregas ese botón
    public void GoToMainMenu()
    {
        PlayClickSound();
        StartCoroutine(LoadSceneWithDelay("Menu", 0.9f)); // Cambia "MainMenu" por el nombre de tu escena de menú
    }

    private IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        // WaitForSecondsRealtime asegura que funcione incluso si el juego está pausado
        yield return new WaitForSecondsRealtime(delay); 
        SceneManager.LoadScene(sceneName);
    }

    public void PlayHoverSound()
    {
        if (sfxSource != null && hoverSound != null)
        {
            sfxSource.PlayOneShot(hoverSound);
        }
    }

    public void PlayClickSound()
    {
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }
    /* Método opcional y genérico si prefieres pasar el nombre desde el propio Inspector
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }*/
    // Este método lo puedes conectar a un Slider de UI si agregas un menú de opciones más adelante
    public void SetVolume(float volume)
    {
        musicVolume = volume;
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }
}