using UnityEngine;
using TMPro; // Librería estándar de Unity para textos modernos

/// <summary>
/// Hace que un componente TextMeshPro parpadee activándose y desactivándose.
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class BlinkingText : MonoBehaviour
{
    [Tooltip("Tiempo en segundos que el texto estará visible u oculto antes de cambiar.")]
    public float blinkInterval = 0.5f;

    private TextMeshPro textMesh;
    private float timer;

    private void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        // Aumentamos el temporizador con el tiempo real que pasa
        timer += Time.deltaTime;

        // Si el temporizador supera el intervalo...
        if (timer >= blinkInterval)
        {
            timer = 0f; // Reiniciamos el reloj
            textMesh.enabled = !textMesh.enabled; // Invertimos la visibilidad del texto
        }
    }
}
