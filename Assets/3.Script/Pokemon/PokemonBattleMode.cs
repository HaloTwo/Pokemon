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
        BattleManager.instance.onBattle.AddListener(Pokemon_Roar);
        BattleManager.instance.Battle_Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pokemon_Roar()
    {
        anim.SetBool("Battle", true);
        Debug.Log("배틀시작!");
    }
}
