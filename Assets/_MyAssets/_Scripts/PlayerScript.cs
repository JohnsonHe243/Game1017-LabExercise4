using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Transform groundDetect;
    [SerializeField] private bool isGrounded; // Just so we can see in Editor.
    [SerializeField] private float moveForce;
    [SerializeField] private float jumpForce;
    public LayerMask groundLayer;
    private float groundCheckWidth = 0.75f;
    private float groundCheckHeight = 0.1f;
    private Animator an;
    private Rigidbody2D rb;

    void Start()
    {
        an = GetComponentInChildren<Animator>();
        isGrounded = false; // Always start in air.
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GroundedCheck();

        // Horizontal movement.
        float moveInput = Input.GetAxis("Horizontal");
        float moveInputRaw = Input.GetAxisRaw("Horizontal"); // Clamped to -1, 0, or 1.
        // bool isMoving = Mathf.Abs(moveInputRaw) > 0f;
        an.SetBool("isMoving", Mathf.Abs(moveInputRaw) > 0f);
        // Set horizontal force in player. Use current vertical velocity.
        rb.velocity = new Vector2(moveInput * moveForce * Time.fixedDeltaTime, rb.velocity.y);
        
        // Flip the player. Could use down and up functions for not every frame.
        if (moveInputRaw != 0)
            transform.localScale = new Vector3(moveInputRaw, 1, 1);

        // Trigger jump. Use current horizontal velocity.
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            Game.Instance.SOMA.PlaySound("Jump");
        }

        // CheckPoints and Respawn
        if (rb.position.x < 66 && rb.position.y < -24)
        {
            Game.Instance.SOMA.PlaySound("Fall");
            transform.position = new Vector3(16, -21, 0);
        }
        if (rb.position.x > 66 && rb.position.x < 200 && rb.position.y < -24)
        {
            Game.Instance.SOMA.PlaySound("Fall");
            transform.position = new Vector3(112, -21, 0);
        }
        if (rb.position.x > 200 && rb.position.x < 315 && rb.position.y < -24)
        {
            Game.Instance.SOMA.PlaySound("Fall");
            transform.position = new Vector3(240, -21, 0);
        }
        if (rb.position.x > 311 && rb.position.y > -21)
        {
            Game.Instance.SOMA.PlaySound("Victory");
        }
    }

    private void GroundedCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundDetect.position, 
            new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer);
        an.SetBool("isJumping", !isGrounded);
    }
}
