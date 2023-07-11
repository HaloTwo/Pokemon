using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBag : MonoBehaviour
{
    public List<GameObject> PlayerPokemon = new List<GameObject>();
    public List<GameObject> NowPokemon = new List<GameObject>();
    public List<GameObject> PokemonBox = new List<GameObject>();
    public ItemData[] Itemdata;

    private void Awake()
    {
        Itemdata = Resources.LoadAll<ItemData>("Item");
    }


    void Start()
    {

        //�ʵ忡 �÷��̾� ���ϸ� ����
        Playerpokemon_Create();

        BattleManager.instance.Battle_Ready.AddListener(PlayerPokemon_Battle_go);
    }

    // Update is called once per frame
    void Update()
    {

    }


    //���ϸ� �߰�
    public void AddPokemon(GameObject pokemon)
    {
        if (NowPokemon.Count < 6)
        {
            for (int i = 0; i < NowPokemon.Count; i++)
            {
                if (NowPokemon[i] == null)
                {
                    PlayerPokemon.RemoveAt(i);
                    PlayerPokemon.Insert(i, pokemon);
                    //GameObject newPokemon = Instantiate(PlayerPokemon[i]);

                    NowPokemon.RemoveAt(i);
                    NowPokemon.Insert(i, pokemon);
                    pokemon.GetComponent<PokemonMove>().enabled = false;

                    PokemonBattleMode newPokemonBattleMode = pokemon.GetComponent<PokemonBattleMode>();
                    newPokemonBattleMode.isWild = false;
                    newPokemonBattleMode.enabled = true;
                    pokemon.SetActive(false);
                    break;
                }
                else
                {
                    Debug.Log("null�ƴ���");
                    break;
                }
            }
        }
        else
        {
            Debug.Log("���ϸ� ���� �ڽ��� ����.");
            PokemonBox.Add(pokemon);
        }

    }

    public void PlayerPokemon_Battle_go()
    {

        Debug.Log("�÷��̾� ���ϸ� ��Ʋ �̺�Ʈ �鰣��!");


        //ù��° ���ϸ� ������
        NowPokemon[0].SetActive(true);
        BattleManager.instance.playerPokemon = NowPokemon[0];
    }

    void Playerpokemon_Create()
    {
        //tudo ���߿� �� �����ؾ߉�
        if (NowPokemon.Count <= 0)
        {
            for (int i = 0; i < PlayerPokemon.Count; i++)
            {
                if (PlayerPokemon[i] == null)
                {
                    NowPokemon.Add(null);
                    continue;
                }

                GameObject newPokemon = Instantiate(PlayerPokemon[i]);

                NowPokemon.Add(newPokemon);
                newPokemon.GetComponent<PokemonMove>().enabled = false;
                newPokemon.GetComponent<PokemonStats>().PlayerOwned = true;

                PokemonBattleMode newPokemonBattleMode = newPokemon.GetComponent<PokemonBattleMode>();
                newPokemonBattleMode.isWild = false;
                newPokemonBattleMode.enabled = true;
                newPokemon.SetActive(false);
            }
        }
        else
        {
            Debug.Log("������ ���ϸ��� �����ϴ�.");
        }
    }

    void PlayerItem()
    {

    }
}
