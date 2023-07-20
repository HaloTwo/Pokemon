using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Friend : MonoBehaviour
{
    [TextArea]
    [SerializeField] private List<string> Talk = new List<string>();
    [SerializeField] private GameObject[] FriendPokemono = new GameObject[5];
    [SerializeField] private GameObject[] in_FriendPokemono = new GameObject[5];
    Animator anim;
    [SerializeField] GameObject playerPos;
    [SerializeField] GameObject enemyPos;
    [SerializeField] GameObject enemyPokemonPos;
    bool isBattle;

    private void Awake()
    {
        TryGetComponent(out anim);
    }

    void Start()
    {
        for (int i = 0; i < FriendPokemono.Length; i++)
        {
            in_FriendPokemono[i] = Instantiate(FriendPokemono[i], enemyPokemonPos.transform);
            in_FriendPokemono[i].SetActive(false);

            in_FriendPokemono[i].GetComponent<PokemonMove>().enabled = false;
            in_FriendPokemono[i].GetComponent<PokemonBattleMode>().isWild = false;
            in_FriendPokemono[i].GetComponent<PokemonBattleMode>().enabled = true;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isBattle)
        {
            isBattle = true;
            anim.SetBool("Walk", true);
            transform.rotation = Quaternion.LookRotation(other.transform.position - transform.position);

            StartCoroutine(FriendBattle(other.gameObject));
        }
    }



    IEnumerator FriendBattle(GameObject player)
    {
        player.GetComponent<PlayerMovement>().ismove = false;
        yield return new WaitForSeconds(4f);


        anim.SetBool("Walk", false);
        yield return new WaitForSeconds(0.5f);

        TextBox.instance.NPC_Textbox_OnOff(true);
        yield return StartCoroutine(TextBox.instance.TypeText(Talk));

        StartCoroutine(player.GetComponent<PlayerMovement>().apply_motion_wait(10f));
        TextBox.instance.NPC_Textbox_OnOff(false);


        //StopCoroutine(player.GetComponent<PlayerMovement>().apply_motion_wait(1f));
        //player.transform.position = playerPos.transform.position;
        //player.transform.rotation = playerPos.transform.rotation;

        anim.SetBool("Battle", isBattle);
        player.GetComponent<PlayerMovement>().isBattle = true;

        transform.position = enemyPos.transform.position;
        transform.rotation = enemyPos.transform.rotation;


        BattleManager.instance.Battle_Start(in_FriendPokemono, player, gameObject);

        yield break;
    }
}

