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

    private void Awake()
    {
        Itemdata = Resources.LoadAll<ItemData>("Item");

        //필드에 플레이어 포켓몬 생성
        PlayerInfo_Road();
    }


    void Start()
    {
        BattleManager.instance.Battle_Ready.AddListener(PlayerPokemon_Battle_go);
    }

    // Update is called once per frame
    void Update()
    {

    }


    //포켓몬 추가
    public void AddPokemon(GameObject pokemon)
    {

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
                if (i == 5)
                {
                    Debug.Log("자리가 없습니다. 박스로 가주세요~");
                    PokemonBox.Add(pokemon);
                }
            }
        }
    }

    public void PlayerPokemon_Battle_go()
    {
        //첫번째 포켓몬 꺼내기
        PlayerPokemon[0].SetActive(true);
        BattleManager.instance.playerPokemon = PlayerPokemon[0];
    }


    void PlayerInfo_Road()
    {
        //TextAsset jsonFile = Resources.Load<TextAsset>("DataBase/PlayerData");
        //PlayerPokemonData = JsonMapper.ToObject<PlayerPokemonData[]>(jsonFile.text);

        string jsonFilePath = Path.Combine(Application.dataPath, "Resources/Database", "PlayerData.json");
        string jsonContent = File.ReadAllText(jsonFilePath);
        PlayerPokemonData playerPokemonData = JsonUtility.FromJson<PlayerPokemonData>(jsonContent);


        transform.position = playerPokemonData.PlayerLocation;

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
                    if (playerNames == dataManager.pokemon[j].name + "(Clone)")
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
    }



    public void PlayerInfo_Save()
    {
        PlayerPokemonData playerData = new PlayerPokemonData(PlayerPokemon.Count);

        //플레이어 위치 저장
        playerData.PlayerLocation = gameObject.transform.position;


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
    public Vector3 PlayerLocation;

    public string[] Mypokemon_name;
    public string[] Mypokemon_korean_name;
    public int[] Mypokemon_level;
    public int[] Mypokemon_currenthp;
    public bool[] Mypokmon_Die;

    public string[] inBox_Mypokemon_name;
    public string[] inBox_Mypokemon_korean_name;
    public int[] inBox_Mypokemon_level;
    public int[] inBox_Mypokemon_currenthp;
    public bool[] inBox_Mypokmon_Die;

    public PlayerPokemonData(int size)
    {
        Mypokemon_name = new string[size];
        Mypokemon_korean_name = new string[size];
        Mypokemon_level = new int[size];
        Mypokemon_currenthp = new int[size];
        Mypokmon_Die = new bool[size];
    }
}

