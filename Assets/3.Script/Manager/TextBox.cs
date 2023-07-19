using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    public static TextBox instance = null;

    public Text TalkText;
    public Text NPC_TalkText;

    public int currentIndex;
    public int beforeIndex;

    public GameObject Menu;
    public GameObject Shop;
    public RectTransform select;

    [SerializeField]private UIManger uIManger;
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
        if (Menu.activeSelf)
        {
            uIManger.UI_stack = new Stack<GameObject>();
            uIManger.UI_stack.Push(Menu);

            Button[] currentbuttons = uIManger.UI_stack.Peek().GetComponentsInChildren<Button>();

            currentbuttons[currentIndex].Select();

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

            }

            uIManger.OnButtonSelected(currentbuttons[currentIndex], select);
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

}
