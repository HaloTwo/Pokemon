using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    public static TextBox instance = null;

    public Text TalkText;
    public Text NPC_TalkText;

    [SerializeField] private Text Item_name;
    [SerializeField] private Text Item_explanation;
    [SerializeField] private int currentIndex;
    [SerializeField]private int beforeIndex;

    public GameObject Menu;
    public GameObject Shop;
    public GameObject Shop_Menu;
    public RectTransform select;

    public ItemData[] Itemdata;

    [SerializeField] private UIManger uIManger;

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

    private void Update()
    {
        if (Menu.activeSelf || Shop.activeSelf)
        {
            if (uIManger.UI_stack.Peek() == Shop)
            {
                itemCheck();
            }

            Button[] currentbuttons = uIManger.UI_stack.Peek().GetComponentsInChildren<Button>();

            currentbuttons[currentIndex].Select();
            uIManger.OnButtonSelected(currentbuttons[currentIndex], select);

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = currentbuttons.Length - 1;
                }

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex++;
                if (currentIndex >= currentbuttons.Length)
                {
                    currentIndex = 0;
                }

            }else if (Input.GetKeyDown(KeyCode.Escape))
            {

                GameObject topUI = uIManger.UI_stack.Peek();
                topUI.SetActive(false);
                uIManger.UI_stack.Pop();

                GameObject nextUI = uIManger.UI_stack.Peek();
                nextUI.SetActive(true);

                if (uIManger.UI_stack.Peek() == Menu)
                {
                    currentIndex = 0;
                    MenuOpen(false);               
                }
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
    public void ShopOpen()
    {
        Shop.SetActive(true);
        uIManger.UI_stack.Push(Shop);
        beforeIndex = currentIndex;
    }

    //�޴� �� ��
    public void MenuOpen(bool open)
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
        Itemdata[beforeIndex].Quantity++;

        uIManger.UI_stack.Pop();
        Shop_Menu.SetActive(false);

        currentIndex = beforeIndex;
    }

    void itemCheck()
    {
        Item_name.text = Itemdata[currentIndex].Name;
        Item_explanation.text = Itemdata[currentIndex].Explanation;
    }
}
