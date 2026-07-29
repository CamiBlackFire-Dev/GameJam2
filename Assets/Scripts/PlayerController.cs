using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Script principal para controlar al jugador. 
/// Maneja movimiento, salto (con Game Feel), animaciones y sistema de ataque.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    [Range(0f, 1f)]
    [Tooltip("Multiplicador aplicado a la velocidad vertical si el jugador suelta el botón de salto antes de tiempo.")]
    public float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    [Tooltip("Objeto vacío colocado exactamente en los pies del jugador.")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    [Tooltip("Capa que define qué objetos cuentan como suelo.")]
    public LayerMask groundLayer;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference attackAction;

    [Header("Attack Settings")]
    [Tooltip("Radio máximo de alcance para hacer clic y romper objetos.")]
    public float attackRange = 2f;
    [Tooltip("Desfase para centrar el área de ataque en el pecho/cabeza del jugador en lugar de los pies.")]
    public Vector2 attackOffset = new Vector2(0f, 0.5f);
    public int attackDamage = 1;
    [Tooltip("Tiempo en segundos que se reproducirá la animación de ataque antes de volver a correr/idle.")]
    public float attackAnimDuration = 0.3f;
    public LayerMask destructibleLayer;

    /// <summary>
    /// Calcula desde dónde nace realmente el ataque sumando el Offset a la posición base del jugador.
    /// </summary>
    public Vector2 GetAttackCenter()
    {
        return (Vector2)transform.position + attackOffset;
    }

    [Header("Animations")]
    public string idleAnim = "Player_Idle";
    public string runAnim = "Player_Run";
    public string attackAnim = "Player_Attack";

    // --- COMPONENTES ---
    private Rigidbody2D rb;
    private Animator anim;

    // --- VARIABLES DE ESTADO ---
    private string currentState;
    private float attackTimer;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isJumping;
    private bool isJumpPressed;
    private bool isFacingRight = true;

    [Header("Game Feel")]
    [Tooltip("Tiempo de gracia para poder saltar tras caerse por el borde de una plataforma.")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [Tooltip("Tiempo que se 'recuerda' la pulsación del botón salto antes de tocar el suelo.")]
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // --- SUSCRIPCIÓN A EVENTOS DEL INPUT SYSTEM (NECESARIO EN UNITY 6) ---
    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
        }

        if (jumpAction != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.performed += OnJump;
            jumpAction.action.canceled += OnJumpCanceled;
        }

        if (attackAction != null)
        {
            attackAction.action.Enable();
            attackAction.action.performed += OnAttack;
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
            moveAction.action.Disable();
        }

        if (jumpAction != null)
        {
            jumpAction.action.performed -= OnJump;
            jumpAction.action.canceled -= OnJumpCanceled;
            jumpAction.action.Disable();
        }

        if (attackAction != null)
        {
            attackAction.action.performed -= OnAttack;
            attackAction.action.Disable();
        }
    }

    // --- CALLBACKS DEL INPUT ---
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = true;
        jumpBufferCounter = jumpBufferTime; // Iniciar el buffer de salto
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumpPressed = false;

        // Si el jugador suelta el botón mientras sube, cortamos la velocidad para un salto cortito
        if (rb.linearVelocity.y > 0 && isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            isJumping = false;
            coyoteTimeCounter = 0f;
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (Camera.main == null) return;

        // Iniciar temporizador para evitar que la animación de idle/correr interrumpa el ataque
        attackTimer = attackAnimDuration;

        // 1. Obtener la posición del ratón en la pantalla y convertirla a coordenadas del mundo 2D
        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
        Vector3 screenPosConZ = new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(Camera.main.transform.position.z));
        Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(screenPosConZ);
        Vector2 mouseWorldPos = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);

        // 2. Validar si el clic está dentro de nuestro radio máximo de ataque
        float distanceToMouse = Vector2.Distance(GetAttackCenter(), mouseWorldPos);
        if (distanceToMouse <= attackRange)
        {
            // 3. Detectar colliders en el punto exacto del ratón que pertenezcan a la capa Destructible
            Collider2D hit = Physics2D.OverlapCircle(mouseWorldPos, 0.1f, destructibleLayer);

            if (hit != null)
            {
                Destructible destructible = hit.GetComponent<Destructible>();
                if (destructible != null)
                {
                    destructible.TakeDamage(attackDamage);
                }
            }
        }
    }

    // --- CICLO PRINCIPAL (Lógica y Físicas) ---
    private void Update()
    {
        CheckGrounded();
        HandleJumpLogic();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    // --- DETECCIÓN DE SUELO Y COYOTE TIME ---
    private void CheckGrounded()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Renovar coyote time al pisar el suelo

            // Si tocamos suelo y ya no vamos hacia arriba, finaliza el estado de salto
            if (rb.linearVelocity.y <= 0f)
            {
                isJumping = false;
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Consumir coyote time en el aire
        }
    }

    // --- LÓGICA DE SALTO (CON JUMP BUFFERING) ---
    private void HandleJumpLogic()
    {
        jumpBufferCounter -= Time.deltaTime; // Consumir tiempo del buffer

        // Solo saltamos si el jugador pulsó hace muy poco (buffer) y está tocando suelo o en coyote time
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Si el jugador soltó el botón *exactamente* en el mismo frame que se procesó el salto
            if (!isJumpPressed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpCutMultiplier);
                isJumping = false;
            }

            // Consumir contadores para evitar hacer doble salto accidental
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    // --- MOVIMIENTO HORIZONTAL ---
    private void ApplyMovement()
    {
        // Aplicar la velocidad horizontal del input manteniendo la vertical (gravedad) inalterada
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Voltear (Flip) al jugador dependiendo de si va a la derecha o izquierda
        if (moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f; // Invertir el eje X rota visualmente a todos los hijos (sprites, colliders extra)
        transform.localScale = localScale;
    }

    // --- SISTEMA DE ANIMACIÓN (Máquina de Estados Simple por Código) ---
    private void UpdateAnimations()
    {
        if (anim == null) return;

        // Prioridad 1: Animación de ataque (bloquea a las demás hasta que se consuma su temporizador)
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            ChangeAnimationState(attackAnim);
            return;
        }

        // Prioridad 2: Movimiento y Reposo
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            ChangeAnimationState(runAnim);
        }
        else
        {
            ChangeAnimationState(idleAnim);
        }
    }

    /// <summary>
    /// Cambia la animación actual solo si es distinta a la que ya se está reproduciendo.
    /// Esto evita que la animación se reinicie continuamente desde el frame 0.
    /// </summary>
    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);
        currentState = newState;
    }

    // --- DIBUJO DE AYUDAS VISUALES EN EL EDITOR (Gizmos) ---
    private void OnDrawGizmosSelected()
    {
        // Dibujar el área de detección de suelo
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Dibujar el rango de ataque con su offset
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetAttackCenter(), attackRange);
    }
}