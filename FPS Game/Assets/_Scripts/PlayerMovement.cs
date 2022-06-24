using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    [Header("Mouse Look")]
    [SerializeField] float mouseSensitivity = 1.5f;
    private float verticalLookRot;

    [Header("Movement")]
    [SerializeField] float runSpeed = 5.5f;
    [SerializeField] float walkSpeed = 2.5f;
    [SerializeField] float smoothTime = .15f;
    [HideInInspector] public float speedAffector = 1f;
    private float speed;
    private float smoothMoveSpeed;

    [Header("Gravity")]
    [SerializeField] float gravityScale = 4f;
    [SerializeField] float fallGravityScale = 7f;
    private const float gravityConstant = -9.81f;
    private float gravity;

    [Header("Jumping")]
    [SerializeField] float jumpHeight = .8f;
    [SerializeField] float groundDistance = .5f;
    [SerializeField] float hangTime = .05f;
    [SerializeField] float jumpBuffer = .08f;
    private Vector3 velocity;
    private float hangTimeCounter;
    private float jumpBufferCounter;
    private bool isGrounded;
    private bool isJumping;

    [Header("Components")]
    [SerializeField] GameObject cameraHolder;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    private PhotonView pv;
    private CharacterController controller;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        gravity = gravityConstant * gravityScale;
    }

    private void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }

        Look();
        Move();
        CheckGrounded();
        CheckJumpAllowed();
        Jump();
        ApplyGravity();
    }

    void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalLookRot += mouseY;
        verticalLookRot = Mathf.Clamp(verticalLookRot, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRot;
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = (transform.right * horizontal + transform.forward * vertical).normalized;
        speed = Mathf.SmoothDamp(speed, Input.GetKey(KeyCode.LeftShift) ? walkSpeed : runSpeed, ref smoothMoveSpeed, smoothTime);

        controller.Move(direction * speed * speedAffector * Time.deltaTime);
    }

    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -5f;
        }
    }

    void CheckJumpAllowed()
    {
        if (isGrounded)
        {
            hangTimeCounter = hangTime;
        }
        else
        {
            hangTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    void Jump()
    {
        if (!isJumping && jumpBufferCounter > 0f && hangTimeCounter > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferCounter = 0f;
            StartCoroutine(JumpCooldown());
        }

        if (velocity.y < 0)
        {
            gravity = gravityConstant * fallGravityScale;
        }
        else
        {
            gravity = gravityConstant * gravityScale;
        }

        if (Input.GetButtonUp("Jump") && velocity.y > 0f)
        {
            velocity.y *= .7f;
            hangTimeCounter = 0f;
        }
    }

    IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(.4f);
        isJumping = false;
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
