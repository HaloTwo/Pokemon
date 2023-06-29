using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonBattleMode : MonoBehaviour
{
    public bool isWild = true;
    
    Animator anim;
    BattleManager battlemaneger = BattleManager.instance;


    private void Awake()
    {
        TryGetComponent(out anim);
    }

    void Start()
    {
        if (isWild)
        {
            anim.SetTrigger("Roar");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


}
