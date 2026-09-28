using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 4f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpQueued;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
        {
            moveInput.y += 1f;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            moveInput.y -= 1f;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            moveInput.x -= 1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            moveInput.x += 1f;
        }

        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpQueued = true;
        }
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.x = moveInput.x * moveSpeed;
        velocity.z = moveInput.y * moveSpeed;

        if (jumpQueued && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
        }

        jumpQueued = false;

        rb.linearVelocity = velocity;
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}