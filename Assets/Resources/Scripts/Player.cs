using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{

    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private CharacterController controller;

    [SerializeField] Animator animator;
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    private AudioSource Audio;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float verticalSpeed;

    public LayerMask groundLayer;
    [SerializeField] private Transform feetTransform;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float goundCheckRadius = 0.01f;

    [SerializeField] private float runningFactor;

    [SerializeField] private float minFallDistanceForAnim = 3f;

    private bool isLandingAnimationTriggered = false;
    private bool isPlayingLandingAnimation = false;

    [SerializeField] private bool isJumping = false;

    [SerializeField] private SkillManager skillManager;

    private bool isDead = false;
    private bool isDying = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        verticalSpeed = -10.0f;
        // Add skills
        skillManager.AddSkill(gameObject.AddComponent<FireballSkill>());
        skillManager.AddSkill(gameObject.AddComponent<FlySkill>());
        skillManager.InitializeSkills();
    }

    public IEnumerator Die()
    {
        if (isDying) yield break;
        
        isDying = true;
        animator.SetTrigger("Die");
        isDead = true;

        // Wait until the death animation is finished
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        
        isDying = false;
    }

    private void Start()
    {
        playerInput.OnJumpPerformed += PlayerInput_OnJumpPerformed;
        playerInput.OnInteractPerformed += PlayerInput_OnInteractPerformed;
        GameManager.ShowMouse(false);
        HandleMovement();
        HandleGravity();

        playerInput.OnSkill1Performed += PlayerInput_OnSkill1Performed;
        playerInput.OnSkill2Performed += PlayerInput_OnSkill2Performed;
    }

    private void PlayerInput_OnInteractPerformed(object sender, System.EventArgs e)
    {
        // TODO: Implement interact, when you press key 'E'.
        // In this function, you're noticed by your Input System that key 'E' is pressed.
        // You could check whether a door at the front of the Player using
        // bool Physics.Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask);
        // https://docs.unity.cn/cn/2019.4/ScriptReference/Physics.Raycast.html is helpful.
        throw new System.NotImplementedException();
    }

    private void PlayerInput_OnJumpPerformed(object sender, System.EventArgs e)
    {
        if (isGrounded)
        {
            isJumping = true;
            animator.SetTrigger("Jump");
            animator.SetBool("IsJumping", true);
            verticalSpeed = jumpForce;
        }
    }

    private void PlayerInput_OnSkill1Performed(object sender, System.EventArgs e)
    {
        skillManager.ActivateSkill(0, gameObject);
    }

    private void PlayerInput_OnSkill2Performed(object sender, System.EventArgs e)
    {
        skillManager.ActivateSkill(1, gameObject);
    }

    private void OnDestroy()
    {
        playerInput.OnSkill1Performed -= PlayerInput_OnSkill1Performed;
        playerInput.OnSkill2Performed -= PlayerInput_OnSkill2Performed;
    }

    private void FixedUpdate()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleGravity();
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(feetTransform.position, goundCheckRadius, groundLayer);
        float distanceToGround = GetDistanceToGround();
        
        if (distanceToGround < minFallDistanceForAnim && verticalSpeed < 0 && !isLandingAnimationTriggered && !isGrounded)
        {
            animator.SetTrigger("Landing");
            isLandingAnimationTriggered = true;
            isPlayingLandingAnimation = true;
            StartCoroutine(ResetLandingAnimation());
        } else if (isGrounded)
        {
            isLandingAnimationTriggered = false;
            if(isJumping && verticalSpeed < 1f)
            {
                isJumping = false;
                animator.SetBool("IsJumping", false);
            }
        }
    }

    private float GetDistanceToGround()
    {
        if (Physics.Raycast(feetTransform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return hit.distance;
        }
        return Mathf.Infinity;
    }

    private void HandleMovement()
    {
        if (isPlayingLandingAnimation || isDead)
        {
            return;
        }

        Vector2 inputVector = playerInput.GetMovementVectorNormalized();

        Transform mainCamera = Camera.main.transform;
        Vector3 forward = mainCamera.forward;
        Vector3 right = mainCamera.right;

        forward.y = 0;
        right.y = 0;

        Vector3 moveDirection = forward * inputVector.y + right * inputVector.x;
        moveDirection = moveDirection.normalized;

        // Rotate the Player. The forward direction of Player should be the same as Camera
        transform.forward = Vector3.Slerp(transform.forward,
                                          moveDirection,
                                          rotateSpeed * Time.deltaTime);
        
        float currentMoveSpeed = playerInput.IsRunning() ? moveSpeed * runningFactor : moveSpeed;

        controller.Move(moveDirection * currentMoveSpeed * Time.deltaTime);

        float actualSpeed = Vector3.ProjectOnPlane(controller.velocity, Vector3.up).magnitude;

        float speed = actualSpeed / moveSpeed; 
        animator.SetFloat("HorizontalSpeed", speed, StartAnimTime, Time.deltaTime);
    }

    private void HandleGravity()
    {
        Vector3 verticalVec = new Vector3(0, verticalSpeed * Time.deltaTime, 0);
        controller.Move(verticalVec);
        
        if (!isGrounded)
        {
            verticalSpeed -= 9.8f * Time.deltaTime;
        }
    }

    private IEnumerator ResetLandingAnimation()
    {
        yield return new WaitForSeconds(1.1f); // the length of landing animation
        isPlayingLandingAnimation = false;
    }
}
