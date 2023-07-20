using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManger : MonoBehaviour
{

    public Stack<GameObject> UI_stack;
    [Header("스킬속성 이미지를 넣어주세요")]
    [SerializeField] private Sprite[] propertyType_skill_img;
    [Header("포켓몬 속성 이미지를 넣어주세요")]
    [SerializeField] private Sprite[] propertyType_pokemon_img;
    [Header("이동하는 버튼 이미지")]
    [SerializeField] private RectTransform selectImage_battle;
    [SerializeField] private RectTransform selectImage_main;


    [SerializeField] private PokemonStats playerpokemon;
    public int currentIndex;
    public int beforeIndex;
    [SerializeField] private int Item_index;
    [SerializeField] private int pokemon_index;
    [SerializeField] private Button[] buttons;
    private PlayerBag playerBag;
    private PlayerMovement playermovement;


    [Header("메인메뉴 UI")]
    [Space(50f)]
    [SerializeField] private GameObject Main_UI;
    [SerializeField] private GameObject MainUI_Default_UI;
    public bool main_bool = true;

    [Header("메인메뉴 기본 포켓몬UI")]
    [Space(20f)]
    [SerializeField] private Text[] mainUI_pokemon_name_txt;
    [SerializeField] private Text[] mainUI_change_pokemon_Lv_txt;
    [SerializeField] private Image[] mainUI_change_pokemon_img;
    [SerializeField] private Slider[] mainUI_change_pokemon_hPbar;
    [SerializeField] private Text[] mainUI_change_pokemon_hp_txt;
    [SerializeField] private GameObject mainUI_pokemon_menu;

    [Header("메인메뉴 가방UI")]
    [Space(20f)]
    [SerializeField] private GameObject mainUI_Bag_UI;
    [SerializeField] private Image[] mainUI_bag_pokemon_img;
    [SerializeField] private Slider[] mainUI_bag_pokemon_hPbar;
    [SerializeField] private Text[] mainUI_bag_pokemon_hp_txt;
    [SerializeField] private GameObject mainUI_Menu_Choise_UI;
    [SerializeField] private GameObject[] mainUI_Item;
    [SerializeField] private Text mainUI_Item_name;
    [SerializeField] private Text mainUI_Item_explanation;
    [SerializeField] private ScrollRect mainUI_scrollRect;

    [Header("박스UI")]
    [Space(20f)]
    [SerializeField] private GameObject Box_UI;
    [SerializeField] private Image[] BoxUI_pokemon_img;
    [SerializeField] private Text[] BoxUI_pokemon_name_txt;
    [SerializeField] private Text[] BoxUI_change_pokemon_Lv_txt;

    [SerializeField] private Image[] BoxUI_inbox_pokemon_img;
    [SerializeField] private GameObject Box_UI_Choise_UI;
    [SerializeField] private GameObject choise_pokemon;
    [SerializeField] private bool choise_pokemon_move;

    [Header("배틀메뉴 UI")]
    [Space(50f)]
    [SerializeField] private GameObject Battle_UI;
    [HideInInspector] public bool isBattle;


    [Header("0. 볼UI")]
    [Space(20f)]
    [SerializeField] private GameObject ball_image;
    [SerializeField] private GameObject ball_button;
    [SerializeField] private GameObject Ball_UI;
    [SerializeField] private Text ball_number_txt;


    [Header("0. 기본UI")]
    [Space(20f)]
    [SerializeField] private GameObject Default_UI;

    [Header("HP관련 UI")]
    [SerializeField] private GameObject HP_UI;
    [SerializeField] private Text hp_txt;
    public Slider hPbar;

    [Header("기본UI 포켓몬 이름")]
    [SerializeField] private Text pokemon_name;
    [Header("기본UI 포켓몬 레벨")]
    [SerializeField] private Text pokemon_lv;
    [Header("기본UI 소지한 포켓몬")]
    [SerializeField] private Image[] pokemon_ball;
    [SerializeField] private Sprite alive_ball_sprite;
    [SerializeField] private Sprite die_ball_sprite;


    [Header("1. 스킬 UI")]
    [Space(20f)]
    [SerializeField] private GameObject Skill_UI;
    [Header("배틀 할 때,현재 스킬UI")]
    [SerializeField] private Text[] Inbattle_skill_txt;
    [SerializeField] private Text[] Inbattle_skill_pp_txt;
    [SerializeField] private Image[] Inbattle_skill_img;


    [Header("2. 포켓몬 교체UI")]
    [Space(20f)]
    [SerializeField] private GameObject Change_UI;
    [Header("포켓몬 선택시 메뉴")]
    [SerializeField] private GameObject Menu_UI;
    [Header("교체 가능한 포켓몬UI")]
    [SerializeField] private Text[] change_pokemon_name_txt;
    [SerializeField] private Text[] change_pokemon_Lv_txt;
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


    [Header("3. 가방UI")]
    [Space(20f)]
    [SerializeField] private GameObject Bag_UI;
    [SerializeField] private Image[] bag_pokemon_img;
    [SerializeField] private Slider[] bag_pokemon_hPbar;
    [SerializeField] private Text[] bag_pokemon_hp_txt;
    [SerializeField] private GameObject Menu_Choise_UI;
    [SerializeField] private GameObject Item_prefab;
    [SerializeField] private GameObject[] Item;
    [SerializeField] private Text Item_name;
    [SerializeField] private Text Item_explanation;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private bool pokemon_choise;

    [Header("스탯 UI")]
    [SerializeField] private GameObject Status_UI;
    [Header("포켓몬 정보")]
    [SerializeField] private Text status_pokemon_name_txt;
    [SerializeField] private Image[] status_pokemon_img;
    [Header("포켓몬 스텟 정보")]
    [SerializeField] private Text status_pokemon_Lv_txt;
    [SerializeField] private Text status_pokemon_in_name_txt;
    [SerializeField] private Text status_pokemon_hp_txt;
    [SerializeField] private Text status_pokemon_spatk_txt;
    [SerializeField] private Text status_pokemon_spdef_txt;
    [SerializeField] private Text status_pokemon_speed_txt;
    [SerializeField] private Text status_pokemon_atk_txt;
    [SerializeField] private Text status_pokemon_def_txt;
    [SerializeField] private Image status_pokemon_type_img;
    [SerializeField] private Image status_pokemon_type2_img;
    [Header("스탯UI의 스킬")]
    [SerializeField] private Text[] status_skill_txt;
    [SerializeField] private Text[] status_skill_pp_txt;
    [SerializeField] private Image[] status_skill_img;

    [Header("박스 안의 스텟 UI")]
    [SerializeField] private GameObject inbox_Status_UI;
    [SerializeField] private Text inbox_status_pokemon_Lv_txt;
    [SerializeField] private Text inbox_status_pokemon_name_txt;
    [SerializeField] private Text inbox_status_pokemon_hp_txt;
    [SerializeField] private Text inbox_status_pokemon_spatk_txt;
    [SerializeField] private Text inbox_status_pokemon_spdef_txt;
    [SerializeField] private Text inbox_status_pokemon_speed_txt;
    [SerializeField] private Text inbox_status_pokemon_atk_txt;
    [SerializeField] private Text inbox_status_pokemon_def_txt;
    [SerializeField] private Image inbox_status_pokemon_type_img;
    [SerializeField] private Image inbox_status_pokemon_type2_img;
    [Header("박스 안에있는 스탯UI의 스킬")]
    [SerializeField] private Text[] inbox_status_skill_txt;
    [SerializeField] private Image[] inbox_status_skill_img;

    private void Start()
    {
        playerBag = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBag>();
        playermovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playermovement.ismove = true;
    }


    private void Update()
    {
        Button[] currentbuttons;

        #region 배틀UI
        if (Battle_UI.activeSelf)
        {
            if (UI_stack == null)
            {
                UI_stack = new Stack<GameObject>();
            }

            if (!isBattle)
            {
                isBattle = true;
                if (Main_UI.activeSelf)
                {
                    Main_UI.SetActive(false);
                }

                //스택 다시 정리
                Reset_BattleUI();
            }

            //현재 플레이어 포켓몬의 상태 확인와 현재 UI 버튼 객수 상태 확인
            playerpokemon = BattleManager.instance.playerPokemon.GetComponent<PokemonStats>();

            //현재 플레이어 포켓몬 볼 이미지로 체크
            Current_Playerpokemon_Check(playerpokemon);

            //현재 UI의 버튼 

            if (pokemon_choise)
            {
                currentbuttons = UI_stack.Peek().transform.GetChild(2).GetComponentsInChildren<Button>();
            }
            else if (UI_stack.Peek() == Bag_UI)
            {
                currentbuttons = UI_stack.Peek().transform.GetChild(1).GetComponentsInChildren<Button>();
            }
            else
            {
                currentbuttons = UI_stack.Peek().GetComponentsInChildren<Button>();
            }

            //현재 스택에 있는 UI의 버튼들을 선택가능
            buttons = currentbuttons;
            buttons[currentIndex].Select();

            Debug.Log(UI_stack.Peek());

            //버튼 이미지 이동
            OnButtonSelected(buttons[currentIndex], selectImage_battle);

            //위 아래로 이동 가능
            BattleButton_Move(buttons);

            //Change_UI, Bag_UI일때, 행동들
            UI_Page(currentbuttons);

            //포켓몬 볼 UI 들어가기
            BallUI_Input();

            if (Input.GetKeyDown(KeyCode.Escape)
                && UI_stack.Peek() != Default_UI
                && (!playerpokemon.isDie || (playerpokemon.isDie && (UI_stack.Peek() == Status_UI))))
            {
                //나가기
                ExitButton();
                //isBattle = false;
            }
        }
        #endregion
        #region 메인UI
        else if (main_bool)
        {

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                playermovement.ismove = false;
                Main_UI.SetActive(true);
                currentIndex = 0;
                UI_stack = new Stack<GameObject>();
                UI_stack.Push(MainUI_Default_UI);
                mainUI_Item = new GameObject[playerBag.Itemdata.Length];
            }
            else if (Main_UI.activeSelf)
            {
                //if (!main_bool)
                //{
                //    main_bool = true;
                //    Reset_UI();

                //}


                MainUI_pokemon();


                if (pokemon_choise)
                {
                    currentbuttons = UI_stack.Peek().transform.GetChild(2).GetComponentsInChildren<Button>();
                }
                else if (UI_stack.Peek() == mainUI_Bag_UI)
                {
                    currentbuttons = UI_stack.Peek().transform.GetChild(1).GetComponentsInChildren<Button>();
                }
                else
                {
                    currentbuttons = UI_stack.Peek().GetComponentsInChildren<Button>();
                }


                //현재 스택에 있는 UI의 버튼들을 선택가능
                buttons = currentbuttons;
                buttons[currentIndex].Select();


                //버튼 이미지 이동
                OnButtonSelected(buttons[currentIndex], selectImage_main);

                //위 아래로 이동 가능
                MainButton_Move(buttons);

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (UI_stack.Peek() == MainUI_Default_UI)
                    {
                        UI_stack = new Stack<GameObject>();
                        choise_pokemon_move = false;
                        playermovement.ismove = true;
                        main_bool = true;
                        Main_UI.SetActive(false);
                    }
                    else
                    {
                        ExitButton();
                    }
                }
            }
        }
        #endregion
    }

    //실시간 체크
    #region 실시간 체크

    //현재 전투중인 플레이어 포켓몬 체크
    void Current_Playerpokemon_Check(PokemonStats playerpokemon)
    {
        pokemon_name.text = playerpokemon.Name;
        pokemon_lv.text = "Lv." + playerpokemon.Level;
        hp_txt.text = playerpokemon.Hp + "/" + playerpokemon.MaxHp;
        hPbar.value = (float)playerpokemon.Hp / playerpokemon.MaxHp;

        Hpbar_Color(this.hPbar);
    }

    //현재 포켓몬 교체 페이지에서 확인할 창 체크
    void UI_Page(Button[] currentbuttons)
    {
        if (UI_stack.Peek() == Change_UI)
        {
            for (int i = 0; i < currentbuttons.Length; i++)
            {
                if (currentbuttons[i].gameObject == EventSystem.current.currentSelectedGameObject)
                {
                    PokemonStats pokemon = playerBag.PlayerPokemon[i].GetComponent<PokemonStats>();

                    //포켓몬 타입 확인
                    PokemonTypeCheck(pokemon, change_pokemon_type1_img, change_pokemon_type2_img);
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
        else if (UI_stack.Peek() == Bag_UI)
        {
            if (pokemon_choise)
            {
                Item_name.text = playerBag.Itemdata[Item_index].Name;
                Item_explanation.text = playerBag.Itemdata[Item_index].Explanation;
            }
            else
            {
                Item_name.text = playerBag.Itemdata[currentIndex].Name;
                Item_explanation.text = playerBag.Itemdata[currentIndex].Explanation;
            }
        }
    }

    //현재 남아있는 플레이어의 포켓몬 숫자를 몬스터볼 이미지로 판단
    void Current_pokemon_ball()
    {
        for (int i = 0; i < 6; i++)
        {
            if (playerBag.PlayerPokemon[i] == null)
            {
                pokemon_ball[i].gameObject.SetActive(false);
            }
            else if (playerBag.PlayerPokemon[i].GetComponent<PokemonStats>().Hp <= 0)
            {
                //만약 꺼져있으면 키고
                if (!pokemon_ball[i].gameObject.activeSelf)
                {
                    pokemon_ball[i].gameObject.SetActive(true);
                }

                pokemon_ball[i].sprite = die_ball_sprite;
            }
            else if (playerBag.PlayerPokemon[i].GetComponent<PokemonStats>().Hp > 0)
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

    #endregion


    //버튼 이동
    #region 버튼이동

    void BallUI_Input()
    {
        if (UI_stack.Peek() == Default_UI)
        {

            if (Input.GetKeyDown(KeyCode.X) && BattleManager.instance.enemyPokemon.GetComponent<PokemonBattleMode>().isWild)
            {
                currentIndex = 0;
                ball_button.SetActive(true);
                UI_stack.Push(ball_button);
                ball_number_txt.text = playerBag.ball.Quantity.ToString();
            }
        }


    }

    //배틀UI 움직임
    void BattleButton_Move(Button[] buttons)
    {
        if (UI_stack.Peek() == Bag_UI)
        {

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = buttons.Length - 1;
                    Scroll(scrollRect, -10f);
                }


                if (currentIndex <= 1)
                {
                    Scroll(scrollRect, 0.35f);
                }

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex++;
                if (currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                    Scroll(scrollRect, 10f);
                }


                // 현재 currentIndex가 10보다 크면 스크롤을 아래로 이동
                if (currentIndex >= 10)
                {
                    Scroll(scrollRect, -0.35f);
                }


            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = buttons.Length - 1;
                }

                if (UI_stack.Peek() == Status_UI)
                {
                    Status_UI_Update();
                }

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex++;
                if (currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                }

                if (UI_stack.Peek() == Status_UI)
                {
                    Status_UI_Update();
                }

            }
        }
    }

    //메인UI 움직임
    void MainButton_Move(Button[] buttons)
    {

        //메인 일 때
        if (UI_stack.Peek() == MainUI_Default_UI)
        {
            if (choise_pokemon_move)
            {
                buttons = UI_stack.Peek().transform.GetChild(1).transform.GetComponentsInChildren<Button>();

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        currentIndex = buttons.Length - 1;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    currentIndex++;
                    if (currentIndex >= buttons.Length)
                    {
                        currentIndex = 0;
                    }
                }
            }
            else
            {
                Image select_pokemon_img = selectImage_main.gameObject.transform.GetChild(0).GetComponent<Image>();
                select_pokemon_img.gameObject.SetActive(false);


                int subtractnum = UI_stack.Peek().transform.GetChild(1).GetComponentsInChildren<Button>().Length;

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        currentIndex = buttons.Length - 1;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    currentIndex++;
                    if (currentIndex >= buttons.Length)
                    {
                        currentIndex = 0;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {

                    if (currentIndex < subtractnum)
                    {
                        currentIndex += subtractnum;

                        if (currentIndex >= buttons.Length)
                        {
                            currentIndex = buttons.Length - 1;
                        }
                    }
                    else if (currentIndex >= subtractnum)
                    {
                        currentIndex -= subtractnum;

                        if (currentIndex >= buttons.Length)
                        {
                            currentIndex = buttons.Length - 1;
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {

                    if (currentIndex < subtractnum)
                    {
                        currentIndex += subtractnum;

                        if (currentIndex >= buttons.Length)
                        {
                            currentIndex = buttons.Length - 1;
                        }
                    }
                    else if (currentIndex >= subtractnum)
                    {
                        currentIndex -= subtractnum;

                        if (currentIndex >= buttons.Length)
                        {
                            currentIndex = buttons.Length - 1;
                        }
                    }
                }
            }

        }

        //가방 일 때
        else if (UI_stack.Peek() == mainUI_Bag_UI)
        {
            //체크
            if (pokemon_choise)
            {
                mainUI_Item_name.text = playerBag.Itemdata[Item_index].Name;
                mainUI_Item_explanation.text = playerBag.Itemdata[Item_index].Explanation;
            }
            else
            {
                mainUI_Item_name.text = playerBag.Itemdata[currentIndex].Name;
                mainUI_Item_explanation.text = playerBag.Itemdata[currentIndex].Explanation;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = buttons.Length - 1;
                    Scroll(mainUI_scrollRect, -10f);
                }


                if (currentIndex <= 1)
                {
                    Scroll(mainUI_scrollRect, 0.35f);
                }

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex++;
                if (currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                    Scroll(mainUI_scrollRect, 10f);
                }


                // 현재 currentIndex가 10보다 크면 스크롤을 아래로 이동
                if (currentIndex >= 10)
                {
                    Scroll(mainUI_scrollRect, -0.35f);
                }


            }
        }

        //박스 일 때
        else if (UI_stack.Peek() == Box_UI)
        {

            if (!choise_pokemon_move)
            {
                Image select_pokemon_img = selectImage_main.gameObject.transform.GetChild(0).GetComponent<Image>();
                select_pokemon_img.gameObject.SetActive(false);
            }


            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = buttons.Length - 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex++;
                if (currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                int pokemonNum = 6;
                int boxNum = 5;


                if (currentIndex < pokemonNum)
                {
                    if (currentIndex == 5)
                    {
                        currentIndex += 30;
                    }
                    else
                    {
                        currentIndex += 31;

                    }
                }
                else if (currentIndex >= 6 && currentIndex <= 10)
                {
                    currentIndex -= pokemonNum;
                }
                else if (currentIndex >= pokemonNum)
                {
                    currentIndex -= boxNum;
                }
                else
                {
                    currentIndex = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //포켓몬 세로 숫자
                int pokemonNum = 6;

                //박스의 세로 숫자
                int boxNum = 5;

                //총 갯수는 36개인데
                //6이 넘으면 +5

                if (currentIndex > 30 && currentIndex <= 36)
                {
                    currentIndex -= 31;
                }
                else if (currentIndex < pokemonNum)
                {
                    if (currentIndex == 5)
                    {
                        currentIndex += 5;
                    }
                    else
                    {
                        currentIndex += pokemonNum;

                    }
                }
                else if (currentIndex >= pokemonNum)
                {
                    currentIndex += boxNum;
                }
            }
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = buttons.Length - 1;
                }

                if (UI_stack.Peek() == Status_UI)
                {
                    Status_UI_Update();
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex++;
                if (currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                }

                if (UI_stack.Peek() == Status_UI)
                {
                    Status_UI_Update();
                }
            }
        }

    }

    public void OnButtonSelected(Button selectedButton, RectTransform selectimg)
    {
        RectTransform selectedButtonRect;

        if (UI_stack.Peek() == Box_UI)
        {
            selectedButtonRect = selectedButton.transform.GetChild(0).GetComponent<RectTransform>();
        }
        else
        {
            selectedButtonRect = selectedButton.GetComponent<RectTransform>();

        }

        Vector3 newPosition = selectedButtonRect.position;
        float offsetX = -(selectedButtonRect.rect.width / 2f); // 버튼의 너비의 절반만큼 왼쪽으로 이동
        float offsetY = selectedButtonRect.rect.center.y; // 버튼의 너비의 절반만큼 왼쪽으로 이동

        newPosition.x += offsetX;
        newPosition.y += offsetY;

        selectimg.position = newPosition;

        StartCoroutine(SelectedMove_co(newPosition, selectimg));
    }

    //좌우로 이동하는 애니메이션
    IEnumerator SelectedMove_co(Vector3 newPosition, RectTransform selectimg)
    {
        float startX = newPosition.x - 10f;
        float endX = newPosition.x + 10f;
        float moveSpeed = 2f;

        Vector3 startPosition = newPosition;

        float t = 0f;
        while (true)
        {
            //이동하면 코루틴 종료
            if (startPosition.y != selectimg.position.y || (selectimg.position.x < startX || selectimg.position.x > endX))
            {
                yield break;
            }

            t += Time.deltaTime * moveSpeed;
            float newX = Mathf.Lerp(startX, endX, Mathf.PingPong(t, 1f));

            Vector3 updatedPosition = newPosition;
            updatedPosition.x = newX;

            selectimg.position = updatedPosition;

            yield return null;
        }
    }
    #endregion

    //속성 찾기 메서드
    #region 속성 찾기
    //스킬 속성 찾기
    void TypeCheck_propertyType(PokemonStats pokemonskills, Text[] skill_txt, Text[] skill_pp_txt, Image[] skill_img)
    {
        #region 속성찾기~
        for (int i = 0; i < pokemonskills.skills.Count; i++)
        {
            skill_txt[i].text = pokemonskills.skills[i].Name;
            if (skill_pp_txt != null)
            {
                skill_pp_txt[i].text = pokemonskills.SkillPP[i] + "/" + pokemonskills.skills[i].MaxPP;
            }

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
    void PokemonTypeCheck(PokemonStats Pokemonstats, Image pokemon_type_img, Image pokemon_type2_img)
    {
        switch ((int)Pokemonstats.Type1)
        {
            case 0:
                pokemon_type_img.sprite = propertyType_pokemon_img[0];
                break;
            case 1:
                pokemon_type_img.sprite = propertyType_pokemon_img[1];
                break;
            case 2:
                pokemon_type_img.sprite = propertyType_pokemon_img[2];
                break;
            case 3:
                pokemon_type_img.sprite = propertyType_pokemon_img[3];
                break;
            case 4:
                pokemon_type_img.sprite = propertyType_pokemon_img[4];
                break;
            case 5:
                pokemon_type_img.sprite = propertyType_pokemon_img[5];
                break;
            case 6:
                pokemon_type_img.sprite = propertyType_pokemon_img[6];
                break;
            case 7:
                pokemon_type_img.sprite = propertyType_pokemon_img[7];
                break;
            case 8:
                pokemon_type_img.sprite = propertyType_pokemon_img[8];
                break;
            case 9:
                pokemon_type_img.sprite = propertyType_pokemon_img[9];
                break;
            case 10:
                pokemon_type_img.sprite = propertyType_pokemon_img[10];
                break;
            case 11:
                pokemon_type_img.sprite = propertyType_pokemon_img[11];
                break;
            case 12:
                pokemon_type_img.sprite = propertyType_pokemon_img[12];
                break;
            case 13:
                pokemon_type_img.sprite = propertyType_pokemon_img[13];
                break;
            case 14:
                pokemon_type_img.sprite = propertyType_pokemon_img[14];
                break;
            case 15:
                pokemon_type_img.sprite = propertyType_pokemon_img[15];
                break;
            case 16:
                pokemon_type_img.sprite = propertyType_pokemon_img[16];
                break;
            case 17:
                pokemon_type_img.sprite = propertyType_pokemon_img[17];
                break;
            default:
                pokemon_type_img.sprite = propertyType_pokemon_img[0];
                break;
        }


        if (Pokemonstats.Type2 != PokemonStats.Type.None)
        {
            pokemon_type2_img.gameObject.SetActive(true);
        }
        switch ((int)Pokemonstats.Type2)
        {
            case 0:
                pokemon_type2_img.sprite = propertyType_pokemon_img[0];
                break;
            case 1:
                pokemon_type2_img.sprite = propertyType_pokemon_img[1];
                break;
            case 2:
                pokemon_type2_img.sprite = propertyType_pokemon_img[2];
                break;
            case 3:
                pokemon_type2_img.sprite = propertyType_pokemon_img[3];
                break;
            case 4:
                pokemon_type2_img.sprite = propertyType_pokemon_img[4];
                break;
            case 5:
                pokemon_type2_img.sprite = propertyType_pokemon_img[5];
                break;
            case 6:
                pokemon_type2_img.sprite = propertyType_pokemon_img[6];
                break;
            case 7:
                pokemon_type2_img.sprite = propertyType_pokemon_img[7];
                break;
            case 8:
                pokemon_type2_img.sprite = propertyType_pokemon_img[8];
                break;
            case 9:
                pokemon_type2_img.sprite = propertyType_pokemon_img[9];
                break;
            case 10:
                pokemon_type2_img.sprite = propertyType_pokemon_img[10];
                break;
            case 11:
                pokemon_type2_img.sprite = propertyType_pokemon_img[11];
                break;
            case 12:
                pokemon_type2_img.sprite = propertyType_pokemon_img[12];
                break;
            case 13:
                pokemon_type2_img.sprite = propertyType_pokemon_img[13];
                break;
            case 14:
                pokemon_type2_img.sprite = propertyType_pokemon_img[14];
                break;
            case 15:
                pokemon_type2_img.sprite = propertyType_pokemon_img[15];
                break;
            case 16:
                pokemon_type2_img.sprite = propertyType_pokemon_img[16];
                break;
            case 17:
                pokemon_type2_img.sprite = propertyType_pokemon_img[17];
                break;
            case 18:
                pokemon_type2_img.gameObject.SetActive(false);
                break;
        }

    }

    #endregion

    //배틀UI 터치 이벤트
    #region 배틀 온 클릭 이벤트


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

        for (int i = 0; i < playerBag.PlayerPokemon.Count; i++)
        {

            //꺼져있으면 다시 킴
            if (!change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.activeSelf)
            {
                change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.SetActive(true);
            }

            //null이면 끔
            if (playerBag.PlayerPokemon[i] == null)
            {
                change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.SetActive(false);
                continue;
            }

            // 포켓몬이 있는 슬롯일 경우
            PokemonStats pokemon = playerBag.PlayerPokemon[i].GetComponent<PokemonStats>();

            change_pokemon_name_txt[i].text = pokemon.Name;
            change_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
            change_pokemon_img[i].color = Color.white;
            change_pokemon_img[i].sprite = pokemon.image;
            change_pokemon_Lv_txt[i].text = "Lv " + pokemon.Level;
            change_pokemon_hPbar[i].value = (float)pokemon.Hp / pokemon.MaxHp;
            Hpbar_Color(change_pokemon_hPbar[i]);
        }
    }
    //포켓몬 사용 메뉴로 이동
    public void UI_Change_Pokemon()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        RectTransform transform = clickedObject.GetComponent<RectTransform>();

        beforeIndex = currentIndex;
        currentIndex = 0;

        Menu_UI.gameObject.SetActive(true);
        Menu_UI.gameObject.transform.position = transform.position + new Vector3(450f, -80f, 0f);

        UI_stack.Push(Menu_UI);
    }
    public void UI_Next_Pokemon()
    {

        if (playerBag.PlayerPokemon[beforeIndex].GetComponent<PokemonStats>().Hp > 0)
        {
            Menu_UI.SetActive(false);
            Change_UI.SetActive(false);
            selectImage_battle.gameObject.SetActive(false);

            BattleManager.instance.player_pokemon_change = true;
        }


    }



    //가방으로 이동
    public void UI_Bag()
    {
        if (Battle_UI.activeSelf)
        {
            HP_UI.SetActive(false);
            Default_UI.SetActive(false);
            Bag_UI.SetActive(true);
        }

        for (int i = 0; i < playerBag.Itemdata.Length; i++)
        {

            if (playerBag.Itemdata[i].Quantity == 0)
            {
                if (Item[i].activeSelf)
                {
                    Item[i].SetActive(false);
                }
                continue;
            }
            else if (Item[i] == null)
            {
                Debug.Log("아이템 생성");
                Item[i] = Instantiate(Item_prefab, Bag_UI.transform.Find("Scroll View/Viewport/Content").transform);
                Item[i].transform.GetChild(0).GetComponent<Text>().text = playerBag.Itemdata[i].Name;
                Item[i].transform.GetChild(1).GetComponent<Image>().sprite = playerBag.Itemdata[i].Image;
                Item[i].transform.GetChild(2).GetComponent<Text>().text = "✕ " + playerBag.Itemdata[i].Quantity;
                Item[i].name = "Item" + i;

                Item[i].GetComponent<Button>().onClick.AddListener(UI_Item_Pokemon);

                Debug.Log(Item[i].name);
            }
            else
            {
                Item[i].SetActive(true);
            }



        }

        UI_stack.Push(Bag_UI);
        scrollRect.verticalScrollbar.value = 1f;
        beforeIndex = currentIndex;
        currentIndex = 0;

        for (int i = 0; i < playerBag.PlayerPokemon.Count; i++)
        {

            if (playerBag.PlayerPokemon[i] == null)
            {
                bag_pokemon_img[i].gameObject.transform.parent.gameObject.SetActive(false);
                continue;
            }

            // 포켓몬이 있는 슬롯일 경우
            PokemonStats pokemon = playerBag.PlayerPokemon[i].GetComponent<PokemonStats>();

            bag_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
            //bag_pokemon_img[i].color = Color.white;
            bag_pokemon_img[i].sprite = pokemon.image;
            bag_pokemon_hPbar[i].value = (float)pokemon.Hp / pokemon.MaxHp;

            Hpbar_Color(bag_pokemon_hPbar[i]);

        }
    }

    //아이템 사용 메뉴로 이동
    public void UI_Item_Pokemon()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        RectTransform transform = clickedObject.GetComponent<RectTransform>();

        beforeIndex = currentIndex;
        currentIndex = 0;
        Menu_Choise_UI.gameObject.SetActive(true);
        Menu_Choise_UI.gameObject.transform.position = transform.position + new Vector3(300f, -80f, 0f);

        UI_stack.Push(Menu_Choise_UI);
    }

    //아이템 사용
    public void UI_Use_Item()
    {
        pokemon_choise = true;
        Item_index = beforeIndex;
        beforeIndex = 0;

        GameObject topUI = UI_stack.Peek();
        topUI.SetActive(false);
        UI_stack.Pop();

        GameObject nextUI = UI_stack.Peek();
        nextUI.SetActive(true);

        //아이템 사용
        //BattleManager.instance.player_using_Item = true;

        //playerpokemon.Hp += playerBag.Itemdata[beforeIndex].HealingHp;

        //Reset_BattleUI();

        //HP_UI.SetActive(true);
        //Default_UI.SetActive(false);
        //selectImage_battle.gameObject.SetActive(false);
    }

    //아이템 사용할 포켓몬 선택
    public void UI_Use_Pokemon()
    {
        //아이템 사용
        PokemonStats choise_pokemon = playerBag.PlayerPokemon[currentIndex].GetComponent<PokemonStats>();

        Debug.Log("체력: " + choise_pokemon.Hp + "체력회복" + playerBag.Itemdata[Item_index].HealingHp);
        choise_pokemon.Hp += playerBag.Itemdata[Item_index].HealingHp;
        choise_pokemon.Hp += (int)(choise_pokemon.MaxHp * (playerBag.Itemdata[Item_index].HealingHpPercent * 0.01));


        Debug.Log("체력 회복 한 후체력: " + choise_pokemon.Hp);
        Reset_BattleUI();

        HP_UI.SetActive(true);
        Default_UI.SetActive(false);
        selectImage_battle.gameObject.SetActive(false);
        pokemon_choise = false;

        BattleManager.instance.player_using_Item = true;
    }

    //볼 사용
    public void UI_Use_ball()
    {
        if (playerBag.ball.Quantity <= 0)
        {
            Debug.Log("볼이없습니다.");
        }
        else
        {
            BattleManager.instance.ball_throw = true;
            ball_button.SetActive(false);
            Battle_UI.SetActive(false);
            playerBag.ball.Quantity--;
        }

        UI_stack.Clear();
        UI_stack.Push(Default_UI);
        currentIndex = 0;

    }




    //도망가기
    public void UI_Run()
    {
        BattleManager.instance.isRun = true;
        Battle_UI.SetActive(false);
    }

    #endregion

    //메인UI 터치 이벤트
    #region 메인 온 클릭 이벤트


    //기본 창 포켓몬
    void MainUI_pokemon()
    {
        for (int i = 0; i < playerBag.PlayerPokemon.Count; i++)
        {

            //꺼져있으면 다시 킴
            if (!mainUI_change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.activeSelf)
            {
                mainUI_change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.SetActive(true);
            }

            //null이면 끔
            if (playerBag.PlayerPokemon[i] == null)
            {
                mainUI_change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.SetActive(false);
                continue;
            }

            // 포켓몬이 있는 슬롯일 경우
            PokemonStats pokemon = playerBag.PlayerPokemon[i].GetComponent<PokemonStats>();

            mainUI_pokemon_name_txt[i].text = pokemon.Name;
            mainUI_change_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
            mainUI_change_pokemon_img[i].color = Color.white;
            mainUI_change_pokemon_img[i].sprite = pokemon.image;
            mainUI_change_pokemon_Lv_txt[i].text = "Lv " + pokemon.Level;
            mainUI_change_pokemon_hPbar[i].value = (float)pokemon.Hp / pokemon.MaxHp;
            Hpbar_Color(mainUI_change_pokemon_hPbar[i]);
        }
    }

    //포켓몬 선택
    public void MainUI_pokemon_Choise()
    {
        if (choise_pokemon_move)
        {
            Button[] btns = UI_stack.Peek().transform.GetComponentsInChildren<Button>();

            GameObject changed_pokemon_img = btns[currentIndex].gameObject.transform.GetChild(0).GetComponent<GameObject>();
            Image select_pokemon_img = btns[pokemon_index].gameObject.transform.GetChild(0).GetComponent<Image>();


            GameObject Obejct = playerBag.PlayerPokemon[currentIndex];
            playerBag.PlayerPokemon[currentIndex] = playerBag.PlayerPokemon[pokemon_index];
            playerBag.PlayerPokemon[pokemon_index] = Obejct;

            choise_pokemon_move = false;
        }
        else
        {
            GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
            RectTransform transform = clickedObject.GetComponent<RectTransform>();

            beforeIndex = currentIndex;
            currentIndex = 0;

            mainUI_pokemon_menu.transform.position = transform.position + new Vector3(300f, -80f, 0f);
            mainUI_pokemon_menu.SetActive(true);
            UI_stack.Push(mainUI_pokemon_menu);
        }
    }
    public void MainUI_pokemon_Move()
    {
        pokemon_index = beforeIndex;
        currentIndex = beforeIndex;

        GameObject topUI = UI_stack.Peek();
        topUI.SetActive(false);
        UI_stack.Pop();

        GameObject nextUI = UI_stack.Peek();
        nextUI.SetActive(true);

        Button[] btns = nextUI.transform.GetComponentsInChildren<Button>();

        Image select_pokemon_img = selectImage_main.gameObject.transform.GetChild(0).GetComponent<Image>();
        Image choise_pokemon_img = btns[pokemon_index].gameObject.transform.GetChild(0).GetComponent<Image>();


        select_pokemon_img.sprite = choise_pokemon_img.sprite;
        choise_pokemon_img.color = new Color(0, 0, 0, 0);
        select_pokemon_img.gameObject.SetActive(true);

        choise_pokemon_move = true;
    }




    //아이템 메뉴
    public void MainUI_Bag()
    {

        if (Main_UI.activeSelf)
        {
            MainUI_Default_UI.SetActive(false);
            mainUI_Bag_UI.SetActive(true);
        }


        for (int i = 0; i < playerBag.Itemdata.Length; i++)
        {

            if (playerBag.Itemdata[i].Quantity == 0)
            {
                if (mainUI_Item[i].activeSelf)
                {
                    mainUI_Item[i].SetActive(false);
                }
                continue;
            }
            else if (mainUI_Item[i] == null)
            {
                Debug.Log("아이템 생성");
                mainUI_Item[i] = Instantiate(Item_prefab, mainUI_Bag_UI.transform.Find("Scroll View/Viewport/Content").transform);
                mainUI_Item[i].transform.GetChild(0).GetComponent<Text>().text = playerBag.Itemdata[i].Name;
                mainUI_Item[i].transform.GetChild(1).GetComponent<Image>().sprite = playerBag.Itemdata[i].Image;
                mainUI_Item[i].transform.GetChild(2).GetComponent<Text>().text = "✕ " + playerBag.Itemdata[i].Quantity;
                mainUI_Item[i].name = "Item" + i;

                mainUI_Item[i].GetComponent<Button>().onClick.AddListener(MainUI_Item_Pokemon);
            }
            else
            {
                mainUI_Item[i].SetActive(true);
                mainUI_Item[i].transform.GetChild(0).GetComponent<Text>().text = playerBag.Itemdata[i].Name;
                mainUI_Item[i].transform.GetChild(1).GetComponent<Image>().sprite = playerBag.Itemdata[i].Image;
                mainUI_Item[i].transform.GetChild(2).GetComponent<Text>().text = "✕ " + playerBag.Itemdata[i].Quantity;
                mainUI_Item[i].name = "Item" + i;
            }
        }

        UI_stack.Push(mainUI_Bag_UI);
        mainUI_scrollRect.verticalScrollbar.value = 1f;
        beforeIndex = currentIndex;
        currentIndex = 0;

        for (int i = 0; i < playerBag.PlayerPokemon.Count; i++)
        {

            if (playerBag.PlayerPokemon[i] == null)
            {
                mainUI_bag_pokemon_img[i].color = new Color(0, 0, 0, 0);
                mainUI_bag_pokemon_img[i].gameObject.transform.parent.gameObject.SetActive(false);
                continue;
            }

            // 포켓몬이 있는 슬롯일 경우
            PokemonStats pokemon = playerBag.PlayerPokemon[i].GetComponent<PokemonStats>();

            mainUI_bag_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
            mainUI_bag_pokemon_img[i].color = Color.white;
            mainUI_bag_pokemon_img[i].sprite = pokemon.image;
            mainUI_bag_pokemon_hPbar[i].value = (float)pokemon.Hp / pokemon.MaxHp;

            Hpbar_Color(mainUI_bag_pokemon_hPbar[i]);

        }
    }

    //아이템 사용 메뉴로 이동
    public void MainUI_Item_Pokemon()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        RectTransform transform = clickedObject.GetComponent<RectTransform>();

        beforeIndex = currentIndex;
        currentIndex = 0;
        mainUI_Menu_Choise_UI.gameObject.transform.position = transform.position + new Vector3(300f, -80f, 0f);
        mainUI_Menu_Choise_UI.gameObject.SetActive(true);
        UI_stack.Push(mainUI_Menu_Choise_UI);
    }

    public void MainUI_Use_Item()
    {
        pokemon_choise = true;
        Item_index = beforeIndex;
        beforeIndex = 0;

        GameObject topUI = UI_stack.Peek();
        topUI.SetActive(false);
        UI_stack.Pop();

        GameObject nextUI = UI_stack.Peek();
        nextUI.SetActive(true);
    }

    public void MainUI_Use_Pokemon()
    {
        if (playerBag.Itemdata[Item_index].Quantity > 0)
        {
            //아이템 사용
            PokemonStats choise_pokemon = playerBag.PlayerPokemon[currentIndex].GetComponent<PokemonStats>();

            if (choise_pokemon.isDie)
            {
                if (playerBag.Itemdata[Item_index].Name.Contains("기력"))
                {
                    Debug.Log("체력: " + choise_pokemon.Hp + "체력회복" + playerBag.Itemdata[Item_index].HealingHp);
                    choise_pokemon.Hp += playerBag.Itemdata[Item_index].HealingHp;
                    choise_pokemon.Hp += (int)(choise_pokemon.MaxHp * (playerBag.Itemdata[Item_index].HealingHpPercent * 0.01));

                    choise_pokemon.isDie = false;
                    pokemon_choise = false;
                    playerBag.Itemdata[Item_index].Quantity--;
                }
            }
            else if (playerBag.Itemdata[Item_index].Name == "이상한 사탕")
            {
                choise_pokemon.Level++;
                choise_pokemon.LevelUp();

                pokemon_choise = false;
                playerBag.Itemdata[Item_index].Quantity--;
            }
            else
            {

                choise_pokemon.Hp += playerBag.Itemdata[Item_index].HealingHp;
                choise_pokemon.Hp += (int)(choise_pokemon.MaxHp * (playerBag.Itemdata[Item_index].HealingHpPercent * 0.01));
                for (int i = 0; i < choise_pokemon.skills.Count; i++)
                {
                    choise_pokemon.skills[i].PP += playerBag.Itemdata[Item_index].HealingPp;
                    choise_pokemon.skills[i].PP += (int)(choise_pokemon.skills[i].MaxPP * (playerBag.Itemdata[Item_index].HealingPpPercent * 0.01));
                }

                for (int i = 0; i < playerBag.PlayerPokemon.Count; i++)
                {

                    if (playerBag.PlayerPokemon[i] == null)
                    {
                        bag_pokemon_img[i].gameObject.transform.parent.gameObject.SetActive(false);
                        continue;
                    }

                    // 포켓몬이 있는 슬롯일 경우
                    PokemonStats pokemon = playerBag.PlayerPokemon[i].GetComponent<PokemonStats>();

                    mainUI_bag_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
                    mainUI_bag_pokemon_img[i].sprite = pokemon.image;
                    mainUI_bag_pokemon_hPbar[i].value = (float)pokemon.Hp / pokemon.MaxHp;

                    Hpbar_Color(mainUI_bag_pokemon_hPbar[i]);
                }

                pokemon_choise = false;
                playerBag.Itemdata[Item_index].Quantity--;
            }
        }
        //아이템 확인
        for (int i = 0; i < playerBag.Itemdata.Length; i++)
        {

            if (playerBag.Itemdata[i].Quantity == 0)
            {
                if (mainUI_Item[i].activeSelf)
                {
                    mainUI_Item[i].SetActive(false);
                }
                continue;
            }
            else if (mainUI_Item[i] == null)
            {
                Debug.Log("아이템 생성");
                mainUI_Item[i] = Instantiate(Item_prefab, mainUI_Bag_UI.transform.Find("Scroll View/Viewport/Content").transform);
                mainUI_Item[i].transform.GetChild(0).GetComponent<Text>().text = playerBag.Itemdata[i].Name;
                mainUI_Item[i].transform.GetChild(1).GetComponent<Image>().sprite = playerBag.Itemdata[i].Image;
                mainUI_Item[i].transform.GetChild(2).GetComponent<Text>().text = "✕ " + playerBag.Itemdata[i].Quantity;
                mainUI_Item[i].name = "Item" + i;

                mainUI_Item[i].GetComponent<Button>().onClick.AddListener(MainUI_Item_Pokemon);
            }
            else
            {
                mainUI_Item[i].SetActive(true);
                mainUI_Item[i].transform.GetChild(0).GetComponent<Text>().text = playerBag.Itemdata[i].Name;
                mainUI_Item[i].transform.GetChild(1).GetComponent<Image>().sprite = playerBag.Itemdata[i].Image;
                mainUI_Item[i].transform.GetChild(2).GetComponent<Text>().text = "✕ " + playerBag.Itemdata[i].Quantity;
                mainUI_Item[i].name = "Item" + i;
            }
        }

        //포켓몬 체력 확인
        for (int i = 0; i < playerBag.PlayerPokemon.Count; i++)
        {

            if (playerBag.PlayerPokemon[i] == null)
            {
                bag_pokemon_img[i].gameObject.transform.parent.gameObject.SetActive(false);
                continue;
            }

            // 포켓몬이 있는 슬롯일 경우
            PokemonStats pokemon = playerBag.PlayerPokemon[i].GetComponent<PokemonStats>();

            mainUI_bag_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
            //bag_pokemon_img[i].color = Color.white;
            mainUI_bag_pokemon_img[i].sprite = pokemon.image;
            mainUI_bag_pokemon_hPbar[i].value = (float)pokemon.Hp / pokemon.MaxHp;

            Hpbar_Color(mainUI_bag_pokemon_hPbar[i]);

        }
    }



    //박스 메뉴
    public void MainUI_Box()
    {
        beforeIndex = currentIndex;
        currentIndex = 0;

        Box_UI.SetActive(true);

        UI_stack.Push(Box_UI);

        for (int i = 0; i < playerBag.PokemonBox.Count; i++)
        {
            if (playerBag.PokemonBox[i] == null)
            {
                BoxUI_inbox_pokemon_img[i].color = new Color(0, 0, 0, 0);
                //BoxUI_inbox_pokemon_img[i].sprite;
            }
            else
            {
                PokemonStats pokemon = playerBag.PokemonBox[i].GetComponent<PokemonStats>();

                BoxUI_inbox_pokemon_img[i].color = Color.white;
                BoxUI_inbox_pokemon_img[i].sprite = pokemon.image;
            }
        }


        for (int i = 0; i < playerBag.PlayerPokemon.Count; i++)
        {
            //꺼져있으면 다시 킴
            if (!BoxUI_change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.activeSelf)
            {
                BoxUI_change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.SetActive(true);
            }

            //null이면 끔
            if (playerBag.PlayerPokemon[i] == null)
            {
                BoxUI_pokemon_img[i].color = new Color(0, 0, 0, 0);
                BoxUI_pokemon_name_txt[i].text = "";
                BoxUI_change_pokemon_Lv_txt[i].text = "";
            }
            else
            {
                // 포켓몬이 있는 슬롯일 경우
                PokemonStats pokemon = playerBag.PlayerPokemon[i].GetComponent<PokemonStats>();

                BoxUI_pokemon_name_txt[i].text = pokemon.Name;
                BoxUI_pokemon_img[i].color = Color.white;
                BoxUI_pokemon_img[i].sprite = pokemon.image;
                BoxUI_change_pokemon_Lv_txt[i].text = "Lv " + pokemon.Level;
            }
        }
    }

    public void MainUI_Box_Choise()
    {

        if (choise_pokemon_move)
        {
            Button[] btns = UI_stack.Peek().transform.GetComponentsInChildren<Button>();

            GameObject changed_pokemon_img = btns[currentIndex].gameObject.transform.GetChild(0).GetComponent<GameObject>();
            Image select_pokemon_img = btns[pokemon_index].gameObject.transform.GetChild(0).GetComponent<Image>();

            //둘 다 박스에 있는 포켓몬 일 때,
            if (currentIndex >= 6 && pokemon_index >= 6)
            {
                GameObject Obejct = playerBag.PokemonBox[currentIndex - 6];
                playerBag.PokemonBox[currentIndex - 6] = playerBag.PokemonBox[pokemon_index - 6];
                playerBag.PokemonBox[pokemon_index - 6] = Obejct;
            }
            //둘 다 가지고 있는 포켓몬 일 때,
            else if (currentIndex < 6 && pokemon_index < 6)
            {
                GameObject Obejct = playerBag.PlayerPokemon[currentIndex];
                playerBag.PlayerPokemon[currentIndex] = playerBag.PlayerPokemon[pokemon_index];
                playerBag.PlayerPokemon[pokemon_index] = Obejct;
            }
            //바뀔 포켓몬은 박스에 있고, pokemon_index는 가지고 있는 포켓몬 일 때
            else if (currentIndex >= 6 && pokemon_index < 6)
            {
                GameObject Obejct = playerBag.PokemonBox[currentIndex - 6];
                playerBag.PokemonBox[currentIndex - 6] = playerBag.PlayerPokemon[pokemon_index];
                playerBag.PlayerPokemon[pokemon_index] = Obejct;
            }
            //바뀔 포켓몬은 가지고 있고, pokemon_index는 박스에 있는 포켓몬 일 때
            else if (currentIndex < 6 && pokemon_index >= 6)
            {
                GameObject Obejct = playerBag.PlayerPokemon[currentIndex];
                playerBag.PlayerPokemon[currentIndex] = playerBag.PokemonBox[pokemon_index - 6];
                playerBag.PokemonBox[pokemon_index - 6] = Obejct;
            }


            choise_pokemon_move = false;

            for (int i = 0; i < playerBag.PokemonBox.Count; i++)
            {
                if (playerBag.PokemonBox[i] == null)
                {
                    BoxUI_inbox_pokemon_img[i].color = new Color(0, 0, 0, 0);
                    //BoxUI_inbox_pokemon_img[i].sprite;
                }
                else
                {
                    PokemonStats pokemon = playerBag.PokemonBox[i].GetComponent<PokemonStats>();

                    BoxUI_inbox_pokemon_img[i].color = Color.white;
                    BoxUI_inbox_pokemon_img[i].sprite = pokemon.image;
                }
            }
            for (int i = 0; i < playerBag.PlayerPokemon.Count; i++)
            {
                //꺼져있으면 다시 킴
                if (!BoxUI_change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.activeSelf)
                {
                    BoxUI_change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.SetActive(true);
                }

                //null이면 끔
                if (playerBag.PlayerPokemon[i] == null)
                {
                    for (int j = i; j < playerBag.PlayerPokemon.Count; j++)
                    {
                        //null이 아닌것을 발견했다면?
                        if (playerBag.PlayerPokemon[j] != null)
                        {
                            PokemonStats pokemon = playerBag.PlayerPokemon[j].GetComponent<PokemonStats>();

                            //가져와서 자리변경
                            GameObject Change_pokemon = playerBag.PlayerPokemon[j];
                            playerBag.PlayerPokemon[j] = playerBag.PlayerPokemon[i];
                            playerBag.PlayerPokemon[i] = Change_pokemon;

                            BoxUI_pokemon_name_txt[i].text = pokemon.Name;
                            BoxUI_pokemon_img[i].color = Color.white;
                            BoxUI_pokemon_img[i].sprite = pokemon.image;
                            BoxUI_change_pokemon_Lv_txt[i].text = "Lv " + pokemon.Level;

                            break;
                        }
                        else if (playerBag.PlayerPokemon[j] == null)
                        {
                            BoxUI_pokemon_name_txt[i].text = "";
                            BoxUI_pokemon_img[i].color = new Color(0, 0, 0, 0);
                            BoxUI_change_pokemon_Lv_txt[i].text = "";
                            continue;
                        }
                    }
                }
                else
                {
                    // 포켓몬이 있는 슬롯일 경우
                    PokemonStats pokemon = playerBag.PlayerPokemon[i].GetComponent<PokemonStats>();

                    BoxUI_pokemon_name_txt[i].text = pokemon.Name;
                    BoxUI_pokemon_img[i].color = Color.white;
                    BoxUI_pokemon_img[i].sprite = pokemon.image;
                    BoxUI_change_pokemon_Lv_txt[i].text = "Lv " + pokemon.Level;
                }
            }

        }
        else if (currentIndex >= 6)
        {
            if (playerBag.PokemonBox[currentIndex - 6] == null)
            {
                Debug.Log(playerBag.PokemonBox[currentIndex - 6]);
                return;
            }
            else
            {
                GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
                RectTransform transform = clickedObject.GetComponent<RectTransform>();

                beforeIndex = currentIndex;
                currentIndex = 0;
                Box_UI_Choise_UI.gameObject.SetActive(true);
                Box_UI_Choise_UI.gameObject.transform.position = transform.position + new Vector3(100f, -80f, 0f);

                UI_stack.Push(Box_UI_Choise_UI);
            }
        }
        else
        {
            GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
            RectTransform transform = clickedObject.GetComponent<RectTransform>();

            beforeIndex = currentIndex;
            currentIndex = 0;
            Box_UI_Choise_UI.gameObject.SetActive(true);
            Box_UI_Choise_UI.gameObject.transform.position = transform.position + new Vector3(100f, -80f, 0f);

            UI_stack.Push(Box_UI_Choise_UI);
        }
    }

    public void MainUI_Box_Move()
    {
        pokemon_index = beforeIndex;
        currentIndex = beforeIndex;

        GameObject topUI = UI_stack.Peek();
        topUI.SetActive(false);
        UI_stack.Pop();

        GameObject nextUI = UI_stack.Peek();
        nextUI.SetActive(true);

        Button[] btns = nextUI.transform.GetComponentsInChildren<Button>();

        Image select_pokemon_img = selectImage_main.gameObject.transform.GetChild(0).GetComponent<Image>();
        Image choise_pokemon_img = btns[pokemon_index].gameObject.transform.GetChild(0).GetComponent<Image>();


        select_pokemon_img.sprite = choise_pokemon_img.sprite;
        choise_pokemon_img.color = new Color(0, 0, 0, 0);
        select_pokemon_img.gameObject.SetActive(true);

        choise_pokemon_move = true;
    }

    public void MainUI_Box_Delect()
    {
        if (beforeIndex < 6)
        {
            BoxUI_pokemon_img[beforeIndex].color = new Color(0, 0, 0, 0);
            BoxUI_pokemon_name_txt[beforeIndex].text = "";
            playerBag.PlayerPokemon.RemoveAt(beforeIndex);
            playerBag.PlayerPokemon.Insert(beforeIndex, null);
        }
        else
        {
            BoxUI_inbox_pokemon_img[beforeIndex-6].color = new Color(0, 0, 0, 0);
            playerBag.PokemonBox.RemoveAt(beforeIndex - 6);
            playerBag.PokemonBox.Insert(beforeIndex - 6, null);
        }

        GameObject topUI = UI_stack.Peek();
        topUI.SetActive(false);
        UI_stack.Pop();

        GameObject nextUI = UI_stack.Peek();
        nextUI.SetActive(true);
    }


    //저장
    public void MainUI_Report_Save()
    {
        playerBag.PlayerInfo_Save();
    }


    //게임 나가기
    public void MainUI_GameExit()
    {
        Application.Quit();
    }

    #endregion

    //리셋, 나가기, 체력 확인, 스크롤
    #region 리셋과 나가기 버튼

    //나가기
    public void ExitButton()
    {
        pokemon_choise = false;
        choise_pokemon_move = false;
        currentIndex = beforeIndex;

        if (UI_stack.Peek() == Bag_UI)
        {
            currentIndex = 2;
        }
        else if (UI_stack.Peek() == Change_UI)
        {
            currentIndex = 1;
        }
        else if (UI_stack.Peek() == Box_UI)
        {
            currentIndex = MainUI_Default_UI.GetComponentsInChildren<Button>().Length - 3;
        }


        if (!HP_UI.activeSelf && Battle_UI.activeSelf && !Change_UI.activeSelf)
        {
            HP_UI.SetActive(true);
        }


        GameObject topUI = UI_stack.Peek();
        topUI.SetActive(false);
        UI_stack.Pop();

        if (UI_stack.Peek() == mainUI_Bag_UI)
        {
            currentIndex = buttons.Length - 4;
        }


        GameObject nextUI = UI_stack.Peek();
        nextUI.SetActive(true);
    }

    //리셋
    void Reset_UI()
    {
        UI_stack.Clear();

        currentIndex = 0;

        //스택 다시 이용
        UI_stack.Push(Main_UI);
    }

    public void Reset_BattleUI()
    {
        Item = new GameObject[playerBag.Itemdata.Length];

        beforeIndex = currentIndex;
        currentIndex = 0;


        if (!Default_UI.activeSelf)
        {
            Default_UI.SetActive(true);
        }

        //포켓볼 색깔
        Current_pokemon_ball();

        //체력바 확인
        PokemonStats pokemon = playerBag.PlayerPokemon[0].GetComponent<PokemonStats>();
        hPbar.value = (float)pokemon.Hp / pokemon.MaxHp;
        Hpbar_Color(hPbar);


        //스택 다시 이용
        UI_stack.Push(Default_UI);

        if (BattleManager.instance.enemyPokemon.GetComponent<PokemonBattleMode>().isWild)
        {
            Ball_UI.SetActive(true);
        }
        selectImage_battle.gameObject.SetActive(true);
        Skill_UI.SetActive(false);
        Change_UI.SetActive(false);
        Bag_UI.SetActive(false);
    }

    //가방에서 스크롤
    void Scroll(ScrollRect scrollRect, float offset)
    {
        float scrollbarSize = scrollRect.verticalScrollbar.size;

        // 스크롤 바 크기에 따른 비율 계산
        float ratio = scrollbarSize / 0.6f;

        float scrollRange = offset * ratio;

        float targetPosition = scrollRect.normalizedPosition.y + scrollRange; // 스크롤 위치 설정

        // 유효 범위 내에서 스크롤 위치 제한
        targetPosition = Mathf.Clamp(targetPosition, 0f, 1f);

        // 스크롤 이동
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, targetPosition);
    }

    //체력 확인
    void Hpbar_Color(Slider hpbar)
    {
        if (hpbar.value < 0.3f)
        {
            hpbar.gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.red;
        }
        else if (hpbar.value < 0.6f)
        {
            hpbar.gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            hpbar.gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.green;
        }
    }
    #endregion


    //정보UI
    public void Status_UI_Button()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        clickedObject.transform.parent.gameObject.SetActive(false);
        UI_stack.Pop();

        if (beforeIndex < 6)
        {
            currentIndex = beforeIndex;

            UI_stack.Push(Status_UI);
            Status_UI.SetActive(true);

            Status_UI_Update();

            for (int i = 0; i < playerBag.PlayerPokemon.Count; i++)
            {
                if (playerBag.PlayerPokemon[i] == null)
                {
                    status_pokemon_img[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    status_pokemon_img[i].transform.parent.gameObject.SetActive(true);
                    status_pokemon_img[i].sprite = playerBag.PlayerPokemon[i].GetComponent<PokemonStats>().image;
                }
            }
        }
        else if (beforeIndex >= 6)
        {
            currentIndex = beforeIndex;

            UI_stack.Push(inbox_Status_UI);
            inbox_Status_UI.SetActive(true);

            inbox_Status_UI_Update();
        }
    }

    void Status_UI_Update()
    {
        PokemonStats pokemon = playerBag.PlayerPokemon[currentIndex].GetComponent<PokemonStats>();

        //포켓몬 타입 확인
        PokemonTypeCheck(pokemon, status_pokemon_type_img, status_pokemon_type2_img);
        TypeCheck_propertyType(pokemon, status_skill_txt, status_skill_pp_txt, status_skill_img);

        status_pokemon_in_name_txt.text = $"{pokemon.Name}";
        status_pokemon_name_txt.text = $"{pokemon.Name}";

        status_pokemon_Lv_txt.text = $"LV.{pokemon.Level}";

        status_pokemon_hp_txt.text = $"{pokemon.Hp} / {pokemon.MaxHp}";
        status_pokemon_atk_txt.text = $"{pokemon.Attack}";
        status_pokemon_def_txt.text = $"{pokemon.Defence}";
        status_pokemon_speed_txt.text = $"{pokemon.Speed}";
        status_pokemon_spatk_txt.text = $"{pokemon.SpAttack}";
        status_pokemon_spdef_txt.text = $"{pokemon.SpDefence}";
    }

    void inbox_Status_UI_Update()
    {
        int num = currentIndex - 6;
        currentIndex = 0;

        PokemonStats pokemon = playerBag.PokemonBox[num].GetComponent<PokemonStats>();

        //포켓몬 타입 확인
        PokemonTypeCheck(pokemon, inbox_status_pokemon_type_img, inbox_status_pokemon_type2_img);
        TypeCheck_propertyType(pokemon, inbox_status_skill_txt, null, inbox_status_skill_img);

        inbox_status_pokemon_name_txt.text = $"{pokemon.Name}";
        inbox_status_pokemon_name_txt.text = $"{pokemon.Name}";

        inbox_status_pokemon_Lv_txt.text = $"LV.{pokemon.Level}";

        inbox_status_pokemon_hp_txt.text = $"{pokemon.Hp} / {pokemon.MaxHp}";
        inbox_status_pokemon_atk_txt.text = $"{pokemon.Attack}";
        inbox_status_pokemon_def_txt.text = $"{pokemon.Defence}";
        inbox_status_pokemon_speed_txt.text = $"{pokemon.Speed}";
        inbox_status_pokemon_spatk_txt.text = $"{pokemon.SpAttack}";
        inbox_status_pokemon_spdef_txt.text = $"{pokemon.SpDefence}";
    }
}
