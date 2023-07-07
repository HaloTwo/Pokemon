using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ItemData", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    [Header("������ �̹���")]
    public Sprite Image;
    [Header("������ �̸�")]
    public string Name;
    [Header("������ ����")]
    public int Quantity;
    [Header("��ȹ��")]
    public float Catchrate;
    [Header("ȸ����")]
    public int HealingHp;
    public int HealingHpPercent;
    public int HealingPp;
    public int HealingPpPercent;
    [Header("����")]
    public int Price;
    public int SellPrice;
    //[SerializeField] private int MachineNum;
    [Header("������ Ÿ��")]
    public ItmeType Type;

    [Multiline(5)]
    [Header("������ ����")]
    public string Explanation;
    public enum ItmeType
    {
        Portion, Important, Ball, SkillMachine, Equip, ETC
    }


}
