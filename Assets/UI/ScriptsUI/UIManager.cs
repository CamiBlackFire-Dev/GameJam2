using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 

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

    [Header("Paneles de Menú de Pausa")]
    [SerializeField] private GameObject pauseMenuPanel; // Tu panel 'MenuPuase'
    [SerializeField] private GameObject controlsPanel;  
    
    private bool isPaused = false;

    private void Start()
    {
        
        if (audioSource != null && menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.volume = musicVolume;
            audioSource.loop = true; 
            audioSource.Play();
        }
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        ResumeGamePhysicsAndCursor();
    }

    private void Update()
    {
        
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
           
            if (controlsPanel != null && controlsPanel.activeSelf)
            {
                CloseControls();
            }
            else
            {
                TogglePause();
            }
        }
    }
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Congela el juego
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);

        // Mostrar y liberar el cursor para poder usar la UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        PlayClickSound();
        isPaused = false;
        Time.timeScale = 1f; // Reanuda el tiempo del juego
        
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);

        // Volver a ocultar y bloquear el cursor para apuntar/jugar
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    private void ResumeGamePhysicsAndCursor()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenControls()
    {
        PlayClickSound();
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        PlayClickSound();
        if (controlsPanel != null) controlsPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
    }

   public void PlayGame()
    {
        PlayClickSound(); 
        StartCoroutine(LoadSceneWithDelay("CamiloTests", 0.9f)); 
    }

    public void OpenTutorial()
    {
        PlayClickSound(); // Reproduce el clic
        StartCoroutine(LoadSceneWithDelay("CamiloTests2", 0.9f));
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
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