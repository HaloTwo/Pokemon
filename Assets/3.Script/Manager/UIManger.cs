using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManger : MonoBehaviour
{
    private Stack<GameObject> UI_stack = new Stack<GameObject>();
    private PlayerBag playerBag;

    [Header("기본UI")]
    [SerializeField] private GameObject Default_UI;
    [Header("HP관련 UI")]
    [SerializeField] private GameObject HP_UI;
    public Slider hPbar;
    [SerializeField] private Text hp_txt;
    [Header("기본UI 포켓몬 이름")]
    [SerializeField] private Text pokemon_name;
    [Header("기본UI 포켓몬 레벨")]
    [SerializeField] private Text pokemon_lv;
    [Header("기본UI 소지한 포켓몬")]
    [SerializeField] private Image[] pokemon_ball;
    [SerializeField] private Sprite alive_ball_sprite;
    [SerializeField] private Sprite die_ball_sprite;


    [Header("싸운다 -> 스킬 UI")]
    [SerializeField] private GameObject Skill_UI;
    [Header("배틀 할 때, 스킬UI")]
    [SerializeField] private Text[] Inbattle_skill_txt;
    [SerializeField] private Text[] Inbattle_skill_pp_txt;
    [SerializeField] private Image[] Inbattle_skill_img;

    [Header("포켓몬 교체 -> 현재 갖고있는 포켓몬UI")]
    [SerializeField] private GameObject Change_UI;
    [Header("포켓몬 선택시")]
    [SerializeField] private GameObject Menu_UI;
    [Header("교체 가능한 포켓몬UI")]
    [SerializeField] private Text[] change_pokemon_name_txt;
    [SerializeField] private Text[] change_pokemon_Lv_txt;
    [SerializeField] private Image[] change_pokemon_type_img;
    [SerializeField] private Image[] change_pokemon_img;
    [SerializeField] private Slider[] change_pokemon_hPbar;
    [SerializeField] private Text[] change_pokemon_hp_txt;
    [Header("거기서 메인 페이지 UI")]
    [SerializeField] private Text change_pokemon_stats_txt;
    [SerializeField] private Image change_pokemon_type1_img;
    [SerializeField] private Image change_pokemon_type2_img;
    [SerializeField] private Image[] change_pokemon_skill_img;
    [SerializeField] private Text[] change_pokemon_skill_name;
    [SerializeField] private Text[] change_pokemon_skill_pp;


    [Header("가방 -> 가방UI")]
    [SerializeField] private GameObject Bag_UI;
    [SerializeField] private Image[] bag_pokemon_img;
    [SerializeField] private Slider[] bag_pokemon_hPbar;
    [SerializeField] private Text[] bag_pokemon_hp_txt;

    [Header("스킬속성 이미지를 넣어주세요")]
    [SerializeField] private Sprite[] propertyType_skill_img;
    [Header("포켓몬 속성 이미지를 넣어주세요")]
    [SerializeField] private Sprite[] propertyType_pokemon_img;

    private Button[] buttons;
    private int currentIndex;
    private int beforeIndex;

    [SerializeField] private RectTransform selectImage;



    private void OnEnable()
    {
        // 초기 버튼 선택
        playerBag = FindObjectOfType<PlayerBag>();

        //포켓볼 색깔
        Current_pokemon_ball();

        //스택 다시 정리
        Reset_UI();

    }

    private void OnDisable()
    {
        UI_stack.Clear();
    }

    private void Update()
    {
        //현재 플레이어 포켓몬의 상태 확인
        PokemonStats playerpokemon = BattleManager.instance.playerPokemon.GetComponent<PokemonStats>();
        //현재 플레이어 포켓몬 볼 이미지로 체크
        Current_Playerpokemon_Check(playerpokemon);


        //현재 UI의 버튼 
        Button[] currentbuttons = UI_stack.Peek().GetComponentsInChildren<Button>();

        //현재 스택에 있는 UI의 버튼들을 선택가능
        buttons = currentbuttons;
        buttons[currentIndex].Select();

        //버튼 이미지 이동
        OnButtonSelected(buttons[currentIndex]);


        //Change_UI일때, 행동들
        Change_UI_Page(currentbuttons);



        //위 아래로 이동 가능
        UpDownButton();

        //나가기
        ExitButton();

    }


    void Current_Playerpokemon_Check(PokemonStats playerpokemon)
    {
        pokemon_name.text = playerpokemon.Name;
        pokemon_lv.text = "Lv." + playerpokemon.Level;
        hp_txt.text = playerpokemon.Hp + "/" + playerpokemon.MaxHp;
        hPbar.value = (float)playerpokemon.Hp / playerpokemon.MaxHp;
    }
    void Change_UI_Page(Button[] currentbuttons)
    {
        if (UI_stack.Peek() == Change_UI)
        {
            for (int i = 0; i < currentbuttons.Length; i++)
            {
                if (currentbuttons[i].gameObject == EventSystem.current.currentSelectedGameObject)
                {
                    PokemonStats pokemon = playerBag.NowPokemon[i].GetComponent<PokemonStats>();

                    //포켓몬 타입 확인
                    PokemonTypeCheck(pokemon);
                    TypeCheck_propertyType(pokemon, change_pokemon_skill_name, change_pokemon_skill_pp, change_pokemon_skill_img);

                    if (pokemon.Hp > 0)
                    {
                        change_pokemon_stats_txt.text = "싸울 수 있다.";
                    }
                    else
                    {
                        change_pokemon_stats_txt.text = "상태 불능";
                    }
                }
            }
        }
    }

    //이동 입력
    void UpDownButton()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = buttons.Length - 1;
            }
            buttons[currentIndex].Select();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex++;
            if (currentIndex >= buttons.Length)
            {
                currentIndex = 0;
            }
            buttons[currentIndex].Select();
        }
    }

    //나가기 입력
    void ExitButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && UI_stack.Peek() != Default_UI)
        {
            currentIndex = beforeIndex;

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

    //리셋
    public void Reset_UI()
    {
        beforeIndex = currentIndex;
        currentIndex = 0;

        if (!Default_UI.activeSelf)
        {
            Default_UI.SetActive(true);
        }

        UI_stack.Push(Default_UI);

        selectImage.gameObject.SetActive(true);
        Skill_UI.SetActive(false);
        Change_UI.SetActive(false);
        Bag_UI.SetActive(false);
    }

    //현재 남아있는 볼 숫자 세기
    void Current_pokemon_ball()
    {
        for (int i = 0; i < 6; i++)
        {
            if (playerBag.NowPokemon[i] == null)
            {
                pokemon_ball[i].gameObject.SetActive(false);
            }
            else if (playerBag.NowPokemon[i].GetComponent<PokemonStats>().Hp <= 0)
            {
                //만약 꺼져있으면 키고
                if (!pokemon_ball[i].gameObject.activeSelf)
                {
                    pokemon_ball[i].gameObject.SetActive(true);
                }

                pokemon_ball[i].sprite = die_ball_sprite;
            }
            else if (playerBag.NowPokemon[i].GetComponent<PokemonStats>().Hp > 0)
            {
                //만약 꺼져있으면 키고
                if (!pokemon_ball[i].gameObject.activeSelf)
                {
                    pokemon_ball[i].gameObject.SetActive(true);
                }

                pokemon_ball[i].sprite = alive_ball_sprite;
            }
        }
    }

    //입력에 따라 버튼 이동하기
    void OnButtonSelected(Button selectedButton)
    {
        // 선택된 버튼의 RectTransform을 가져옵니다.
        RectTransform selectedButtonRect = selectedButton.GetComponent<RectTransform>();

        //절대값으로 하면 안됌./////////////////////////////
        Vector3 newPosition = selectedButtonRect.position;

        // "Select" 이미지의 위치를 갱신합니다.
        selectImage.position = newPosition;

       // StartCoroutine(SelectedMove_co(newPosition));
    }

    //아직 안됌
    IEnumerator SelectedMove_co(Vector3 newPosition)
    {

        float startX = newPosition.x - 3f;
        float endX = newPosition.x + 3f;

        while (true)
        {
            // 왼쪽으로 이동
            while (selectImage.position.x > startX)
            {
                selectImage.position += Vector3.left * Time.deltaTime;
                yield return new WaitForSeconds(0.5f);
            }

            // 오른쪽으로 이동
            while (selectImage.position.x < endX)
            {
                selectImage.position += Vector3.right * Time.deltaTime;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }


    //속성 찾기 도구
    #region 속성 찾기
    //스킬 속성 찾기
    void TypeCheck_propertyType(PokemonStats pokemonskills, Text[] skill_txt, Text[] skill_pp_txt, Image[] skill_img)
    {
        #region 속성찾기~
        for (int i = 0; i < pokemonskills.skills.Count; i++)
        {
            skill_txt[i].text = pokemonskills.skills[i].Name;
            skill_pp_txt[i].text = pokemonskills.SkillPP[i] + "/" + pokemonskills.skills[i].MaxPP;

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
                case 17:
                    skill_img[i].sprite = propertyType_skill_img[17];
                    break;
                default:
                    skill_img[i].sprite = propertyType_skill_img[0];
                    break;
            }
        }
        #endregion
    }

    //포켓몬 속성 찾기
    void PokemonTypeCheck(PokemonStats Pokemonstats)
    {
        switch ((int)Pokemonstats.Type1)
        {
            case 0:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[0];
                break;
            case 1:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[1];
                break;
            case 2:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[2];
                break;
            case 3:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[3];
                break;
            case 4:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[4];
                break;
            case 5:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[5];
                break;
            case 6:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[6];
                break;
            case 7:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[7];
                break;
            case 8:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[8];
                break;
            case 9:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[9];
                break;
            case 10:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[10];
                break;
            case 11:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[11];
                break;
            case 12:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[12];
                break;
            case 13:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[13];
                break;
            case 14:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[14];
                break;
            case 15:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[15];
                break;
            case 16:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[16];
                break;
            case 17:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[17];
                break;
            default:
                change_pokemon_type1_img.sprite = propertyType_pokemon_img[0];
                break;
        }


        if (Pokemonstats.Type2 != PokemonStats.Type.None)
        {
            change_pokemon_type2_img.gameObject.SetActive(true);
        }
        switch ((int)Pokemonstats.Type2)
        {
            case 0:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[0];
                break;
            case 1:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[1];
                break;
            case 2:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[2];
                break;
            case 3:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[3];
                break;
            case 4:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[4];
                break;
            case 5:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[5];
                break;
            case 6:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[6];
                break;
            case 7:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[7];
                break;
            case 8:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[8];
                break;
            case 9:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[9];
                break;
            case 10:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[10];
                break;
            case 11:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[11];
                break;
            case 12:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[12];
                break;
            case 13:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[13];
                break;
            case 14:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[14];
                break;
            case 15:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[15];
                break;
            case 16:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[16];
                break;
            case 17:
                change_pokemon_type2_img.sprite = propertyType_pokemon_img[17];
                break;
            case 18:
                change_pokemon_type2_img.gameObject.SetActive(false);
                break;
        }

    }

    #endregion 

    //터치 이벤트
    #region 온 클릭 이벤트
    //전투로 이동 , 스킬 사용창
    public void UI_Fight()
    {
        beforeIndex = currentIndex;
        currentIndex = 0;

        Default_UI.SetActive(false);
        Skill_UI.SetActive(true);

        UI_stack.Push(Skill_UI);

        Debug.Log(UI_stack.Peek());
        PokemonStats pokemonskills = BattleManager.instance.playerPokemon.GetComponent<PokemonStats>();

        //속성이랑 이름 파악해서 넣고 pp도 넣기
        TypeCheck_propertyType(pokemonskills, Inbattle_skill_txt, Inbattle_skill_pp_txt, Inbattle_skill_img);

    }

    //포켓몬 교환창으로 이동
    public void UI_Change()
    {
        beforeIndex = currentIndex;
        currentIndex = 0;

        HP_UI.SetActive(false);
        Default_UI.SetActive(false);
        Change_UI.SetActive(true);

        UI_stack.Push(Change_UI);


        for (int i = 0; i < playerBag.NowPokemon.Count; i++)
        {

            //꺼져있으면 다시 킴
            if (!change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.activeSelf)
            {
                change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.SetActive(true);
            }

            //null이면 끔
            if (playerBag.NowPokemon[i] == null)
            {
                change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.SetActive(false);
                continue;
            }

            // 포켓몬이 있는 슬롯일 경우
            PokemonStats pokemon = playerBag.NowPokemon[i].GetComponent<PokemonStats>();

            change_pokemon_name_txt[i].text = pokemon.Name;
            change_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
            change_pokemon_img[i].color = Color.white;
            change_pokemon_img[i].sprite = pokemon.image;
            change_pokemon_Lv_txt[i].text = "Lv " + pokemon.Level;
            change_pokemon_hPbar[i].value = (float)pokemon.Hp / pokemon.MaxHp;
        }
    }

    //가방으로 이동
    public void UI_Bag()
    {
        beforeIndex = currentIndex;
        currentIndex = 0;

        HP_UI.SetActive(false);
        Default_UI.SetActive(false);
        Bag_UI.SetActive(true);

        UI_stack.Push(Bag_UI);


        for (int i = 0; i < playerBag.NowPokemon.Count; i++)
        {

            if (playerBag.NowPokemon[i] == null)
            {
                bag_pokemon_img[i].gameObject.transform.parent.gameObject.SetActive(false);
                continue;
            }

            // 포켓몬이 있는 슬롯일 경우
            PokemonStats pokemon = playerBag.NowPokemon[i].GetComponent<PokemonStats>();

            bag_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
            //bag_pokemon_img[i].color = Color.white;
            bag_pokemon_img[i].sprite = pokemon.image;
            bag_pokemon_hPbar[i].value = (float)pokemon.Hp / pokemon.MaxHp;



        }
    }

    //도망가기
    public void UI_Run()
    {
        BattleManager.instance.isRun = true;
        gameObject.SetActive(false);
    }

    public void UI_Change_Pokemon(RectTransform transform)
    {
        beforeIndex = currentIndex;
        currentIndex = 0;
        Menu_UI.gameObject.SetActive(true);
        Menu_UI.gameObject.transform.position = transform.position + new Vector3(450f, -80f, 0f);

        UI_stack.Push(Menu_UI);
    }

    public void UI_inputExit()
    {
        beforeIndex = currentIndex;
        currentIndex = 0;

        GameObject topUI = UI_stack.Peek();
        topUI.SetActive(false);
        UI_stack.Pop();

        GameObject nextUI = UI_stack.Peek();
        nextUI.SetActive(true);
    }
    #endregion
}
