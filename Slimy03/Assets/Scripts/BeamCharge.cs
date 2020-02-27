using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCharge : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip chargeClip;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = chargeClip;
        audioSource.volume = 0.04f;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
