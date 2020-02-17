using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class SlimyEvents
{
    public static HitEvent hitEvent = new HitEvent();
    public static DieEvent dieEvent = new DieEvent();
    public static GameStartEvent gameStartEvent = new GameStartEvent();
    public static LevelStartEvent levelStartEvent = new LevelStartEvent();
}

public class HitEvent : UnityEvent<HitEventData> { }
public class HitEventData
{
    public GameObject shooter;
    public GameObject victim;
    public GameObject tama; //only be used when shoot
    public float meleeDamage; //only be used when melee

    public HitEventData(GameObject shooter, GameObject victim, GameObject tama, float meleeDamage)
    {
        this.shooter = shooter;
        this.victim = victim;
        this.tama = tama;
        this.meleeDamage = meleeDamage;
    }

    public HitEventData(GameObject shooter, GameObject victim, GameObject tama)
    {
        this.shooter = shooter;
        this.victim = victim;
        this.tama = tama;
    }

    public HitEventData(GameObject shooter, GameObject victim, float meleeDamage)
    {
        this.shooter = shooter;
        this.victim = victim;
        this.meleeDamage = meleeDamage;
    }
}

public class DieEvent : UnityEvent{}

public class GameStartEvent : UnityEvent{}

public class LevelStartEvent : UnityEvent{}