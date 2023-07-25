using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{

    public void StartButton_btn()
    {
        SceneManager.LoadScene("InGame");

        // Resources 폴더 내에 Database 폴더를 생성하기 위한 경로를 설정합니다.
        string databaseFolderPath = Path.Combine(Application.dataPath, "Resources", "Database");

        // Database 폴더가 존재하지 않는 경우에만 생성합니다.
        if (!Directory.Exists(databaseFolderPath))
        {
            // Database 폴더를 생성합니다.
            Directory.CreateDirectory(databaseFolderPath);
        }

        string jsonFilePath = Path.Combine(Application.dataPath, "Resources/Database", "PlayerData.json");

        // 파일이 존재하지 않는 경우에만 생성합니다.
        if (!File.Exists(jsonFilePath))
        {
            int pokemon = 6;
            int boxpokemon = 30;

            PlayerPokemonData playerData = new PlayerPokemonData(pokemon, boxpokemon);

            //플레이어 위치 저장
            playerData.PlayerPosition = new Vector3(2502.919f, 70.02077f, -1180.353f);
            playerData.PlayerRotation = new Quaternion(0f, 0.3f, 0f, -0.9f);
            playerData.PlayerMoney = 30000;

            playerData.Mypokemon_name[0] = "0025.Pikachu(Cap)";
            playerData.Mypokemon_korean_name[0] = "피카츄";
            playerData.Mypokemon_level[0] = 20;
            playerData.Mypokemon_currenthp[0] = 60;
            playerData.Mypokmon_Die[0] = false;

            for (int i = 1; i < pokemon; i++)
            {
                playerData.Mypokemon_name[i] = null;
                playerData.Mypokemon_korean_name[i] = null;
                playerData.Mypokemon_level[i] = 0;
                playerData.Mypokemon_currenthp[i] = 0;
                playerData.Mypokmon_Die[i] = false;
            }

            for (int i = 0; i < boxpokemon; i++)
            {
                playerData.inBox_Mypokemon_name[i] = null;
                playerData.inBox_Mypokemon_korean_name[i] = null;
                playerData.inBox_Mypokemon_level[i] = 0;
                playerData.inBox_Mypokemon_currenthp[i] = 0;
            }


            //Json에 저장
            string pokemondata = JsonUtility.ToJson(playerData);
            print(pokemondata);

            string fileName = "PlayerData.json";
            string filePath = Path.Combine(Application.dataPath, "Resources/DataBase", fileName);

            // 파일에 데이터 저장
            File.WriteAllText(filePath, pokemondata);


            // 생성된 파일의 경로를 로그로 출력합니다.
            Debug.Log("PlayerData.json 파일이 생성되었습니다. 경로: " + jsonFilePath);
        }
    }
}
