using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance = null;

    private void Awake()
    {

        if (instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            instance = this; //내자신을 instance로 넣어줍니다.
            DontDestroyOnLoad(gameObject); //OnLoad(씬이 로드 되었을때) 자신을 파괴하지 않고 유지
        }
        else
        {
            if (instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }
    }

    //public PokemonData PlayerPokemon;
    //public PokemonData EnemyPokemon;

    [Header("랭크")]
    //타입랭크
    public float DamageRank = 1;
    //자속랭크
    public float PropertyRank = 1;
    //스테이터스 랭크
    public float AttackerAttackRank = 1;
    public float AttackerSpAttackRank = 1;
    public float AttackerDefenceRank = 1;
    public float AttackerSpDefenceRank = 1;
    public float AttackerSpeedRank = 1;
    public float AttackerHitrateRank = 1;

    public float TargetAttackRank = 1;
    public float TargetSpAttackRank = 1;
    public float TargetDefenceRank = 1;
    public float TargetSpDefenceRank = 1;
    public float TargetSpeedRank = 1;
    public float TargetHitrateRank = 1;

    //최종 데미지
    [SerializeField] private float Damage = 0;

    [Header("적 야생 포켓몬과의 거리")]
    [SerializeField] private float distance_wild = 1f;
    [Header("적 플레이어와 서로의 거리")]
    [SerializeField] private float distance_battle = 20f;
    private GameObject betweenObject;

    [Header("카메라")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineFreeLook PlayerCamera;

    [Header("포켓몬")]
    public GameObject playerPokemon;
    [SerializeField] private int playerskillnum;
    public GameObject[] enemyPokemon_List;
    public GameObject enemyPokemon;
    [SerializeField] private int enemyskillnum;

    [Header("플레이어")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemyplayer;
    [Header("몬스터볼")]
    [SerializeField] private GameObject ball;

    private PlayerBag playerbag;
    [SerializeField] private UIManger uIManger;

    [Header("플레이어 UI")]
    [SerializeField] private GameObject Battle_UI;

    [Header("배틀 턴")]
    [SerializeField] private int turn;

    public bool islose;
    public bool isWin;
    public bool isRun;

    public bool player_using_Item = false;
    public bool enemy_using_Item = false;
    public bool player_pokemon_change = false;
    public bool enemy_pokemon_change = false;
    public bool ball_throw = false;
    public bool iscatch = false;

    public UnityEvent Battle_Ready;

    Transform newTransform;
    //배틀 시작!

    private void Start()
    {
        uIManger = Battle_UI.transform.parent.GetComponent<UIManger>();
    }
    public void Battle_Start(GameObject[] enemyPokemon_List, GameObject player, GameObject enemyplayer)
    {
        Debug.Log("배틀시작!");

        this.enemyPokemon_List = new GameObject[enemyPokemon_List.Length];
        this.enemyPokemon_List = enemyPokemon_List;

        this.enemyPokemon = enemyPokemon_List[0];
        this.player = player;
        Battle_Ready.Invoke();

        this.enemyplayer = enemyplayer;

        //포켓몬 생성 하고 playerPokemon을 채워줌
        if (enemyPokemon.GetComponent<PokemonBattleMode>().isWild || enemyplayer == null)
        {
            //포켓몬 위치 조정
            Wild_pokemonMove();

            //플레이어 위치
            Wild_playerMove(out Vector3 offset);

            //카메라 연출 시작(아직 야생만)
            StartCoroutine(Wild_BattleCamera_co(offset));
        }
        else
        {
            this.enemyplayer = enemyplayer;

            Trainer_Battle_playerMove(out Vector3 playerpos);

            Trainer_Battle_pokemonMove(playerpos);


            StartCoroutine(Trainer_BattleCamera_co());
        }

        //끝나면
        StartCoroutine(Battle());
    }

    #region 플레이어 배틀 연출
    void Trainer_Battle_playerMove(out Vector3 playerpos)
    {
        // 플레이어를 적으로부터 20f만큼 앞으로 이동시킵니다.
        Vector3 enemyplayer_loc = enemyplayer.transform.position;
        playerpos = enemyplayer_loc + enemyplayer.transform.forward * distance_battle;
        player.transform.position = playerpos;

        //회전
        Vector3 enemy_look = player.transform.position - enemyplayer.transform.position;
        player.transform.rotation = Quaternion.LookRotation(-enemy_look);
    }

    void Trainer_Battle_pokemonMove(Vector3 playerpos)
    {
        Vector3 playerpokemonpos = playerpos + player.transform.forward * 6.5f;
        //playerpokemonpos.z = playerpokemonpos.z - (playerPokemon.GetComponentInChildren<Renderer>().bounds.size.z / 2);
        Vector3 offset = player.transform.forward * -(playerPokemon.GetComponentInChildren<Renderer>().bounds.size.z / 2);
        playerpokemonpos += offset;


        playerPokemon.transform.position = playerpokemonpos;
        playerPokemon.transform.rotation = player.transform.rotation;

        playerPokemon.SetActive(false);

        if (betweenObject == null)
        {
            betweenObject = new GameObject("Between");
        }

        Vector3 objectPosition = (enemyplayer.transform.position + player.transform.position) / 2f;
        objectPosition.y = player.transform.position.y + 4f;

        betweenObject.transform.rotation = Quaternion.identity;
        betweenObject.transform.position = objectPosition;
    }
    IEnumerator Trainer_BattleCamera_co()
    {
        //카메라 이동
        virtualCamera.Priority = 15;

        //BGM변경
        SoundManager.instance.PlayBGM("Trainer_Battle");

        //적 포켓몬의 중심을 찾고, 크기에 따라 카메라 이동
        EnemyLookatCamera(enemyplayer, 0.7f);

        enemyplayer.GetComponent<Animator>().SetTrigger("Throw");
        //볼 던질떄까지
        yield return new WaitUntil(() => enemyplayer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("tr0040_00_trmdl|tr0040_00_01320_ballthrow01_gfbanm")
                                      && enemyplayer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f);

        //포켓몬 켜지고 카메라 이동
        enemyPokemon.SetActive(true);
        SoundManager.instance.PlayEffect("Ball_Out");

        Renderer enemypokmonRender = enemyPokemon.GetComponentInChildren<Renderer>();
        pokemonTarget(enemyPokemon.transform, enemypokmonRender, false, false, true);
        yield return StartCoroutine(enemyPokemon.GetComponent<PokemonBattleMode>().StartAnim_co());
        //포켓몬 울음이 끝나면

        //플레이어에게 카메라 이동하고 볼 던지기
        virtualCamera.LookAt = player.transform;
        player.GetComponent<Animator>().SetTrigger("Turn");

        yield return YieldInstructionCache.WaitForSeconds(1.6f);

        //플레이어 포켓몬에게 카메라 이동
        playerPokemon.SetActive(true);

        Renderer playerpokemonRender = playerPokemon.GetComponentInChildren<Renderer>();
        pokemonTarget(playerPokemon.transform, playerpokemonRender, false, false, true);

        SoundManager.instance.PlayEffect("Ball_Out");
        yield return StartCoroutine(playerPokemon.GetComponent<PokemonBattleMode>().StartAnim_co());

        //다시 원래 카메라로
        virtualCamera.Priority = 0;

        //카메라 타겟을 가운데쯤으로
        CameraMove();

        //카메라가 돌아온 다음, UI생성
        yield return YieldInstructionCache.WaitForSeconds(1.5f);

        //UI 생성
        uIManger.enemypokemon = enemyPokemon.GetComponent<PokemonStats>();
        Battle_UI.SetActive(true);
        Battle_UI.transform.Find("0.enemyPokemon").gameObject.SetActive(true);
    }

    #endregion

    #region 야생 포켓몬 연출
    void Wild_pokemonMove()
    {
        Vector3 pokemonpos = enemyPokemon.transform.position + enemyPokemon.transform.forward * distance_wild;
        Vector3 offset = enemyPokemon.transform.forward
                      * (playerPokemon.GetComponentInChildren<Renderer>().bounds.size.z / 2
                      + (enemyPokemon.GetComponentInChildren<Renderer>().bounds.size.z / 2));

        pokemonpos += offset;

        playerPokemon.transform.position = pokemonpos;

        Vector3 enemy_look = playerPokemon.transform.position - enemyPokemon.transform.position;
        playerPokemon.transform.rotation = Quaternion.LookRotation(-enemy_look);

        if (betweenObject == null)
        {
            betweenObject = new GameObject("Between");
        }

        float distance = Vector3.Distance(playerPokemon.transform.position, enemyPokemon.transform.position);

        // 거리를 출력합니다.
        Debug.Log("두 개체 사이의 거리: " + distance);


        Vector3 objectPosition = (enemyPokemon.transform.position + playerPokemon.transform.position) / 2f;
        objectPosition.y = enemyPokemon.transform.position.y + enemyPokemon.GetComponentInChildren<Renderer>().bounds.size.y;

        betweenObject.transform.rotation = Quaternion.identity;
        betweenObject.transform.position = objectPosition;

    }

    void Wild_playerMove(out Vector3 offset)
    {
        // 플레이어 위치
        Quaternion rotation = Quaternion.Euler(0f, 25f, 0f);
        float playerorigin_Y = player.transform.position.y;
        offset = rotation * -playerPokemon.transform.forward;


        Vector3 playerPos = player.transform.position + offset * 4f;
        //playerPos.y = playerorigin_Y;
        Debug.Log("플레이어 원래 y값" + playerorigin_Y + "이동한 y값 : " + playerPos);
        player.transform.position = playerPos;
        Debug.Log("이동한 y값 : " + playerPos);
        Vector3 loc = player.transform.position - enemyPokemon.transform.position;
        Quaternion playerLook = Quaternion.LookRotation(-loc);
        playerLook.x = 0;
        playerLook.z = 0;
        player.transform.rotation = playerLook;
    }

    //카메라 연출(아직 야생만)
    IEnumerator Wild_BattleCamera_co(Vector3 offset)
    {
        //사운드 출력
        SoundManager.instance.PlayBGM("Wild_Battle");

        //카메라 이동
        virtualCamera.Priority = 15;

        //적 포켓몬의 중심을 찾고, 크기에 따라 카메라 이동
        EnemyLookatCamera(enemyPokemon, 0.65f);

        //몬스터가 소리 지를때까지 
        yield return new WaitUntil(() => enemyPokemon.GetComponent<PokemonBattleMode>().anim.GetCurrentAnimatorStateInfo(0).IsName("roar01")
                                      && enemyPokemon.GetComponent<PokemonBattleMode>().anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f);


        //다시 원래 카메라로
        virtualCamera.Priority = 0;

        //카메라 타겟을 가운데쯤으로
        CameraMove();

        //카메라가 돌아온 다음, UI생성
        yield return YieldInstructionCache.WaitForSeconds(1.5f);

        //UI 생성
        Battle_UI.SetActive(true);
    }


    #endregion

    //타겟의 높이를 찾아 카메라 조준하기
    void EnemyLookatCamera(GameObject target, float look_height)
    {

        Renderer target_renderer = target.GetComponentInChildren<Renderer>();
        Vector3 targetCenter = target.transform.position + target.transform.up * target_renderer.bounds.size.y * look_height;

        if (target == ball)
        {
            virtualCamera.LookAt = target.transform;

            Vector3 targetPosition = ball.transform.position + ball.transform.forward * 1f;
            targetPosition.y = enemyPokemon.transform.position.y;
            virtualCamera.transform.position = targetPosition;

        }
        else if (target != ball)
        {
            virtualCamera.LookAt = null;
            virtualCamera.transform.position = betweenObject.transform.position;
        }

        virtualCamera.transform.rotation = Quaternion.LookRotation(targetCenter - virtualCamera.transform.position);
    }

    void pokemonTarget(Transform Transform, Renderer py_pokemon_renderer, bool playerCamera, bool follow, bool lookat)
    {
        Vector3 position = Transform.position + Transform.up * py_pokemon_renderer.bounds.size.y * 0.5f;
        Quaternion py_rotation = Transform.rotation;
        Vector3 scale = Transform.localScale;

        if (newTransform == null)
        {
            newTransform = new GameObject().transform;
        }
        else
        {
            newTransform.gameObject.SetActive(true);
        }

        newTransform.position = position;
        newTransform.rotation = py_rotation;
        newTransform.localScale = scale;

        if (playerCamera)
        {
            //PlayerPokemon을 쳐다보도록
            if (follow)
            {
                PlayerCamera.Follow = newTransform;
            }
            if (lookat)
            {
                PlayerCamera.LookAt = newTransform;
            }
        }
        else
        {
            if (follow)
            {
                virtualCamera.Follow = newTransform;
            }
            if (lookat)
            {
                virtualCamera.LookAt = newTransform;
            }
        }
    }

    void CameraMove()
    {
        Transform playerTransform = playerPokemon.transform;
        Renderer py_pokemon_renderer = playerPokemon.GetComponentInChildren<Renderer>();

        pokemonTarget(playerTransform, py_pokemon_renderer, true, true, true);

        if (newTransform == null)
        {
            newTransform = new GameObject().transform;
        }
        else
        {
            newTransform.gameObject.SetActive(true);
        }


        if (py_pokemon_renderer.bounds.size.y >= 2)
        {
            PlayerCamera.m_Orbits[1].m_Radius = py_pokemon_renderer.bounds.size.z * 1.65f;
        }
        else if (py_pokemon_renderer.bounds.size.y < 2)
        {
            PlayerCamera.m_Orbits[1].m_Radius = 4.5f;
        }



        //보고싶은 카메라의 각도 = 포켓몬 위치에서 10도
        float playerPokemonAngle = Quaternion.LookRotation(playerPokemon.transform.forward, Vector3.up).eulerAngles.y;
        float cameraXAngle_local = 25f;

        //각도 계산
        float cameraXAngle = Mathf.Repeat(playerPokemonAngle - cameraXAngle_local + 180f, 360f) - 180f;

        //플레이어 카메라의 값
        PlayerCamera.m_XAxis.Value = cameraXAngle;
        PlayerCamera.m_YAxis.Value = 0.45f;
    }


    //배틀 시작
    IEnumerator Battle()
    {
        while (true)
        {
            //=========================================================================배틀====================================================================================

            //초기화
            islose = false;
            isWin = false;
            isRun = false;

            iscatch = false;
            turn = 0;

            playerbag = player.GetComponent<PlayerBag>();
            Animator player_anim = player.GetComponent<PlayerMovement>().animator;

            PokemonStats enemy_pokemon_Stats = enemyPokemon.GetComponent<PokemonStats>();
            PokemonStats player_pokemon_Stats = playerPokemon.GetComponent<PokemonStats>();
            PokemonBattleMode enemy_pokemon_battlemode = enemyPokemon.GetComponent<PokemonBattleMode>();
            PokemonBattleMode player_pokemon_battlemode = enemyPokemon.GetComponent<PokemonBattleMode>();
            Slider player_pokemon_slider = uIManger.hPbar;
            Slider enemy_pokemon_slider;
            PokemonStats first_attack_pokemon;
            PokemonStats next_attacker_pokemon;
            Slider first_attack_pokemon_slider;
            Slider next_attacker_pokemon_slider;

            if (enemyplayer != null)
            {
                Animator enemyplayer_anim = enemyplayer.GetComponent<Animator>();
                enemy_pokemon_slider = uIManger.enemy_hPbar;
            }
            else
            {
                enemy_pokemon_slider = enemy_pokemon_battlemode.pokemon_slider;
            }

            //=====================================================================포켓몬 등장=================================================================================

            while (true)
            {
                //============================================================== UI에서 행동선택 ==============================================================================

                playerskillnum = -1;
                enemyskillnum = Random.Range(0, 4);

                yield return null;
                yield return new WaitUntil(() =>
                playerskillnum != -1
                || isRun
                || player_pokemon_change
                || enemy_pokemon_change
                || player_using_Item
                || enemy_using_Item
                || ball_throw);
                yield return null;

                //도망가기
                if (isRun)
                {
                    break;
                }
                //==================================================================스피드 비교================================================================================

                CompareSpeed(enemy_pokemon_Stats, player_pokemon_Stats, enemy_pokemon_slider, player_pokemon_slider, player_using_Item, enemy_using_Item, player_pokemon_change, enemy_pokemon_change,
                         out first_attack_pokemon, out next_attacker_pokemon, out first_attack_pokemon_slider, out next_attacker_pokemon_slider);


                //================================================================선공 포켓몬 턴===============================================================================

                //몬스터볼 투척
                if (ball_throw)
                {
                    //몬스터볼 던지는 과정

                    virtualCamera.Priority = 15;
                    virtualCamera.transform.position = betweenObject.transform.position;
                    virtualCamera.LookAt = player.transform;

                    playerPokemon.SetActive(false);
                    player_anim.SetTrigger("Throw");
                    yield return new WaitUntil(() => player_anim.GetCurrentAnimatorStateInfo(0).IsName("tr0050_00_trmdl_throw02_gfbanm") && player_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f);
                    playerPokemon.SetActive(true);
                    //---------------------------------------------
                    //virtualCamera.Priority = 15;
                    //몬스터 볼로 카메라 이동
                    EnemyLookatCamera(ball, 0.5f);

                    Rigidbody ball_rb = ball.GetComponent<Rigidbody>();
                    while (ball_rb.velocity != Vector3.zero)
                    {
                        yield return null;
                    }
                    ball_rb.freezeRotation = true;
                    yield return new WaitUntil(() => ball_rb.velocity == Vector3.zero);

                    ball_rb.velocity = Vector3.zero;
                    ball_rb.angularVelocity = Vector3.zero;
                    Quaternion ball_rotation = ball.transform.rotation;
                    ball_rotation.z = 0;
                    ball_rotation.x = 0;
                    ball.transform.rotation = ball_rotation;
                    //---------------볼 정지 후 계산-------------------------------

                    yield return null;
                    yield return StartCoroutine(Catch_count_co());

                    ball_throw = false;

                    //잡으면
                    if (iscatch)
                    {
                        if (ball_throw!)
                        {
                            ball_throw = false;
                        }
                        virtualCamera.Priority = 0;
                        break;
                    }//놓치면
                    else
                    {
                        ball.SetActive(false);
                        enemyPokemon.SetActive(true);
                        SoundManager.instance.PlayEffect("Ball_Out");
                        EnemyLookatCamera(enemyPokemon, 0.65f);
                        yield return null;
                        yield return new WaitUntil(() => enemyPokemon.GetComponent<PokemonBattleMode>().anim.GetCurrentAnimatorStateInfo(0).IsName("roar01")
                                                      && enemyPokemon.GetComponent<PokemonBattleMode>().anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
                        yield return null;

                        //다시 원래 카메라로
                        virtualCamera.Priority = 0;
                        SoundManager.instance.PlayBGM("Wild_Battle");
                    }

                }

                //포켓몬 체인지
                else if (player_pokemon_change || enemy_pokemon_change)
                {
                    #region 체인지
                    player_anim.SetTrigger("Back");

                    TextBox.instance.Textbox_OnOff(true);
                    TextBox.instance.TalkText.text = $"{playerPokemon.GetComponent<PokemonStats>().Name} 수고했어!";

                    yield return new WaitUntil(() => player_anim.GetCurrentAnimatorStateInfo(0).IsName("tr0050_00_trmdl|tr0050_00_01332_ballreturn01_end_gfbanm"));
                    playerPokemon.SetActive(false);
                    TextBox.instance.Textbox_OnOff(false);

                    Vector3 pokemon_loc = playerPokemon.transform.position;
                    Quaternion pokemon_rot = playerPokemon.transform.rotation;
                    playerPokemon = playerbag.PlayerPokemon[uIManger.beforeIndex];


                    yield return new WaitUntil(() => player_anim.GetCurrentAnimatorStateInfo(0).IsName("tr0050_00_trmdl|tr0050_00_01002_battlewait01_loop_gfbanm"));
                    TextBox.instance.Textbox_OnOff(true);
                    TextBox.instance.TalkText.text = $"{playerPokemon.GetComponent<PokemonStats>().Name} 가라!";
                    player_anim.SetTrigger("Turn");

                    yield return new WaitUntil(() =>
                    player_anim.GetCurrentAnimatorStateInfo(0).IsName("tr0050_00_trmdl|tr0050_00_01320_ballthrow01_gfbanm")
                    && player_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

                    playerPokemon.transform.position = pokemon_loc;
                    playerPokemon.transform.rotation = pokemon_rot;
                    playerPokemon.SetActive(true);
                    SoundManager.instance.PlayEffect("Ball_Out");

                    yield return StartCoroutine(playerPokemon.GetComponent<PokemonBattleMode>().StartAnim_co());

                    first_attack_pokemon = playerPokemon.GetComponent<PokemonStats>();
                    player_pokemon_Stats = playerPokemon.GetComponent<PokemonStats>();

                    TextBox.instance.Textbox_OnOff(false);
                    #endregion

                    if (enemyplayer != null)
                    {
                        //플레이어 위치
                        Wild_playerMove(out Vector3 offset);
                    }
                    else
                    {
                        Trainer_Battle_playerMove(out Vector3 playerpos);
                    }

                    //카메라 조정
                    CameraMove();
                }
                //아이템 사용
                else if (player_using_Item || enemy_using_Item)
                {
                    Debug.Log("아이템 사용!");
                }
                //공격
                else
                {
                    yield return YieldInstructionCache.WaitForSeconds(1f);

                    //공격 페이즈
                    AttackPhase(first_attack_pokemon, next_attacker_pokemon);

                    //공격 모션 중간쯤에 피격 맞음
                    yield return new WaitUntil(() =>
                    first_attack_pokemon.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                    first_attack_pokemon.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);
                    yield return null;

                    //피격맞고 체력바 떨어짐
                    HitPhase(next_attacker_pokemon, next_attacker_pokemon_slider);
                }

                //사망시 아웃
                if (islose || isWin || isRun || iscatch || iscatch || next_attacker_pokemon.isDie)
                {
                    yield return YieldInstructionCache.WaitForSeconds(2f);
                    break;
                }
                //=============================================================================================================================================================


                yield return new WaitUntil(() => next_attacker_pokemon.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2);
                yield return YieldInstructionCache.WaitForSeconds(4f);
                yield return null;


                //================================================================후공 포켓몬 턴===============================================================================

                //다음 포켓몬 공격
                AttackPhase(next_attacker_pokemon, first_attack_pokemon);

                //공격 모션 중간쯤에 피격 맞음
                yield return null;
                yield return new WaitUntil(() =>
                next_attacker_pokemon.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                next_attacker_pokemon.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);
                yield return null;

                //피격맞고 체력바 떨어짐
                HitPhase(first_attack_pokemon, first_attack_pokemon_slider);

                //조건 검사
                if (islose || isWin || isRun || iscatch || first_attack_pokemon.isDie)
                {
                    yield return YieldInstructionCache.WaitForSeconds(2f);
                    break;
                }

                yield return YieldInstructionCache.WaitForSeconds(2f);
                //=================================================================== 턴 종료 =================================================================================

                //UI 리셋!
                #region UI 리셋
                if (!Battle_UI.transform.Find("0.Default").gameObject.activeSelf)
                {
                    uIManger.Reset_BattleUI();
                    uIManger.currentIndex = 0;
                    Battle_UI.transform.Find("Select").gameObject.SetActive(true);
                    Battle_UI.transform.Find("0.Default").gameObject.SetActive(true);

                }
                else if (!Battle_UI.activeSelf)
                {
                    uIManger.currentIndex = 0;
                    Battle_UI.SetActive(true);
                }
                #endregion

                //리셋
                #region 조건문 리셋

                while (true)
                {
                    player_using_Item = false;
                    enemy_using_Item = false;
                    player_pokemon_change = false;
                    enemy_pokemon_change = false;
                    ball_throw = false;
                    iscatch = false;
                    uIManger.isBattle = false;

                    if (!player_using_Item && !enemy_using_Item && !player_pokemon_change && !enemy_pokemon_change && !ball_throw && !iscatch)
                    {
                        break;
                    }
                    yield return null;
                }


                #endregion

                turn++;
                Debug.Log("턴 끝남");
            }//첫번째 while문 나감 턴 종료 끝

            if (iscatch)
            {
                playerbag.AddPokemon(enemyPokemon);
                SoundManager.instance.PlayEffect("Ball_Gotcha");
                yield return YieldInstructionCache.WaitForSeconds(0.5f);
                SoundManager.instance.PlayBGM("PokeCaught");
                yield return YieldInstructionCache.WaitForSeconds(2f);
                ball.SetActive(false);


                TextBox.instance.Textbox_OnOff(true);
                TextBox.instance.TalkText.text = enemy_pokemon_Stats.Name + "을 잡았습니다";
                yield return YieldInstructionCache.WaitForSeconds(1f);
                TextBox.instance.Textbox_OnOff(false);
                break;
            }
            else if (isRun)
            {
                TextBox.instance.Textbox_OnOff(true);
                TextBox.instance.TalkText.text = "무사히 도망갔습니다.";
                yield return YieldInstructionCache.WaitForSeconds(1f);
                TextBox.instance.Textbox_OnOff(false);

                enemy_pokemon_battlemode.enabled = false;
                enemy_pokemon_battlemode.anim.SetBool("Battle", false);
                enemy_pokemon_battlemode.anim.SetBool("Walk", true);

                enemyPokemon.GetComponent<PokemonMove>().enabled = true;
                break;
            }
            else if (player_pokemon_Stats.isDie)
            {
                bool allPokemonsDead = true; // 모든 포켓몬이 사망한 상태인지 여부를 나타내는 변수

                // playerbag.PlayerPokemon의 포켓몬들을 검사
                for (int i = 0; i < playerbag.PlayerPokemon.Count; i++)
                {
                    // 생존한 포켓몬 없음
                    if (playerbag.PlayerPokemon[i] == null)
                    {
                        allPokemonsDead = false;
                        break;
                    }
                    else
                    {
                        PokemonStats pokemonStats = playerbag.PlayerPokemon[i].GetComponent<PokemonStats>();

                        // 예외처리 생존한 포켓몬 없음
                        if (pokemonStats.Hp > 0)
                        {
                            allPokemonsDead = false;
                            break;
                        }
                    }
                }

                if (!allPokemonsDead)
                {
                    Debug.Log("My Pokemon digim...");

                    // 추가적인 처리를 수행
                    uIManger.UI_Change();
                    uIManger.beforeIndex = 0;
                }
                else
                {

                    break;
                }
            }
            else if (enemy_pokemon_Stats.isDie)
            {
                player_pokemon_Stats.Exp += enemy_pokemon_Stats.Level * 50;
                player_pokemon_Stats.CheckLevelUp();

                if (enemyplayer != null)
                {
                    enemyplayer.GetComponent<Animator>().SetTrigger("Back");

                    bool allPokemonsDead = true; // 모든 포켓몬이 사망한 상태인지 여부를 나타내는 변수

                    // playerbag.PlayerPokemon의 포켓몬들을 검사
                    for (int i = 0; i < enemyPokemon_List.Length; i++)
                    {
                        PokemonStats pokemonStats = enemyPokemon_List[i].GetComponent<PokemonStats>();

                        // 예외처리 생존한 포켓몬 없음
                        if (!pokemonStats.isDie)
                        {
                            allPokemonsDead = false;
                            enemyPokemon = enemyPokemon_List[i];
                            break;
                        }

                    }

                    if (!allPokemonsDead)
                    {
                        //적 포켓몬의 중심을 찾고, 크기에 따라 카메라 이동
                        EnemyLookatCamera(enemyplayer, 0.7f);

                        enemyplayer.GetComponent<Animator>().SetTrigger("Throw");
                        //볼 던질떄까지
                        yield return new WaitUntil(() => enemyplayer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("tr0040_00_trmdl|tr0040_00_01320_ballthrow01_gfbanm")
                                                      && enemyplayer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f);

                        //포켓몬 켜지고 카메라 이동
                        enemyPokemon.SetActive(true);
                        Renderer enemypokmonRender = enemyPokemon.GetComponentInChildren<Renderer>();
                        pokemonTarget(enemyPokemon.transform, enemypokmonRender, false, false, true);
                        yield return StartCoroutine(enemyPokemon.GetComponent<PokemonBattleMode>().StartAnim_co());

                        //다시 원래 카메라로
                        virtualCamera.Priority = 0;

                        //카메라 타겟을 가운데쯤으로
                        CameraMove();

                        //UI 생성
                        uIManger.enemypokemon = enemyPokemon.GetComponent<PokemonStats>();
                        Battle_UI.SetActive(true);
                        Battle_UI.transform.Find("0.enemyPokemon").gameObject.SetActive(true);
                        Battle_UI.transform.Find("0.Default").gameObject.SetActive(true);
                    }
                    else
                    {
                        enemyplayer.GetComponent<Animator>().SetTrigger("Lose");
                        playerbag.playermoney += enemyplayer.GetComponent<Friend>().getmoney;

                        //텍스트 출력
                        TextBox.instance.NPC_Textbox_OnOff(true);
                        List<string> Lose = new List<string>();
                        string Losetxt = "승리했습니다!!!";
                        string Losetxt_2 = $"{enemyplayer.GetComponent<Friend>().getmoney}원을 얻었습니다.";

                        Lose.Add(Losetxt);
                        Lose.Add(Losetxt_2);

                        yield return StartCoroutine(TextBox.instance.TypeText(Lose));
                        TextBox.instance.NPC_Textbox_OnOff(false);
                        //텍스트 출력 끝
                        break;
                    }
                }
                else
                {
                    yield return new WaitUntil(() => enemy_pokemon_battlemode.anim.GetCurrentAnimatorStateInfo(0).IsName("down01_loop_gfbanm"));

                    Debug.Log("상대 포켓몬 디짐..");
                    break;
                }
            }

        }//두번째 while문 나옴 전투 끝
        for (int i = 0; i < playerbag.PlayerPokemon.Count; i++)
        {
            if (playerbag.PlayerPokemon[i] != null)
            {
                PokemonStats mypokemon = playerbag.PlayerPokemon[i].GetComponent<PokemonStats>();
                mypokemon.AttackRank = 0;
                mypokemon.DefenceRank = 0;
                mypokemon.SpAttackRank = 0;
                mypokemon.SpDefenceRank = 0;
                mypokemon.SpeedRank = 0;
                mypokemon.HitrateRank = 0;

            }
        }

        yield return null;

        uIManger.isBattle = false;
        Battle_UI.SetActive(false);

        player.GetComponent<PlayerMovement>().isBattle = false;
        player.GetComponent<PlayerMovement>().ismove = true;
        playerPokemon.SetActive(false);

        newTransform.gameObject.SetActive(false);

        PlayerCamera.m_Orbits[1].m_Radius = 5f;
        PlayerCamera.Follow = player.transform;
        PlayerCamera.LookAt = player.transform;

        SoundManager.instance.PlayBGM("City");
    }



    #region 잡는거

    //잡는 확률
    IEnumerator Catch_count_co()
    {
        //포획률
        float catchRate_1 = 0.9f;
        float catchRate_2 = 0.8f;

        // 회전 애니메이션 시작
        yield return StartCoroutine(SwingAnimation(40f, -40f, 0.4f));
        yield return YieldInstructionCache.WaitForSeconds(0.5f);

        if (Random.value <= catchRate_1)
        {
            yield return StartCoroutine(SwingAnimation(40f, -40f, 0.45f));
            yield return YieldInstructionCache.WaitForSeconds(0.5f);

            if (Random.value <= catchRate_2)
            {
                yield return StartCoroutine(SwingAnimation(40f, -40f, 0.45f));
                yield return YieldInstructionCache.WaitForSeconds(0.5f);

                iscatch = true;
            }
        }
    }


    IEnumerator SwingAnimation(float fromAngle, float toAngle, float duration)
    {
        Transform target = ball.GetComponentInChildren<Transform>();

        Quaternion startRotation = target.localRotation; // 시작 회전값 저장
        Vector3 startPosition = target.localPosition;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // interpolate rotation angle over time
            float t = elapsedTime / duration;
            float angle = Mathf.Lerp(fromAngle, toAngle, t);

            // update current rotation value
            Quaternion currentRotation = Quaternion.Euler(target.localRotation.eulerAngles.x, target.localRotation.eulerAngles.y, angle);
            target.localRotation = currentRotation;

            // interpolate position left and right over time
            float xPosition = Mathf.Lerp(0f, -0.03f, t);
            Vector3 currentPosition = startPosition + new Vector3(xPosition, 0f, 0f);
            target.localPosition = currentPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        SoundManager.instance.PlaySFX("BallShake");

        // 애니메이션 완료 후 원래 위치로 돌아오는 애니메이션 추가
        float returnDuration = duration / 3f; // 돌아오는 애니메이션의 시간
        float returnElapsedTime = 0f;

        Vector3 returnStartPosition = target.localPosition;
        Quaternion returnStartRotation = target.localRotation;

        while (returnElapsedTime < returnDuration)
        {
            float t = returnElapsedTime / returnDuration;
            Vector3 returnPosition = Vector3.Lerp(returnStartPosition, startPosition, t);
            Quaternion returnRotation = Quaternion.Lerp(returnStartRotation, startRotation, t);

            target.localPosition = returnPosition;
            target.localRotation = returnRotation;

            returnElapsedTime += Time.deltaTime;
            yield return null;
        }

        // 애니메이션 완료 후 원래 회전값으로 복원
        target.localRotation = startRotation;
    }

    #endregion

    //스피드 비교
    void CompareSpeed(PokemonStats Enemy_pokemon, PokemonStats Player_pokemon, Slider Enemy_slider, Slider player_slider,
                  bool player_using_Item, bool enemy_using_Item, bool player_pokemon_change, bool enemy_pokemon_change,
              out PokemonStats firstattcker, out PokemonStats nextattacker, out Slider first_attack_pokemon_slider, out Slider next_tattacker_pokemon_slider)
    {
        #region 속도 체크
        firstattcker = null;
        nextattacker = null;
        first_attack_pokemon_slider = null;
        next_tattacker_pokemon_slider = null;


        if (player_pokemon_change || player_using_Item || ball_throw)
        {
            firstattcker = Player_pokemon;
            first_attack_pokemon_slider = player_slider;

            nextattacker = Enemy_pokemon;
            next_tattacker_pokemon_slider = Enemy_slider;
        }
        else if (enemy_pokemon_change || enemy_using_Item)
        {
            firstattcker = Enemy_pokemon;
            first_attack_pokemon_slider = Enemy_slider;

            nextattacker = Player_pokemon;
            next_tattacker_pokemon_slider = player_slider;
        }
        else
        {
            //내 스킬의 우선도가 더 높을때
            if (Player_pokemon.skills[playerskillnum].Priority > Enemy_pokemon.skills[enemyskillnum].Priority)
            {
                firstattcker = Player_pokemon;
                first_attack_pokemon_slider = player_slider;

                nextattacker = Enemy_pokemon;
                next_tattacker_pokemon_slider = Enemy_slider;
            }
            //적의 스킬의 우선도가 더 높을때
            else if (Player_pokemon.skills[playerskillnum].Priority < Enemy_pokemon.skills[enemyskillnum].Priority)
            {
                firstattcker = Enemy_pokemon;
                first_attack_pokemon_slider = Enemy_slider;

                nextattacker = Player_pokemon;
                next_tattacker_pokemon_slider = player_slider;
            }
            //내 포켓몬의 속도가 더 높을때
            else if (Player_pokemon.Speed > Enemy_pokemon.Speed)
            {
                firstattcker = Player_pokemon;
                first_attack_pokemon_slider = player_slider;

                nextattacker = Enemy_pokemon;
                next_tattacker_pokemon_slider = Enemy_slider;
            }
            //적의 스피드가 더 높을 때
            else if (Player_pokemon.Speed < Enemy_pokemon.Speed)
            {
                firstattcker = Enemy_pokemon;
                first_attack_pokemon_slider = Enemy_slider;

                nextattacker = Player_pokemon;
                next_tattacker_pokemon_slider = player_slider;
            }
            //스피드가 똑같을때는 랜덤으로 결정
            else if (Enemy_pokemon.Speed == Player_pokemon.Speed)
            {
                int randomStart = Random.Range(1, 3);

                //나 먼저
                if (randomStart == 1)
                {
                    firstattcker = Player_pokemon;
                    first_attack_pokemon_slider = player_slider;

                    nextattacker = Enemy_pokemon;
                    next_tattacker_pokemon_slider = Enemy_slider;
                }
                //적 먼저
                else if (randomStart == 2)
                {
                    firstattcker = Enemy_pokemon;
                    first_attack_pokemon_slider = Enemy_slider;

                    nextattacker = Player_pokemon;
                    next_tattacker_pokemon_slider = player_slider;
                }
            }
        }
        #endregion
    }


    //공격 페이즈
    void AttackPhase(PokemonStats attacker, PokemonStats target)
    {
        int skillnum, skilltype;

        //스킬 판별
        CompareSkillType(attacker, out skillnum, out skilltype);

        //데미지 계산
        OnDamage(attacker.skills[skillnum], attacker, target);

        //4번째 지속 공격은 이상함
        attacker.GetComponent<PokemonBattleMode>().anim.SetFloat("AttackNum", skilltype);
        attacker.GetComponent<PokemonBattleMode>().anim.SetTrigger("Attack");
        TextBox.instance.Textbox_OnOff(true);
        TextBox.instance.TalkText.text = $" {attacker.Name}의 {attacker.skills[skillnum].Name}!!";
    }

    void setoff()
    {
        TextBox.instance.TalkText.text = "";
        TextBox.instance.Textbox_OnOff(false);
    }



    //맞는 판정
    void HitPhase(PokemonStats Target, Slider Target_Slider)
    {
        if (Target == playerPokemon.GetComponent<PokemonStats>())
        {
            if (!Battle_UI.activeSelf)
            {
                Battle_UI.SetActive(true);
            }
            Battle_UI.transform.Find("0.CurrentPokemon_Hp").gameObject.SetActive(true);
            Battle_UI.transform.Find("0.Default").gameObject.SetActive(false);
            Battle_UI.transform.Find("Select").gameObject.SetActive(false);
        }

        Target.Hp -= (int)Damage;
        Debug.Log($"{Target.Name}에게 {(int)Damage}만큼의 데미지를 주었다!");

        if (DamageRank > 1)
        {
            Target.gameObject.GetComponent<Animator>().SetTrigger("SuperHit");
            SoundManager.instance.PlayEffect("SuperHit");
            TextBox.instance.TalkText.text = "효과가 굉장했다!";
        }
        else if (DamageRank < 1)
        {
            Target.gameObject.GetComponent<Animator>().SetTrigger("Hit");
            SoundManager.instance.PlayEffect("BadHit");
            TextBox.instance.TalkText.text = "효과가 별로인듯하다...";
        }
        else if (DamageRank == 0)
        {
            TextBox.instance.TalkText.text = "효과가 없는거같다.";
        }
        else if (Damage == 0)
        {

        }
        else
        {
            Target.gameObject.GetComponent<Animator>().SetTrigger("Hit");
            SoundManager.instance.PlayEffect("NormalHit");
        }
        Invoke("setoff", 1.5f);


        if (Target.name.Contains("0975.Eiscue"))
        {
            if (Target.GetComponent<PokemonBattleMode>().Eiscue_head.activeSelf)
            {
                Target.gameObject.GetComponent<Animator>().SetTrigger("HitChange");
            }
        }

        float targetHp_Value = (float)Target.Hp / Target.MaxHp;
        float durationTime = 1f;

        StartCoroutine(HpUpdate_Co(Target, targetHp_Value, durationTime, Target_Slider));
    }

    //자연스럽게 HPbar를 내리기위한 코루틴
    private IEnumerator HpUpdate_Co(PokemonStats Target_stats, float targetHp_Value, float durationTime, Slider Target)
    {
        float elapsedTime = 0f;
        float startHpValue = Target.value;
        while (elapsedTime < durationTime)
        {
            //체력 소모시 색상 변경
            if (Target.value < 0.3f)
            {
                Target.gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.red;
            }
            else if (Target.value < 0.6f)
            {
                Target.gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                Target.gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.green;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / durationTime);
            float smoothedValue = Mathf.Lerp(startHpValue, targetHp_Value, t);
            Target.value = smoothedValue;

            yield return null;
        }

        if (Target.gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>().color == Color.red)
        {
            if (!Target_stats.isDie)
            {
                SoundManager.instance.PlayEffect("HealthLow");
            }
        }

        Target.value = targetHp_Value;

        Damage = 0f;
    }

    #region 데미지 총 계산 공식
    //스킬 판별
    void CompareSkillType(PokemonStats firstpokemon, out int num, out int type)
    {
        #region 스킬 판명
        num = 0;
        type = 0;



        if (firstpokemon == enemyPokemon.GetComponent<PokemonStats>())
        {
            if (firstpokemon.SkillPP[num] <= 0)
            {
                List<int> randomskillnum = new List<int>();

                for (int i = 0; i < 4; i++)
                {
                    if (i != enemyskillnum)
                    {
                        randomskillnum.Add(i);
                    }
                }

                num = randomskillnum[Random.Range(0, randomskillnum.Count)];

                if (firstpokemon.SkillPP[num] <= 0)
                {
                    List<int> randomskillnum2 = new List<int>();

                    for (int i = 0; i < 3; i++)
                    {
                        if (i != enemyskillnum)
                        {
                            randomskillnum2.Add(i);
                        }
                    }

                    num = randomskillnum2[Random.Range(0, randomskillnum2.Count)];
                }
            }
            else
            {
                num = enemyskillnum;
            }

            if (enemyplayer != null)
            {
                enemyplayer.GetComponent<Animator>().SetTrigger("Order");
            }

        }
        else if (firstpokemon == playerPokemon.GetComponent<PokemonStats>())
        {
            if (firstpokemon.SkillPP[num] <= 0)
            {
                List<int> randomskillnum = new List<int>();

                for (int i = 0; i < 4; i++)
                {
                    if (i != playerskillnum)
                    {
                        randomskillnum.Add(i);
                    }
                }

                num = randomskillnum[Random.Range(0, randomskillnum.Count)];

                if (firstpokemon.SkillPP[num] <= 0)
                {
                    List<int> randomskillnum2 = new List<int>();

                    for (int i = 0; i < 3; i++)
                    {
                        if (i != playerskillnum)
                        {
                            randomskillnum2.Add(i);
                        }
                    }

                    num = randomskillnum2[Random.Range(0, randomskillnum2.Count)];
                }


            }
            else
            {
                num = playerskillnum;
            }

            player.GetComponent<Animator>().SetTrigger("Order");
        }

        if (firstpokemon.skills[num].AttackType == SkillData.attackType.Attack)
        {

            if (firstpokemon.skills[num].isPunch_isBite
                || firstpokemon.skills[num].Name.Contains("엄니")
                || firstpokemon.skills[num].Name.Contains("깨물")
                || firstpokemon.skills[num].Name.Contains("펀치"))
            {
                type = 1;
            }
            else
            {
                type = 2;
            }

        }
        else if (firstpokemon.skills[num].AttackType == SkillData.attackType.Speicial)
        {
            if (firstpokemon.skills[num].isPunch_isBite)
            {
                type = 1;
            }
            else
            {
                type = 4;
            }
        }
        else if (firstpokemon.skills[num].AttackType == SkillData.attackType.None)
        {
            type = 3;
        }


        firstpokemon.SkillPP[num]--;

        #endregion
    }

    //데미지 판별
    void OnDamage(SkillData skill, PokemonStats attacker, PokemonStats target)
    {
        ////배틀 변수값들 초기화
        Damage = 0;
        PropertyRank = 1;
        DamageRank = 1;

        CheckProPertyType(skill, attacker);
        CheckDamageType(skill, target);
        CheckStateRank(attacker, target);

        if (skill.AttackType == SkillData.attackType.Attack)
        {
            Damage = ((attacker.Attack * AttackerAttackRank) * skill.Damage * (attacker.Level * 2 / 5 + 2) / (target.Defence * TargetDefenceRank) / 50 + 2) * PropertyRank * DamageRank;
        }
        if (skill.AttackType == SkillData.attackType.Speicial)
        {
            Damage = ((attacker.SpAttack * AttackerSpAttackRank) * skill.Damage * (attacker.Level * 2 / 5 + 2) / (target.SpDefence * TargetSpDefenceRank) / 50 + 2) * PropertyRank * DamageRank;
        }

        attacker.Attack += skill.AttackRankUp;
        attacker.SpAttackRank += skill.AttackRankUp;
        attacker.DefenceRank += skill.DefenceRankUp;
        attacker.SpDefenceRank += skill.SpDefenceRankUp;
        attacker.SpeedRank += skill.SpeedRankUp;

        target.Attack += skill.EnemyAttackRankUp;
        target.SpAttackRank += skill.EnemySpAttackRankUp;
        target.DefenceRank += skill.EnemyDefenceRankUp;
        target.SpDefenceRank += skill.EnemySpDefenceRankUp;
        target.SpeedRank += skill.EnemySpeedRankUp;



        //target.Hp -= (int)Damage;
        //Debug.Log($"{target.Name}에게 {(int)Damage}만큼의 데미지를 주었다!");

    }

    //타입 판별
    void CheckDamageType(SkillData skill, PokemonStats pokemon)
    {
        #region 타입판별
        if (skill.propertyType == SkillData.PropertyType.Normal)
        {
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Ghost || pokemon.Type2 == PokemonStats.Type.Ghost)
            {
                DamageRank *= 0f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0.5f;
            }

        } //노말타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Fight)
        {
            if (pokemon.Type1 == PokemonStats.Type.Normal || pokemon.Type2 == PokemonStats.Type.Normal)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Ice || pokemon.Type2 == PokemonStats.Type.Ice)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Poison || pokemon.Type2 == PokemonStats.Type.Poison)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Flight || pokemon.Type2 == PokemonStats.Type.Flight)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Esper || pokemon.Type2 == PokemonStats.Type.Esper)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Bug || pokemon.Type2 == PokemonStats.Type.Bug)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Ghost || pokemon.Type2 == PokemonStats.Type.Ghost)
            {
                DamageRank *= 0f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Evil || pokemon.Type2 == PokemonStats.Type.Evil)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Fairy || pokemon.Type2 == PokemonStats.Type.Fairy)
            {
                DamageRank *= 0.5f;
            }
        }//격투타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Poison)
        {
            if (pokemon.Type1 == PokemonStats.Type.Grass || pokemon.Type2 == PokemonStats.Type.Grass)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Poison || pokemon.Type2 == PokemonStats.Type.Poison)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Earth || pokemon.Type2 == PokemonStats.Type.Earth)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Ghost || pokemon.Type2 == PokemonStats.Type.Ghost)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Fairy || pokemon.Type2 == PokemonStats.Type.Fairy)
            {
                DamageRank *= 2f;
            }

        }//독타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Earth)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fire || pokemon.Type2 == PokemonStats.Type.Fire)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Grass || pokemon.Type2 == PokemonStats.Type.Grass)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Electricty || pokemon.Type2 == PokemonStats.Type.Electricty)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Poison || pokemon.Type2 == PokemonStats.Type.Poison)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Flight || pokemon.Type2 == PokemonStats.Type.Flight)
            {
                DamageRank *= 0f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Bug || pokemon.Type2 == PokemonStats.Type.Bug)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 2f;
            }
        }//땅타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Flight)
        {
            if (pokemon.Type1 == PokemonStats.Type.Grass || pokemon.Type2 == PokemonStats.Type.Grass)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Electricty || pokemon.Type2 == PokemonStats.Type.Electricty)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Fight || pokemon.Type2 == PokemonStats.Type.Fight)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Bug || pokemon.Type2 == PokemonStats.Type.Bug)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0.5f;
            }
        }//비행타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Bug)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fire || pokemon.Type2 == PokemonStats.Type.Fire)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Grass || pokemon.Type2 == PokemonStats.Type.Grass)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Fight || pokemon.Type2 == PokemonStats.Type.Fight)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Poison || pokemon.Type2 == PokemonStats.Type.Poison)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Flight || pokemon.Type2 == PokemonStats.Type.Flight)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Esper || pokemon.Type2 == PokemonStats.Type.Esper)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Ghost || pokemon.Type2 == PokemonStats.Type.Ghost)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Evil || pokemon.Type2 == PokemonStats.Type.Evil)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Fairy || pokemon.Type2 == PokemonStats.Type.Fairy)
            {
                DamageRank *= 0.5f;
            }
        }//벌레타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Rock)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fire || pokemon.Type2 == PokemonStats.Type.Fire)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Ice || pokemon.Type2 == PokemonStats.Type.Ice)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Fight || pokemon.Type2 == PokemonStats.Type.Fight)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Earth || pokemon.Type2 == PokemonStats.Type.Earth)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Flight || pokemon.Type2 == PokemonStats.Type.Flight)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Bug || pokemon.Type2 == PokemonStats.Type.Bug)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0.5f;
            }
        }//바위타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Ghost)
        {
            if (pokemon.Type1 == PokemonStats.Type.Normal || pokemon.Type2 == PokemonStats.Type.Normal)
            {
                DamageRank *= 0f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Esper || pokemon.Type2 == PokemonStats.Type.Esper)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Ghost || pokemon.Type2 == PokemonStats.Type.Ghost)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Evil || pokemon.Type2 == PokemonStats.Type.Evil)
            {
                DamageRank *= 0.5f;
            }
        }//고스트타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Steel)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fire || pokemon.Type2 == PokemonStats.Type.Fire)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Water || pokemon.Type2 == PokemonStats.Type.Water)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Electricty || pokemon.Type2 == PokemonStats.Type.Electricty)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Ice || pokemon.Type2 == PokemonStats.Type.Ice)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Fairy || pokemon.Type2 == PokemonStats.Type.Fairy)
            {
                DamageRank *= 2f;
            }
        }//강철타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Fire)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fire || pokemon.Type2 == PokemonStats.Type.Fire)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Water || pokemon.Type2 == PokemonStats.Type.Water)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Grass || pokemon.Type2 == PokemonStats.Type.Grass)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Ice || pokemon.Type2 == PokemonStats.Type.Ice)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Bug || pokemon.Type2 == PokemonStats.Type.Bug)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Dragon || pokemon.Type2 == PokemonStats.Type.Dragon)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 2f;
            }
        }//불타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Water)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fire || pokemon.Type2 == PokemonStats.Type.Fire)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Water || pokemon.Type2 == PokemonStats.Type.Water)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Grass || pokemon.Type2 == PokemonStats.Type.Grass)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Earth || pokemon.Type2 == PokemonStats.Type.Earth)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Dragon || pokemon.Type2 == PokemonStats.Type.Dragon)
            {
                DamageRank *= 0.5f;
            }
        }//물타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Electricty)
        {
            if (pokemon.Type1 == PokemonStats.Type.Water || pokemon.Type2 == PokemonStats.Type.Water)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Grass || pokemon.Type2 == PokemonStats.Type.Grass)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Electricty || pokemon.Type2 == PokemonStats.Type.Electricty)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Earth || pokemon.Type2 == PokemonStats.Type.Earth)
            {
                DamageRank *= 0f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Flight || pokemon.Type2 == PokemonStats.Type.Flight)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Dragon || pokemon.Type2 == PokemonStats.Type.Dragon)
            {
                DamageRank *= 0.5f;
            }
        }//전기타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Grass)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fire || pokemon.Type2 == PokemonStats.Type.Fire)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Water || pokemon.Type2 == PokemonStats.Type.Water)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Grass || pokemon.Type2 == PokemonStats.Type.Grass)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Poison || pokemon.Type2 == PokemonStats.Type.Poison)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Earth || pokemon.Type2 == PokemonStats.Type.Earth)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Flight || pokemon.Type2 == PokemonStats.Type.Flight)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Bug || pokemon.Type2 == PokemonStats.Type.Bug)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Dragon || pokemon.Type2 == PokemonStats.Type.Dragon)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0.5f;
            }
        }//풀타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Ice)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fire || pokemon.Type2 == PokemonStats.Type.Fire)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Water || pokemon.Type2 == PokemonStats.Type.Water)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Grass || pokemon.Type2 == PokemonStats.Type.Grass)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Ice || pokemon.Type2 == PokemonStats.Type.Ice)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Earth || pokemon.Type2 == PokemonStats.Type.Earth)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Flight || pokemon.Type2 == PokemonStats.Type.Flight)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Dragon || pokemon.Type2 == PokemonStats.Type.Dragon)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0.5f;
            }
        }//얼음타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Esper)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fight || pokemon.Type2 == PokemonStats.Type.Fight)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Poison || pokemon.Type2 == PokemonStats.Type.Poison)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Esper || pokemon.Type2 == PokemonStats.Type.Esper)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Evil || pokemon.Type2 == PokemonStats.Type.Evil)
            {
                DamageRank *= 0f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0.5f;
            }
        }//에스퍼타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Dragon)
        {
            if (pokemon.Type1 == PokemonStats.Type.Dragon || pokemon.Type2 == PokemonStats.Type.Dragon)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Fairy || pokemon.Type2 == PokemonStats.Type.Fairy)
            {
                DamageRank *= 0f;
            }
        }//드래곤타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Evil)
        {
            if (pokemon.Type1 == PokemonStats.Type.Ice || pokemon.Type2 == PokemonStats.Type.Ice)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Esper || pokemon.Type2 == PokemonStats.Type.Esper)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Dragon || pokemon.Type2 == PokemonStats.Type.Dragon)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Fairy || pokemon.Type2 == PokemonStats.Type.Fairy)
            {
                DamageRank *= 0.5f;
            }
        }//악타입 랭크 데미지 판별
        if (skill.propertyType == SkillData.PropertyType.Fairy)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fire || pokemon.Type2 == PokemonStats.Type.Fire)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Fight || pokemon.Type2 == PokemonStats.Type.Fight)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Poison || pokemon.Type2 == PokemonStats.Type.Poison)
            {
                DamageRank *= 0.5f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Dragon || pokemon.Type2 == PokemonStats.Type.Dragon)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Evil || pokemon.Type2 == PokemonStats.Type.Evil)
            {
                DamageRank *= 2f;
            }
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                DamageRank *= 0.5f;
            }
        }//페어리타입 랭크 데미지 판별
        #endregion
    }

    //자기 속성 판별
    void CheckProPertyType(SkillData skill, PokemonStats pokemon)
    {
        if (skill.propertyType == (SkillData.PropertyType)pokemon.Type1 || skill.propertyType == (SkillData.PropertyType)pokemon.Type2)
        {
            PropertyRank *= 1.5f;
        }
        #region 주석(자속확인)
        /*
        if (skill.propertyType == SkillData.PropertyType.Normal)
        {
            if (pokemon.Type1 == PokemonStats.Type.Normal || pokemon.Type2 == PokemonStats.Type.Normal)
            {
                PropertyRank *= 1.5f;
            }
        }//노말타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Fight)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fight || pokemon.Type2 == PokemonStats.Type.Fight)
            {
                PropertyRank *= 1.5f;
            }
        } //격투타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Poison)
        {
            if (pokemon.Type1 == PokemonStats.Type.Poison || pokemon.Type2 == PokemonStats.Type.Poison)
            {
                PropertyRank *= 1.5f;
            }
        }//독타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Earth)
        {
            if (pokemon.Type1 == PokemonStats.Type.Earth || pokemon.Type2 == PokemonStats.Type.Earth)
            {
                PropertyRank *= 1.5f;
            }
        }//땅타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Flight)
        {
            if (pokemon.Type1 == PokemonStats.Type.Flight || pokemon.Type2 == PokemonStats.Type.Flight)
            {
                PropertyRank *= 1.5f;
            }
        }//비행타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Bug)
        {
            if (pokemon.Type1 == PokemonStats.Type.Bug || pokemon.Type2 == PokemonStats.Type.Bug)
            {
                PropertyRank *= 1.5f;
            }
        } // 벌레타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Rock)
        {
            if (pokemon.Type1 == PokemonStats.Type.Rock || pokemon.Type2 == PokemonStats.Type.Rock)
            {
                PropertyRank *= 1.5f;
            }
        } //바위타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Ghost)
        {
            if (pokemon.Type1 == PokemonStats.Type.Ghost || pokemon.Type2 == PokemonStats.Type.Ghost)
            {
                PropertyRank *= 1.5f;
            }
        }//고스트타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Steel)
        {
            if (pokemon.Type1 == PokemonStats.Type.Steel || pokemon.Type2 == PokemonStats.Type.Steel)
            {
                PropertyRank *= 1.5f;
            }
        }//강철타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Fire)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fire || pokemon.Type2 == PokemonStats.Type.Fire)
            {
                PropertyRank *= 1.5f;
            }
        }//불타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Water)
        {
            if (pokemon.Type1 == PokemonStats.Type.Water || pokemon.Type2 == PokemonStats.Type.Water)
            {
                PropertyRank *= 1.5f;
            }
        }//물타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Electricty)
        {
            if (pokemon.Type1 == PokemonStats.Type.Electricty || pokemon.Type2 == PokemonStats.Type.Electricty)
            {
                PropertyRank *= 1.5f;
            }
        }//전기타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Grass)
        {
            if (pokemon.Type1 == PokemonStats.Type.Grass || pokemon.Type2 == PokemonStats.Type.Grass)
            {
                PropertyRank *= 1.5f;
            }
        }//풀타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Ice)
        {
            if (pokemon.Type1 == PokemonStats.Type.Ice || pokemon.Type2 == PokemonStats.Type.Ice)
            {
                PropertyRank *= 1.5f;
            }
        }//얼음타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Esper)
        {
            if (pokemon.Type1 == PokemonStats.Type.Esper || pokemon.Type2 == PokemonStats.Type.Esper)
            {
                PropertyRank *= 1.5f;
            }
        }//에스퍼타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Dragon)
        {
            if (pokemon.Type1 == PokemonStats.Type.Dragon || pokemon.Type2 == PokemonStats.Type.Dragon)
            {
                PropertyRank *= 1.5f;
            }
        }//드래곤타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Evil)
        {
            if (pokemon.Type1 == PokemonStats.Type.Evil || pokemon.Type2 == PokemonStats.Type.Evil)
            {
                PropertyRank *= 1.5f;
            }
        }//악타입 자속확인
        if (skill.propertyType == SkillData.PropertyType.Fairy)
        {
            if (pokemon.Type1 == PokemonStats.Type.Fairy || pokemon.Type2 == PokemonStats.Type.Fairy)
            {
                PropertyRank *= 1.5f;
            }
        }//페어리타입 자속확인
        */
        #endregion
    }

    //랭크 판별
    void CheckStateRank(PokemonStats attacker, PokemonStats target)
    {
        #region 랭크업 확인
        switch (attacker.AttackRank)
        {
            case 6:
                {
                    AttackerAttackRank = 4f;
                }
                break;
            case 5:
                {
                    AttackerAttackRank = 3.5f;
                }
                break;
            case 4:
                {
                    AttackerAttackRank = 3f;
                }
                break;
            case 3:
                {
                    AttackerAttackRank = 2.5f;
                }
                break;
            case 2:
                {
                    AttackerAttackRank = 2f;
                }
                break;
            case 1:
                {
                    AttackerAttackRank = 1.5f;
                }
                break;
            case -1:
                {
                    AttackerAttackRank = 0.66f;
                }
                break;
            case -2:
                {
                    AttackerAttackRank = 0.5f;
                }
                break;
            case -3:
                {
                    AttackerAttackRank = 0.4f;
                }
                break;
            case -4:
                {
                    AttackerAttackRank = 0.33f;
                }
                break;
            case -5:
                {
                    AttackerAttackRank = 0.29f;
                }
                break;
            case -6:
                {
                    AttackerAttackRank = 0.25f;
                }
                break;

        }//공격 랭크업 확인
        switch (attacker.SpAttackRank)
        {
            case 6:
                {
                    AttackerSpAttackRank = 4f;
                }
                break;
            case 5:
                {
                    AttackerSpAttackRank = 3.5f;
                }
                break;
            case 4:
                {
                    AttackerSpAttackRank = 3f;
                }
                break;
            case 3:
                {
                    AttackerSpAttackRank = 2.5f;
                }
                break;
            case 2:
                {
                    AttackerSpAttackRank = 2f;
                }
                break;
            case 1:
                {
                    AttackerSpAttackRank = 1.5f;
                }
                break;
            case -1:
                {
                    AttackerSpAttackRank = 0.66f;
                }
                break;
            case -2:
                {
                    AttackerSpAttackRank = 0.5f;
                }
                break;
            case -3:
                {
                    AttackerSpAttackRank = 0.4f;
                }
                break;
            case -4:
                {
                    AttackerSpAttackRank = 0.33f;
                }
                break;
            case -5:
                {
                    AttackerSpAttackRank = 0.29f;
                }
                break;
            case -6:
                {
                    AttackerSpAttackRank = 0.25f;
                }
                break;

        }//특수공격 랭크업 확인
        switch (target.DefenceRank)
        {
            case 6:
                {
                    TargetDefenceRank = 4f;
                }
                break;
            case 5:
                {
                    TargetDefenceRank = 3.5f;
                }
                break;
            case 4:
                {
                    TargetDefenceRank = 3f;
                }
                break;
            case 3:
                {
                    TargetDefenceRank = 2.5f;
                }
                break;
            case 2:
                {
                    TargetDefenceRank = 2f;
                }
                break;
            case 1:
                {
                    TargetDefenceRank = 1.5f;
                }
                break;
            case -1:
                {
                    TargetDefenceRank = 0.66f;
                }
                break;
            case -2:
                {
                    TargetDefenceRank = 0.5f;
                }
                break;
            case -3:
                {
                    TargetDefenceRank = 0.4f;
                }
                break;
            case -4:
                {
                    TargetDefenceRank = 0.33f;
                }
                break;
            case -5:
                {
                    TargetDefenceRank = 0.29f;
                }
                break;
            case -6:
                {
                    TargetDefenceRank = 0.25f;
                }
                break;

        } // 타겟 방어 랭크다운 확인
        switch (target.SpDefenceRank)
        {
            case 6:
                {
                    TargetSpDefenceRank = 4f;
                }
                break;
            case 5:
                {
                    TargetSpDefenceRank = 3.5f;
                }
                break;
            case 4:
                {
                    TargetSpDefenceRank = 3f;
                }
                break;
            case 3:
                {
                    TargetSpDefenceRank = 2.5f;
                }
                break;
            case 2:
                {
                    TargetSpDefenceRank = 2f;
                }
                break;
            case 1:
                {
                    TargetSpDefenceRank = 1.5f;
                }
                break;
            case -1:
                {
                    TargetSpDefenceRank = 0.66f;
                }
                break;
            case -2:
                {
                    TargetSpDefenceRank = 0.5f;
                }
                break;
            case -3:
                {
                    TargetSpDefenceRank = 0.4f;
                }
                break;
            case -4:
                {
                    TargetSpDefenceRank = 0.33f;
                }
                break;
            case -5:
                {
                    TargetSpDefenceRank = 0.29f;
                }
                break;
            case -6:
                {
                    TargetSpDefenceRank = 0.25f;
                }
                break;

        } // 타겟 특수방어 랭크다운 확인

        #region 속도비교하는거 여기서 하는지모르겠음 todo 김민수
        /*
        switch (attacker.SpeedRank)
        {
            case 6:
                {
                    AttackerSpeedRank = 4f;
                }
                break;
            case 5:
                {
                    AttackerSpeedRank = 3.5f;
                }
                break;
            case 4:
                {
                    AttackerSpeedRank = 3f;
                }
                break;
            case 3:
                {
                    AttackerSpeedRank = 2.5f;
                }
                break;
            case 2:
                {
                    AttackerSpeedRank = 2f;
                }
                break;
            case 1:
                {
                    AttackerSpeedRank = 1.5f;
                }
                break;
            case -1:
                {
                    AttackerSpeedRank = 0.66f;
                }
                break;
            case -2:
                {
                    AttackerSpeedRank = 0.5f;
                }
                break;
            case -3:
                {
                    AttackerSpeedRank = 0.4f;
                }
                break;
            case -4:
                {
                    AttackerSpeedRank = 0.33f;
                }
                break;
            case -5:
                {
                    AttackerSpeedRank = 0.29f;
                }
                break;
            case -6:
                {
                    AttackerSpeedRank = 0.25f;
                }
                break;

        } // 공격자 스피드 랭크다운 확인
        switch (target.SpeedRank)
        {
            case 6:
                {
                    TargetSpeedRank = 4f;
                }
                break;
            case 5:
                {
                    TargetSpeedRank = 3.5f;
                }
                break;
            case 4:
                {
                    TargetSpeedRank = 3f;
                }
                break;
            case 3:
                {
                    TargetSpeedRank = 2.5f;
                }
                break;
            case 2:
                {
                    TargetSpeedRank = 2f;
                }
                break;
            case 1:
                {
                    TargetSpeedRank = 1.5f;
                }
                break;
            case -1:
                {
                    TargetSpeedRank = 0.66f;
                }
                break;
            case -2:
                {
                    TargetSpeedRank = 0.5f;
                }
                break;
            case -3:
                {
                    TargetSpeedRank = 0.4f;
                }
                break;
            case -4:
                {
                    TargetSpeedRank = 0.33f;
                }
                break;
            case -5:
                {
                    TargetSpeedRank = 0.29f;
                }
                break;
            case -6:
                {
                    TargetSpeedRank = 0.25f;
                }
                break;

        } // 타겟 스피드 랭크다운 확인
        */


        #endregion
        #endregion
    }

    #endregion

    //그냥 입력 이벤트
    public void Player_Choise_Skill(int num)
    {
        playerskillnum = num;
        if (enemyPokemon.GetComponent<PokemonBattleMode>().isWild)
        {
            //UI 끔!
            Battle_UI.transform.GetChild(1).gameObject.SetActive(false);
            Battle_UI.transform.GetChild(2).gameObject.SetActive(false);
            Battle_UI.transform.GetChild(3).gameObject.SetActive(false);
            Battle_UI.transform.GetChild(4).gameObject.SetActive(false);
            Battle_UI.transform.GetChild(5).gameObject.SetActive(false);
            Battle_UI.transform.GetChild(6).gameObject.SetActive(false);
        }
        else
        {
            Battle_UI.transform.GetChild(2).gameObject.SetActive(false);
            Battle_UI.transform.GetChild(3).gameObject.SetActive(false);
            Battle_UI.transform.GetChild(4).gameObject.SetActive(false);
            Battle_UI.transform.GetChild(5).gameObject.SetActive(false);
            Battle_UI.transform.GetChild(6).gameObject.SetActive(false);
        }

        //스택 다시 이용
        uIManger.UI_stack.Push(Battle_UI.transform.GetChild(2).gameObject);
    }



    //코루틴 캐싱 
    #region 코루틴 캐싱
    internal static class YieldInstructionCache
    {
        class FloatComparer : IEqualityComparer<float>
        {
            bool IEqualityComparer<float>.Equals(float x, float y)
            {
                return x == y;
            }
            int IEqualityComparer<float>.GetHashCode(float obj)
            {
                return obj.GetHashCode();
            }
        }

        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

        private static readonly Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(new FloatComparer());

        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            WaitForSeconds wfs;
            if (!_timeInterval.TryGetValue(seconds, out wfs))
                _timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));
            return wfs;
        }
    }
    #endregion
}

