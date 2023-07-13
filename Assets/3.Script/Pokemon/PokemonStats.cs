using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class PokemonStats : MonoBehaviour
{
    PokemonBattleMode pokemonBattle;
    System.Random random = new System.Random();
    [SerializeField] private int hp;
    public int Hp
    {
        get { return hp; }
        set
        {
            if (value <= 0)
            {
                hp = 0;
                isDie = true;
                pokemonBattle.OnDie();
            }
            else if (value > 0 && value <= MaxHp)
            {
                hp = value;
            }
        }
    }

    public bool isDie;
    [Header("�⺻ �ɷ�ġ")]
    [SerializeField] public int Default_MaxHp;
    [SerializeField] public int Default_Attack;
    [SerializeField] public int Default_Defence;
    [SerializeField] public int Default_SpAttack;
    [SerializeField] public int Default_SpDefence;
    [SerializeField] public int Default_Speed;
    [SerializeField] public int Default_Level;

    [Header("���� �ɷ�ġ")]
    //��ü�� ��ü��
    [SerializeField] public bool PlayerOwned;
    [SerializeField] public string Name;
    [SerializeField] public Sprite image;
    [SerializeField] public int MaxHp;
    [SerializeField] public int Attack;
    [SerializeField] public int Defence;
    [SerializeField] public int SpAttack;
    [SerializeField] public int SpDefence;
    [SerializeField] public int Speed;
    [SerializeField] public int Level;
    [SerializeField] public int Exp;
    [SerializeField] public Type Type1;
    [SerializeField] public Type Type2;
    [SerializeField] public int[] SkillPP;
    public enum Type
    {
        Normal, Fight, Poison, Earth, Flight, Bug, Rock, Ghost, Steel, Fire, Water, Electricty, Grass, Ice, Esper, Dragon, Evil, Fairy, None
    }

    //���� ��ũ��,�ٿ�
    public float AttackRank = 0;
    public float SpAttackRank = 0;
    public float DefenceRank = 0;
    public float SpDefenceRank = 0;
    public float SpeedRank = 0;
    public float HitrateRank = 0;

    public void LevelUp()
    {
        MaxHp = Default_MaxHp + (Level * 2);
        Hp = MaxHp;
        Attack = Default_Attack + (Level * 2);
        Defence = Default_Defence + (Level * 2);
        SpAttack = Default_MaxHp + (Level * 2);
        SpDefence = Default_SpAttack + (Level * 2);
        Default_Speed = Default_Speed + (Level * 2);
    }

    //��ų��
    public List<SkillData> skills = new List<SkillData>();

    public void AddSkill(SkillData skill)
    {
        skills.Add(skill);
    }
    public void RemoveSkill(int count)
    {
        skills.RemoveAt(count);
    }
    public void ClearSkill()
    {
        skills.Clear();
    }
    string jsonFileName = "DataBase/Pokemon";
    PokemonData[] pokemonArray;
    protected virtual void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        pokemonArray = JsonMapper.ToObject<PokemonData[]>(jsonFile.text);
        TryGetComponent(out pokemonBattle);

        if (pokemonBattle.isWild)
        {
            //���ϸ� ����
            Level = Random.Range(10, 30);
        }

        LevelUp();
    }
    private void Start()
    {
    }


    public PokemonData[] GetPokemonArray()
    {
        return pokemonArray;
    }
}
public class PokemonData
{
    public string Name;
    public Sprite image;
    public int Num;
    public int MaxHp;
    public int Hp;
    public int Attack;
    public int Defence;
    public int SpAttack;
    public int SpDefence;
    public int Speed;
    public int Type1;
    public int Type2;
    public int Skill1;
    public int Skill2;
    public int Skill3;
    public int Skill4;
}

