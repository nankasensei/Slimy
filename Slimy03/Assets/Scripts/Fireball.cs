using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public Vector3 velocity;
    public GameObject boss;

    public float damage = 4.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position + velocity * Time.deltaTime;
        Vector3 direction = velocity;
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, (newPosition - transform.position).magnitude * 2.0f))
        {
            GameObject other = hit.collider.gameObject;
            if (other != boss)
            {
                if (other.CompareTag("Player"))
                {
                    //SlimyEvents.hitEvent.Invoke(new HitEventData(player, other, gameObject));
                    Destroy(gameObject);
                }
                if (other.CompareTag("Pot") || other.CompareTag("Rock"))
                {
                    Item item = other.GetComponent<Item>();
                    item.TakeDamage(damage);
                    Destroy(gameObject);
                }
                if (other.CompareTag("OuterWall") || other.CompareTag("Exit") || other.CompareTag("Enter"))
                {
                    Destroy(gameObject);
                }
            }
        }
        transform.position = newPosition;
    }
}
