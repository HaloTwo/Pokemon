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
            Debug.Log("���ϸ��� �� �̻� �߰��� �� �����ϴ�. �ִ� 6���������� ������ �� �ֽ��ϴ�.");
        }
    }

    public void PlayerPokemon_Battle_go()
    {

        Debug.Log("�÷��̾� ���ϸ� ��Ʋ �̺�Ʈ �鰣��!");

        //tudo ���߿� �� �����ؾ߉�
        if (NowPokemon.Count <= 0)
        {
            Debug.Log("���� ���ϸ��� ��� �ʵ�� ������");
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
            Debug.Log("������ ���ϸ��� �����ϴ�.");
        }

        //ù��° ���ϸ� ������
        NowPokemon[0].SetActive(true);
        BattleManager.instance.playerPokemon = NowPokemon[0];
    }
}
