using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManger : MonoBehaviour
{
    private Stack<GameObject> UI_stack = new Stack<GameObject>();
    private PlayerBag playerBag;
    int MaxPokemon = 6;

    [Header("기본UI")]
    [SerializeField] private GameObject Default_UI;
    [Header("HP관련 UI")]
    [SerializeField] private GameObject HP_UI;
    [SerializeField] private Text pokemon_name;
    [SerializeField] private Slider hPbar;
    [SerializeField] private Text hp_txt;
    [Header("싸운다 -> 스킬 UI")]
    [SerializeField] private GameObject Skill_UI;

    [Header("배틀 할 때, 스킬UI")]
    [SerializeField] private Text[] Inbattle_skill_txt;
    [SerializeField] private Text[] Inbattle_skill_pp_txt;
    [SerializeField] private Image[] Inbattle_skill_img;

    [Header("포켓몬 교체 -> 현재 갖고있는 포켓몬UI")]
    [SerializeField] private GameObject Change_UI;
    [Header("교체 가능한 포켓몬UI")]
    [SerializeField] private Text[] change_pokemon_name_txt;
    [SerializeField] private Text[] change_pokemon_Lv_txt;
    [SerializeField] private Image[] change_pokemon_type_img;
    [SerializeField] private Image[] change_pokemon_img;
    [SerializeField] private Slider[] change_pokemon_hPbar;
    [SerializeField] private Text[] change_pokemon_hp_txt;

    [Header("가방 -> 가방UI")]
    [SerializeField] private GameObject Bag_UI;
    [SerializeField] private Image[] bag_pokemon_img;
    [SerializeField] private Slider[] bag_pokemon_hPbar;
    [SerializeField] private Text[] bag_pokemon_hp_txt;

    [Header("스킬속성 이미지를 넣어주세요")]
    [SerializeField] private Sprite[] propertyType_skill_img;


    private void OnEnable()
    {
        Debug.Log("들어감?");
        UI_stack.Push(Default_UI);
        Debug.Log(UI_stack.Peek());
        playerBag = FindObjectOfType<PlayerBag>();

    }

    private void OnDisable()
    {
        UI_stack.Clear();
    }

    private void Update()
    {
        PokemonStats pokemon = BattleManager.instance.playerPokemon.GetComponent<PokemonStats>();

        pokemon_name.text = pokemon.Name;
        hp_txt.text = pokemon.Hp + "/" + pokemon.MaxHp;


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

        //속성이랑 이름 파악해서 넣고 pp도 넣기
        TypeCheck_propertyType(pokemonskills, Inbattle_skill_txt, Inbattle_skill_pp_txt, Inbattle_skill_img);

    }

    public void UI_Change()
    {
        HP_UI.SetActive(false);
        Default_UI.SetActive(false);
        Change_UI.SetActive(true);

        UI_stack.Push(Change_UI);


        for (int i = 0; i < MaxPokemon; i++)
        {
            if (i < playerBag.NowPokemon.Count)
            {
                // 포켓몬이 있는 슬롯일 경우
                PokemonStats pokemon = playerBag.NowPokemon[i].GetComponent<PokemonStats>();

                change_pokemon_name_txt[i].text = pokemon.Name;
                change_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
                change_pokemon_img[i].color = Color.white;
                change_pokemon_img[i].sprite = pokemon.image;
                change_pokemon_Lv_txt[i].text = "Lv " + pokemon.Level;
            }
            else
            {
                // 포켓몬이 없는 슬롯일 경우
                change_pokemon_name_txt[i].text = "";
                change_pokemon_hp_txt[i].text = "";
                change_pokemon_img[i].color = new Color(1f, 1f, 1f, 0f);
                change_pokemon_Lv_txt[i].text = "";
            }
        }
    }

    public void UI_Bag()
    {
        HP_UI.SetActive(false);
        Default_UI.SetActive(false);
        Bag_UI.SetActive(true);

        UI_stack.Push(Bag_UI);


        for (int i = 0; i < MaxPokemon; i++)
        {
            if (i < playerBag.NowPokemon.Count)
            {
                // 포켓몬이 있는 슬롯일 경우
                PokemonStats pokemon = playerBag.NowPokemon[i].GetComponent<PokemonStats>();

                bag_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
                bag_pokemon_img[i].color = Color.white;
                bag_pokemon_img[i].sprite = pokemon.image;
            }
            else
            {
                // 포켓몬이 없는 슬롯일 경우
                bag_pokemon_hp_txt[i].text = "";
                bag_pokemon_img[i].color = new Color(1f, 1f, 1f, 0f);
            }
        }
    }
    public void UI_Run()
    {
        //gameObject.SetActive(false);
    }

    void TypeCheck_propertyType(PokemonStats pokemonskills, Text[] skill_txt, Text[] skill_pp_txt, Image[] skill_img)
    {
        #region 속성찾기~
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
