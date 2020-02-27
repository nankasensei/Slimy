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
    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip beamClip;
    public AudioClip growlClip;

    private Timer mainTimer;
    private Timer summonTimer;
    private Timer fireworkLoopTimer;
    private Timer beamLoopTimer;

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
        beamLoopTimer = new Timer();
    }

    // Update is called once per frame
    void Update()
    {
        if(body.GetComponent<BossBody>().isAlive)
        {
            if (mainTimer.elapasedTime > 2 && GameObject.Find("Player").GetComponent<PlayerController>().isAlive)
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

                    if (summonTimer.isStart && summonTimer.elapasedTime > 10)
                    {
                        Summon();
                        summonTimer.Stop();
                    }
                }

                if (!(leftArm == null && rightArm == null))
                {
                    //deal with Firework
                    if (!fireworkLoopTimer.isStart)
                    {
                        fireworkLoopTimer.Start();
                    }
                    else
                    {
                        if (fireworkLoopTimer.elapasedTime > 8)
                        {
                            FireWorkStart();
                            fireworkLoopTimer.Stop();
                        }
                    }
                }
                else
                {
                    //deal with Beam
                    if (!beamLoopTimer.isStart)
                    {
                        beamLoopTimer.Start();
                    }
                    else
                    {
                        if (beamLoopTimer.elapasedTime > 8)
                        {
                            BeamStart();
                            beamLoopTimer.Stop();
                        }
                    }
                }

                BodyDir();
                FireworkProgress();
                BeamProgress();
            }
            else
            {
                audioSource.Stop();
                if (GameObject.Find("BeamCharge(Clone)") != null)
                {
                    Destroy(GameObject.Find("BeamCharge(Clone)"));
                }
            }
        }
        else
        {
            if (leftArm != null)
                leftArm.GetComponent<BossArm>().Die();
            if (rightArm != null)
                rightArm.GetComponent<BossArm>().Die();
            for(int i = 0;i < GameManager.instance.enemies.Count;i++)
            {
                GameManager.instance.enemies[i].GetComponent<LittleDemon>().Die();
            }
            if (GameObject.Find("BeamCharge(Clone)") != null)
            {
                Destroy(GameObject.Find("BeamCharge(Clone)"));
            }
            audioSource.Stop();
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
        audioSource.clip = beamClip;
        audioSource.volume = 0.07f;
        audioSource.PlayDelayed(1);
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
                    if(beamTimer.elapasedTime > 1.5f)
                    {
                        Destroy(GameObject.Find("BeamCharge(Clone)"));
                        beamStep++;
                    }
                    break;
                case 2:
                    if(beamTimer.elapasedTime < 4f)
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
                        audioSource.Stop();
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
            if (rightArm != null)
                rightArm.transform.localEulerAngles = new Vector3(0, 0, 70f * Mathf.Sin(2 * Mathf.PI * fireworkTimer.elapasedTime / 3f));
            if (leftArm != null)
                leftArm.transform.localEulerAngles = new Vector3(0, 0, -70f * Mathf.Sin(2 * Mathf.PI * fireworkTimer.elapasedTime / 3f));

            if (fireworkTimer.elapasedTime - lastFireTime > 0.25f)
            {
                if(leftArm != null)
                    FireballShoot(0);
                if (rightArm != null)
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

        audioSource.PlayOneShot(shootClip, 0.03f);

        Destroy(fireball, 3.0f);
    }
}
