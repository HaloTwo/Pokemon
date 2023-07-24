using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friend : MonoBehaviour
{
    [Header("몬스터 볼 관련")]
    [SerializeField] private float ThrowPower = 20f;
    [SerializeField] private Transform ball_loc;
    [SerializeField] private GameObject ball_prefab;
    [SerializeField] private GameObject show_Ball;
    private Rigidbody ball_rb;

    [TextArea]
    [SerializeField] private List<string> Talk = new List<string>();
    [SerializeField] private GameObject[] FriendPokemono = new GameObject[5];
    [SerializeField] private GameObject[] in_FriendPokemono = new GameObject[5];
    [SerializeField] GameObject enemyPos;
    private Animator anim;
    private bool isBattle;
    [Header("포켓몬 레벨")]
    [SerializeField] int level;
    [Header("얻는 돈")]
    public int getmoney = 10000;

    private void Awake()
    {
        ball_prefab.TryGetComponent(out ball_rb);
        TryGetComponent(out anim);
    }

    void Start()
    {

        for (int i = 0; i < FriendPokemono.Length; i++)
        {
            in_FriendPokemono[i] = Instantiate(FriendPokemono[i]);
            in_FriendPokemono[i].SetActive(false);

            in_FriendPokemono[i].GetComponent<PokemonMove>().enabled = false;
            in_FriendPokemono[i].GetComponent<PokemonBattleMode>().isWild = false;
            in_FriendPokemono[i].GetComponent<PokemonBattleMode>().enabled = true;
            in_FriendPokemono[i].GetComponent<PokemonStats>().Level = level;
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

        player.GetComponent<PlayerMovement>().ismove = false;
        StartCoroutine(player.GetComponent<PlayerMovement>().apply_motion_wait(10f));
        TextBox.instance.NPC_Textbox_OnOff(false);

        anim.SetBool("Battle", isBattle);
        player.GetComponent<PlayerMovement>().isBattle = true;

        transform.position = enemyPos.transform.position;
        transform.rotation = enemyPos.transform.rotation;

        friends_pokemon_Move();

        BattleManager.instance.Battle_Start(in_FriendPokemono, player, gameObject);


        yield break;
    }

    void friends_pokemon_Move()
    {
        for (int i = 0; i < in_FriendPokemono.Length; i++)
        {
            Vector3 pokemonpos = transform.position + transform.forward * 6.5f;
            Vector3 offset = transform.forward * -(in_FriendPokemono[i].GetComponentInChildren<Renderer>().bounds.size.z / 2);
            pokemonpos += offset;


            in_FriendPokemono[i].transform.position = pokemonpos;
            in_FriendPokemono[i].transform.rotation = transform.rotation;
        }

    }


    #region 볼던지기
    public void Bullthrow()
    {
        SoundManager.instance.PlayEffect("Pokeball");
        ball_rb.useGravity = true;
        ball_rb.velocity = Vector3.zero;
        ball_rb.angularVelocity = Vector3.zero;
        ball_prefab.transform.rotation = Quaternion.identity;
        ball_prefab.transform.position = ball_loc.position;
        ball_prefab.SetActive(true);

        ball_rb.AddForce(transform.forward * ThrowPower / 3, ForceMode.Impulse);

        Invoke("DisableBallPrefab", 0.4f);
    }

    void DisableBallPrefab()
    {
        ball_prefab.SetActive(false);
    }

    public void BallOn_Event(int num)
    {
        if (num == 1)
        {
            show_Ball.SetActive(true);
        }
        else
        {
            show_Ball.SetActive(false);
        }
    }
    #endregion
}

