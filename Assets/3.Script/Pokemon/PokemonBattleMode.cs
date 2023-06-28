using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonBattleMode : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        TryGetComponent(out anim);
    }

    void Start()
    {
        anim.SetBool("Battle", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
