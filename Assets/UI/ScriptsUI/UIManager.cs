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
    [SerializeField] private AudioSource sfxSource; 
    [SerializeField] private AudioClip hoverSound;   
    [SerializeField] private AudioClip clickSound;   
    

    private void Start()
    {
        // Configurar y reproducir la música de fondo al iniciar el menú
        if (audioSource != null && menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.volume = musicVolume;
            audioSource.loop = true; 
            audioSource.Play();
        }
    }
   public void PlayGame()
    {
        PlayClickSound(); // Reproduce el clic
        StartCoroutine(LoadSceneWithDelay("CamiloTests", 0.9f)); // Espera 0.15s y cambia de escena
    }

    public void OpenTutorial()
    {
        PlayClickSound(); // Reproduce el clic
        StartCoroutine(LoadSceneWithDelay("Tutorial", 0.9f)); 
    }

    
    public void RetryGame()
    {
        PlayClickSound(); 
        StartCoroutine(LoadSceneWithDelay("CamiloTests", 0.9f)); 

        // string currentScene = SceneManager.GetActiveScene().name;
        // StartCoroutine(LoadSceneWithDelay(currentScene, 0.9f));
    }

    public void GoToMainMenu()
    {
        PlayClickSound();
        StartCoroutine(LoadSceneWithDelay("Menu", 0.9f)); 
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
    /* 
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }*/
    
    public void SetVolume(float volume)
    {
        musicVolume = volume;
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }
}