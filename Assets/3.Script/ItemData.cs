using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ItemData", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    [Header("아이템 이미지")]
    [SerializeField] private Sprite Image;
    [Header("아이템 이름")]
    [SerializeField] private string Name;
    [Header("아이템 갯수")]
    [SerializeField] private int Quantity;
    [Header("포획률")]
    [SerializeField] private float Catchrate;
    [Header("회복량")]
    [SerializeField] private int HealingHp;
    [SerializeField] private int HealingHpPercent;
    [SerializeField] private int HealingPp;
    [SerializeField] private int HealingPpPercent;
    [Header("가격")]
    [SerializeField] private int Price;
    [SerializeField] private int SellPrice;
    //[SerializeField] private int MachineNum;
    [Header("아이템 타입")]
    [SerializeField] private ItmeType Type;
    public enum ItmeType
    {
        Portion, Important, Ball, SkillMachine, Equip, ETC
    }


}
