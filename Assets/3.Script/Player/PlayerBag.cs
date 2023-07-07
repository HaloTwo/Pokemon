using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBag : MonoBehaviour
{
    public List<GameObject> PlayerPokemon = new List<GameObject>();
    public List<GameObject> NowPokemon = new List<GameObject>();
    public ItemData[] Itemdata;

    private void Awake()
    {
        Itemdata = Resources.LoadAll<ItemData>("Item");
    }


    void Start()
    {

        //필드에 플레이어 포켓몬 생성
        Playerpokemon_Create();

        BattleManager.instance.Battle_Ready.AddListener(PlayerPokemon_Battle_go);
    }

    // Update is called once per frame
    void Update()
    {

    }


    //포켓몬 추가
    public void AddPokemon(GameObject pokemon)
    {
        if (PlayerPokemon.Count < 6)
        {
            for (int i = 0; i < PlayerPokemon.Count; i++)
            {
                if (PlayerPokemon[i] == null)
                {
                    PlayerPokemon.Add(pokemon);
                    GameObject newPokemon = Instantiate(PlayerPokemon[i]);

                    NowPokemon.Add(newPokemon);
                    newPokemon.GetComponent<PokemonMove>().enabled = false;

                    PokemonBattleMode newPokemonBattleMode = newPokemon.GetComponent<PokemonBattleMode>();
                    newPokemonBattleMode.isWild = false;
                    newPokemonBattleMode.enabled = true;
                    newPokemon.SetActive(false);
                    break;
                }
            }
        }
        else
        {
            Debug.Log("포켓몬을 더 이상 추가할 수 없습니다. 최대 6마리까지만 보유할 수 있습니다.");
        }
    }

    public void PlayerPokemon_Battle_go()
    {

        Debug.Log("플레이어 포켓몬 배틀 이벤트 들간다!");


        //첫번째 포켓몬 꺼내기
        NowPokemon[0].SetActive(true);
        BattleManager.instance.playerPokemon = NowPokemon[0];
    }

    void Playerpokemon_Create()
    {
        //tudo 나중에 꼭 수정해야됌
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

                PokemonBattleMode newPokemonBattleMode = newPokemon.GetComponent<PokemonBattleMode>();
                newPokemonBattleMode.isWild = false;
                newPokemonBattleMode.enabled = true;
                newPokemon.SetActive(false);
            }
        }
        else
        {
            Debug.Log("소유한 포켓몬이 없습니다.");
        }
    }

    void PlayerItem()
    {

    }
}
