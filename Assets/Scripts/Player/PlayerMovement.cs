using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private TrailRenderer tr;

    private InputSystem_Actions inputSystem;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator anim;

    private bool canDash = true;
    private bool isDashing;
    private bool isMoving;
    public float dashingPower = 24f;
    private float dashingTime = 0.2f;
    public float dashingCooldown = 0.5f;

    private void Awake()
    {
        inputSystem = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputSystem.Enable();
    }

    private void Update()
    {
        if (isDashing) return;

        PlayerInput();
        UpdateAnimations();

        if(!PauseMenu.isPaused)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) && canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        Move();
    }

    private void PlayerInput()
    {
        movement = inputSystem.Player.Move.ReadValue<Vector2>();
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void UpdateAnimations()
    {
        if (!PauseMenu.isPaused)
        {
            isMoving = movement != Vector2.zero;

            anim.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                anim.SetFloat("MovementX", movement.x);
                anim.SetFloat("MovementY", movement.y);
            }
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rb.linearVelocity = movement.normalized * dashingPower;
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
