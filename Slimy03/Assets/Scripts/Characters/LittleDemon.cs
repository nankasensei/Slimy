/*---------------------------------------Little Demon Script----------------------------------------
 The Hit, Horizontal, Vertical parameters are not being used, only Magnitude works
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleDemon : Enemy
{
    //private Vector3 movementDirection;
    private const float ATK = 5;
    private float attackRadius;
    private Timer attackTimer;
    public Transform attackPos;
    public Transform attackAnchor;
    public LayerMask playerLayer;
    private float buffHp;
    private Timer mainTimer;

    private void Awake()
    {
        //call TakeDamage() when hirEvent happens, also pass HitEventData at the same time
        SlimyEvents.hitEvent.AddListener(TakeDamage);
    }

    // Start is called before the first frame update
    void Start()
    {
        id = "LittleDemon";
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        agent.updateRotation = false;
        transform.rotation = Quaternion.Euler(90, 0, 0);
        isAlive = true;
        isInvokeSet = false;
        hp = HP_MAX;
        attackRadius = 0.4f;
        buffHp = 15;
        mainTimer = new Timer();
        mainTimer.Start();
        attackTimer = new Timer();
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive)
        {
            if (mainTimer.elapasedTime > 2 && GameObject.Find("Player").GetComponent<PlayerController>().isAlive)
            {
                BodyDir();

                animator.SetFloat("Magnitude", agent.desiredVelocity.magnitude);

                if ((GameObject.Find("Player").transform.position - transform.position).magnitude < float.MaxValue)
                {
                    CancelInvoke("Stroll");
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

                //start walking
                if (agent.desiredVelocity.magnitude > 0.1f)
                {
                    direction = new Vector2(agent.desiredVelocity.x, agent.desiredVelocity.z).normalized;

                    attackPos.LookAt(GameObject.Find("Player").transform);
                }
                else if (isInvokeSet == false)
                {
                    Vector3 vector = GameObject.Find("Player").transform.position - transform.position;
                    Vector2 direction = new Vector2(vector.x, vector.z).normalized;

                    attackPos.LookAt(GameObject.Find("Player").transform);
                }

                //start attacking
                if (agent.desiredVelocity.magnitude < 0.1f && (GameObject.Find("Player").transform.position - transform.position).magnitude < 3.0f)
                {
                    if (attackTimer.elapasedTime > 2)
                    {
                        Attack();
                        attackTimer.Start(); //restart the timer to count down
                    }
                }
            }
        }
    }

    void BodyDir()
    {
        if (GameObject.Find("Player").transform.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Attack()
    {
        //animator.SetTrigger("Attack");

        Collider[] hitColliders = Physics.OverlapSphere(attackAnchor.position, attackRadius, playerLayer);
        int i = 0;
        while (i < hitColliders.Length)
        {
            SlimyEvents.hitEvent.Invoke(new HitEventData(gameObject, hitColliders[i].gameObject, ATK));
            i++;
        }
    }

    public override void Buff()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().SetHp(buffHp);
    }

    public override void Die()
    {
        isAlive = false;
        animator.enabled = false;
        agent.speed = 0.0f;
        spriteRenderer.color = new Color(90f / 255f, 90f / 255f, 90f / 255f);
        hp = 0;
        hpBar.SetActive(false);
        GameManager.instance.enemies.Remove(gameObject);
        //GameManager.instance.EnemiesCleared();//detect if all enemies cleared
        //GetComponent<Collider>().isTrigger = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(attackAnchor.position, attackRadius);
    }
}
