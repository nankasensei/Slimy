using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : Item
{
    // Start is called before the first frame update
    void Start()
    {
        HP_MAX = 8.0f;
        hp = HP_MAX;
        isAlive = true;
    }

    public override void Buff()
    {

    }
}
