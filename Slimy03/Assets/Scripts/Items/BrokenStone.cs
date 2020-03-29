using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenStone : Item
{
    public override void Buff()
    {
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.SetDef(1.0f, 1000.0f);
    }
}
