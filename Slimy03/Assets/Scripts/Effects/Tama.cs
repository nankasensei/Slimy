using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tama : MonoBehaviour
{
    public Vector3 velocity;
    public GameObject player;

    public float damage = 4.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position + velocity * Time.deltaTime;
        Vector3 direction = velocity;
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit,(newPosition - transform.position).magnitude*7.0f))
        {
            GameObject other = hit.collider.gameObject;
            if (other != player)
            {
                if (other.CompareTag("Enemy") && other.GetComponent<Enemy>().isAlive)
                {
                    SlimyEvents.hitEvent.Invoke(new HitEventData(player, other, gameObject));
                    Destroy(gameObject);
                    //Debug.Log(hit.transform.name + " " + transform.position);
                }
                if (other.CompareTag("Arm") && other.GetComponent<BossArm>().isAlive)
                {
                    SlimyEvents.hitEvent.Invoke(new HitEventData(player, other, gameObject));
                    Destroy(gameObject);
                }
                if (other.CompareTag("BossBody") && other.GetComponent<BossBody>().isAlive)
                {
                    SlimyEvents.hitEvent.Invoke(new HitEventData(player, other, gameObject));
                    Destroy(gameObject);
                }
                if (other.CompareTag("breakableItem"))
                {
                    Item item = other.GetComponent<Item>();
                    item.TakeDamage(damage * GameObject.Find("Player").GetComponent<PlayerController>().ATK);
                    Destroy(gameObject);
                }
                if (other.CompareTag("unbreakableItem"))
                {
                }
                if (other.CompareTag("OuterWall") || other.CompareTag("Exit") || other.CompareTag("Enter"))
                {
                    Destroy(gameObject);
                }
                if (other.CompareTag("Torch"))
                {
                    other.GetComponent<Torch>().Light();
                    Destroy(gameObject);
                }
            }
        }
        transform.position = newPosition;
    }
}
