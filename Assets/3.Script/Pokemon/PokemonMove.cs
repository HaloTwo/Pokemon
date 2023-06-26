using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonMove : MonoBehaviour
{
    [SerializeField]Animator anim;

    void Start()
    {
        TryGetComponent(out anim);

        StartCoroutine(StartMotion());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartMotion()
    {
        yield return null;
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
    }
}
