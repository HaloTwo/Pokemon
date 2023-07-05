using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;

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

    public PokemonData PlayerPokemon;
    public PokemonData EnemyPokemon;

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

    [Header("서로의 거리")]
    [SerializeField] private float distance = 8f;

    [Header("카메라")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineFreeLook PlayerCamera;

    [Header("포켓몬")]
    public GameObject playerPokemon;
    [SerializeField] private int playerskillnum;
    public GameObject enemyPokemon;
    [SerializeField] private int enemyskillnum;

    [Header("플레이어")]
    [SerializeField] private GameObject player;

    [Header("플레이어 UI")]
    [SerializeField] private Canvas Battle_UI;

    [Header("배틀 턴")]
    [SerializeField] private int turn;

    public bool islose;
    public bool isWin;
    public bool isRun;

    public UnityEvent Battle_Ready;

    [SerializeField] Transform newTransform;
    //배틀 시작!
    public void Battle_Start(GameObject enemyPokemon, GameObject player)
    {
        this.enemyPokemon = enemyPokemon;
        this.player = player;

        //포켓몬 생성 하고 playerPokemon을 채워줌
        Battle_Ready.Invoke();

        //포켓몬과 플레이어 위치 조정
        //포켓몬 위치
        //playerPokemon.transform.position = enemyPokemon.transform.position + enemyPokemon.transform.forward * distance;
        //playerPokemon.transform.LookAt(enemyPokemon.transform.position);

        float enemySize = enemyPokemon.GetComponentInChildren<Renderer>().bounds.size.z; // 상대 포켓몬의 크기 (z 축을 기준으로)
        float playerSize = playerPokemon.GetComponentInChildren<Renderer>().bounds.size.z; // 자신의 포켓몬의 크기 (z 축을 기준으로)

        float totalSize = enemySize + playerSize; // 두 포켓몬의 크기를 합친 값

        float adjustedDistance = distance + totalSize; // 크기를 고려한 조정된 거리

        Vector3 direction = enemyPokemon.transform.forward;
        Vector3 targetPosition = enemyPokemon.transform.position + direction * adjustedDistance;
        playerPokemon.transform.position = targetPosition;
        playerPokemon.transform.rotation = Quaternion.LookRotation(-direction);



        //플레이어 위치
        Quaternion rotation = Quaternion.Euler(0f, 25f, 0f);
        Vector3 offset = rotation * -playerPokemon.transform.forward;
        player.transform.position = playerPokemon.transform.position + offset * 4f;
        player.transform.LookAt(enemyPokemon.transform.position);

        //카메라 연출 시작(아직 야생만)
        StartCoroutine(Wild_BattleCamera_co(offset));

        //끝나면
        StartCoroutine(Battle(enemyPokemon, playerPokemon, player));
    }

    //카메라 연출(아직 야생만)
    IEnumerator Wild_BattleCamera_co(Vector3 offset)
    {

        #region 이동을 안했을 시 예외처리
        //내 포켓몬이 이동 안했을시 예외처리
        if (playerPokemon.transform.position != enemyPokemon.transform.position + enemyPokemon.transform.forward * distance)
        {
            playerPokemon.transform.position = enemyPokemon.transform.position + enemyPokemon.transform.forward * distance;
            playerPokemon.transform.LookAt(enemyPokemon.transform.position);
        }

        //플레이어가 이동 안했을시 예외처리
        if (player.transform.position != playerPokemon.transform.position + offset * 4f)
        {
            player.transform.position = playerPokemon.transform.position + offset * 4f;
            player.transform.LookAt(enemyPokemon.transform.position);
        }
        #endregion

        //카메라 이동
        virtualCamera.Priority = 15;

        //적 포켓몬의 중심을 찾고, 크기에 따라 카메라 이동
        #region 적 포켓몬 크기에 따라 카메라 이동
        Renderer enemy_renderer = enemyPokemon.GetComponentInChildren<Renderer>();
        Vector3 targetCenter = enemyPokemon.transform.position + enemyPokemon.transform.up * enemy_renderer.bounds.size.y * 0.55f;
        if (enemy_renderer.bounds.size.y >= 5.8f)
        {
            targetCenter = enemyPokemon.transform.position + enemyPokemon.transform.up * enemy_renderer.bounds.size.y * 0.8f;
            virtualCamera.transform.position = targetCenter + enemyPokemon.transform.forward * 8f;
        }
        else if (enemy_renderer.bounds.size.y > 3f)
        {
            virtualCamera.transform.position = targetCenter + enemyPokemon.transform.forward * 7f;
        }
        else
        {
            virtualCamera.transform.position = targetCenter + enemyPokemon.transform.forward * 3f;
        }
        virtualCamera.transform.rotation = Quaternion.LookRotation(targetCenter - virtualCamera.transform.position);

        #endregion


        //몬스터가 소리 지를때까지 구경, 3초
        Debug.Log("상대 포켓몬 크기: " + enemy_renderer.bounds.size);
        yield return new WaitForSeconds(3f);

        //다시 원래 카메라로
        virtualCamera.Priority = 0;

        //카메라 타겟을 가운데쯤으로
        #region 내 포켓몬 크기에 따라 내 카메라 이동
        Transform playerTransform = playerPokemon.transform;
        Renderer py_pokemon_renderer = playerPokemon.GetComponentInChildren<Renderer>();

        Vector3 position = playerTransform.position + playerTransform.up * py_pokemon_renderer.bounds.size.y * 0.5f;
        Quaternion py_rotation = playerTransform.rotation;
        Vector3 scale = playerTransform.localScale;

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

        if (py_pokemon_renderer.bounds.size.y >= 2)
        {
            PlayerCamera.m_Orbits[1].m_Radius = py_pokemon_renderer.bounds.size.z * 1.65f;
        }
        else if (py_pokemon_renderer.bounds.size.y < 2)
        {
            PlayerCamera.m_Orbits[1].m_Radius = 4;
        }

        //PlayerPokemon을 쳐다보도록
        PlayerCamera.Follow = newTransform;
        PlayerCamera.LookAt = newTransform;

        //보고싶은 카메라의 각도 = 포켓몬 위치에서 10도
        float playerPokemonAngle = Quaternion.LookRotation(playerPokemon.transform.forward, Vector3.up).eulerAngles.y;
        float cameraXAngle_local = 25f;

        //각도 계산
        float cameraXAngle = Mathf.Repeat(playerPokemonAngle - cameraXAngle_local + 180f, 360f) - 180f;

        //플레이어 카메라의 값
        PlayerCamera.m_XAxis.Value = cameraXAngle;
        PlayerCamera.m_YAxis.Value = 0.45f;
        #endregion

        //카메라가 돌아온 다음, UI생성
        Debug.Log("내 포켓몬 크기: " + py_pokemon_renderer.bounds.size);
        yield return new WaitForSeconds(1.5f);

        //UI 생성
        Battle_UI.gameObject.SetActive(true);

    }

    //배틀 시작
    public IEnumerator Battle(GameObject enemyPokemon, GameObject playerPokemon, GameObject player)
    {
        PokemonStats enemy_pokemon_Stats = enemyPokemon.GetComponent<PokemonStats>();
        PokemonStats player_pokemon_Stats = playerPokemon.GetComponent<PokemonStats>();

        PokemonBattleMode enemy_pokemon_battlemode = enemyPokemon.GetComponent<PokemonBattleMode>();
        PokemonBattleMode player_pokemon_battlemode = enemyPokemon.GetComponent<PokemonBattleMode>();

        Slider player_pokemon_slider = Battle_UI.GetComponent<UIManger>().hPbar;
        Slider enemy_pokemon_slider = enemy_pokemon_battlemode.pokemon_slider;

        PokemonStats first_attack_pokemon;
        PokemonStats next_tattacker_pokemon;

        Slider first_attack_pokemon_slider;
        Slider next_tattacker_pokemon_slider;


        //초기화
        islose = false;
        isWin = false;
        isRun = false;

        while (true)
        {

            playerskillnum = -1;
            enemyskillnum = Random.Range(0, 4);

            yield return new WaitUntil(() => playerskillnum != -1 || isRun);
            //바로 빠져나가기
            if (islose || isWin || isRun)
            {
                break;
            }

            //스피드 비교
            CompareSpeed(enemy_pokemon_Stats, player_pokemon_Stats, enemy_pokemon_slider, player_pokemon_slider,
                     out first_attack_pokemon, out next_tattacker_pokemon, out first_attack_pokemon_slider, out next_tattacker_pokemon_slider);

            yield return new WaitForSeconds(1f);

            //공격 페이즈
            AttackPhase(first_attack_pokemon, next_tattacker_pokemon);

            //공격 모션 중간쯤에 피격 맞음
            yield return new WaitUntil(() =>
            first_attack_pokemon.gameObject.GetComponent<PokemonBattleMode>().anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            first_attack_pokemon.gameObject.GetComponent<PokemonBattleMode>().anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);

            //피격맞고 체력바 떨어짐
            HitPhase(next_tattacker_pokemon, next_tattacker_pokemon_slider);

            //사망시 아웃
            if (islose || isWin || isRun || next_tattacker_pokemon.isDie)
            {
                break;
            }

            yield return new WaitForSeconds(3f);

            //다음 포켓몬 공격
            AttackPhase(next_tattacker_pokemon, first_attack_pokemon);

            //공격 모션 중간쯤에 피격 맞음
            yield return new WaitUntil(() =>
            next_tattacker_pokemon.gameObject.GetComponent<PokemonBattleMode>().anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            next_tattacker_pokemon.gameObject.GetComponent<PokemonBattleMode>().anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);

            //피격맞고 체력바 떨어짐
            HitPhase(first_attack_pokemon, first_attack_pokemon_slider);

            //조건 검사
            if (islose || isWin || isRun || first_attack_pokemon.isDie)
            {
                break;
            }

            yield return new WaitForSeconds(3f);


            turn++;

            //UI 리셋!
            #region UI 리셋
            if (!Battle_UI.gameObject.transform.GetChild(1).gameObject.activeSelf)
            {
                Battle_UI.gameObject.transform.Find("Select").gameObject.SetActive(true);
                Battle_UI.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            else if (!Battle_UI.gameObject.activeSelf)
            {
                Battle_UI.gameObject.SetActive(true);
            }
            #endregion

        }


        if (isRun)
        {
            TextBox.instance.Textbox_OnOff(true);
            TextBox.instance.TalkText.text = "무사히 도망갔습니다.";
            yield return new WaitForSeconds(1f);
            TextBox.instance.Textbox_OnOff(false);

            enemy_pokemon_battlemode.enabled = false;
            enemy_pokemon_battlemode.anim.SetBool("Battle", false);
            enemy_pokemon_battlemode.anim.SetBool("Walk", true);

            enemyPokemon.GetComponent<PokemonMove>().enabled = true;
        }
        else if (islose)
        {
            player_pokemon_battlemode.anim.SetTrigger("Die");
            TextBox.instance.Textbox_OnOff(true);

            TextBox.instance.TalkText.text = "졌습니다.";
            yield return new WaitForSeconds(1.5f);
            TextBox.instance.Textbox_OnOff(false);

            enemy_pokemon_battlemode.enabled = false;
            enemy_pokemon_battlemode.anim.SetBool("Battle", false);
            enemy_pokemon_battlemode.anim.SetBool("Walk", true);

            enemyPokemon.GetComponent<PokemonMove>().enabled = true;
        }

        Battle_UI.gameObject.SetActive(false);

        player.GetComponent<PlayerMovement>().isBattle = false;
        playerPokemon.SetActive(false);

        newTransform.gameObject.SetActive(false);

        PlayerCamera.m_Orbits[1].m_Radius = 5f;
        PlayerCamera.Follow = player.transform;
        PlayerCamera.LookAt = player.transform;

    }

    //스피드 비교
    void CompareSpeed(PokemonStats Enemy_pokemon, PokemonStats Player_pokemon, Slider Enemy_slider, Slider player_slider,
        out PokemonStats firstattcker, out PokemonStats nextattacker,
        out Slider first_attack_pokemon_slider, out Slider next_tattacker_pokemon_slider)
    {
        #region 속도 체크
        firstattcker = null;
        nextattacker = null;
        first_attack_pokemon_slider = null;
        next_tattacker_pokemon_slider = null;

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

        Invoke("setoff", 2f);
    }
    void setoff()
    {
        TextBox.instance.TalkText.text = "";
        TextBox.instance.Textbox_OnOff(false);
    }


    //맞는 판정
    public void HitPhase(PokemonStats Target, Slider Target_Slider) 
    {
        if (Target == playerPokemon.GetComponent<PokemonStats>())
        {
            Battle_UI.gameObject.SetActive(true);
            Battle_UI.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            Battle_UI.gameObject.transform.Find("Select").gameObject.SetActive(false);
        }

        Target.Hp -= (int)Damage;
        Debug.Log($"{Target.Name}에게 {(int)Damage}만큼의 데미지를 주었다!");
        Target.gameObject.GetComponent<PokemonBattleMode>().anim.SetTrigger("Hit");


        float targetHp_Value = (float)Target.Hp / Target.MaxHp;
        float durationTime = 1f;

        StartCoroutine(HpUpdate_Co(targetHp_Value, durationTime, Target_Slider));
    }

    //자연스럽게 HPbar를 내리기위한 코루틴
    private IEnumerator HpUpdate_Co(float targetHp_Value, float durationTime, Slider Target) 
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
        Target.value = targetHp_Value;

        Damage = 0f;
    }


    //스킬 판별
    void CompareSkillType(PokemonStats firstpokemon, out int num, out int type)
    {
        #region 스킬 판명
        num = 0;
        type = 0;

        firstpokemon.SkillPP[num]--;


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
            }
            else
            {
                num = enemyskillnum;
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

        #endregion
    }

    //데미지 판별
    public void OnDamage(SkillData skill, PokemonStats attacker, PokemonStats target)
    {
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
        if (Damage <= 0)
        {
            Damage = 1;
        }

        if (DamageRank > 1)
        {
            TextBox.instance.TalkText.text = "효과가 굉장했다!";
        }
        else if (DamageRank < 1)
        {

            TextBox.instance.TalkText.text = "효과가 별로인듯하다...";
        }
        else if (DamageRank == 0)
        {
            TextBox.instance.TalkText.text = "효과가 없는거같다.";
            Damage = 0;
        }
        else
        {

        }

        //target.Hp -= (int)Damage;
        //Debug.Log($"{target.Name}에게 {(int)Damage}만큼의 데미지를 주었다!");

        //배틀 변수값들 초기화
        PropertyRank = 1;
        DamageRank = 1;
    }

    //타입 판별
    public void CheckDamageType(SkillData skill, PokemonStats pokemon)
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
    public void CheckProPertyType(SkillData skill, PokemonStats pokemon)
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
    public void CheckStateRank(PokemonStats attacker, PokemonStats target)
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


    //그냥 입력 이벤트
    public void Player_Choise_Skill(int num)
    {
        playerskillnum = num;

        //UI 끔!
        Battle_UI.gameObject.SetActive(false);
    }
}

