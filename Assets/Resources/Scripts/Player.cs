using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class Player : MonoBehaviour, IDamageable
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

    private bool isPlayingLandingAnimation = false;

    [SerializeField] private SkillManager skillManager;

    private bool isDead = false;
    private bool isDying = false;

    [SerializeField] private bool isStunned = false;
    [SerializeField] private float currentStunTime = 0f;

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackStunTime = 1.4f;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        verticalSpeed = -10.0f;
        // Initialize health
        currentHealth = maxHealth;
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
        playerInput.OnAttackPerformed += PlayerInput_OnAttackPerformed;
    }

    private void PlayerInput_OnInteractPerformed(object sender, System.EventArgs e)
    {
        // Skip if player is dead or stunned
        if (isDead || isStunned) return;

        // Define interaction distance
        float interactDistance = 2f;

        // Find all objects with Interactable component in scene
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactDistance);
        
        // Find the closest interactable object
        float closestDistance = float.MaxValue;
        IInteractable closestInteractable = null;
        
        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        // Interact with the closest object if found
        if (closestInteractable != null)
        {
            closestInteractable.Interact(gameObject);
        }
    }

    private void PlayerInput_OnJumpPerformed(object sender, System.EventArgs e)
    {
        if (isGrounded && !isDead)
        {
            animator.SetTrigger("IsJumping");
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

    private void PlayerInput_OnAttackPerformed(object sender, System.EventArgs e)
    {
        Debug.Log("attack!");
        if (isDead || isStunned) return;
        
        // Play attack animation
        animator.SetTrigger("Attack");
        
        // Detect enemies in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        
        foreach (var hitCollider in hitColliders)
        {
            // Check if object is in front of player using dot product
            Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToTarget);
            
            // Only damage enemies in front of player (dot product > 0.5 means within ~90 degrees)
            if (dotProduct > 0.5f)
            {
                // Try to get IDamageable interface
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }
        ApplyStun(attackStunTime);
    }


    private void OnDestroy()
    {
        playerInput.OnSkill1Performed -= PlayerInput_OnSkill1Performed;
        playerInput.OnSkill2Performed -= PlayerInput_OnSkill2Performed;
        playerInput.OnJumpPerformed -= PlayerInput_OnJumpPerformed;
        playerInput.OnInteractPerformed -= PlayerInput_OnInteractPerformed;
    }

    private void FixedUpdate()
    {
        // Add stun timer update
        if (isStunned)
        {
            currentStunTime -= Time.fixedDeltaTime;
            if (currentStunTime <= 0)
            {
                isStunned = false;
                animator.SetBool("IsStunned", false);
            }
        }

        HandleGroundCheck();
        HandleMovement();
        HandleGravity();
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(feetTransform.position, goundCheckRadius, groundLayer);

        float verticalSpeed = controller.velocity.y;

        if (verticalSpeed < 0 && !isGrounded){
            animator.SetBool("Falling",true);
        } 
        else{
            animator.SetBool("Falling", false);
        }
    }

    private void HandleMovement()
    {
        // Add stun check at the beginning
        if (isStunned || isPlayingLandingAnimation || isDead)
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
        } else{
            verticalSpeed -= 0.1f;
        }
    }


    public void ApplyStun(float stunDuration)
    {
        if (!isStunned)
        {
            isStunned = true;
            currentStunTime = stunDuration;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        
        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
}
