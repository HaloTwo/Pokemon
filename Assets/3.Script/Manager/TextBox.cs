using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    //�̱��� ����
    #region �̱���
    public static TextBox instance = null;
    private void Awake()
    {
        if (instance == null) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            instance = this; //���ڽ��� instance�� �־��ݴϴ�.
            DontDestroyOnLoad(gameObject); //OnLoad(���� �ε� �Ǿ�����) �ڽ��� �ı����� �ʰ� ����
        }
        else
        {
            if (instance != this) //instance�� ���� �ƴ϶�� �̹� instance�� �ϳ� �����ϰ� �ִٴ� �ǹ�
                Destroy(this.gameObject); //�� �̻� �����ϸ� �ȵǴ� ��ü�̴� ��� AWake�� �ڽ��� ����
        }
    }

    #endregion

    public Text TalkText;
    public Text NPC_TalkText;

    public int currentIndex;
    [SerializeField] private int beforeIndex;

    [Header("Shop UI")]
    [SerializeField] private Text Item_name;
    [SerializeField] private Text Item_explanation;
    [SerializeField] private Text Item_num;
    [SerializeField] private Text player_money;

    [Header("PokemonShop UI")]
    [SerializeField] private Text[] pokemon_name;
    [SerializeField] private Image[] pokemon_img;

    [Header("UI Object")]
    public GameObject Menu;
    public GameObject Shop;
    public GameObject Shop_Menu;
    public GameObject Pokemon_Shop;
    public GameObject Pokemon_Shop_Menu;
    public RectTransform select;

    [Header("Itemdata")]
    public ItemData[] Itemdata;

    [Header("Component")]
    [SerializeField] private UIManger uIManger;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private PlayerBag playerbag;



    private void Update()
    {
        if (Menu.activeSelf || Shop.activeSelf || Pokemon_Shop.activeSelf)
        {
            if (uIManger.UI_stack.Peek() == Shop)
            {
                itemCheck();
            }

            Button[] currentbuttons = uIManger.UI_stack.Peek().GetComponentsInChildren<Button>();

            currentbuttons[currentIndex].Select();
            uIManger.OnButtonSelected(currentbuttons[currentIndex], select);

            //��ư ������
            Button_Move(currentbuttons);

            //������ ��ư
            inputButton_Exit();

        }
    }

    void Button_Move(Button[] buttons)
    {
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

        if (uIManger.UI_stack.Peek() == Pokemon_Shop)
        {

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {


                currentIndex -= 12;

                if (currentIndex < 0)
                {
                    int num = currentIndex;
                    currentIndex = num + 12;
                }

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {


                currentIndex += 12;

                if (currentIndex >= buttons.Length)
                {
                    int num = currentIndex;
                    currentIndex = num - 12;
                }
            }
        }
    }

    void inputButton_Exit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currentIndex = beforeIndex;
            beforeIndex = 0;

            GameObject topUI = uIManger.UI_stack.Peek();
            topUI.SetActive(false);
            uIManger.UI_stack.Pop();

            GameObject nextUI = uIManger.UI_stack.Peek();
            nextUI.SetActive(true);

            if (uIManger.UI_stack.Peek() == Menu)
            {
                currentIndex = 0;
                Shop_MenuOpen(false);
            }
        }
    }

    public void Textbox_OnOff(bool onoff)
    {
        TalkText.gameObject.SetActive(onoff);
    }
    public void NPC_Textbox_OnOff(bool onoff)
    {
        NPC_TalkText.transform.parent.gameObject.SetActive(onoff);
    }

    //��
    #region �� ����
    public void ShopOpen()
    {
        Shop.SetActive(true);
        uIManger.UI_stack.Push(Shop);
        beforeIndex = currentIndex;
        player_money.text = $"{playerbag.playermoney}��";
    }

    //�޴� �� ��
    public void Shop_MenuOpen(bool open)
    {
        Menu.SetActive(open);

        if (open == true)
        {
            uIManger.UI_stack = new Stack<GameObject>();
            uIManger.UI_stack.Push(Menu);
        }
        else
        {
            uIManger.UI_stack = null;
            uIManger.main_bool = true;
            FindObjectOfType<PlayerMovement>().ismove = true;

            select.gameObject.SetActive(false);
        }
    }

    public void Shop_Choise()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        RectTransform transform = clickedObject.GetComponent<RectTransform>();

        beforeIndex = currentIndex;
        currentIndex = 0;
        Shop_Menu.gameObject.SetActive(true);
        Shop_Menu.gameObject.transform.position = transform.position + new Vector3(300f, -80f, 0f);

        uIManger.UI_stack.Push(Shop_Menu);
    }
    public void Shop_pay()
    {
        if (playerbag.playermoney != 0)
        {
            Itemdata[beforeIndex].Quantity++;
            playerbag.playermoney -= Itemdata[beforeIndex].Price;
        }

        uIManger.UI_stack.Pop();
        Shop_Menu.SetActive(false);

        currentIndex = beforeIndex;
        player_money.text = $"{playerbag.playermoney}��";
    }

    void itemCheck()
    {
        Item_name.text = Itemdata[currentIndex].Name;
        Item_explanation.text = Itemdata[currentIndex].Explanation;
        Item_num.text = $"{Itemdata[currentIndex].Quantity}";
    }
    #endregion

    //���ϸ� ��
    #region ���ϸ� �� ����
    public void Pokemon_ShopOpen()
    {
        Pokemon_Shop.SetActive(true);
        uIManger.UI_stack.Push(Pokemon_Shop);
        beforeIndex = currentIndex;

        for (int i = 0; i < dataManager.pokemon.Length; i++)
        {
            pokemon_img[i].sprite = dataManager.pokemon[i].GetComponent<PokemonStats>().image;
            pokemon_name[i].text = $"{i + 1}.{dataManager.pokemon[i].GetComponent<PokemonStats>().Name}";
        }
    }

    public void PokemonShop_Choise()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        RectTransform transform = clickedObject.GetComponent<RectTransform>();

        beforeIndex = currentIndex;
        currentIndex = 0;
        Pokemon_Shop_Menu.gameObject.SetActive(true);
        Pokemon_Shop_Menu.gameObject.transform.position = transform.position + new Vector3(300f, -80f, 0f);

        uIManger.UI_stack.Push(Pokemon_Shop_Menu);
    }

    public void PokemonShop_pay()
    {
        GameObject pokemon = Instantiate(dataManager.pokemon[beforeIndex]);
        pokemon.GetComponent<PokemonStats>().Level = 20;
        pokemon.GetComponent<PokemonStats>().LevelUp();

        playerbag.AddPokemon(pokemon);

        uIManger.UI_stack.Pop();
        Pokemon_Shop_Menu.SetActive(false);

        currentIndex = beforeIndex;
    }

    #endregion


    public IEnumerator TypeText(List<string> fullText)
    {

        for (int j = 0; j < fullText.Count; j++)
        {
            for (int i = 0; i <= fullText[j].Length; i++)
            {
                string displayedText = fullText[j].Substring(0, i);
                NPC_TalkText.text = displayedText;

                yield return null;
                yield return new WaitForSeconds(0.02f);
            }
            Debug.Log("���ϴ�");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }
    }
}
