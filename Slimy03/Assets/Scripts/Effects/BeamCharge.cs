using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCharge : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip chargeClip;
    public float chargeVolume;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = chargeClip;
        audioSource.volume = chargeVolume;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
