using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonBattleMode : MonoBehaviour
{
    public bool isWild = true;

    public Animator anim;
    BattleManager battlemaneger = BattleManager.instance;
    PokemonStats pokemonStats;
    public Slider pokemon_slider;
    

    private void Awake()
    {
        TryGetComponent(out pokemonStats);
        TryGetComponent(out anim);
    }


    private void OnEnable()
    {

        anim.SetBool("Battle", true);

        if (isWild)
        {
            anim.SetTrigger("Roar");
            pokemonStats.Hp = pokemonStats.MaxHp;
            pokemon_slider.gameObject.SetActive(true);
        }

        if (pokemonStats.isDie)
        {

        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDie()
    {
        anim.SetTrigger("Die");
    }

}
