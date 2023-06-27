using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonMove : MonoBehaviour
{
    Animator anim;
    PokemonBattleMode pokemonbattle;

    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out pokemonbattle);
    }

    private void Start()
    {
        anim.SetBool("Walk", true);    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                collision.gameObject.SetActive(false);
            }
            StartCoroutine(Battle_Go_co());
        }
    }

    IEnumerator Battle_Go_co()
    {
        anim.SetBool("Walk", false);
        yield return new WaitForSeconds(2f);

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        Vector3 directionToPlayer = Player.gameObject.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        anim.SetBool("Battle", true);

        //배틀모드로 돌입
        pokemonbattle.enabled = true;
        this.enabled = false;
        BattleManager.instance.Battle_Start();
    }
}
