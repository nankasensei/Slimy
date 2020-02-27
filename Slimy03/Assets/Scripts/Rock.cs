using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Item
{
    // Start is called before the first frame update
    void Start()
    {
        hp = HP_MAX;
        isAlive = true;
    }

    public override void Buff()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().SetDef(1.0f, 8.0f);
    }
}
