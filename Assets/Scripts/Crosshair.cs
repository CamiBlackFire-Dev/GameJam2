using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gestiona la mira del jugador (Crosshair) que sigue al ratón.
/// Cambia de color dinámicamente si apuntamos a un objeto destructible dentro del rango.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Crosshair : MonoBehaviour
{
    [Tooltip("Referencia al controlador del jugador para leer su rango de ataque dinámicamente.")]
    public PlayerController playerController;
    
    [Tooltip("La misma capa (Layer) que configuraste para los objetos destructibles.")]
    public LayerMask destructibleLayer;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color inRangeTargetColor = Color.red;
    public Color outOfRangeColor = new Color(1, 1, 1, 0.3f);

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        // Restaurar el cursor de Windows al detener el juego o desactivar el script
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Camera.main == null || playerController == null) return;

        // 1. Obtener la posición del ratón en la pantalla
        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
        
        // 2. Comprobar si el ratón está dentro de los límites de la ventana de juego
        bool isInsideScreen = mouseScreenPos.x >= 0 && mouseScreenPos.x <= Screen.width &&
                              mouseScreenPos.y >= 0 && mouseScreenPos.y <= Screen.height;

        // Mostrar u ocultar el cursor de Windows dependiendo de si estamos en la ventana
        Cursor.visible = !isInsideScreen;
        spriteRenderer.enabled = isInsideScreen;

        // Si salimos de la ventana, no hace falta calcular físicas ni renderizado de la mira
        if (!isInsideScreen) return; 

        // 3. Convertir la posición de la pantalla a coordenadas del mundo 2D
        Vector3 screenPosConZ = new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(Camera.main.transform.position.z));
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(screenPosConZ);
        
        // Mover el sprite de la mira a la posición del ratón en el mundo
        transform.position = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        // 4. Calcular la distancia real entre el centro de ataque del jugador y la mira
        float distance = Vector2.Distance(playerController.GetAttackCenter(), transform.position);

        // 5. Lógica de colores según la distancia y si tocamos algo destructible
        if (distance > playerController.attackRange)
        {
            spriteRenderer.color = outOfRangeColor; // Muy lejos (fuera de rango)
        }
        else
        {
            // Usamos un pequeño círculo invisible (0.1f de radio) para detectar si tocamos un Destructible
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, destructibleLayer);
            if (hit != null)
            {
                spriteRenderer.color = inRangeTargetColor; // En rango y apuntando a un objetivo
            }
            else
            {
                spriteRenderer.color = normalColor; // En rango pero apuntando al vacío
            }
        }
    }
}
