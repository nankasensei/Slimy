using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRat : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public const int HP_MAX = 9;

    private int hp;
    private bool isAlive;
    private bool isInvokeSet;

    //private Vector3 movementDirection;

    private void Awake()
    {
        SlimyEvents.hitEvent.AddListener(TakeDamage);
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        agent.updateRotation = false;
        transform.rotation = Quaternion.Euler(90, 0, 0);
        isAlive = true;
        isInvokeSet = false;
        hp = HP_MAX;
    }

    // Update is called once per frame
    void Update()
    {
        if ((GameObject.Find("Player").transform.position - transform.position).magnitude < 4.0f)
        {
            CancelInvoke();
            isInvokeSet = false;

            agent.SetDestination(GameObject.Find("Player").transform.position);
        }
        else
        {
            if (!isInvokeSet)
            {
                InvokeRepeating("Stroll", 1.0f, 3.0f);
                isInvokeSet = true;
            }
        }


        if(agent.desiredVelocity.magnitude > 0.1f)
        {
            animator.SetFloat("Horizontal", agent.desiredVelocity.x);
            animator.SetFloat("Vertical", agent.desiredVelocity.z);
        }
        animator.SetFloat("Magnitude", agent.desiredVelocity.magnitude);
        animator.speed = (agent.desiredVelocity.magnitude / 2.0f) * 2f;
    }

    public void TakeDamage(HitEventData data)
    {
        if(data.victim == gameObject)
        {
            hp -= data.tama.GetComponent<Tama>().damage;
            if (hp <= 0 && isAlive == true)
            { 
                isAlive = false;
                animator.enabled = false;
                agent.speed = 0.0f;
                spriteRenderer.color = new Color(90f / 255f, 90f / 255f, 90f / 255f);
                hp = 0;
                GameManager.instance.enemies.Remove(gameObject);
                GameManager.instance.EnemiesCleared();//detect if all enemies cleared
                GetComponent<Collider>().isTrigger = true;
            }
            if (hp <= HP_MAX * -1 && isAlive == false)
            {
                Gone();
            }
        }
    }

    void Stroll()
    {
        float newX = Random.Range(-10, 10);
        float newZ = Random.Range(-10, 10);
        Vector3 offset = new Vector3(newX, 0f, newZ);
        offset.Normalize();
        Vector3 strollPosition = transform.position + offset*2.0f;

        agent.SetDestination(strollPosition);
    }

    public void Gone()
    {
        Destroy(gameObject);
    }

    public int GetHP_MAX()
    {
        return HP_MAX;
    }
}
