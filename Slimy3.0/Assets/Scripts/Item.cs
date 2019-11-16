using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    const int POT_HP = 6;
    const int ROCK_HP = 10;

    public int hp;
    public int hpMax;
    public SpriteRenderer spriteRenderer;

    private bool isAlive;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag == "Pot")
            hpMax = POT_HP;
        if (gameObject.tag == "Rock")
            hpMax = ROCK_HP;

        hp = hpMax;
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0 && isAlive == true)
        {
            isAlive = false;
            spriteRenderer.color = new Color(90f / 255f, 90f / 255f, 90f / 255f);
            hp = 0;
            GameManager.instance.enemies.Remove(gameObject);
            GameManager.instance.EnemiesCleared();//detect if all enemies cleared
            GetComponent<Collider>().isTrigger = true;
        }
        if (hp <= hpMax * -1 && isAlive == false)
        {
            Gone();
        }
    }

    public void Gone()
    {
        Destroy(gameObject);
    }
}
