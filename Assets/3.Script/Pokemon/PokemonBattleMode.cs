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
        //BattleManager.instance.onBattle += player13;
        //BattleManager.instance.onBattle.AddListener(player13);
        BattleManager.instance.onBattle.AddListener(player13);
        BattleManager.instance.Battle_Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void player13()
    {
        Debug.Log("µé¾î°¬Áö·Õ");
    }
}
