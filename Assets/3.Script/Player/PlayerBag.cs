using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBag : MonoBehaviour
{
    public List<GameObject> PlayerPokemon = new List<GameObject>();
    public List<GameObject> NowPokemon = new List<GameObject>();

    void Start()
    {
        BattleManager.instance.Battle_Ready.AddListener(PlayerPokemon_Battle_go);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddPokemon(GameObject pokemon)
    {
        if (PlayerPokemon.Count < 6)
        {
            PlayerPokemon.Add(pokemon);
        }
        else
        {
            Debug.Log("포켓몬을 더 이상 추가할 수 없습니다. 최대 6마리까지만 보유할 수 있습니다.");
        }
    }

    public void PlayerPokemon_Battle_go()
    {

        Debug.Log("플레이어 포켓몬 배틀 이벤트 들간다!");

        //tudo 나중에 꼭 수정해야됌
        if (NowPokemon.Count <= 0)
        {
            Debug.Log("현재 포켓몬이 없어서 필드로 꺼낸다");
            for (int i = 0; i < PlayerPokemon.Count; i++)
            {
                GameObject newPokemon = Instantiate(PlayerPokemon[i]);

                NowPokemon.Add(newPokemon);
                newPokemon.GetComponent<PokemonMove>().enabled = false;

                PokemonBattleMode newPokemonBattleMode = newPokemon.GetComponent<PokemonBattleMode>();
                newPokemonBattleMode.enabled = true;
                newPokemonBattleMode.isWild = false;
                newPokemon.SetActive(false);
            }
        }
        else
        {
            Debug.Log("소유한 포켓몬이 없습니다.");
        }

        //첫번째 포켓몬 꺼내기
        NowPokemon[0].SetActive(true);
        BattleManager.instance.playerPokemon = NowPokemon[0];
    }
}
