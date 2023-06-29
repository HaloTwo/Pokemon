using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManger : MonoBehaviour
{
    [Header("�⺻ ��ü")]
    [SerializeField] private GameObject Default_UI;
    [Header("HP���� UI")]
    [SerializeField] private GameObject HP_UI;
    [SerializeField] private Slider hPbar;
    [SerializeField] private Text hp_txt;
    [Header("�ο�� -> ��ų UI")]
    [SerializeField] private GameObject Skill_UI;
    [Header("���ϸ� ��ü -> ���� �����ִ� ���ϸ�UI")]
    [SerializeField] private GameObject Change_UI;
    [Header("���� -> ����UI")]
    [SerializeField] private GameObject Bag_UI;

    private void Update()
    {
        
    }

    public void UI_Fight()
    {
        Default_UI.SetActive(false);
        Skill_UI.SetActive(true);
    }

    public void UI_Change()
    {
        HP_UI.SetActive(false);
        Default_UI.SetActive(false);
        Change_UI.SetActive(true);
    }

    public void UI_Bag()
    {

    }
    public void UI_Run()
    {

    }
}
