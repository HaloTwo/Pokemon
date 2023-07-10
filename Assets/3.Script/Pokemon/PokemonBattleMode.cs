using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonBattleMode : MonoBehaviour
{

    public bool isWild = true;
    public Animator anim;

    public Slider pokemon_slider;
    [SerializeField] private Text pokemon_name;
    [SerializeField] private Text pokemon_lv;

    PokemonStats pokemonStats;
    private GameObject maincamera;

    private void Awake()
    {
        maincamera = GameObject.FindWithTag("MainCamera");
        TryGetComponent(out pokemonStats);
        TryGetComponent(out anim);
    }



    private void OnEnable()
    {
        anim.SetBool("Battle", true);

        if (isWild)
        {
            anim.SetTrigger("Roar");
            pokemon_slider.transform.parent.gameObject.SetActive(true);
            pokemon_name.text = pokemonStats.Name;
        }

    }

    void Start()
    {
        if (isWild)
        {
            pokemonStats.Level = Random.Range(10, 30);
            pokemon_lv.text = "Lv." + pokemonStats.Level;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isWild)
        {
            Vector3 direction = maincamera.transform.position - pokemon_slider.transform.parent.position;
            pokemon_slider.transform.parent.rotation = Quaternion.LookRotation(-direction);
        }
    }

    private void OnDisable()
    {
        if (isWild)
        {
            pokemon_slider.transform.parent.gameObject.SetActive(false);
        }
    }

    public void OnDie()
    {
        anim.SetTrigger("Die");

        StartCoroutine(DieAnimation_co());
    }

    IEnumerator DieAnimation_co()
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.5f && anim.GetCurrentAnimatorStateInfo(0).IsName("down01_loop_gfbanm"));
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") && BattleManager.instance.ball_throw)
        {
            gameObject.SetActive(false);

            Rigidbody ballRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
            ballRigidbody.useGravity = true;

            collision.gameObject.transform.LookAt(BattleManager.instance.playerPokemon.transform);
            //ballRigidbody.AddForce(new Vector3(0f, 0f, 0f), ForceMode.Impulse);
        }

    }
}
