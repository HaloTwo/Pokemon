using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManger : MonoBehaviour
{
    private Stack<GameObject> UI_stack = new Stack<GameObject>();

    [Header("�⺻UI")]
    [SerializeField] private GameObject Default_UI;
    [Header("HP���� UI")]
    [SerializeField] private GameObject HP_UI;
    [SerializeField] private Slider hPbar;
    [SerializeField] private Text hp_txt;
    [Header("�ο�� -> ��ų UI")]
    [SerializeField] private GameObject Skill_UI;
    [Header("���ϸ� ��ü -> ���� �����ִ� ���ϸ�UI")]
    [SerializeField] private GameObject Change_UI;

    [Header("��Ʋ �� ��, ��ųUI")]
    [SerializeField] private Text[] skill_txt;
    [SerializeField] private Text[] skill_pp_txt;
    [SerializeField] private Image[] skill_img;

    [Header("���� -> ����UI")]
    [SerializeField] private GameObject Bag_UI;

    [Header("��ų�Ӽ� �̹����� �־��ּ���")]
    [SerializeField] private Sprite[] propertyType_skill_img;


    private void OnEnable()
    {
        Debug.Log("��?");
        UI_stack.Push(Default_UI);
        Debug.Log(UI_stack.Peek());

    }

    private void OnDisable()
    {
        UI_stack.Clear();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && UI_stack.Peek() != Default_UI)
        {
            if (!HP_UI.activeSelf)
            {
                HP_UI.SetActive(true);
            }

            GameObject topUI = UI_stack.Peek();
            topUI.SetActive(false);
            UI_stack.Pop();

            GameObject nextUI = UI_stack.Peek();
            nextUI.SetActive(true);
        }
    }


    public void UI_Fight()
    {

        Default_UI.SetActive(false);
        Skill_UI.SetActive(true);

        UI_stack.Push(Skill_UI);

        Debug.Log(UI_stack.Peek());
        PokemonStats pokemonskills = BattleManager.instance.playerPokemon.GetComponent<PokemonStats>();

        //�Ӽ��̶� �̸� �ľ��ؼ� �ְ� pp�� �ֱ�
        TypeCheck_propertyType(pokemonskills);

    }

    public void UI_Change()
    {
        HP_UI.SetActive(false);
        Default_UI.SetActive(false);
        Change_UI.SetActive(true);

        UI_stack.Push(Change_UI);
    }

    public void UI_Bag()
    {

    }
    public void UI_Run()
    {

    }

    void TypeCheck_propertyType(PokemonStats pokemonskills)
    {
        #region �Ӽ�ã��~
        for (int i = 0; i < pokemonskills.skills.Count; i++)
        {
            skill_txt[i].text = pokemonskills.skills[i].Name;
            skill_pp_txt[i].text = pokemonskills.skills[i].PP + "/" + pokemonskills.skills[i].MaxPP;

            switch ((int)pokemonskills.skills[i].propertyType)
            {
                case 0:
                    skill_img[i].sprite = propertyType_skill_img[0];
                    break;
                case 1:
                    skill_img[i].sprite = propertyType_skill_img[1];
                    break;
                case 2:
                    skill_img[i].sprite = propertyType_skill_img[2];
                    break;
                case 3:
                    skill_img[i].sprite = propertyType_skill_img[3];
                    break;
                case 4:
                    skill_img[i].sprite = propertyType_skill_img[4];
                    break;
                case 5:
                    skill_img[i].sprite = propertyType_skill_img[5];
                    break;
                case 6:
                    skill_img[i].sprite = propertyType_skill_img[6];
                    break;
                case 7:
                    skill_img[i].sprite = propertyType_skill_img[7];
                    break;
                case 8:
                    skill_img[i].sprite = propertyType_skill_img[8];
                    break;
                case 9:
                    skill_img[i].sprite = propertyType_skill_img[9];
                    break;
                case 10:
                    skill_img[i].sprite = propertyType_skill_img[10];
                    break;
                case 11:
                    skill_img[i].sprite = propertyType_skill_img[11];
                    break;
                case 12:
                    skill_img[i].sprite = propertyType_skill_img[12];
                    break;
                case 13:
                    skill_img[i].sprite = propertyType_skill_img[13];
                    break;
                case 14:
                    skill_img[i].sprite = propertyType_skill_img[14];
                    break;
                case 15:
                    skill_img[i].sprite = propertyType_skill_img[15];
                    break;
                case 16:
                    skill_img[i].sprite = propertyType_skill_img[16];
                    break;
                default:
                    skill_img[i].sprite = propertyType_skill_img[0];
                    break;
            }
        }
        #endregion
    }
}
