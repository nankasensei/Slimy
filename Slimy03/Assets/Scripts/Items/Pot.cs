using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : Item
{
    public GameObject raspberryS;
    public GameObject raspberryM;
    public GameObject mushroomATK;
    public GameObject brokenStoneDEF;

    // Start is called before the first frame update
    void Start()
    {
        hp = HP_MAX;
        isAlive = true;
    }

    public override void Buff()
    {
        int randomIndex = Random.Range(0,100);
        if(randomIndex < 30)
            Instantiate(raspberryS, transform.position, Quaternion.Euler(90, 0, 0));
        else if(randomIndex < 50)
            Instantiate(raspberryM, transform.position, Quaternion.Euler(90, 0, 0));
        else if (randomIndex < 60)
            Instantiate(mushroomATK, transform.position, Quaternion.Euler(90, 0, 0));
        else if (randomIndex < 70)
            Instantiate(brokenStoneDEF, transform.position, Quaternion.Euler(90, 0, 0));
        else if (randomIndex < 100)
        { }
    }
}
