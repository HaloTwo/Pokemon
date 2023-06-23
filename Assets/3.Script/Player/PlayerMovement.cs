using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float defaultSpeed = 600;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float walkSpeed = 200;
    [SerializeField] private float rotationSpeed = 10f;

    private PlayerControlsButton inputActions;
    private CharacterController controller;
    private Animator animator;


    private Vector3 moveDirection;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private bool isLookon;

    [SerializeField] private GameObject FollowCamera;


    private void OnEnable()
    {
        if (inputActions == null) inputActions = new PlayerControlsButton();
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        currentSpeed = defaultSpeed;

        TryGetComponent(out controller);
        TryGetComponent(out animator);
    }

    // Update is called once per frame
    void Update()
    {
        bool hasControl = (moveDirection != Vector3.zero);

        // 회전
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 cameraForward = Vector3.Scale(FollowCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        moveDirection = (input.x * FollowCamera.transform.right + input.y * cameraForward).normalized;

        if (hasControl && isGrounded && !isLookon)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            controller.Move(transform.forward * (currentSpeed * 0.01f) * Time.fixedDeltaTime);


            if (animator.GetFloat("Velocity") != 1)
            {
                animator.SetFloat("Velocity", 2);
            }
        }
        else
        {
            animator.SetFloat("Velocity", 0);
        }


    }

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();

        animator.SetBool("isGround", isGrounded);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(StartThorw_co());
        }
    }

    IEnumerator StartThorw_co()
    {
        isLookon = true;
        animator.SetTrigger("Throw");
        //while (true)
        //{
        //    yield return null;
        //}
        yield return animator.GetCurrentAnimatorStateInfo(0).IsName("tr0050_00_trmdl_throw02_gfbanm");
        isLookon = false;
    }


    public void onWalk(InputAction.CallbackContext context)
    {
        //대쉬 버튼을 꾹 눌렀을때
        if (context.performed)
        {
            currentSpeed = walkSpeed;
            animator.SetFloat("Velocity", 1);
        }
        //대쉬 버튼을 땠을때
        else if (context.canceled)
        {
            currentSpeed = defaultSpeed;
            animator.SetFloat("Velocity", 2);
        }
    }

    public bool IsGrounded()
    {
        Vector3 boxsize = transform.lossyScale;
        int groundLayerMask = LayerMask.GetMask("GroundLayer");
        bool isGrounded = Physics.CheckBox(groundCheck.position, boxsize, Quaternion.identity, groundLayerMask);

        return isGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 boxsize = transform.lossyScale;
        Gizmos.DrawWireCube(groundCheck.position, boxsize);
    }

}
