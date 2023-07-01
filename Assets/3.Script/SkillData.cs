using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Skill", fileName = "Skill")]
public class SkillData : ScriptableObject
{
    [Header("기술머신")]
    [SerializeField] public int Num;
    [SerializeField] public string Name;
    [SerializeField] public int Damage;
    [SerializeField] public int Hitrate;
    [SerializeField] public int PP;
    [SerializeField] public int MaxPP;
    [SerializeField] public int Number_of_Attack = 1;

    [Header("속성")]
    [SerializeField] public PropertyType propertyType;


    [Header("우선도")]
    [SerializeField] public int Priority;
    [Header("물공인지 특공인지")]
    [SerializeField] public attackType AttackType;

    [Header("필중기인지")]
    [SerializeField] public bool isMustDamage;
    [Header("일격기인지")]
    [SerializeField] public bool isStriker;
    [Header("펀치모션이나 물기모션")]
    [SerializeField] public bool isPunch_isBite;
    public enum attackType
    {
        Attack, Speicial, None
    }
    public enum PropertyType
    {
        Normal, Fight, Poison, Earth, Flight, Bug, Rock, Ghost, Steel, Fire, Water, Electricty, Grass, Ice, Esper, Dragon, Evil, Fairy, None
    }
}