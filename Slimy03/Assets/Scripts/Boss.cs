using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform body;
    public Transform shootPos;
    public Transform leftArm;
    public Transform leftArmShootPos;
    public Transform rightArm;
    public Transform rightArmShootPos;

    private Timer mainTimer;
    private Timer summonTimer;
    private Timer fireworkLoopTimer;

    public GameObject LittleDemon;

    public GameObject fireballPrefab;
    public float FIREBALL_BASE_SPEED = 1.0f;
    public Timer fireworkTimer;
    private float lastFireTime;

    public GameObject beamPrefab;
    public GameObject beamChargePrefab;
    public float BEAM_BASE_SPEED = 1.0f;
    public Timer beamTimer;
    private int beamStep;
    private float lastBeamtime;

    // Start is called before the first frame update
    void Start()
    {
        mainTimer = new Timer();
        mainTimer.Start();
        summonTimer = new Timer();
        fireworkTimer = new Timer();
        beamTimer = new Timer();
        fireworkLoopTimer = new Timer();
    }

    // Update is called once per frame
    void Update()
    {
        if(mainTimer.elapasedTime > 2)
        {
            if (Input.GetMouseButtonUp(0))
            {
                //BeamStart();
            }

            //deal with Summon
            {
                if (GameManager.instance.enemies.Count == 0)
                {
                    if (!summonTimer.isStart)
                        summonTimer.Start();
                }

                if (summonTimer.isStart && summonTimer.elapasedTime > 5)
                {
                    Summon();
                    summonTimer.Stop();
                }
            }

            //deal with Firework
            {
                if (!fireworkLoopTimer.isStart)
                {
                    fireworkLoopTimer.Start();
                }
                else
                {
                    if (fireworkLoopTimer.elapasedTime > 6)
                    {
                        FireWorkStart();
                        fireworkLoopTimer.Stop();
                    }
                }
            }

            BodyDir();
            FireworkProgress();
            BeamProgress();
        }
    }

    void BodyDir()
    {
        if (GameObject.Find("Player").transform.position.x > transform.position.x)
            body.localScale = new Vector3(1, 1, 1);
        else
            body.localScale = new Vector3(-1, 1, 1);
    }

    void Summon()
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject newObject = Instantiate(LittleDemon, new Vector3(5 + 3*i, 0, 4), Quaternion.Euler(90, 0, 0));
            GameManager.instance.enemies.Add(newObject);
        }
    }

    void BeamStart()
    {
        beamTimer.Start();
        lastBeamtime = 0;
        beamStep = 0;
        if(GameObject.Find("BeamCharge(Clone)") != null)
        {
            Destroy(GameObject.Find("BeamCharge(Clone)"));
        }
    }

    void BeamProgress()
    {
        if(beamTimer.isStart)
        {
            switch(beamStep)
            {
                case 0:
                    GameObject beamCharge = Instantiate(beamChargePrefab, shootPos.position, Quaternion.Euler(90, 0, 0));
                    beamStep++;
                    break;
                case 1:
                    if(beamTimer.elapasedTime > 1)
                    {
                        Destroy(GameObject.Find("BeamCharge(Clone)"));
                        beamStep++;
                    }
                    break;
                case 2:
                    if(beamTimer.elapasedTime < 3)
                    {
                        if (beamTimer.elapasedTime - lastBeamtime > 0.0001f)
                        {
                            BeamOneShot();
                            lastBeamtime = beamTimer.elapasedTime;
                        }
                    }
                    else
                    {
                        beamTimer.Stop();
                        lastBeamtime = 0;
                        beamStep = 0;
                    }
                    break;
            }
        }
    }

    void FireWorkStart()
    {
        fireworkTimer.Start();
        lastFireTime = 0;
    }

    void FireworkProgress()
    {
        if (fireworkTimer.isStart)
        {
            rightArm.transform.localEulerAngles = new Vector3(0, 0, 70f * Mathf.Sin(2 * Mathf.PI * fireworkTimer.elapasedTime / 3f));
            leftArm.transform.localEulerAngles = new Vector3(0, 0, -70f * Mathf.Sin(2 * Mathf.PI * fireworkTimer.elapasedTime / 3f));

            if (fireworkTimer.elapasedTime - lastFireTime > 0.25f)
            {
                FireballShoot(0);
                FireballShoot(1);
                lastFireTime = fireworkTimer.elapasedTime;
            }

            if (fireworkTimer.elapasedTime >= 3)
            {
                fireworkTimer.Stop();
                lastFireTime = 0;
            }
        }
    }

    void BeamOneShot()
    {
        Vector3 shootPosition;
        Vector3 startPos;
        Vector3 desPos;

        shootPosition = shootPos.position;
        startPos = transform.position;
        desPos = GameObject.Find("Player").transform.position;

        Vector3 shootingDirection = new Vector3(desPos.x - startPos.x, 0.0f, desPos.z - startPos.z);
        shootingDirection.Normalize();

        GameObject beam = Instantiate(beamPrefab, shootPosition, Quaternion.Euler(90, 0, Mathf.Atan2(shootingDirection.z, shootingDirection.x) * Mathf.Rad2Deg));
        Fireball beamScript = beam.GetComponent<Fireball>();
        beamScript.boss = gameObject;
        beamScript.velocity = shootingDirection * BEAM_BASE_SPEED;

        Destroy(beam, 3.0f);
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
