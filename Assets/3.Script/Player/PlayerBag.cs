using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class PlayerBag : MonoBehaviour
{
    [SerializeField] private DataManager dataManager;
    public List<GameObject> PlayerPokemon = new List<GameObject>();
    public List<GameObject> PokemonBox = new List<GameObject>();
    public ItemData[] Itemdata;
    public ItemData ball;


    void Start()
    {
        Itemdata = Resources.LoadAll<ItemData>("Item");

        //�ʵ忡 �÷��̾� ���ϸ� ����
        PlayerInfo_Road();

        BattleManager.instance.Battle_Ready.AddListener(PlayerPokemon_Battle_go);
    }


    //���ϸ� �߰�
    public void AddPokemon(GameObject pokemon)
    {
        Debug.Log("�÷��̾����ϸ� ����");
        for (int i = 0; i < PlayerPokemon.Count; i++)
        {

            if (PlayerPokemon[i] == null)
            {
                PlayerPokemon.RemoveAt(i);
                PlayerPokemon.Insert(i, pokemon);
                //GameObject newPokemon = Instantiate(PlayerPokemon[i]);

                PlayerPokemon.RemoveAt(i);
                PlayerPokemon.Insert(i, pokemon);
                pokemon.GetComponent<PokemonMove>().enabled = false;

                PokemonBattleMode newPokemonBattleMode = pokemon.GetComponent<PokemonBattleMode>();
                newPokemonBattleMode.isWild = false;
                newPokemonBattleMode.enabled = true;
                pokemon.SetActive(false);
                break;
            }
            else
            {
                if (i <= 5)
                {
                    Debug.Log("�ڸ��� �����ϴ�. �ڽ��� ���ּ���~");
                    for (int j = 0; j < PokemonBox.Count; j++)
                    {

                        if (PokemonBox[j] == null)
                        {
                            PokemonBox.RemoveAt(j);
                            PokemonBox.Insert(j , pokemon);
                            return;
                        }
                        else
                        {

                        }
                    }

                    //�����ؾ߉�
                }
            }
        }
    }

    public void PlayerPokemon_Battle_go()
    {
        for (int i = 0; i < PlayerPokemon.Count; i++)
        {
            if (PlayerPokemon[i].GetComponent<PokemonStats>().Hp > 0)
            {
                PlayerPokemon[i].SetActive(true);
                BattleManager.instance.playerPokemon = PlayerPokemon[i];

                break;
            }
            else
            {

            }
        }
    }


    //���嵥���� �ҷ�����
    void PlayerInfo_Road()
    {
        //TextAsset jsonFile = Resources.Load<TextAsset>("DataBase/PlayerData");
        //PlayerPokemonData = JsonMapper.ToObject<PlayerPokemonData[]>(jsonFile.text);

        string jsonFilePath = Path.Combine(Application.dataPath, "Resources/Database", "PlayerData.json");
        string jsonContent = File.ReadAllText(jsonFilePath);
        PlayerPokemonData playerPokemonData = JsonUtility.FromJson<PlayerPokemonData>(jsonContent);


        transform.position = playerPokemonData.PlayerPosition;
        transform.rotation = playerPokemonData.PlayerRotation;

        //���ϸ� ����
        for (int i = 0; i < playerPokemonData.Mypokemon_name.Length; i++)
        {
            //�켱 �������� �̸��� ã�´�.
            string playerNames = playerPokemonData.Mypokemon_name[i];

            //�̸��� ������̸� ������� �߰��Ѵ�.
            if (playerNames == "")
            {
                //PlayerPokemon[i] = null;
                Debug.Log("������̿�");
                PlayerPokemon.Add(null);
            }
            else
            {
                //�ƴϸ� �����Ϳ� �ִ� �����߿��� ���� �̸��� ã�Ƽ� �߰��Ѵ�.
                for (int j = 0; j < dataManager.pokemon.Length; j++)
                {
                    if (playerNames.Contains(dataManager.pokemon[j].name))
                    {
                        GameObject newPokemon = Instantiate(dataManager.pokemon[j]);
                        PokemonStats mypokemonstates = newPokemon.GetComponent<PokemonStats>();

                        PlayerPokemon.Add(newPokemon);
                        newPokemon.GetComponent<PokemonMove>().enabled = false;

                        mypokemonstates.PlayerOwned = true;
                        mypokemonstates.Level = playerPokemonData.Mypokemon_level[i];

                        mypokemonstates.LevelUp();
                        mypokemonstates.Name = playerPokemonData.Mypokemon_korean_name[i];
                        mypokemonstates.Hp = playerPokemonData.Mypokemon_currenthp[i];
                        mypokemonstates.isDie = playerPokemonData.Mypokmon_Die[i];

                        PokemonBattleMode newPokemonBattleMode = newPokemon.GetComponent<PokemonBattleMode>();
                        newPokemonBattleMode.isWild = false;
                        newPokemonBattleMode.enabled = true;
                        newPokemon.SetActive(false);
                    }
                }
            }
        }

        for (int i = 0; i < playerPokemonData.inBox_Mypokemon_name.Length; i++)
        {
            //�켱 �������� �̸��� ã�´�.
            string inbox_pokemon_name = playerPokemonData.inBox_Mypokemon_name[i];
            //�̸��� ������̸� ������� �߰��Ѵ�.
            if (inbox_pokemon_name == "")
            {
                PokemonBox.Add(null);
            }
            else
            {
                //�ƴϸ� �����Ϳ� �ִ� �����߿��� ���� �̸��� ã�Ƽ� �߰��Ѵ�.
                for (int j = 0; j < dataManager.pokemon.Length; j++)
                {

                    if (inbox_pokemon_name.Contains(dataManager.pokemon[j].name))
                    {
                        GameObject inbox_pokemon = dataManager.pokemon[j];
                        PokemonStats mypokemonstates = inbox_pokemon.GetComponent<PokemonStats>();
                        inbox_pokemon.GetComponent<PokemonMove>().enabled = false;

                        mypokemonstates.PlayerOwned = true;
                        mypokemonstates.Level = playerPokemonData.inBox_Mypokemon_level[i];

                        mypokemonstates.LevelUp();
                        mypokemonstates.Name = playerPokemonData.inBox_Mypokemon_korean_name[i];
                        mypokemonstates.Hp = playerPokemonData.inBox_Mypokemon_currenthp[i];

                        PokemonBox.Add(inbox_pokemon);
                    }
                }
            }
        }

    }


    //���� ������ ����
    public void PlayerInfo_Save()
    {
        PlayerPokemonData playerData = new PlayerPokemonData(PlayerPokemon.Count, PokemonBox.Count);

        //�÷��̾� ��ġ ����
        playerData.PlayerPosition = transform.position;
        playerData.PlayerRotation = transform.rotation;


        //�÷��̾ ���� �����ִ� ���ϸ� ����
        for (int i = 0; i < PlayerPokemon.Count; i++)
        {
            PokemonStats MypokemonStats;

            if (PlayerPokemon[i] == null)
            {
                playerData.Mypokemon_name[i] = null;
                playerData.Mypokemon_korean_name[i] = null;
                playerData.Mypokemon_level[i] = 0;
                playerData.Mypokemon_currenthp[i] = 0;
                playerData.Mypokmon_Die[i] = false;
            }
            else
            {
                MypokemonStats = PlayerPokemon[i].GetComponent<PokemonStats>();

                playerData.Mypokemon_name[i] = MypokemonStats.name;
                playerData.Mypokemon_korean_name[i] = MypokemonStats.Name;
                playerData.Mypokemon_level[i] = MypokemonStats.Level;
                playerData.Mypokemon_currenthp[i] = MypokemonStats.Hp;
                playerData.Mypokmon_Die[i] = MypokemonStats.isDie;
            }
        }


        //���ϸ� �ڽ��� �ִ� ������ ����
        for (int i = 0; i < PokemonBox.Count; i++)
        {
            if (PokemonBox[i] == null)
            {
                playerData.inBox_Mypokemon_name[i] = null;
                playerData.inBox_Mypokemon_korean_name[i] = null;
                playerData.inBox_Mypokemon_level[i] = 0;
                playerData.inBox_Mypokemon_currenthp[i] = 0;
            }
            else
            {
                PokemonStats MypokemonStats = PokemonBox[i].GetComponent<PokemonStats>();

                playerData.inBox_Mypokemon_name[i] = MypokemonStats.name;
                playerData.inBox_Mypokemon_korean_name[i] = MypokemonStats.Name;
                playerData.inBox_Mypokemon_level[i] = MypokemonStats.Level;
                playerData.inBox_Mypokemon_currenthp[i] = MypokemonStats.Hp;
            }
        }



        //Json�� ����
        string pokemondata = JsonUtility.ToJson(playerData);
        print(pokemondata);

        string fileName = "PlayerData.json";
        string filePath = Path.Combine(Application.dataPath, "Resources/DataBase", fileName);

        // ���Ͽ� ������ ����
        File.WriteAllText(filePath, pokemondata);
    }

}

public class PlayerPokemonData
{
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;

    public string[] Mypokemon_name;
    public string[] Mypokemon_korean_name;
    public int[] Mypokemon_level;
    public int[] Mypokemon_currenthp;
    public bool[] Mypokmon_Die;

    public string[] inBox_Mypokemon_name;
    public string[] inBox_Mypokemon_korean_name;
    public int[] inBox_Mypokemon_level;
    public int[] inBox_Mypokemon_currenthp;

    public PlayerPokemonData(int pokemonsize, int boxsize)
    {
        Mypokemon_name = new string[pokemonsize];
        Mypokemon_korean_name = new string[pokemonsize];
        Mypokemon_level = new int[pokemonsize];
        Mypokemon_currenthp = new int[pokemonsize];
        Mypokmon_Die = new bool[pokemonsize];

        inBox_Mypokemon_name = new string[boxsize];
        inBox_Mypokemon_korean_name = new string[boxsize];
        inBox_Mypokemon_level = new int[boxsize];
        inBox_Mypokemon_currenthp = new int[boxsize];
    }
}

