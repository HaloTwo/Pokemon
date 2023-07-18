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

        //필드에 플레이어 포켓몬 생성
        PlayerInfo_Road();

        BattleManager.instance.Battle_Ready.AddListener(PlayerPokemon_Battle_go);
    }


    //포켓몬 추가
    public void AddPokemon(GameObject pokemon)
    {
        Debug.Log("플레이어포켓몬 생성");
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
                    Debug.Log("자리가 없습니다. 박스로 가주세요~");
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

                    //수정해야됌
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


    //저장데이터 불러오기
    void PlayerInfo_Road()
    {
        //TextAsset jsonFile = Resources.Load<TextAsset>("DataBase/PlayerData");
        //PlayerPokemonData = JsonMapper.ToObject<PlayerPokemonData[]>(jsonFile.text);

        string jsonFilePath = Path.Combine(Application.dataPath, "Resources/Database", "PlayerData.json");
        string jsonContent = File.ReadAllText(jsonFilePath);
        PlayerPokemonData playerPokemonData = JsonUtility.FromJson<PlayerPokemonData>(jsonContent);


        transform.position = playerPokemonData.PlayerPosition;
        transform.rotation = playerPokemonData.PlayerRotation;

        //포켓몬 생성
        for (int i = 0; i < playerPokemonData.Mypokemon_name.Length; i++)
        {
            //우선 원본파일 이름을 찾는다.
            string playerNames = playerPokemonData.Mypokemon_name[i];

            //이름이 빈공백이면 빈공간을 추가한다.
            if (playerNames == "")
            {
                //PlayerPokemon[i] = null;
                Debug.Log("빈공간이오");
                PlayerPokemon.Add(null);
            }
            else
            {
                //아니면 데이터에 있는 몬스터중에서 같은 이름을 찾아서 추가한다.
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
            //우선 원본파일 이름을 찾는다.
            string inbox_pokemon_name = playerPokemonData.inBox_Mypokemon_name[i];
            //이름이 빈공백이면 빈공간을 추가한다.
            if (inbox_pokemon_name == "")
            {
                PokemonBox.Add(null);
            }
            else
            {
                //아니면 데이터에 있는 몬스터중에서 같은 이름을 찾아서 추가한다.
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


    //저장 데이터 저장
    public void PlayerInfo_Save()
    {
        PlayerPokemonData playerData = new PlayerPokemonData(PlayerPokemon.Count, PokemonBox.Count);

        //플레이어 위치 저장
        playerData.PlayerPosition = transform.position;
        playerData.PlayerRotation = transform.rotation;


        //플레이어가 현재 갖고있는 포켓몬 저장
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


        //포켓몬 박스에 있는 데이터 저장
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



        //Json에 저장
        string pokemondata = JsonUtility.ToJson(playerData);
        print(pokemondata);

        string fileName = "PlayerData.json";
        string filePath = Path.Combine(Application.dataPath, "Resources/DataBase", fileName);

        // 파일에 데이터 저장
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

