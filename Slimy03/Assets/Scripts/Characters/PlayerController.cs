using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Player statistics")]
    public static float PLAYER_HP_MAX = 100;
    public Vector3 movementDirection;
    public float movementSpeed;
    public float hp;
    public float defence;
    public float attack;
    public bool isAlive;
    public int mode; //0=Normal, 1=EX

    [Header("Player attributes")]
    public float MOVEMENT_BASE_SPEED = 1.0f;
    public float TAMA_BASE_SPEED = 1.0f;
    public int TAMA_COST = 1;
    public float DEF = 1;
    public float ATK = 1;
    public float TAMA_RATE = 0.33f;


    [Header("References")]
    public Rigidbody rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform spriteTransform;
    public AudioSource audioSource;
    public DS4Controllor ds4;
    public AudioClip walking;
    public AudioClip swallowing;
    public AudioClip attacking;
    public AudioClip hitting;
    public AudioClip dying;
    public float walkingVolume;
    public float swallowingVolume;
    public float attackingVolume;
    public float hittingVolume;
    public float dyingVolume;
    public Transform effectsTrans;

    [Header("Prefabs")]
    public GameObject slimyTama;
    public GameObject slimyTamaEX;
    public GameObject slimyTamaLarge;
    public GameObject effectPrefab;

    private float enterExitY;
    private float exitExitY;
    private bool endOfAiming;
    private float movementSpeedOffset;
    private int eatArmCount;
    private GameObject tamaPrefab;
    private float lastShotTime;

    void Awake()
    {
        SlimyEvents.hitEvent.AddListener(TakeDamage);
    }
    // Start is called before the first frame update
    void Start()
    {
        hp = GameManager.instance.playerHp;
        defence = DEF;
        attack = ATK;
        isAlive = true;
        movementSpeedOffset = 1f;
        tamaPrefab = slimyTama;
        StateSetup();
        mode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(hp);
        if(!GameManager.instance.sceneStartTimer.isStart && isAlive)
        {
            ProcessInputs();
            Move();
            ShootWithMouse();
        }
    }

    private void OnDisable()
    {
        GameManager.instance.playerHp = hp;
    }

    void ProcessInputs()
    {
        movementDirection = new Vector3(Input.GetAxis("Horizontal"),0.0f, Input.GetAxis("Vertical"));
        movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0.0f, 1.0f);
        movementDirection.Normalize();

        if(movementDirection.magnitude > 0)
        {
            animator.SetFloat("Horizontal", movementDirection.x);
            animator.SetFloat("Vertical", movementDirection.z);
        }
        animator.SetFloat("Magnitude", movementDirection.magnitude);

        endOfAiming = Input.GetMouseButtonUp(0);
    }

    public IEnumerator SetHitpoints(float offset, float time)
    {
        hp += offset;
        StateSetup();

        if (offset > 0)
        {
            GameObject effectObject = Instantiate(effectPrefab, effectsTrans);
            ParticleSystem effect = effectObject.GetComponent<ParticleSystem>();

            var main = effect.main;
            main.duration = time;
            main.startColor = new Color(0.31f, 0.95f, 0.66f, 1);
            effect.Play();

            yield return new WaitForSeconds(time);
            Destroy(effectObject);
        }
    }

        public void SetHp(float offset)
    {
        StartCoroutine(SetHitpoints(offset, 2.0f));
    }

    public IEnumerator SetDefence(float offset, float time)
    {
        float preDef = defence;
        defence = DEF + offset;
        StateSetup();

        if(offset > 0 && preDef <= DEF)
        {
            GameObject effectObject = Instantiate(effectPrefab, effectsTrans);
            ParticleSystem effect = effectObject.GetComponent<ParticleSystem>();

            var main = effect.main;
            main.duration = time;
            main.startColor = new Color(0.943f, 0.524f, 0.302f, 1);
            effect.Play();

            yield return new WaitForSeconds(time);
            Destroy(effectObject);
            defence = DEF;
            StateSetup();
        }
    }

    public void SetDef(float offset, float time)
    {
        StartCoroutine(SetDefence(offset, time));
    }

    public IEnumerator SetAttack(float offset, float time)
    {
        float preAtk = attack;
        attack = ATK + offset;
        StateSetup();

        if (offset > 0 && preAtk <= ATK)
        {
            GameObject effectObject = Instantiate(effectPrefab, effectsTrans);
            ParticleSystem effect = effectObject.GetComponent<ParticleSystem>();

            var main = effect.main;
            main.duration = time;
            main.startColor = new Color(0.6821f, 0, 1, 1);

            effect.Play();

            yield return new WaitForSeconds(time);
            Destroy(effectObject);
            defence = ATK;
            StateSetup();
        }
    }

    public void SetAtk(float offset, float time)
    {
        StartCoroutine(SetAttack(offset, time));
    }

    //upgrade the state
    public void StateSetup()
    {
        if(hp <= 0)
            Die();
        else if(hp > 2 * PLAYER_HP_MAX)
            hp = 2 * PLAYER_HP_MAX;

        float scale = 1.0f*hp / PLAYER_HP_MAX;

        if (scale < 0.5f)
            scale = 0.5f;
        else if (scale > 2.0f)
            scale = 2.0f;

        transform.localScale = new Vector3(scale, scale ,1.0f);

        if(hp > PLAYER_HP_MAX)
        {
            movementSpeedOffset = 1.0f * (PLAYER_HP_MAX / hp) * (PLAYER_HP_MAX / hp);
        }
        else
        {
            movementSpeedOffset = 1.0f * (PLAYER_HP_MAX / hp);
        }

        if (movementSpeedOffset < 0.75f)
            movementSpeedOffset = 0.75f;
        else if (movementSpeedOffset > 1.25f)
            movementSpeedOffset = 1.25f;

        if(defence > 2.0f)
            defence = 2.0f;
        else if(defence < 0.5f)
            defence = 0.5f;

        if (attack > 2.0f)
            attack = 2.0f;
        else if (attack < 0.5f)
            attack = 0.5f;
    }

    void Move()
    {
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED * movementSpeedOffset;

        audioSource.clip = walking;
        audioSource.volume = walkingVolume;
        if(rb.velocity.magnitude != 0 && !audioSource.isPlaying) audioSource.Play();
    }

    void ShootWithMouse()
    {
        if (endOfAiming && Time.time - lastShotTime > TAMA_RATE)
        {
            Vector3 mousePosition3d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 shootingDirection = new Vector3(mousePosition3d.x - transform.position.x, 0.0f,mousePosition3d.z - transform.position.z);
            shootingDirection.Normalize();

            GameObject tama = Instantiate(tamaPrefab, transform.position, Quaternion.Euler(90, 0, Mathf.Atan2(shootingDirection.z, shootingDirection.x) * Mathf.Rad2Deg));
            //tama.GetComponent<Rigidbody>().velocity = shootingDirection * TAMA_BASE_SPEED;
            Tama tamaScript = tama.GetComponent<Tama>();
            tamaScript.player = gameObject;
            tamaScript.velocity = shootingDirection * TAMA_BASE_SPEED;
            lastShotTime = Time.time;
            Destroy(tama, 3.0f);

            audioSource.PlayOneShot(attacking, attackingVolume);

            hp -= TAMA_COST;
            StateSetup();
        }
    }

    public void ShootWithGamepad(float x, float y)
    {
        if (Time.time - lastShotTime > TAMA_RATE)
        {
            Vector3 shootingDirection = new Vector3(x, 0.0f, y);
            shootingDirection.Normalize();

            GameObject tama = Instantiate(tamaPrefab, transform.position, Quaternion.Euler(90, 0, Mathf.Atan2(shootingDirection.z, shootingDirection.x) * Mathf.Rad2Deg));

            Tama tamaScript = tama.GetComponent<Tama>();
            tamaScript.player = gameObject;
            tamaScript.velocity = shootingDirection * TAMA_BASE_SPEED;
            lastShotTime = Time.time;
            Destroy(tama, 3.0f);

            audioSource.PlayOneShot(attacking, attackingVolume);

            ds4.Vibrate(0, 1, 0.1f);
            ds4.Vibrate(1, 1, 0.1f);

            hp -= TAMA_COST;
            StateSetup();
        }
    }

    public void ChangeTama(int index)
    {
        switch(index)
        {
            //normal
            case 0:
                tamaPrefab = slimyTama;
                break;
            //large
            case 1:
                tamaPrefab = slimyTamaLarge;
                break;
            //EX
            case 2:
                tamaPrefab = slimyTamaEX;
                break;
        }
    }

    void TakeDamage(HitEventData data)
    {
        if(data.victim == gameObject && hp > 0)
        {
            Vector3 damageVector = data.shooter.transform.position - transform.position;
            Vector2 damageDirection = new Vector2(damageVector.x, damageVector.z).normalized;
            animator.SetFloat("Horizontal", damageDirection.x);
            animator.SetFloat("Vertical", damageDirection.y);
            animator.SetTrigger("Hit");

            ds4.Vibrate(0,1,0.2f);
            ds4.Vibrate(1, 1, 0.2f);

            audioSource.PlayOneShot(hitting, hittingVolume);

            hp -= data.meleeDamage  / defence;
            
            StateSetup();
        }
    }

    void Die()
    {
        isAlive = false;
        audioSource.PlayOneShot(dying, dyingVolume);
        animator.enabled = false;
        spriteRenderer.color = new Color(90f / 255f, 90f / 255f, 90f / 255f);
        movementDirection = Vector3.zero;
        //this.enabled = false;
        rb.velocity = Vector3.zero;
        SlimyEvents.dieEvent.Invoke();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Exit")
        {
            enterExitY = transform.position.z;
        }

        if (other.tag == "unbreakableItem")
        {
            audioSource.PlayOneShot(swallowing, swallowingVolume);
            other.gameObject.GetComponent<Item>().Buff();
            other.gameObject.GetComponent<Item>().Gone();
        }

        if (other.tag == "Enemy" && !other.GetComponent<Enemy>().isAlive)
        {
            audioSource.PlayOneShot(swallowing, swallowingVolume);

            other.gameObject.GetComponent<Enemy>().Buff();
            other.gameObject.GetComponent<Enemy>().Gone();
        }

        if (other.tag == "Arm" && !other.GetComponent<BossArm>().isAlive)
        {
            audioSource.PlayOneShot(swallowing, swallowingVolume);

            eatArmCount++;
            if (eatArmCount == 2)
            {
                mode = 1;
            }
            other.gameObject.GetComponent<BossArm>().Gone();
        }

        if (other.tag == "BossBody" && !GameObject.Find("Boss(Clone)").GetComponent<Boss>().isAlive)
        {
            audioSource.PlayOneShot(swallowing, swallowingVolume);

            other.gameObject.GetComponent<BossBody>().Buff();
            //other.gameObject.GetComponent<BossBody>().Gone();
            Destroy(GameObject.Find("Boss(Clone)"));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Exit")
        {
            exitExitY = transform.position.z;
            if(Mathf.Abs(enterExitY - exitExitY) > 1.0f)
            {
                Invoke("Restart", 0.5f);
                enabled = false;
            }
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
