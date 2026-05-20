using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerMovement : MonoBehaviour
{
    public float pushForce = 8f;
    public float turnSpeed = 80f;
    public float maxSpeed = 15f;
    public float friction = 0.97f;

    private Rigidbody rb;

    [Header("Jump Settings")]
    public float jumpforce = 8f;
    public float gravity = -20f;
    public float groundCheckDistance = 0.2f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;


    private float verticalVelocity;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool isGrounded;
    private float colliderHalfHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        colliderHalfHeight = col != null ? col.height / 2f : 1f;
    }

    
    // Update is called once per frame
    void Update()
    {
        float turn = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);

        float vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(vertical) > 0.1f)
        {
            rb.AddForce(transform.forward * pushForce * vertical, ForceMode.Force);
        }

        CheckGrounded();
        HandleCoyoteTime();
        HandleJumpBuffer();
        ApplyGravity();
        ApplyJump();

        
       
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;
        float speed = Vector3.Dot (velocity, transform.forward);
        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);
        rb.linearVelocity = new Vector3(
            transform.forward.x * speed,
            verticalVelocity,
            transform.forward.z * speed);

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x * friction,
            rb.linearVelocity.y,
            rb.linearVelocity.z * friction

            );

    }

    //These are my custom functions for jumping
    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(
            transform.position + Vector3.down * colliderHalfHeight,
            groundCheckDistance,
            groundLayer
            );

        if ( isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
            
           
    }

    private void HandleCoyoteTime()
    {
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

    }

    private void HandleJumpBuffer()
    {
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

    }

    private void ApplyJump()
    { 
        if(jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            verticalVelocity = jumpforce;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        if(Input.GetButtonUp("Jump") && verticalVelocity > 0f)
        {
            verticalVelocity *= 0.5f;

        }

    }
    private void ApplyGravity()
    {
        if (!isGrounded)
            verticalVelocity += gravity * Time.deltaTime;

    }



}

