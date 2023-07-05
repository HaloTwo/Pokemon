using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonBattleMode : MonoBehaviour
{
    BattleManager battlemaneger = BattleManager.instance;

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
    }

}
