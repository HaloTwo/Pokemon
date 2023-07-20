using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonBattleMode : MonoBehaviour
{
    public bool isWild = true;
    [HideInInspector] public Animator anim;

    public Slider pokemon_slider;
    [SerializeField] private Text pokemon_name;
    [SerializeField] private Text pokemon_lv;
    [SerializeField] private GameObject UI;
    public GameObject Eiscue_head;
    public GameObject Eiscue_broken_head;

    [HideInInspector] public PokemonStats pokemonStats;
    private GameObject maincamera;

    private void Awake()
    {
        maincamera = GameObject.FindWithTag("MainCamera");
        TryGetComponent(out pokemonStats);
        TryGetComponent(out anim);


        UI = transform.Find("UI").gameObject;
        pokemon_slider = UI.GetComponentInChildren<Slider>();
        pokemon_name = UI.transform.GetChild(0).gameObject.GetComponent<Text>();
        pokemon_lv = UI.transform.GetChild(2).gameObject.GetComponentInChildren<Text>();
        UI.SetActive(false);

        if (name.Contains("0975.Eiscue"))
        {
            Eiscue_head = GameObject.Find("Head");
            Eiscue_broken_head = GameObject.Find("Broken_Head");
        }
    }



    private void OnEnable()
    {
        anim.SetBool("Battle", true);

        if (isWild)
        {
            RectTransform rectTransform = UI.GetComponent<RectTransform>();
            Vector3 pokemonSize = GetComponentInChildren<Renderer>().bounds.size;

            rectTransform.anchoredPosition3D = new Vector3(0, (pokemonSize.y + 0.08f), (pokemonSize.z * 0.5f));

            StartCoroutine(StartAnim_co());

            pokemon_lv.text = "Lv." + pokemonStats.Level;
            pokemon_name.text = pokemonStats.Name;
        }

    }

    public IEnumerator StartAnim_co()
    {
        anim.SetTrigger("Roar");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("roar01") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

        UI.SetActive(true);
    }

    void Start()
    {
        if (isWild)
        {

        }
        else
        {
            if (pokemon_slider.transform.parent.gameObject != null)
            {
                pokemon_slider.transform.parent.gameObject.SetActive(false);
            }
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
            UI.SetActive(false);
        }
    }

    public void OnDie()
    {
        anim.SetTrigger("Die");

        if (isWild)
        {
            StartCoroutine(DieAnimation_co());
        }
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

            Debug.Log("충돌했어 돌려");
            //collision.gameObject.transform.rotation =  Quaternion.Euler(new Vector3(0, -180f, 0));
            collision.gameObject.transform.LookAt(BattleManager.instance.playerPokemon.transform.forward);
            collision.gameObject.transform.rotation = Quaternion.LookRotation(-BattleManager.instance.playerPokemon.transform.forward);
            //ballRigidbody.AddForce(new Vector3(0f, 0f, 0f), ForceMode.Impulse);
        }

    }

    public void Eiscue_head_broken()
    {
        if (Eiscue_head != null)
        {
            Eiscue_head.SetActive(false);
        }
    }
    public void Eiscue_head_broken_last()
    {
        if (Eiscue_head != null)
        {
            Eiscue_broken_head.SetActive(false);
        }
    }
}
