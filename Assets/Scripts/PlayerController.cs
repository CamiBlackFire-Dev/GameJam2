using UnityEngine;
using UnityEngine.InputSystem;

// Nuestro script principal. Maneja las físicas, los saltos y el ataque del minero.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 6f;

    [Header("Salto")]
    public float jumpForce = 4f;
    [Range(0f, 1f)]
    [Tooltip("Si sueltas el botón rápido, el salto se corta a la mitad")]
    public float jumpCutMultiplier = 0.5f;

    [Header("Suelo")]
    [Tooltip("Un puntito vacío en los pies del jugador para saber si toca el piso")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    [Tooltip("¿Qué cosas son piso?")]
    public LayerMask groundLayer;

    [Header("Controles (Input System)")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference attackAction;

    [Header("Ataque")]
    [Tooltip("Hasta dónde llega el brazo para picar cosas")]
    public float attackRange = 2f;
    [Tooltip("Subimos un poco el centro del ataque para que no salga desde los pies")]
    private Vector2 attackOffset = new Vector2(0f, 0.5f);
    private int attackDamage = 1;
    [Tooltip("Tiempo que tarda la animación de ataque")]
    private float attackAnimDuration = 0.4f;
    [Tooltip("Tiempo de espera antes de poder dar OTRO golpe (Cooldown)")]
    private float attackCooldown = 0.4f;
    public LayerMask destructibleLayer;

    // Esta función nos da la posición del pecho/cabeza del jugador para calcular mejor el ataque
    public Vector2 GetAttackCenter()
    {
        return (Vector2)transform.position + attackOffset;
    }

    [Header("Nombres de las animaciones")]
    private string idleAnim = "Idle";
    private string runAnim = "Run";
    private string jumpAnim = "Jump";
    private string attackAnim = "Attack";

    // Componentes que necesitamos
    private Rigidbody2D rb;
    private Animator anim;

    // Cosas internas que usamos para saber qué está haciendo el jugador
    private string currentState;
    private float attackTimer;
    private float currentCooldown;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isJumping;
    private bool isJumpPressed;
    private bool isFacingRight = true;

    [Header("Mejoras de jugabilidad (Game Feel)")]
    [Tooltip("Le damos un tiempito extra para saltar aunque ya se haya caído de la plataforma")]
    private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [Tooltip("Si aprietas salto justo antes de tocar el piso, el juego te lo guarda y saltas apenas tocas")]
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Prendemos los controles nuevos de Unity
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

    // Apagamos los controles si el jugador se desactiva
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

    // Cuando tocamos las flechas o el joystick
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Cuando apretamos el botón de salto
    private void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = true;
        jumpBufferCounter = jumpBufferTime; // Guardamos el salto por unos milisegundos
    }

    // Cuando soltamos el botón de salto
    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumpPressed = false;

        // Si estábamos subiendo, frenamos un poco para hacer un salto cortito
        if (rb.linearVelocity.y > 0 && isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            isJumping = false;
            coyoteTimeCounter = 0f;
        }
    }

    // Cuando hacemos clic para picar
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (Camera.main == null) return;

        // Si aún no ha pasado el tiempo de enfriamiento, ignoramos el clic
        if (currentCooldown > 0f) return;

        // Reiniciamos el tiempo de enfriamiento
        currentCooldown = attackCooldown;

        // Arrancamos el cronómetro de la animación para que no se corte
        attackTimer = attackAnimDuration;

        // Pasamos el clic de la pantalla al mundo 2D
        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
        Vector3 screenPosConZ = new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(Camera.main.transform.position.z));
        Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(screenPosConZ);
        Vector2 mouseWorldPos = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);

        // Nos fijamos si el clic fue cerca nuestro
        float distanceToMouse = Vector2.Distance(GetAttackCenter(), mouseWorldPos);
        if (distanceToMouse <= attackRange)
        {
            // Buscamos si hay un bloque en la capa destructible justo donde hicimos clic
            Collider2D hit = Physics2D.OverlapCircle(mouseWorldPos, 0.1f, destructibleLayer);

            if (hit != null)
            {
                // Si encontramos algo, le decimos que se rompa
                Destructible destructible = hit.GetComponent<Destructible>();
                if (destructible != null)
                {
                    destructible.TakeDamage(attackDamage);
                }
            }
        }
    }

    // El Update normal corre todo el tiempo (bueno para lógica e inputs)
    private void Update()
    {
        // Bajamos el cronómetro del cooldown si es mayor a 0
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }

        CheckGrounded();
        HandleJumpLogic();
        UpdateAnimations();
    }

    // El FixedUpdate es mejor para aplicar fuerzas y cosas de físicas
    private void FixedUpdate()
    {
        ApplyMovement();
    }

    // Revisamos si estamos tocando el piso
    private void CheckGrounded()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            // Si tocamos piso, tenemos coyote time al máximo
            coyoteTimeCounter = coyoteTime;

            // Ya caímos, así que no estamos saltando
            if (rb.linearVelocity.y <= 0f)
            {
                isJumping = false;
            }
        }
        else
        {
            // Si estamos en el aire, vamos restando el tiempito extra de salto
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    // Lógica para saltar
    private void HandleJumpLogic()
    {
        jumpBufferCounter -= Time.deltaTime; // Restamos tiempo de nuestro salto guardado

        // Si tenemos un salto guardado Y además estamos en el piso (o caímos hace nada)
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Evitamos hacer un súper salto por accidente
            if (!isJumpPressed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpCutMultiplier);
                isJumping = false;
            }

            // Gastamos los contadores para no saltar doble
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    // Lógica de correr de lado a lado
    private void ApplyMovement()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Si nos movemos para la derecha pero miramos a la izquierda, nos damos vuelta
        if (moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    // Da vuelta el dibujito del jugador
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f; // Multiplicar por -1 voltea todo el objeto
        transform.localScale = localScale;
    }

    // Cambia la animación según lo que estemos haciendo
    private void UpdateAnimations()
    {
        if (anim == null) return;

        // Si estamos picando, no hacemos otra animación hasta que termine
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            ChangeAnimationState(attackAnim);
            return;
        }

        // Si estamos en el aire (saltando o cayendo)
        if (!isGrounded)
        {
            ChangeAnimationState(jumpAnim);
            return;
        }

        // Si nos estamos moviendo de verdad (y no quieticos)
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            ChangeAnimationState(runAnim);
        }
        else
        {
            ChangeAnimationState(idleAnim); // Estamos quietos y aburridos
        }
    }

    // Solo reproduce la animación nueva si no es la misma que ya estaba corriendo
    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);
        currentState = newState;
    }

    // Dibujamos unos círculos de guía para que nos sea fácil configurarlo en Unity
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); // El circulo de los pies
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetAttackCenter(), attackRange); // El círculo grande del alcance
    }
}