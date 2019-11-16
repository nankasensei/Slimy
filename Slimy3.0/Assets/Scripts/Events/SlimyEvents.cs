using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class SlimyEvents
{
    public static HitEvent hitEvent = new HitEvent();
}

public class HitEvent : UnityEvent<HitEventData> { }

public class HitEventData
{
    public GameObject shooter;
    public GameObject victim;
    public GameObject tama;

    public HitEventData(GameObject shooter, GameObject victim, GameObject tama)
    {
        this.shooter = shooter;
        this.victim = victim;
        this.tama = tama;
    }
}