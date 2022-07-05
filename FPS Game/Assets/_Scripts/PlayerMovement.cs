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
    [HideInInspector] public bool speedControlled;
    [HideInInspector] public bool directionControlled;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public float speed;
    private float smoothMoveSpeed;

    [Header("Gravity")]
    [SerializeField] float gravityScale = 4f;
    [SerializeField] float fallGravityScale = 7f;
    [HideInInspector] public bool gravityControlled;
    private const float gravityConstant = -9.81f;
    private float gravity;

    [Header("Jumping")]
    [SerializeField] float jumpHeight = .9f;
    [SerializeField] float groundDistance = .5f;
    [SerializeField] float hangTime = .05f;
    [SerializeField] float jumpBuffer = .08f;
    [HideInInspector] public bool jumpControlled;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public bool isGrounded;
    private float hangTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping;

    [Header("Components")]
    [SerializeField] GameObject cameraHolder;
    [SerializeField] LayerMask groundMask;
    [SerializeField] PlayerAudio playerAudio;
    public Transform groundCheck;
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

        if (!directionControlled)
        {
            direction = (transform.right * horizontal + transform.forward * vertical).normalized;
        }

        if (!speedControlled)
        {
            speed = Mathf.SmoothDamp(speed, (Input.GetKey(KeyCode.LeftShift) ? walkSpeed : runSpeed) * speedAffector, ref smoothMoveSpeed, smoothTime);
        }

        controller.Move(direction * speed * Time.deltaTime);

        if (direction != Vector3.zero && controller.velocity != Vector3.zero && !Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            if (!playerAudio.CheckSound("Footsteps"))
            {
                playerAudio.Play("Footsteps");
            }
        }
        else if (playerAudio.CheckSound("Footsteps"))
        {
            playerAudio.Stop("Footsteps");
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < -8f && Time.timeSinceLevelLoad > .5f)
        {
            playerAudio.Play("Jump Land");
        }

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
        if (jumpControlled)
        {
            return;
        }

        if (!isJumping && jumpBufferCounter > 0f && hangTimeCounter > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferCounter = 0f;
            StartCoroutine(JumpCooldown());

            playerAudio.Play("Jump");
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
            velocity.y *= .8f;
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
        if (gravityControlled)
        {
            return;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
