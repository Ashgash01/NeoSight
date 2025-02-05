using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerCont : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D contactFilter;

    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private Vector2 lastMoveDirection;
    private Vector2 input;
    private bool facingLeft = true;

    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);

            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));

                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
            }

          
        }
 
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    private bool TryMove(Vector2 direction)
    {
        int count = rb.Cast(
            direction,
            contactFilter,
            castCollisions,
            moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;
        }
        return false;
    }

    private void Update()
    {
        ProccessInputs();
        Animate();

        if ((input.x > 0 && !facingLeft) || (input.x < 0 && facingLeft))
        {
            Flip();
        }
    }

    private void ProccessInputs()
    {
        float moveX = movementInput.x;
        float moveY = movementInput.y;

        if ((moveX == 0 && moveY == 0) && (input.x != 0 || input.y != 0))
        {
            lastMoveDirection = input;
        }

        input.x = moveX;
        input.y = moveY;
    }

    void Animate()
    {
        if (animator != null)
        {
            animator.SetFloat("MoveX", input.x);
            animator.SetFloat("MoveY", input.y);
            animator.SetFloat("MoveMagnitude", input.magnitude);
            animator.SetFloat("LastMoveX", lastMoveDirection.x);
            animator.SetFloat("LastMoveY", lastMoveDirection.y);
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        facingLeft = !facingLeft;
        transform.localScale = scale;
    }
}
