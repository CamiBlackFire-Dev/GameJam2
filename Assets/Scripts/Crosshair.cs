using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class Crosshair : MonoBehaviour
{
    public Transform player;
    public float attackRange = 2f;
    public LayerMask destructibleLayer;

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
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Camera.main == null || player == null) return;

        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();

        bool isInsideScreen = mouseScreenPos.x >= 0 && mouseScreenPos.x <= Screen.width &&
                              mouseScreenPos.y >= 0 && mouseScreenPos.y <= Screen.height;

        Cursor.visible = !isInsideScreen;
        spriteRenderer.enabled = isInsideScreen;

        if (!isInsideScreen) return;

        Vector3 screenPosConZ = new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(Camera.main.transform.position.z));
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(screenPosConZ);

        transform.position = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        float distance = Vector2.Distance(player.position, transform.position);

        if (distance > attackRange)
        {
            spriteRenderer.color = outOfRangeColor;
        }
        else
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, destructibleLayer);
            if (hit != null)
            {
                spriteRenderer.color = inRangeTargetColor;
            }
            else
            {
                spriteRenderer.color = normalColor;
            }
        }
    }
}
