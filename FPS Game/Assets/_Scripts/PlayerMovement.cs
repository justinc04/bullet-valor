using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    [Header("Mouse Look")]
    [SerializeField] float mouseSensitivity;
    private float verticalLookRot;

    [Header("Movement")]
    public float runSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float speedSmoothTime;
    [SerializeField] float accelerationSmoothTime;
    [SerializeField] float decelerationSmoothTime;
    [SerializeField] float airSmoothTimeMultiplier;
    [SerializeField] float landingSpeedDuration;
    [HideInInspector] public float speedAffector = 1;
    [HideInInspector] public bool speedControlled;
    [HideInInspector] public bool directionControlled;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public float speed;
    private Vector3 smoothMoveDirection;
    private float smoothMoveSpeed;
    private float landingSpeedAffector = 1;

    [Header("Gravity")]
    [SerializeField] float gravityScale;
    [SerializeField] float fallGravityScale;
    [HideInInspector] public bool gravityControlled;
    private const float gravityConstant = -9.81f;
    private float gravity;

    [Header("Jumping")]
    [SerializeField] float jumpHeight;
    [SerializeField] float groundDistance;
    [SerializeField] float hangTime;
    [SerializeField] float jumpBuffer;
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
            Vector3 targetDirection = (transform.right * horizontal + transform.forward * vertical).normalized;
            float smoothTime = direction.magnitude < targetDirection.magnitude ? accelerationSmoothTime : decelerationSmoothTime;
            smoothTime *= (isGrounded ? 1 : airSmoothTimeMultiplier);
            direction = Vector3.SmoothDamp(direction, targetDirection, ref smoothMoveDirection, smoothTime);
        }

        if (!speedControlled)
        {
            float targetSpeed = (Input.GetKey(KeyCode.LeftShift) ? walkSpeed : runSpeed) * speedAffector * landingSpeedAffector;
            speed = Mathf.SmoothDamp(speed, targetSpeed, ref smoothMoveSpeed, speedSmoothTime);
        }

        controller.Move(direction * speed * Time.deltaTime);

        if (direction != Vector3.zero && controller.velocity.magnitude > runSpeed * .2f && !Input.GetKey(KeyCode.LeftShift) && isGrounded)
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
            StartCoroutine(Landed());
            playerAudio.Play("Jump Land");
        }

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -5f;
        }
    }

    IEnumerator Landed()
    {
        landingSpeedAffector = 0;
        yield return new WaitForSeconds(landingSpeedDuration);
        landingSpeedAffector = 1;
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
