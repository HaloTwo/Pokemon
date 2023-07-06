using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManger : MonoBehaviour
{
    private Stack<GameObject> UI_stack = new Stack<GameObject>();
    private PlayerBag playerBag;

    public ScrollRect scrollRect;
    public float scrollSpeed = 100f; // ��ũ�� �ӵ��� �����ϼ���

    [Header("��ų�Ӽ� �̹����� �־��ּ���")]
    [SerializeField] private Sprite[] propertyType_skill_img;
    [Header("���ϸ� �Ӽ� �̹����� �־��ּ���")]
    [SerializeField] private Sprite[] propertyType_pokemon_img;
    [Header("�̵��ϴ� ��ư �̹���")]
    [SerializeField] private RectTransform selectImage;

    [Header("0. �⺻UI")]
    [Space(50f)]
    [SerializeField] private GameObject Default_UI;

    [Header("HP���� UI")]
    [SerializeField] private GameObject HP_UI;
    [SerializeField] private Text hp_txt;
    public Slider hPbar;

    [Header("�⺻UI ���ϸ� �̸�")]
    [SerializeField] private Text pokemon_name;
    [Header("�⺻UI ���ϸ� ����")]
    [SerializeField] private Text pokemon_lv;
    [Header("�⺻UI ������ ���ϸ�")]
    [SerializeField] private Image[] pokemon_ball;
    [SerializeField] private Sprite alive_ball_sprite;
    [SerializeField] private Sprite die_ball_sprite;


    [Header("1. ��ų UI")]
    [Space(50f)]
    [SerializeField] private GameObject Skill_UI;
    [Header("��Ʋ �� ��,���� ��ųUI")]
    [SerializeField] private Text[] Inbattle_skill_txt;
    [SerializeField] private Text[] Inbattle_skill_pp_txt;
    [SerializeField] private Image[] Inbattle_skill_img;

    [Header("2. ���ϸ� ��üUI")]
    [Space(50f)]
    [SerializeField] private GameObject Change_UI;
    [Header("���ϸ� ���ý� �޴�")]
    [SerializeField] private GameObject Menu_UI;
    [Header("��ü ������ ���ϸ�UI")]
    [SerializeField] private Text[] change_pokemon_name_txt;
    [SerializeField] private Text[] change_pokemon_Lv_txt;
    [SerializeField] private Image[] change_pokemon_type_img;
    [SerializeField] private Image[] change_pokemon_img;
    [SerializeField] private Slider[] change_pokemon_hPbar;
    [SerializeField] private Text[] change_pokemon_hp_txt;
    [Header("�ű⼭ ���� ������ UI")]
    [SerializeField] private Text change_pokemon_stats_txt;
    [SerializeField] private Image change_pokemon_type1_img;
    [SerializeField] private Image change_pokemon_type2_img;
    [SerializeField] private Image[] change_pokemon_skill_img;
    [SerializeField] private Text[] change_pokemon_skill_name;
    [SerializeField] private Text[] change_pokemon_skill_pp;


    [Header("3. ����UI")]
    [Space(50f)]
    [SerializeField] private GameObject Bag_UI;
    [SerializeField] private Image[] bag_pokemon_img;
    [SerializeField] private Slider[] bag_pokemon_hPbar;
    [SerializeField] private Text[] bag_pokemon_hp_txt;
    [SerializeField] private GameObject Menu_Choise_UI;
    [SerializeField] private GameObject Item;


    [SerializeField] private Button[] buttons;
    private int currentIndex;
    private int beforeIndex;




    private void OnEnable()
    {
        // �ʱ� ��ư ����
        playerBag = FindObjectOfType<PlayerBag>();

        //���Ϻ� ����
        Current_pokemon_ball();

        //���� �ٽ� ����
        Reset_UI();


        //ü�¹� Ȯ��
        PokemonStats pokemon = playerBag.NowPokemon[0].GetComponent<PokemonStats>();
        hPbar.value = (float)pokemon.Hp / pokemon.MaxHp;
        Hpbar_Color(hPbar);

    }

    private void OnDisable()
    {
        UI_stack.Clear();
    }

    private void Update()
    {

        //���� �÷��̾� ���ϸ��� ���� Ȯ�ο� ���� UI ��ư ���� ���� Ȯ��
        PokemonStats playerpokemon = BattleManager.instance.playerPokemon.GetComponent<PokemonStats>();
        Button[] currentbuttons;
        //���� �÷��̾� ���ϸ� �� �̹����� üũ
        Current_Playerpokemon_Check(playerpokemon);


        //���� UI�� ��ư 
        if (UI_stack.Peek() == Bag_UI)
        {
            currentbuttons = UI_stack.Peek().transform.GetChild(1).GetComponentsInChildren<Button>();
        }
        else
        {
            currentbuttons = UI_stack.Peek().GetComponentsInChildren<Button>();
        }

        //���� ���ÿ� �ִ� UI�� ��ư���� ���ð���
        buttons = currentbuttons;
        buttons[currentIndex].Select();

        //��ư �̹��� �̵�
        OnButtonSelected(buttons[currentIndex]);

        //Change_UI�϶�, �ൿ��
        Change_UI_Page(currentbuttons);

        //�� �Ʒ��� �̵� ����
        UpDownButton();

        if (Input.GetKeyDown(KeyCode.Escape) && UI_stack.Peek() != Default_UI)
        {
            //������
            ExitButton();
        }
    }

    //�ǽð� üũ
    #region �ǽð� üũ

    //���� �������� �÷��̾� ���ϸ� üũ
    void Current_Playerpokemon_Check(PokemonStats playerpokemon)
    {
        pokemon_name.text = playerpokemon.Name;
        pokemon_lv.text = "Lv." + playerpokemon.Level;
        hp_txt.text = playerpokemon.Hp + "/" + playerpokemon.MaxHp;
        hPbar.value = (float)playerpokemon.Hp / playerpokemon.MaxHp;
    }

    //���� ���ϸ� ��ü ���������� Ȯ���� â üũ
    void Change_UI_Page(Button[] currentbuttons)
    {
        if (UI_stack.Peek() == Change_UI)
        {
            for (int i = 0; i < currentbuttons.Length; i++)
            {
                if (currentbuttons[i].gameObject == EventSystem.current.currentSelectedGameObject)
                {
                    PokemonStats pokemon = playerBag.NowPokemon[i].GetComponent<PokemonStats>();

                    //���ϸ� Ÿ�� Ȯ��
                    PokemonTypeCheck(pokemon);
                    TypeCheck_propertyType(pokemon, change_pokemon_skill_name, change_pokemon_skill_pp, change_pokemon_skill_img);

                    if (pokemon.Hp > 0)
                    {
                        change_pokemon_stats_txt.text = "�ο� �� �ִ�.";
                    }
                    else
                    {
                        change_pokemon_stats_txt.text = "���� �Ҵ�";
                    }
                }
            }
        }
    }

    //���� �����ִ� �÷��̾��� ���ϸ� ���ڸ� ���ͺ� �̹����� �Ǵ�
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
                //���� ���������� Ű��
                if (!pokemon_ball[i].gameObject.activeSelf)
                {
                    pokemon_ball[i].gameObject.SetActive(true);
                }

                pokemon_ball[i].sprite = die_ball_sprite;
            }
            else if (playerBag.NowPokemon[i].GetComponent<PokemonStats>().Hp > 0)
            {
                //���� ���������� Ű��
                if (!pokemon_ball[i].gameObject.activeSelf)
                {
                    pokemon_ball[i].gameObject.SetActive(true);
                }

                pokemon_ball[i].sprite = alive_ball_sprite;
            }
        }
    }
    #endregion

    //public void Scroll(float offset)
    //{
    //    // ��ü ���� ��ũ�� ������ ���̸� ����Ͽ� �̵� ���� ���
    //    float scrollRange = Mathf.Max(1f, buttons.Length) * offset;

    //    float targetPosition = scrollRect.normalizedPosition.y + scrollRange; // ��ũ�� ��ġ ����

    //    // �̵� ������ ��ȿ ���� ���� ����
    //    targetPosition = Mathf.Clamp(targetPosition, 0f, 1f);

    //    // ��ũ�� �̵�
    //    scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, targetPosition);
    //}

    public void Scroll(float buttonSize)
    {
        // ��ư�� ������ ��ư�� ũ�⸦ ����Ͽ� �̵� ���� ���
        float scrollRange = Mathf.Max(1f, buttons.Length) * buttonSize;

        float targetPosition = scrollRect.normalizedPosition.y + scrollRange; // ��ũ�� ��ġ ����

        // �̵� ������ ��ȿ ���� ���� ����
        targetPosition = Mathf.Clamp(targetPosition, 0f, 1f);

        // ��ũ�� �̵�
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, targetPosition);
    }




    //��ư �̵�
    #region ��ư�̵�
    void UpDownButton()
    {
        if (UI_stack.Peek() == Bag_UI)
        {

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = buttons.Length - 1;
                    Scroll(-10f);
                }
                buttons[currentIndex].Select();

                if (currentIndex < 6)
                {
                    Scroll(0.17f);
                }

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex++;
                if (currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                    Scroll(10f);
                }
                buttons[currentIndex].Select();

                // ���� currentIndex�� 7���� ũ�� ��ũ���� �Ʒ��� �̵�
                if (currentIndex >= 10)
                {
                    Scroll(-0.17f);
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
    }

    void OnButtonSelected(Button selectedButton)
    {
        RectTransform selectedButtonRect = selectedButton.GetComponent<RectTransform>();

        Vector3 newPosition = selectedButtonRect.position;
        float offsetX = -(selectedButtonRect.rect.width / 2f); // ��ư�� �ʺ��� ���ݸ�ŭ �������� �̵�

        newPosition.x += offsetX;

        selectImage.position = newPosition;

        StartCoroutine(SelectedMove_co(newPosition));
    }

    //�¿�� �̵��ϴ� �ִϸ��̼�
    IEnumerator SelectedMove_co(Vector3 newPosition)
    {
        float startX = newPosition.x - 10f;
        float endX = newPosition.x + 10f;
        float moveSpeed = 2f;

        Vector3 startPosition = newPosition;

        float t = 0f;
        while (true)
        {
            //�̵��ϸ� �ڷ�ƾ ����
            if (startPosition.y != selectImage.position.y)
            {
                yield break;
            }

            t += Time.deltaTime * moveSpeed;
            float newX = Mathf.Lerp(startX, endX, Mathf.PingPong(t, 1f));

            Vector3 updatedPosition = newPosition;
            updatedPosition.x = newX;

            selectImage.position = updatedPosition;

            yield return null;
        }
    }
    #endregion


    //�Ӽ� ã�� �޼���
    #region �Ӽ� ã��
    //��ų �Ӽ� ã��
    void TypeCheck_propertyType(PokemonStats pokemonskills, Text[] skill_txt, Text[] skill_pp_txt, Image[] skill_img)
    {
        #region �Ӽ�ã��~
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

    //���ϸ� �Ӽ� ã��
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

    //��ġ �̺�Ʈ
    #region �� Ŭ�� �̺�Ʈ
    //������ �̵� , ��ų ���â
    public void UI_Fight()
    {
        beforeIndex = currentIndex;
        currentIndex = 0;

        Default_UI.SetActive(false);
        Skill_UI.SetActive(true);

        UI_stack.Push(Skill_UI);

        Debug.Log(UI_stack.Peek());
        PokemonStats pokemonskills = BattleManager.instance.playerPokemon.GetComponent<PokemonStats>();

        //�Ӽ��̶� �̸� �ľ��ؼ� �ְ� pp�� �ֱ�
        TypeCheck_propertyType(pokemonskills, Inbattle_skill_txt, Inbattle_skill_pp_txt, Inbattle_skill_img);

    }

    //���ϸ� ��ȯâ���� �̵�
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

            //���������� �ٽ� Ŵ
            if (!change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.activeSelf)
            {
                change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.SetActive(true);
            }

            //null�̸� ��
            if (playerBag.NowPokemon[i] == null)
            {
                change_pokemon_Lv_txt[i].gameObject.transform.parent.gameObject.SetActive(false);
                continue;
            }

            // ���ϸ��� �ִ� ������ ���
            PokemonStats pokemon = playerBag.NowPokemon[i].GetComponent<PokemonStats>();

            change_pokemon_name_txt[i].text = pokemon.Name;
            change_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
            change_pokemon_img[i].color = Color.white;
            change_pokemon_img[i].sprite = pokemon.image;
            change_pokemon_Lv_txt[i].text = "Lv " + pokemon.Level;
            change_pokemon_hPbar[i].value = (float)pokemon.Hp / pokemon.MaxHp;
            Hpbar_Color(change_pokemon_hPbar[i]);
        }
    }

    //�������� �̵�
    public void UI_Bag()
    {
        beforeIndex = currentIndex;
        currentIndex = 0;

        HP_UI.SetActive(false);
        Default_UI.SetActive(false);
        Bag_UI.SetActive(true);

        UI_stack.Push(Bag_UI);


        Instantiate(Item, Bag_UI.transform.Find("Scroll View/Viewport/Content").transform);



        for (int i = 0; i < playerBag.NowPokemon.Count; i++)
        {

            if (playerBag.NowPokemon[i] == null)
            {
                bag_pokemon_img[i].gameObject.transform.parent.gameObject.SetActive(false);
                continue;
            }

            // ���ϸ��� �ִ� ������ ���
            PokemonStats pokemon = playerBag.NowPokemon[i].GetComponent<PokemonStats>();

            bag_pokemon_hp_txt[i].text = pokemon.Hp + "/" + pokemon.MaxHp;
            //bag_pokemon_img[i].color = Color.white;
            bag_pokemon_img[i].sprite = pokemon.image;
            bag_pokemon_hPbar[i].value = (float)pokemon.Hp / pokemon.MaxHp;

            Hpbar_Color(bag_pokemon_hPbar[i]);

        }
    }


    //��������
    public void UI_Run()
    {
        BattleManager.instance.isRun = true;
        gameObject.SetActive(false);
    }

    //���ϸ� ��� �޴�
    public void UI_Change_Pokemon(RectTransform transform)
    {
        beforeIndex = currentIndex;
        currentIndex = 0;
        Menu_UI.gameObject.SetActive(true);
        Menu_UI.gameObject.transform.position = transform.position + new Vector3(450f, -80f, 0f);

        UI_stack.Push(Menu_UI);
    }

    //������ ��� �޴� 
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

    //������ �Է�
    public void ExitButton()
    {
        currentIndex = beforeIndex;

        if (UI_stack.Peek() == Bag_UI)
        {
            currentIndex = 2;
        }

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

    #endregion

    //����
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

    //ü�� Ȯ��
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
}
