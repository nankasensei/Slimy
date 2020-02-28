using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public GameObject spriteMask;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip lightClip;
    public float lightVolume;
    public bool isLight;

    [Range(0.05f, 0.2f)]
    public float flickTime;

    [Range(0.02f, 0.09f)]
    public float addSize;

    private float time;
    private bool isBecomingLarger;
    // Start is called before the first frame update
    void Start()
    {
        time = 0f;
        isBecomingLarger = false;
        isLight = false;

        if (GameManager.instance.level > 3)
            Light();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time > flickTime)
        {
            Vector3 spriteMaskScale = spriteMask.transform.localScale;

            if (isBecomingLarger)
            {
                spriteMaskScale = new Vector3(spriteMaskScale.x + addSize, spriteMaskScale.y, spriteMaskScale.z + addSize);
                
            }
            else
            {
                spriteMaskScale = new Vector3(spriteMaskScale.x - addSize, spriteMaskScale.y, spriteMaskScale.z - addSize);
            }
            time = 0f;
            isBecomingLarger = !isBecomingLarger;
        }
    }

    public void Light()
    {
        if(!isLight)
        {
            spriteMask.SetActive(true);
            animator.SetBool("isLight", true);
            if (GameManager.instance.level <= 3)
                audioSource.PlayOneShot(lightClip, lightVolume);
            isLight = true;
        }
    }
}
