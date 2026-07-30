using UnityEngine;
using UnityEngine.Rendering.Universal; // Librería de luces de URP 2D

// Script que oscurece el nivel gradualmente a medida que bajas
public class LightingManager : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El jugador para saber a qué profundidad (Y) está.")]
    public Transform player;
    
    [Tooltip("Arrastra aquí la luz global 2D.")]
    public Light2D globalLight;

    [Header("Configuración de Alturas (Y)")]
    [Tooltip("Altura de tu Piso 1")]
    public float topFloorY = 0f;
    
    [Tooltip("Altura de tu Piso 9")]
    public float bottomFloorY = -40f;

    [Header("Configuración de Oscuridad")]
    [Tooltip("Intensidad de la luz arriba de todo (por defecto 1).")]
    public float maxLightIntensity = 1f;
    
    [Tooltip("Intensidad de la luz en el fondo del pozo (te recomiendo entre 0.05 y 0.1).")]
    public float minLightIntensity = 0.05f;

    private void Update()
    {
        if (player == null || globalLight == null) return;

        // 1. Calculamos un porcentaje de 0 a 1 dependiendo de la altura del jugador.
        // Si Y está por encima de topFloor, da 1. Si está por debajo de bottomFloor, da 0.
        float depthPercentage = Mathf.InverseLerp(bottomFloorY, topFloorY, player.position.y);

        // 2. Ajustamos la luz de la escena progresivamente
        globalLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, depthPercentage);
    }
}
