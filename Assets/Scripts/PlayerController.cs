using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, GameManager.IRestartGameElement
{
    [Header("References")]
    private Animator animator;
    public CharacterController characterController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private ParticleSystem walkParticles;
    [SerializeField] private MarioSounds effects;
    [SerializeField] private AudioSource checkPointSound;
    private bool hitByShell;
    private float hitByShellCD;
    [SerializeField] private PickUpShell grabShell;
    [Space]
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float walkSpeed = 0.4f;
    [SerializeField] private float crouchSpeed = 0.4f;
    [SerializeField] private float runSpeed = 1.0f;
    [SerializeField] private float verticalSpeed = 0.0f;
    private Vector3 rotationSpeed;
    [SerializeField] private float lerpRotation = 0.7f;
    private Quaternion lastLookRotation = Quaternion.identity;
    private bool isCouching;
    private bool isSliding;
    [SerializeField] private float initialSlideSpeed = 1.5f;
    private float currentSlideSpeed;
    [SerializeField] private float slideAccelaeration = 0.1f;
    [SerializeField] private Transform feetPos;
    [SerializeField] private float feetCheckSphereRadious = 0.1f;
    [SerializeField] private LayerMask groundLayerMask;
    



    [Space]
    [Header("Jump")]
    [SerializeField] private float gravityMultiplier = 3.0f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float jumpVelocity = 10.0f;
    [SerializeField] private int comboJumpCount = 0;
    [SerializeField] private int maxComboJumps=3;
    [SerializeField] private float multipleJumpSpeedDecreaseMultiplier;
    private Vector3 hitNormal;
    [SerializeField] private float slideFriction;
    //private PhysicMaterial physicMaterial;
    private bool isJumping;
    public float bridgeForce = 5f;

    [Space]
    [Header("Punch")]
    public GameObject PunchParticles;
    public float currentComboTime;
    public float comboTime;
    public int currentComboPunch;
    public bool comboTimeStarted = false;
    public GameObject leftPunchCollider;
    public GameObject rightPunchCollider;
    public GameObject kickCollider;
    public enum PunchType { Right, Left, Kick};
    public float verticalSpeedToKill = 0;
    
    [Space]
    [Header("Checkpoints")]
    [SerializeField] private Transform startPosition;
    private GameObject currentCheckpoint;

    public GameObject currentElevator;
    public float UpElevatorDot = 0.95f;

    [Space]
    public ParticleSystem zzzParticleSystem; 


    #region Setters
    public void SetIsSliding(bool b) 
    { 
        isSliding = b;
        if (b)
        {
            currentSlideSpeed = initialSlideSpeed;
            effects.StartSlide();
        }
        
    }
    #endregion




    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cameraController = Camera.main.GetComponent<CameraController>();
        grabShell = GetComponent<PickUpShell>();
        effects = GetComponent<MarioSounds>();
        //physicMaterial = characterController.material;
    }
    void Start()
    {
        hitByShellCD = 0.5f;
        hitByShell = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.position = startPosition.position;
        transform.rotation = startPosition.rotation;
        GameObject.FindObjectOfType<GameManager>().AddRestartGameElement(this);
    }

    void Update()
    {
        if (GameManager.instance.isDead == false)
        {
            if (InputSystem.instance.Punch && animator.GetBool("punch") == false)
            {

                animator.SetTrigger("punch");
                animator.SetInteger("comboPunch", GetComboPunch());
                //currentComboTime = comboTime;
                //Debug.Log("Punch");

            }

            JumpUpdate();
            
            MovePlayer();

            CrouchUpdate();

            SlideUpdate();

            UpdateComboTime();

            UpdateElevator();
        }




     
    }
    #region movement
    private void SlideUpdate()
    {
        if (isSliding)
        {
            currentSlideSpeed -= slideAccelaeration * Time.deltaTime;
            effects.SetSlideVolume(currentSlideSpeed / initialSlideSpeed);
            if(currentSlideSpeed < 0)
            {
                currentSlideSpeed = 0;
                effects.EndSlide();
            }
            if (!isCouching)
            {
                isSliding = false;
                effects.EndSlide();
            }
        }
        //Debug.Log(isSliding);
        
    }

    private void CrouchUpdate()
    {
        if (isGrounded)
        {
            if (InputSystem.instance.Crouch)
            {
                isCouching = true;
            }
            else
            {
                isCouching = false;
            }
            animator.SetBool("crouch", isCouching);
        }
        
    }

    private void JumpUpdate()
    {
        ////////Debug.Log(isGrounded);
        isJumping = false;
        if (InputSystem.instance.Jump)
        {
            Debug.Log(comboJumpCount + " " + maxComboJumps);
            if (comboJumpCount < maxComboJumps)
            {
                if(Physics.Raycast(new Ray(transform.position + Vector3.up, transform.forward), 1.0f, groundLayerMask))
                {
                    StartCoroutine(WallJump());
                }
                else
                {
                    Jump();
                }
                
                isJumping = true;

            }
            
        }
        if (isGrounded)
        {
            //Debug.Log("grounded");
            if (!isJumping)
            {
                //Debug.Log("resetCombo");
                comboJumpCount = 0;
                animator.SetInteger("comboJump", comboJumpCount);
                isJumping = false;
            }
            //animator.ResetTrigger("jump");
        }

        //AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        //if (info.IsName("land"))
        animator.SetBool("grounded", isGrounded);
    }

    private IEnumerator WallJump()
    {
        
        characterController.enabled = false;
        yield return null;
        
        transform.Rotate(new Vector3(0, 180, 0));
        animator.SetBool("Wall", true);
        yield return null;

        verticalSpeed = jumpVelocity - comboJumpCount * multipleJumpSpeedDecreaseMultiplier;
        comboJumpCount++;
        animator.SetTrigger("jump");
        Invoke("UpdateAnimatorComboJump", 0.1f);
        InputSystem.instance.SetInvertMovementInput(true);
        yield return null;
        animator.SetBool("Wall", false);
        characterController.enabled = true;
    }

    private void Jump()
    {
        //Debug.Log("Jump");

        verticalSpeed = jumpVelocity - comboJumpCount * multipleJumpSpeedDecreaseMultiplier;
        comboJumpCount++;
        animator.SetTrigger("jump");
        InputSystem.instance.SetInvertMovementInput(false);
        Invoke("UpdateAnimatorComboJump", 0.1f);
        //Debug.Log(comboJumpCount);
    }
    private void UpdateAnimatorComboJump()
    {
        animator.SetInteger("comboJump", comboJumpCount);
    }

    private void MovePlayer()
    {
        //get camera axis
        Vector3 right = cameraController.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 forward = cameraController.transform.forward;
        forward.y = 0;
        forward.Normalize();

        //set movement axis
        Vector3 movement = Vector3.zero;
        movement += forward * InputSystem.instance.MovementY;
        movement += right * InputSystem.instance.MovementX;
        movement.Normalize();


        // set movement velocity
        float axisMagnitude = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")).magnitude;
        float speed = walkSpeed * axisMagnitude;

        if (isCouching)
        {
            speed = crouchSpeed * axisMagnitude;
            if (isSliding)
            {
                speed = currentSlideSpeed * axisMagnitude;
            }
        }
        else if (InputSystem.instance.Sprint && speed > 0) //sprint
        {
            if (!grabShell.hasItem)
            {
                speed = runSpeed * axisMagnitude;
            }
            
        }
        

        //set animations
        animator.SetFloat("speed", speed, 0.2f, Time.deltaTime);
        animator.SetBool("isDead", GameManager.instance.isDead);

        //set movement direction
        Quaternion desiredRotation = lastLookRotation;
        if (movement != Vector3.zero)
        {
            desiredRotation = Quaternion.LookRotation(movement);
            lastLookRotation = desiredRotation;
        }


        //movement
        
        movement *= Time.deltaTime * speed * movementSpeed;

        verticalSpeed += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        movement.y = verticalSpeed * Time.deltaTime;


        //collisions
        CollisionFlags l_CollisionFlags = characterController.Move(movement);
        if (Physics.CheckSphere(feetPos.position, feetCheckSphereRadious, groundLayerMask) && verticalSpeed <= 0)
        {
            //Debug.Log("Grounded");
            isGrounded = true;
            verticalSpeed = 0;
            InputSystem.instance.SetInvertMovementInput(false);
        }
        else
        {
            isGrounded = false;
        }

        
        

        //apply rotation
        if (speed == 0)
        {
            desiredRotation = transform.rotation;
            
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, lerpRotation * Time.deltaTime);



       

        //partices run

        if (speed < 0.4f || !isGrounded) 
        {
            walkParticles.Stop();
        }
        if (speed > 0.4f && !isGrounded)
        {
            walkParticles.Play();
        }
        


    }
#endregion
    #region PUNCH
    public void UpdatePunch(PunchType punchType, bool enabled)
    {
        GameObject punchCollider = null;

        switch (punchType)
        {
            case PunchType.Left:
                punchCollider = leftPunchCollider;
                break;
            case PunchType.Right:
                punchCollider = rightPunchCollider;
                break;
            case PunchType.Kick:
                punchCollider = kickCollider;
                break;
            default:
                break;
        }
        punchCollider.SetActive(enabled);
    }


    
    public void StartComboTime()
    {
        currentComboTime = comboTime;
        comboTimeStarted = true;
    }

    public void UpdateComboTime()
    {
        if (currentComboTime >= 0 && comboTimeStarted)
        {
            currentComboTime -= Time.deltaTime;
            if (currentComboTime<= 0)
            {
                comboTimeStarted = false;
            }
        }
    }

    private int GetComboPunch()
    {
        if (currentComboTime <= 0)
        {

            currentComboPunch = 1;
            return 0;
        }
        else
        {
            int currentCombo = currentComboPunch;
            ++currentComboPunch;
            if (currentComboPunch >= 3)
                currentComboPunch = 0;
            return currentCombo;
            
        }
    }
    #endregion
    #region CHECKPOINT
    public void RestartGame()
    {
        
        characterController.enabled = false;


        if(GameManager.instance.checkPoint == true && currentCheckpoint != null)
        {
            transform.position = currentCheckpoint.transform.position;
            transform.rotation = currentCheckpoint.transform.rotation;
        }
        else
        {
            transform.position = startPosition.position;
            transform.rotation = startPosition.rotation;
        }
        characterController.enabled = true;
        gameObject.SetActive(true);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            currentCheckpoint = other.gameObject;
            other.gameObject.SetActive(false);
            checkPointSound.Play();
        }
        else if (other.CompareTag("Elevator"))
        {
            SetCurrentElevator(other.gameObject);
        }
        if (other.tag == "DeadZone")
        {
            FindObjectOfType<HealthController>().Kill();
        }
        if (other.gameObject.CompareTag("Shell"))
        {
            if (CanKillWithFeet())
            {
                other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 1350);
                JumpOverEnemy();
            }
            else
            {
                if (hitByShell == false)
                {
                    if (other.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 3)
                    {
                        if (FindObjectOfType<PickUpShell>().shotShell == false)
                        {
                            FindObjectOfType<HealthController>().SubstractHealth();
                            hitByShell = true;
                            Invoke("CanGetHitByShell", hitByShellCD);
                        }
                    }
                }

            }
        }
    }


    void CanGetHitByShell()
    {
        hitByShell = false;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            this.transform.parent = null;
        }
    }



    #endregion
    #region ELEVATOR
    void SetCurrentElevator(GameObject Elevator)
    {
        if (IsOnElevator(Elevator.transform))
        if (IsOnElevator(Elevator.transform))
        {
            currentElevator = Elevator;
            transform.SetParent(Elevator.transform);
        }
    }
    void UpdateElevator()
    {
        if (currentElevator != null)
        {
            if (!IsOnElevator(currentElevator.transform))
            {
                DetachElevator();
            }
        }
    }
    bool IsOnElevator(Transform ElevatorTransform)
    {
        return Vector3.Dot(ElevatorTransform.transform.forward, Vector3.up) > UpElevatorDot;
    }
    void DetachElevator()
    {
        currentElevator = null;
        transform.SetParent(null);
        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
    }
    #endregion
    public bool CanKillWithFeet()
    {
        return verticalSpeed < verticalSpeedToKill;
    }

    public void JumpOverEnemy()
    {
        verticalSpeed = jumpVelocity;
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Goomba"))
        {
            GoombaEnemy goombaEnemy = hit.gameObject.GetComponent<GoombaEnemy>();
            if (CanKillWithFeet())
            {
                    goombaEnemy.Kill();
                    GameObject particles = Instantiate(PunchParticles, transform.position, Quaternion.identity);
                    Destroy(particles, 1);
                    JumpOverEnemy();
            }
        }
        if (hit.gameObject.CompareTag("Koopa"))
        {
            KoopaEnemy koopaEnemy = hit.gameObject.GetComponent<KoopaEnemy>();
            if (CanKillWithFeet())
            {
                    koopaEnemy.Kill();
                    JumpOverEnemy();
            }
        }
        if (hit.gameObject.tag == "Bridge")
        {
            Rigidbody bridge = hit.collider.GetComponent<Rigidbody>();
            bridge.AddForceAtPosition(-hit.normal * bridgeForce, hit.point);
        }
    }

    

    public void CallStateEnter(float distanceToDrop, float dropVelocity, Transform tf)
    {
        StartCoroutine(StateEnter(distanceToDrop, dropVelocity, tf));
        zzzParticleSystem.Play();
        effects.StartSleep();
    }
    private IEnumerator StateEnter(float distanceToDrop, float dropVelocity, Transform tf)
    {

        float distanceDroped = 0;
        while (distanceDroped < distanceToDrop)
        {
            float distanceToDropThisFrame = dropVelocity * Time.deltaTime;
            distanceDroped += distanceToDropThisFrame;
            tf.Translate(Vector3.down * distanceToDropThisFrame);
            yield return null;
        }
        yield return null;
    }

    public void CallStateExit(float distanceToDrop, float dropVelocity, Transform tf)
    {
        StartCoroutine(StateExit(distanceToDrop, dropVelocity, tf));
        zzzParticleSystem.Stop();
        effects.EndSleep();
    }
    private IEnumerator StateExit(float distanceToDrop, float dropVelocity, Transform tf)
    {

        float distanceDroped = 0;
        while (distanceDroped < distanceToDrop)
        {
            float distanceToDropThisFrame = dropVelocity * Time.deltaTime;
            distanceDroped += distanceToDropThisFrame;
            tf.Translate(Vector3.up * distanceToDropThisFrame);
            yield return null;
        }
        yield return null;
    }


}
