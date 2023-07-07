using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ItemData", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    [Header("아이템 이미지")]
    public Sprite Image;
    [Header("아이템 이름")]
    public string Name;
    [Header("아이템 갯수")]
    public int Quantity;
    [Header("포획률")]
    public float Catchrate;
    [Header("회복량")]
    public int HealingHp;
    public int HealingHpPercent;
    public int HealingPp;
    public int HealingPpPercent;
    [Header("가격")]
    public int Price;
    public int SellPrice;
    //[SerializeField] private int MachineNum;
    [Header("아이템 타입")]
    public ItmeType Type;

    [Multiline(5)]
    [Header("아이템 설명")]
    public string Explanation;
    public enum ItmeType
    {
        Portion, Important, Ball, SkillMachine, Equip, ETC
    }


}
