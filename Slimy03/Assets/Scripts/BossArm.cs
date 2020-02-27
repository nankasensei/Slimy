using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossArm : MonoBehaviour
{
    public float HP_MAX;
    public bool isAlive;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject explosion;
    public RectTransform hitpoints;
    public GameObject hpBar;

    protected float hp;
    private Timer dieTimer;

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
        dieTimer = new Timer();
    }

    // Update is called once per frame
    void Update()
    {
        DieProgress();
    }

    public void TakeDamage(HitEventData data)
    {
        if (data.victim == gameObject && isAlive)
        {
            hp -= data.tama.GetComponent<Tama>().damage;
            hitpoints.localScale = new Vector3(hp / HP_MAX, 1, 1);

            if (hp <= 0)
            {
                Die();
            }
        }
    }

    void DieProgress()
    {
        if(dieTimer.isStart)
        {
            if(dieTimer.elapasedTime < 0.7f)
            {
                float unitDistance = 1 + Mathf.Sin(Mathf.PI + 0.5f * Mathf.PI * dieTimer.elapasedTime / 0.7f);
                if (gameObject.name == "RightArm")
                {
                    transform.position = transform.position + new Vector3(-0.15f * unitDistance, 0, -0.3f * unitDistance);
                    transform.localEulerAngles = new Vector3(90, 0, 100 * unitDistance + 39);
                }
                else
                {
                    transform.position = transform.position + new Vector3(0.15f * unitDistance, 0, -0.3f * unitDistance);
                    transform.localEulerAngles = new Vector3(90, 0, -100 * unitDistance + 127);
                }
            }
            else
            {
                dieTimer.Stop();
            }
        }
    }

    public void Die()
    {
        Instantiate(explosion, transform.position, Quaternion.Euler(90, 0, 0));
        Boss boss = GameObject.Find("Boss(Clone)").GetComponent<Boss>();
        isAlive = false;
        animator.enabled = false;
        spriteRenderer.color = new Color(90f / 255f, 90f / 255f, 90f / 255f);
        hp = 0;
        hpBar.SetActive(false);
        transform.parent = null;
        if(gameObject.name == "LeftArm")
            boss.leftArm = null;
        else
            boss.rightArm = null;
        boss.audioSource.PlayOneShot(boss.growlClip, 0.02f);
        transform.rotation = Quaternion.Euler(90, 0, 0);
        GetComponent<Collider>().isTrigger = true;

        dieTimer.Start();
    }

    public void Gone()
    {
        Destroy(gameObject);
    }

    public void Buff()
    {

    }
}
