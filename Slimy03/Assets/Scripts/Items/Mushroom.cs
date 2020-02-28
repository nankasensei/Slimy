using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Item
{
    public override void Buff()
    {
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.SetAtk(1.0f, 8.0f);
    }
}
