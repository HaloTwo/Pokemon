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
    private float Damage = 0;
    [SerializeField] Text Explain_txt;

    [Header("서로의 거리")]
    [SerializeField] private float distance = 8f;

    [Header("카메라")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineFreeLook PlayerCamera;

    [Header("포켓몬")]
    public GameObject playerPokemon;
    [SerializeField] private int skillnum;
    public GameObject enemyPokemon;
    [SerializeField] private int enemyskillnum;

    [Header("플레이어")]
    [SerializeField] private GameObject player;

    [Header("플레이어 UI")]
    [SerializeField] private Canvas Battle_UI;


    public bool islose = true;
    public UnityEvent Battle_Ready;

    //배틀 시작!
    public void Battle_Start(GameObject enemyPokemon, GameObject player)
    {
        this.enemyPokemon = enemyPokemon;
        this.player = player;

        //애니메이터 잠시 끔
        StartCoroutine(player.GetComponent<PlayerMovement>().apply_motion_wait(5f));

        //플레이어 못움직이게
        player.GetComponent<PlayerMovement>().isBattle = true;

        //포켓몬 생성 하고 playerPokemon을 채워줌
        Battle_Ready.Invoke();

        //포켓몬과 플레이어 위치 조정
        //포켓몬 위치
        playerPokemon.transform.position = enemyPokemon.transform.position + enemyPokemon.transform.forward * distance;
        playerPokemon.transform.LookAt(enemyPokemon.transform.position);

        //플레이어 위치
        Quaternion rotation = Quaternion.Euler(0f, 25f, 0f);
        Vector3 offset = rotation * -playerPokemon.transform.forward;
        player.transform.position = playerPokemon.transform.position + offset * 2f;
        player.transform.LookAt(enemyPokemon.transform.position);

        //카메라 연출 시작(아직 야생만)
        StartCoroutine(Wild_BattleCamera_co(enemyPokemon));

        //과 동시에 배틀 시작
        StartCoroutine(Battle(enemyPokemon, playerPokemon, player));
    }

    //카메라 연출(아직 야생만)
    IEnumerator Wild_BattleCamera_co(GameObject enemyPokemon)
    {
        //카메라 이동
        virtualCamera.Priority = 15;

        //적 포켓몬의 중심
        Vector3 targetCenter = this.enemyPokemon.transform.position + this.enemyPokemon.transform.up * enemyPokemon.GetComponentInChildren<Renderer>().bounds.size.y * 0.5f;

        //중심을 바라보도록
        virtualCamera.transform.position = targetCenter + this.enemyPokemon.transform.forward * 3f;
        virtualCamera.transform.rotation = Quaternion.LookRotation(targetCenter - virtualCamera.transform.position);

        //3초 딜레이
        yield return new WaitForSeconds(3f);

        //다시 원래 카메라로
        virtualCamera.Priority = 0;

        //PlayerPokemon을 쳐다보도록
        PlayerCamera.Follow = playerPokemon.transform;
        PlayerCamera.LookAt = playerPokemon.transform;

        //보고싶은 카메라의 각도 = 포켓몬 위치에서 10도
        float playerPokemonAngle = Quaternion.LookRotation(playerPokemon.transform.forward, Vector3.up).eulerAngles.y;
        float cameraXAngle_local = 10f;
        //각도 계산
        float cameraXAngle = Mathf.Repeat(playerPokemonAngle - cameraXAngle_local + 180f, 360f) - 180f;

        //플레이어 카메라의 값
        PlayerCamera.m_XAxis.Value = cameraXAngle;
        PlayerCamera.m_YAxis.Value = 0.4f;

        yield return new WaitForSeconds(1.5f);

        //UI 시작!
        Battle_UI.gameObject.SetActive(true);
    }

    //배틀 시작
    public IEnumerator Battle(GameObject enemyPokemon, GameObject playerPokemon, GameObject player)
    {
        PokemonStats Enemy_pokemon = enemyPokemon.GetComponent<PokemonStats>();
        PokemonStats Player_pokemon = playerPokemon.GetComponent<PokemonStats>();

        while (islose)
        {
            skillnum = 0;
            enemyskillnum = Random.Range(1, 4);

            yield return new WaitUntil(() => skillnum != 0);

            //서로 스피드 비교
            SpeedComparison(Enemy_pokemon, Player_pokemon);

            //이제 공격을 해야되는데


        }

    }



    public void Player_Choise_Skill(int num)
    {
        skillnum = num;
    }

    //스피드 비교
    void SpeedComparison(PokemonStats Enemy_pokemon, PokemonStats Player_pokemon)
    {

        //내 스킬의 우선도가 더 높을때
        if (Player_pokemon.skills[skillnum].Priority > Enemy_pokemon.skills[enemyskillnum].Priority)
        {
            
        }
        //적의 스킬의 우선도가 더 높을때
        else if (Player_pokemon.skills[skillnum].Priority < Enemy_pokemon.skills[enemyskillnum].Priority)
        {
            
        }
        //내 포켓몬의 속도가 더 높을때
        else if (Player_pokemon.Speed > Enemy_pokemon.Speed)
        {
            
        }
        //적의 스피드가 더 높을 때
        else if (Player_pokemon.Speed < Enemy_pokemon.Speed)
        {
            
        }
        //스피드가 똑같을때는 랜덤으로 결정
        else if (Enemy_pokemon.Speed == Player_pokemon.Speed)
        {
            int randomStart = Random.Range(1, 2);

            if (randomStart == 1)
            {
                
            }
            else if (randomStart == 2)
            {
                
            }
        }
    }



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


        if (DamageRank > 1)
        {
            Explain_txt.text = "효과가 굉장했다!";
        }
        else if (DamageRank < 1)
        {

            Explain_txt.text = "효과가 별로인듯하다...";
        }
        else if (DamageRank == 0)
        {
            Explain_txt.text = "효과가 없는거같다.";
            Damage = 0;
        }
        else
        {

        }
        target.Hp -= (int)Damage;
        Debug.Log("{0}에게 {1}만큼의 데미지를 주었다!" + target.name + Damage);

        //배틀 변수값들 초기화
        PropertyRank = 1;
        DamageRank = 1;
    }
    public void CheckDamageType(SkillData skill, PokemonStats pokemon)
    {
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

    }
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
    public void CheckStateRank(PokemonStats attacker, PokemonStats target)
    {
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
    }
}

