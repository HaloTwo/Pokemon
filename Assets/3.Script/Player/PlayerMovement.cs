using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] private float defaultSpeed = 600;
    private float currentSpeed;
    [SerializeField] private float walkSpeed = 200;
    [Header("회전 속도")]
    [SerializeField] private float rotationSpeed = 5f;
    [Header("몬스터 볼 관련")]
    [SerializeField] private float ThrowPower = 20f;
    [SerializeField] private float distance = 6f;


    [Header("체크")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private bool isLookon;

    [SerializeField] private GameObject FollowCamera;

    [SerializeField] private Transform ball_loc;
    [SerializeField] private GameObject ball_prefab;

    private Vector3 moveDirection;

    private PlayerControlsButton inputActions;
    private CharacterController controller;
    private Animator animator;

    public Collider[] colls;

    //볼 중력
    private Rigidbody ball_rb;

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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        currentSpeed = defaultSpeed;

        if (groundCheck == null) groundCheck = transform.Find("groundCheck");
        if (ball_loc == null) ball_loc = transform.Find("tr0050_00.trmdl/origin/foot_base/waist/spine_01/spine_02/spine_03/right_shoulder/right_arm_width/right_arm_01/right_arm_02/right_hand/right_attach_on");
        ball_rb = ball_prefab.GetComponent<Rigidbody>();
        TryGetComponent(out controller);
        TryGetComponent(out animator);

    }


    private void FixedUpdate()
    {

        isGrounded = IsGrounded();
        animator.SetBool("isGround", isGrounded);

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



    public void onWalk(InputAction.CallbackContext context)
    {
        //걷기 버튼을 꾹 눌렀을때
        if (context.performed)
        {
            currentSpeed = walkSpeed;
            animator.SetFloat("Velocity", 1);
        }
        //걷기 버튼을 땠을때
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


    #region 범위 시각화

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 boxsize = transform.lossyScale;
        Gizmos.DrawWireCube(groundCheck.position, boxsize);

        //Gizmos.DrawSphere(transform.position + transform.forward * (distance - 0.5f), distance);
        //Gizmos.DrawCube(this.transform.position + transform.forward * 5f, new Vector3(10f ,8f, 10f));

        Vector3 position = transform.position + transform.forward * 5f;
        Quaternion rotation = transform.rotation;
        Vector3 size = new Vector3(2f, 6f, 10f);

        // 기즈모 시각화
        Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, size);
    }
    #endregion

    #region 볼던지기

    //버튼 이벤트
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(StartThorw_co());
        }
    }

    //볼 날리는 코루틴
    IEnumerator StartThorw_co()
    {
        isLookon = true;
        animator.SetTrigger("Throw");

        yield return null;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsTag("Move"));
        isLookon = false;
    }

    //볼 날라가는 이벤트
    public void Bullthrow()
    {

        ball_prefab.SetActive(true);
        //float time =Time.fixedDeltaTime++;
        ball_rb.velocity = Vector3.zero;
        ball_rb.angularVelocity = Vector3.zero;
        ball_prefab.transform.rotation = Quaternion.identity;
        ball_prefab.transform.position = ball_loc.position;

        //colls = Physics.OverlapSphere(transform.position + transform.forward * (distance - 0.5f), distance, PokemonLayer);

        Vector3 position = transform.position + transform.forward * 5f;
        Quaternion rotation = transform.rotation;
        Vector3 size = new Vector3(2f, 6f, 10f);

        int PokemonLayer = LayerMask.GetMask("Pokemon");
        colls = Physics.OverlapBox(position, size, rotation, PokemonLayer);


        if (colls.Length > 0)
        {
            Collider closestPokemon = null;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < colls.Length; i++)
            {
                Collider coll = colls[i];
                float distanceToCollider = Vector3.Distance(transform.position, coll.transform.position);

                if (distanceToCollider < closestDistance)
                {
                    closestPokemon = coll;
                    closestDistance = distanceToCollider;
                }
            }

            if (closestPokemon != null)
            {
                Debug.Log("Closest Pokemon: " + closestPokemon.name);
                Vector3 forceDirection = (closestPokemon.transform.position - transform.position).normalized;
                ball_rb.AddForce(forceDirection * ThrowPower, ForceMode.Impulse);
            }
        }
        else
        {
            ball_rb.AddForce(transform.forward * ThrowPower, ForceMode.Impulse);
        }

        Invoke("DisableBallPrefab", 1.2f);
    }
    void DisableBallPrefab()
    {
        ball_prefab.SetActive(false);
    }

    #endregion
}
