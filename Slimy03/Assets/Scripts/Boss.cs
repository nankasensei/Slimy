using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform body;
    public Transform leftArm;
    public Transform leftArmShootPos;
    public Transform rightArm;
    public Transform rightArmShootPos;

    public GameObject fireballPrefab;
    public float FIREBALL_BASE_SPEED = 1.0f;

    public Timer fireworkTimer;
    private float lastFireTime;

    // Start is called before the first frame update
    void Start()
    {
        fireworkTimer = new Timer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            fireworkTimer.Start();

        BodyDir();

        Firework();
    }

    void BodyDir()
    {
        if (GameObject.Find("Player").transform.position.x > transform.position.x)
            body.localScale = new Vector3(1, 1, 1);
        else
            body.localScale = new Vector3(-1, 1, 1);
    }

    void Firework()
    {
        if (fireworkTimer.isStart)
        {
            rightArm.transform.localEulerAngles = new Vector3(0,0,60f * Mathf.Sin(2 * Mathf.PI * fireworkTimer.elapasedTime/3f));
            leftArm.transform.localEulerAngles = new Vector3(0, 0, -60f * Mathf.Sin(2 * Mathf.PI * fireworkTimer.elapasedTime / 3f));

            if(fireworkTimer.elapasedTime - lastFireTime > 0.25f)
            {
                FireballShoot(0);
                FireballShoot(1);
                lastFireTime = fireworkTimer.elapasedTime;
            }

            if(fireworkTimer.elapasedTime >= 3)
            {
                fireworkTimer.Stop();
                lastFireTime =  0;
            }
        }
    }

    void FireballShoot(int shootArm)//0=left hand, 1= right
    {
        Vector3 shootPosition;
        Vector3 startPos;
        Vector3 desPos;
        if (shootArm == 0)
        {
            shootPosition = leftArmShootPos.position;
            startPos = leftArm.position;
        }
        else
        {
            shootPosition = rightArmShootPos.position;
            startPos = rightArm.position;
        }
        desPos = shootPosition;

        Vector3 shootingDirection = new Vector3(desPos.x - startPos.x, 0.0f, desPos.z - startPos.z);
        shootingDirection.Normalize();

        GameObject fireball = Instantiate(fireballPrefab, shootPosition, Quaternion.Euler(90, 0, Mathf.Atan2(shootingDirection.z, shootingDirection.x) * Mathf.Rad2Deg));
        Fireball fireballScript = fireball.GetComponent<Fireball>();
        fireballScript.boss = gameObject;
        fireballScript.velocity = shootingDirection * FIREBALL_BASE_SPEED;

        Destroy(fireball, 3.0f);

        //audioSource.PlayOneShot(attacking);
    }
}
