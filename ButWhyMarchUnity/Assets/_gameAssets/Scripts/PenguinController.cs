using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PenguinController : MonoBehaviour
{
    private PenguinInputs inputActions;
    private CharacterController controller;
    private Animator anim;

    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float slideSpeed = 15f;
    public float rotationSpeed = 180f;
    public float rotationSmoothTime = 0.15f;
    public float jumpHeight = 1.5f;

    [Header("Animation Settings")]
    [Range(0.1f, 3f)]
    public float animSpeedMultiplier = 1.5f;

    [Header("Sliding Settings")]
    public float normalHeight = 1.8f;
    public float slideHeight = 0.6f;
    public Vector3 normalCenter = new Vector3(0, 0.9f, 0);
    public Vector3 slideCenter = new Vector3(0, 0.3f, 0);

    [Header("Visual Juice")]
    public Transform visualModel;
    public float leanAmount = 15f;
    public float leanSpeed = 5f;

    [Header("Physics")]
    public float gravity = -19.62f;

    [Header("Audio")]
    public AudioSource snowStepSound; // Inspector’dan baðla

    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isSprinting;
    private bool isSliding;
    private float smoothMoveAmount;

    private float currentRotationVelocity;
    private float rotationVelocitySmooth;
    private float currentLeanAngle;

    private void Awake()
    {
        inputActions = new PenguinInputs();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        inputActions.PenguinActions.Jump.performed += ctx => Jump();
        inputActions.PenguinActions.Sprint.started += ctx => isSprinting = true;
        inputActions.PenguinActions.Sprint.canceled += ctx => isSprinting = false;
        inputActions.PenguinActions.Slide.started += ctx => StartSlide();
        inputActions.PenguinActions.Slide.canceled += ctx => StopSlide();
    }

    private void OnEnable() => inputActions.PenguinActions.Enable();
    private void OnDisable() => inputActions.PenguinActions.Disable();

    private void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        moveInput = inputActions.PenguinActions.Move.ReadValue<Vector2>();

        ApplyRotation();
        MovePlayer();

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        UpdateAnimations();
        UpdateFootstepSound(); // Ses kontrolü burada
    }

    private void ApplyRotation()
    {
        float targetRotationVelocity = moveInput.x * rotationSpeed;
        currentRotationVelocity = Mathf.SmoothDamp(currentRotationVelocity, targetRotationVelocity, ref rotationVelocitySmooth, rotationSmoothTime);

        transform.Rotate(0, currentRotationVelocity * Time.deltaTime, 0);

        if (visualModel != null)
        {
            float targetLean = -moveInput.x * leanAmount;
            currentLeanAngle = Mathf.Lerp(currentLeanAngle, targetLean, Time.deltaTime * leanSpeed);
            visualModel.localRotation = Quaternion.Euler(0, 0, currentLeanAngle);
        }
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = transform.forward * moveInput.y;

        if (moveDirection.magnitude >= 0.1f || isSliding)
        {
            float currentSpeed = isSliding ? slideSpeed : (isSprinting ? sprintSpeed : walkSpeed);
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
    }

    private void StartSlide()
    {
        if (!controller.isGrounded) return;
        isSliding = true;
        controller.height = slideHeight;
        controller.center = slideCenter;
    }

    private void StopSlide()
    {
        isSliding = false;
        controller.height = normalHeight;
        controller.center = normalCenter;
    }

    private void UpdateAnimations()
    {
        float targetMoveAmount = Mathf.Abs(moveInput.y);
        smoothMoveAmount = Mathf.SmoothStep(smoothMoveAmount, targetMoveAmount, Time.deltaTime * 5f);
        anim.SetFloat("Vert", smoothMoveAmount, 0.3f, Time.deltaTime);
        anim.speed = targetMoveAmount > 0.1f ? (isSprinting ? animSpeedMultiplier * 1.5f : animSpeedMultiplier) : 1f;
        anim.SetFloat("State", isSprinting ? 1f : 0f, 0.2f, Time.deltaTime);
        anim.SetBool("IsSliding", isSliding);
    }

    private void UpdateFootstepSound()
    {
        bool isMoving = moveInput.y != 0 || isSliding;

        if (isMoving && controller.isGrounded)
        {
            if (!snowStepSound.isPlaying)
                snowStepSound.Play();

            // Hareket ederken ses yavaþça normal seviyeye çýkar
            snowStepSound.volume = Mathf.Lerp(snowStepSound.volume, 1f, Time.deltaTime * 5f);
        }
        else
        {
            // Durduðunda ses yavaþça azalýr
            snowStepSound.volume = Mathf.Lerp(snowStepSound.volume, 0f, Time.deltaTime * 5f);

            // Ses tamamen sýfýra indiðinde durdur
            if (snowStepSound.volume <= 0.01f && snowStepSound.isPlaying)
                snowStepSound.Stop();
        }
    }


    private void Jump()
    {
        if (controller.isGrounded && !isSliding)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
