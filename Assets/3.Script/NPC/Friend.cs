using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetTrigger("Walk");

            StartCoroutine(FriendBattle(other.gameObject));



            if (anim.GetBool("Battle"))
            {
                other.transform.rotation = playerPos.transform.rotation;
                other.transform.position = playerPos.transform.position;
                other.GetComponent<PlayerMovement>().isBattle = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (anim.GetBool("Battle"))
            {
                other.transform.rotation = playerPos.transform.rotation;
                other.transform.position = playerPos.transform.position;
                other.GetComponent<PlayerMovement>().isBattle = true;

                BattleManager.instance.Battle_Start(in_FriendPokemono[0], other.gameObject, gameObject);
                in_FriendPokemono[0].SetActive(true);
            }
        }
    }

    IEnumerator FriendBattle(GameObject player)
    {
        player.GetComponent<PlayerMovement>().ismove = false;
        yield return new WaitForSeconds(4f);
        anim.SetTrigger("Walk");

        TextBox.instance.NPC_Textbox_OnOff(true);
        yield return StartCoroutine(TextBox.instance.TypeText(Talk));
        TextBox.instance.NPC_Textbox_OnOff(false);


        //player.transform.position = playerPos.transform.position + new Vector3(0, 5f, 0);
        //player.transform.rotation = playerPos.transform.rotation;

        anim.SetBool("Battle", true);

        transform.position = enemyPos.transform.position;
        transform.rotation = enemyPos.transform.rotation;
    }
}
