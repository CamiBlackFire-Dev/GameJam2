using UnityEngine;
using TMPro; // Usamos esto para los textos en HD de Unity

// Script súper sencillo para hacer que el texto de ayuda parpadee
[RequireComponent(typeof(TextMeshPro))]
public class BlinkingText : MonoBehaviour
{
    [Tooltip("Cada cuánto tiempo se apaga y se prende el texto (en segundos)")]
    public float blinkInterval = 0.5f;

    private TextMeshPro textMesh;
    private float timer;

    private void Start()
    {
        // Guardamos el componente de texto apenas arranca el juego
        textMesh = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        // Vamos sumando el tiempo que pasa
        timer += Time.deltaTime;

        // Si el tiempo es mayor al intervalo que pusimos...
        if (timer >= blinkInterval)
        {
            timer = 0f; // Reiniciamos el cronómetro
            textMesh.enabled = !textMesh.enabled; // Apagamos si estaba prendido, o prendemos si estaba apagado
        }
    }
}
