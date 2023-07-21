using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public bool ismove = true;
    [Header("ȸ�� �ӵ�")]
    [SerializeField] private float rotationSpeed = 5f;
    [Header("���� �� ����")]
    [SerializeField] private float ThrowPower = 20f;
    [SerializeField] private Transform ball_loc;
    [SerializeField] private GameObject ball_prefab;
    [SerializeField] private GameObject show_Ball;

    //�� �߷�
    private Rigidbody ball_rb;

    [Header("üũ")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private bool isLookon;
    public bool isBattle;
    private bool hasControl;
    [SerializeField] private bool isWalking = false;
    [Header("ī�޶�")]
    [SerializeField] private GameObject FollowCamera;


    public Vector3 moveDirection;

    private PlayerControlsButton inputActions;
    private CharacterController controller;
    [HideInInspector]
    public Animator animator;

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
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        if (groundCheck == null) groundCheck = transform.Find("groundCheck");
        if (ball_loc == null) ball_loc = transform.Find("tr0050_00.trmdl/origin/foot_base/waist/spine_01/spine_02/spine_03/right_shoulder/right_arm_width/right_arm_01/right_arm_02/right_hand/right_attach_on");
        if (FollowCamera == null) FollowCamera = GameObject.FindGameObjectWithTag("MainCamera");

        ball_prefab.TryGetComponent(out ball_rb);
        TryGetComponent(out controller);
        TryGetComponent(out animator);

    }


    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        animator.SetBool("isGround", isGrounded);
        animator.SetBool("Battle", isBattle);

        hasControl = (moveDirection != Vector3.zero);


        if (ismove)
        {
            // ȸ��
            Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
            Vector3 cameraForward = Vector3.Scale(FollowCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
            moveDirection = (input.x * FollowCamera.transform.right + input.y * cameraForward).normalized;

            if (hasControl && isGrounded && !isLookon && !isBattle)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

                //controller.Move(transform.forward * (currentSpeed * 0.01f) * Time.fixedDeltaTime);

                if (isWalking)
                {
                    animator.SetFloat("Velocity", 1);
                }
                else if (animator.GetFloat("Velocity") != 1)
                {
                    animator.SetFloat("Velocity", 2);
                }
            }
            else
            {
                if (animator.GetFloat("Velocity") != 1)
                {
                    animator.SetFloat("Velocity", 0);
                }
            }
        }
        else
        {
            animator.SetFloat("Velocity", 0);
        }

    }

    public void onWalk(InputAction.CallbackContext context)
    {
        //�ȱ� ��ư�� �� ��������
        if (context.performed && !isBattle)
        {
            isWalking = true;
            //animator.SetFloat("Velocity", 1);
        }
        //�ȱ� ��ư�� ������
        else if (context.canceled)
        {
            isWalking = false;
            animator.SetFloat("Velocity", 0);
        }
    }
    public bool IsGrounded()
    {
        Vector3 boxsize = transform.lossyScale;
        int groundLayerMask = LayerMask.GetMask("GroundLayer");
        bool isGrounded = Physics.CheckBox(groundCheck.position, boxsize, Quaternion.identity, groundLayerMask);

        return isGrounded;
    }

    public IEnumerator apply_motion_wait(float wait)
    {
        animator.applyRootMotion = false;
        yield return new WaitForSeconds(wait);
        animator.applyRootMotion = true;
        yield break;
    }

    public void BallOn_Event(int num)
    {
        if (num == 1)
        {
            show_Ball.SetActive(true);
        }
        else
        {
            show_Ball.SetActive(false);
        }
    }

    #region ���� �ð�ȭ

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 boxsize = transform.lossyScale;
        Gizmos.DrawWireCube(groundCheck.position, boxsize);

        //Gizmos.DrawSphere(transform.position + transform.forward * (distance - 0.5f), distance);
        //Gizmos.DrawCube(this.transform.position + transform.forward * 5f, new Vector3(10f ,8f, 10f));

        Vector3 position = transform.position + transform.forward * 4f;
        Quaternion rotation = transform.rotation;
        Vector3 size = new Vector3(1f, 6f, 8f);

        // ����� �ð�ȭ
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, size);
    }
    #endregion

    #region ��������

    //��ư �̺�Ʈ
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed && !isBattle)
        {
            StartCoroutine(StartThorw_co());
        }
    }

    //�� ������ ���
    IEnumerator StartThorw_co()
    {
        isLookon = true;
        animator.SetTrigger("Throw");

        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsTag("Move"));
        yield return null;
        isLookon = false;
    }

    //�� ���󰡴� �̺�Ʈ
    public void Bullthrow()
    {
        ball_prefab.SetActive(true);

        ball_rb.useGravity = true;
        ball_rb.velocity = Vector3.zero;
        ball_rb.angularVelocity = Vector3.zero;
        ball_prefab.transform.rotation = Quaternion.identity;
        ball_prefab.transform.position = ball_loc.position;

        Vector3 position = transform.position + transform.forward * 4f;
        Quaternion rotation = transform.rotation;
        Vector3 size = new Vector3(1f, 6f, 8f);

        Collider[] colls;
        int PokemonLayer = LayerMask.GetMask("Pokemon");
        colls = Physics.OverlapBox(position, size, rotation, PokemonLayer);

        //�����߿� ���ϸ��� �������� �� ��, ��� ���ϸ� ����
        if (BattleManager.instance.ball_throw && isBattle)
        {
            Debug.Log("���ϸ� ���� ������");
            ball_rb.useGravity = false;

            GameObject target = BattleManager.instance.enemyPokemon;

            Vector3 targetCenter = target.transform.position + target.transform.up * target.GetComponentInChildren<Renderer>().bounds.size.y * 0.8f;

            Vector3 forceDirection = (targetCenter - ball_loc.position).normalized;
            ball_rb.AddForce(forceDirection * ThrowPower, ForceMode.Impulse);
        }
        else if (isBattle)
        {
            ball_rb.useGravity = false;
            ball_rb.AddForce(transform.forward * ThrowPower / 3, ForceMode.Impulse);

            Invoke("DisableBallPrefab", 0.35f);
        }
        //���ϸ�� �����Ϸ��� �� ��, ���� ����� ���ϸ� ����
        else if (colls.Length > 0)
        {
            Debug.Log("���ϸ� ���� �ο���");
            Collider closestPokemon = null;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < colls.Length; i++)
            {
                Collider coll = colls[i];
                float distanceToCollider = Vector3.Distance(ball_loc.position, coll.transform.position);

                if (distanceToCollider < closestDistance)
                {
                    closestPokemon = coll;
                    closestDistance = distanceToCollider;
                }
            }

            if (closestPokemon != null)
            {
                ball_rb.useGravity = false;

                Vector3 targetCenter = closestPokemon.transform.position + closestPokemon.transform.up * closestPokemon.GetComponentInChildren<Renderer>().bounds.size.y * 0.5f;

                Vector3 forceDirection = (targetCenter - ball_loc.position).normalized;
                ball_rb.AddForce(forceDirection * ThrowPower, ForceMode.Impulse);


                Invoke("DisableBallPrefab", 1f);
            }
        }
        //������ �� ���� ��, �׳� �տ� ����
        else
        {
            ball_rb.AddForce(transform.forward * ThrowPower / 2, ForceMode.Impulse);

            Invoke("DisableBallPrefab", 1f);
        }

    }
    void DisableBallPrefab()
    {
        ball_prefab.SetActive(false);
    }

    #endregion
}
