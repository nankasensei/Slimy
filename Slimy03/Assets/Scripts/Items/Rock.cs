using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Item
{
    public GameObject brokenStoneDEF;

    // Start is called before the first frame update
    void Start()
    {
        hp = HP_MAX;
        isAlive = true;
    }

    public override void Buff()
    {
        Instantiate(brokenStoneDEF, transform.position, Quaternion.Euler(90, 0, 0));
    }
}
