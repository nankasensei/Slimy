using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raspberry : Item
{
    public float buffHP;

    public override void Buff()
    {
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.SetHp(buffHP);
    }
}
