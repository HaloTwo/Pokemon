using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PokeCenter : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerBag playerbag;
    private UIManger uIManger;
    public bool isTalk = false;
    [TextArea]
    public List<string> fullText = new List<string>();
    [SerializeField] private string myname;

    private void Start()
    {
        uIManger = FindObjectOfType<UIManger>();
    }

    void Talk()
    {
        TextBox.instance.NPC_Textbox_OnOff(true);
        StartCoroutine(TypeText(0));
    }

    private IEnumerator TypeText(int num)
    {

        for (int j = 0; j < fullText.Count; j++)
        {
            for (int i = 0; i <= fullText[num + j].Length; i++)
            {
                string displayedText = fullText[num + j].Substring(0, i);
                TextBox.instance.NPC_TalkText.text = displayedText;

                yield return new WaitForSeconds(0.05f);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TextBox.instance.NPC_TalkText.text = fullText[num + j];
                    break;
                }

            }

            yield return null;
        }

        if (gameObject.name == "Nurse Joy")
        {
            TextBox.instance.Menu.transform.GetComponentInChildren<Text>().text = "쉬게한다";
        }
        else if (gameObject.name == "shop Boy")
        {
            TextBox.instance.Menu.transform.GetComponentInChildren<Text>().text = "사러왔다";
        }
        else if (gameObject.name == "PokeBox")
        {
            TextBox.instance.Menu.transform.GetComponentInChildren<Text>().text = "구경한다";
        }
        TextBox.instance.select.gameObject.SetActive(true);
        TextBox.instance.Shop_MenuOpen(true);

    }

    public void PokemonCenter()
    {

        //간호순
        if (name.Equals("Nurse Joy"))
        {
            for (int i = 0; i < playerbag.PlayerPokemon.Count; i++)
            {
                if (playerbag.PlayerPokemon[i] != null)
                {
                    PokemonStats pokemon = playerbag.PlayerPokemon[i].GetComponent<PokemonStats>();

                    pokemon.Hp = pokemon.MaxHp;

                    for (int j = 0; j < pokemon.skills.Count; j++)
                    {
                        pokemon.SkillPP[j] = pokemon.skills[j].MaxPP;
                    }
                }
            }

            SoundManager.instance.PlayEffect("PokeCenter_Heal");
            TextBox.instance.select.gameObject.SetActive(false);

            Invoke("wait", 2);
        }
        //상점
        else if (name.Equals("shop Boy"))
        {
            TextBox.instance.ShopOpen();
        }
        else if (name.Equals("PokeBox"))
        {
            TextBox.instance.Pokemon_ShopOpen();
        }

        TextBox.instance.Menu.SetActive(false);
        TextBox.instance.NPC_Textbox_OnOff(false);
    }

    void wait()
    {
        uIManger.main_bool = true;
        playerMovement.ismove = true;

        isTalk = false;
    }

    public void TalkExit()
    {
        TextBox.instance.currentIndex = 0;

        uIManger.main_bool = true;
        playerMovement.ismove = true;

        isTalk = false;

        TextBox.instance.select.gameObject.SetActive(false);
        TextBox.instance.Menu.SetActive(false);
        TextBox.instance.NPC_Textbox_OnOff(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.instance.SaveBGM();
            SoundManager.instance.PlayBGM("PokemonCenter");

            // 플레이어와 충돌한 오브젝트의 방향 벡터
            Vector3 directionToPlayer = other.transform.position - transform.position;
            directionToPlayer.y = 0f; // y 축은 회전시키지 않기 위해 0으로 설정
            directionToPlayer.Normalize();

            // 플레이어가 오브젝트의 뒤쪽에 있는지 확인
            if (Vector3.Dot(transform.forward, directionToPlayer) < 0f)
            {
                // 180도 회전하여 플레이어를 오브젝트의 앞쪽으로 보이게 함
                transform.Rotate(Vector3.up, 180f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.instance.SaveBGM_Start();
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isTalk && !uIManger.Main_UI.activeSelf)
            {
                SoundManager.instance.PlayEffect("ButtonClick");
                playerbag = other.GetComponent<PlayerBag>();
                playerMovement = other.GetComponent<PlayerMovement>();

                playerMovement.ismove = false;
                uIManger.main_bool = false;

                isTalk = true;
                Talk();

                TextBox.instance.Menu.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                TextBox.instance.Menu.GetComponentsInChildren<Button>()[0].onClick.AddListener(PokemonCenter);
                TextBox.instance.Menu.GetComponentsInChildren<Button>()[1].onClick.AddListener(TalkExit);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (uIManger.UI_stack != null)
                {
                    if ((uIManger.UI_stack.Peek() == TextBox.instance.Menu) || (uIManger.UI_stack.Peek() == TextBox.instance.Shop) || (uIManger.UI_stack.Peek() == TextBox.instance.Pokemon_Shop))
                    {
                        TalkExit();
                    }

                }

            }


        }
    }


}
