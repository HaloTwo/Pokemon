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

        // Resources ���� ���� Database ������ �����ϱ� ���� ��θ� �����մϴ�.
        string databaseFolderPath = Path.Combine(Application.dataPath, "Resources", "Database");

        // Database ������ �������� �ʴ� ��쿡�� �����մϴ�.
        if (!Directory.Exists(databaseFolderPath))
        {
            // Database ������ �����մϴ�.
            Directory.CreateDirectory(databaseFolderPath);
        }

        string jsonFilePath = Path.Combine(Application.dataPath, "Resources/Database", "PlayerData.json");

        // ������ �������� �ʴ� ��쿡�� �����մϴ�.
        if (!File.Exists(jsonFilePath))
        {
            int pokemon = 6;
            int boxpokemon = 30;

            PlayerPokemonData playerData = new PlayerPokemonData(pokemon, boxpokemon);

            //�÷��̾� ��ġ ����
            playerData.PlayerPosition = new Vector3(2502.919f, 70.02077f, -1180.353f);
            playerData.PlayerRotation = new Quaternion(0f, 0.3f, 0f, -0.9f);
            playerData.PlayerMoney = 30000;

            playerData.Mypokemon_name[0] = "0025.Pikachu(Cap)";
            playerData.Mypokemon_korean_name[0] = "��ī��";
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


            //Json�� ����
            string pokemondata = JsonUtility.ToJson(playerData);
            print(pokemondata);

            string fileName = "PlayerData.json";
            string filePath = Path.Combine(Application.dataPath, "Resources/DataBase", fileName);

            // ���Ͽ� ������ ����
            File.WriteAllText(filePath, pokemondata);


            // ������ ������ ��θ� �α׷� ����մϴ�.
            Debug.Log("PlayerData.json ������ �����Ǿ����ϴ�. ���: " + jsonFilePath);
        }
    }
}
