using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBody : MonoBehaviour
{
    public float HP_MAX;
    public bool isAlive;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public AudioSource audioSource;
    public AudioClip superGrowlClip;
    public float growlVolume;
    public RectTransform hitpoints;
    public GameObject hpBar;

    protected float hp;

    private void Awake()
    {
        //call TakeDamage() when hirEvent happens, also pass HitEventData at the same time
        SlimyEvents.hitEvent.AddListener(TakeDamage);
    }

    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        hp = HP_MAX;
        audioSource.PlayOneShot(superGrowlClip, growlVolume);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TakeDamage(HitEventData data)
    {
        if (data.victim == gameObject && isAlive)
        {
            hp -= data.tama.GetComponent<Tama>().damage * GameObject.Find("Player").GetComponent<PlayerController>().ATK;
            hitpoints.localScale = new Vector3(hp/ HP_MAX, 1, 1);

            if (hp <= 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        //audioSource.PlayOneShot(superGrowlClip, growlVolume);
        isAlive = false;
        animator.enabled = false;
        spriteRenderer.color = new Color(90f / 255f, 90f / 255f, 90f / 255f);
        hp = 0;
        hpBar.SetActive(false);
        GetComponent<Collider>().isTrigger = true;
    }

    public void Gone()
    {
        Destroy(gameObject);
    }

    public void Buff()
    {

    }
}
