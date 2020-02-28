using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip explosionClip;
    private AnimatorStateInfo animatorInfo;
    // Start is called before the first frame update
    void Start()
    {
        audioSource.PlayOneShot(explosionClip , 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        animatorInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorInfo.normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
