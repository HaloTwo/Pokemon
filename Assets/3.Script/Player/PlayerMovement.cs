using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private Animator animator;

    private Vector3 moveDirection;
    [SerializeField] private float defaultSpeed = 400f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float runSpeed = 900f;

    void Start()
    {
        currentSpeed = defaultSpeed;

        TryGetComponent(out animator);
    }

    // Update is called once per frame
    void Update()
    {
        bool hasControl = (moveDirection != Vector3.zero);

        if (hasControl)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
            transform.Translate(Vector3.forward * (currentSpeed * 0.01f) * Time.deltaTime);
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
            moveDirection = new Vector3(input.x, 0f, input.y);
            Debug.Log("유니티 이벤트 : " + input.magnitude);
        }

        if (animator.GetFloat("Velocity") != 2)
        {
            animator.SetFloat("Velocity", 1);
        }

    }

    public void onJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
    }
    public void onDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentSpeed = runSpeed;
            animator.SetFloat("Velocity", 2);
        }
        else if (context.canceled)
        {
            currentSpeed = defaultSpeed;
        }
    }
}
