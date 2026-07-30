using UnityEngine;
using UnityEngine.InputSystem;

// Controla la mirilla que sigue al mouse y cambia a verde cuando apuntamos a algo para romper
[RequireComponent(typeof(SpriteRenderer))]
public class Crosshair : MonoBehaviour
{
    [Tooltip("Necesitamos al player para saber desde dónde y hasta dónde llega su ataque")]
    public PlayerController playerController;

    [Tooltip("La capa de los objetos que se pueden romper (Paredes, cajas, etc)")]
    public LayerMask destructibleLayer;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        // Para que vuelva a aparecer el mouse de Windows cuando paramos el juego
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Camera.main == null || playerController == null) return;

        // Agarramos la posición del mouse en la pantalla
        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();

        // Revisamos si el mouse está adentro de la ventana del juego
        bool isInsideScreen = mouseScreenPos.x >= 0 && mouseScreenPos.x <= Screen.width &&
                              mouseScreenPos.y >= 0 && mouseScreenPos.y <= Screen.height;

        // Ocultamos el mouse de Windows solo si estamos jugando
        Cursor.visible = !isInsideScreen;
        spriteRenderer.enabled = isInsideScreen;

        // Si el mouse se salió por un borde, dejamos de hacer cálculos
        if (!isInsideScreen) return;

        // Pasamos las coordenadas de la pantalla al mundo 2D de Unity
        Vector3 screenPosConZ = new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(Camera.main.transform.position.z));
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(screenPosConZ);

        // Movemos el dibujito de la mirilla a donde está el mouse
        transform.position = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        // Medimos qué tan lejos está el mouse del jugador
        float distance = Vector2.Distance(playerController.GetAttackCenter(), transform.position);

        // Por defecto la mira siempre es roja
        spriteRenderer.color = Color.red;

        // Si el mouse está cerquita del jugador (dentro del rango de ataque)...
        if (distance <= playerController.attackRange)
        {
            // Tiramos un circulo chiquitito a ver si tocamos un bloque rompible
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, destructibleLayer);
            if (hit != null)
            {
                // ¡Hay un objetivo! Cambiamos a verde
                spriteRenderer.color = Color.green;
            }
        }
    }
}
