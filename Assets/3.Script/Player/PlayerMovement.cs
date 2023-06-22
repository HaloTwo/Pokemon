using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float defaultSpeed = 900;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float walkSpeed = 400;
    [SerializeField] private float rotationSpeed = 10f;

    private PlayerControlsButton inputActions;
    private CharacterController controller;
    private Animator animator;


    private Vector3 moveDirection;

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

        if (hasControl)
        {
            //transform.localRotation = Quaternion.LookRotation(moveDirection);

            //회전
            float rotationAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, rotationAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            //이동
            controller.Move(transform.forward * (currentSpeed * 0.01f) * Time.fixedDeltaTime);
        }
        else
        {
            animator.SetFloat("Velocity", 0);
        }
    }

    public void onMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (input != null)
        {
            // 카메라의 전방 벡터를 획득
            Vector3 cameraForward = Vector3.Scale(FollowCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

            Debug.Log(cameraForward);
            // 입력값과 카메라의 전방 벡터를 조합하여 이동 방향 계산
            moveDirection = (input.x * FollowCamera.transform.right + input.y * cameraForward).normalized;

            //moveDirection = new Vector3(input.x, 0f, input.y);
            //Debug.Log("유니티 이벤트 : " + input.magnitude);
        }

        if (animator.GetFloat("Velocity") != 1)
        {
            animator.SetFloat("Velocity", 2);
        }
    }

    public void onJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
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
}
