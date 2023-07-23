using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Skill", fileName = "Skill")]
public class SkillData : ScriptableObject
{
    [Header("����ӽ�")]
    [SerializeField] public int Num;
    [SerializeField] public string Name;
    [SerializeField] public int Damage;
    [SerializeField] public int Hitrate;
    [SerializeField] public int PP;
    [SerializeField] public int MaxPP;
    [SerializeField] public int Number_of_Attack = 1;

    [Header("�Ӽ�")]
    [SerializeField] public PropertyType propertyType;

    [Header("�켱��")]
    [SerializeField] public int Priority;
    [Header("�������� Ư������")]
    [SerializeField] public attackType AttackType;
    [Header("��ũ��")]
    [SerializeField] public int AttackRankUp;
    [SerializeField] public int SpAttackRankUp;
    [SerializeField] public int DefenceRankUp;
    [SerializeField] public int SpDefenceRankUp;
    [SerializeField] public int SpeedRankUp;
    [Header("������ ��ũ")]
    [SerializeField] public int EnemyAttackRankUp;
    [SerializeField] public int EnemySpAttackRankUp;
    [SerializeField] public int EnemyDefenceRankUp;
    [SerializeField] public int EnemySpDefenceRankUp;
    [SerializeField] public int EnemySpeedRankUp;

    [Header("���߱�����")]
    [SerializeField] public bool isMustDamage;
    [Header("�ϰݱ�����")]
    [SerializeField] public bool isStriker;
    [Header("��ġ����̳� ������")]
    [SerializeField] public bool isPunch_isBite;
    public enum attackType
    {
        Attack, Speicial, None
    }
    public enum PropertyType
    {
        Normal, Fight, Poison, Earth, Flight, Bug, Rock, Ghost, Steel, Fire, Water, Electricty, Grass, Ice, Esper, Dragon, Evil, Fairy, None
    }public enum rankUp
    {
        None, AttackerAttackRank, AttackerSpAttackRank, AttackerDefenceRank, AttackerSpDefenceRank, AttackerSpeedRank
    }
}