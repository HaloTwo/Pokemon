using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonMove : MonoBehaviour
{
    private Animator anim;
    private PokemonBattleMode pokemonbattle;
    private PokemonStats pokemonStats;

    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out pokemonbattle);
        TryGetComponent(out pokemonStats);
    }

    private void Start()
    {
        anim.SetBool("Walk", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Battle_Go_co(collision));
        }
    }

    IEnumerator Battle_Go_co(Collision collision)
    {
        GameObject Player = null;

        if (collision.gameObject.CompareTag("Ball"))
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            if (Player.GetComponent<PlayerMovement>().isBattle)
            {
                yield break;
            }
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            bool isBattle = collision.gameObject.GetComponent<PlayerMovement>().isBattle;

            if (isBattle)
            {
                yield break;
            }

            isBattle = true;
            Player = collision.gameObject;

        }

        //애니메이터 잠시 끔
        StartCoroutine(Player.GetComponent<PlayerMovement>().apply_motion_wait(5f));

        //플레이어 못움직이게
        Player.GetComponent<PlayerMovement>().isBattle = true;

        //포켓몬도 멈춤
        anim.SetBool("Walk", false);
        yield return new WaitForSeconds(1.5f);

        //플레이어 위치 추적해서 쳐다보기
        Vector3 directionToPlayer = Player.gameObject.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        //배틀모드로 돌입
        pokemonbattle.enabled = true;
        this.enabled = false;

        BattleManager.instance.Battle_Start(gameObject, Player);
    }
}
