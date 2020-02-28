using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float HP_MAX;
    public float hp;
    public AudioSource audioSource;
    public AudioClip breakingClip;
    public float breakingVolume;
    public SpriteRenderer spriteRenderer;

    protected bool isAlive;

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0 && isAlive == true)
        {
            GameManager.instance.audioSourceOfOneShot.PlayOneShot(breakingClip, breakingVolume);
            //isAlive = false;
            //spriteRenderer.color = new Color(90f / 255f, 90f / 255f, 90f / 255f);
            //hp = 0;
            //GetComponent<Collider>().isTrigger = true;
            Buff();
            Gone();
        }
    }

    public virtual void Buff(){}


    public void Gone()
    {
        Destroy(gameObject);
    }
}
