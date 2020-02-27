using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float HP_MAX;
    public float hp;

    public SpriteRenderer spriteRenderer;

    protected bool isAlive;

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0 && isAlive == true)
        {
            isAlive = false;
            spriteRenderer.color = new Color(90f / 255f, 90f / 255f, 90f / 255f);
            hp = 0;
            GetComponent<Collider>().isTrigger = true;
        }
    }

    public virtual void Buff(){}


    public void Gone()
    {
        Destroy(gameObject);
    }
}
