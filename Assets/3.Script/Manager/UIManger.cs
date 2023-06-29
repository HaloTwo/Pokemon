using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManger : MonoBehaviour
{
    [Header("기본 전체")]
    [SerializeField] private GameObject Default_UI;
    [Header("HP관련 UI")]
    [SerializeField] private GameObject HP_UI;
    [SerializeField] private Slider hPbar;
    [SerializeField] private Text hp_txt;
    [Header("싸운다 -> 스킬 UI")]
    [SerializeField] private GameObject Skill_UI;
    [Header("포켓몬 교체 -> 현재 갖고있는 포켓몬UI")]
    [SerializeField] private GameObject Change_UI;
    [Header("가방 -> 가방UI")]
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
