using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Player statistics")]
    public float PLAYER_HP_MAX;
    public Vector3 movementDirection;
    public float movementSpeed;
    public float hp;
    public float defence;


    [Header("Player attributes")]
    public float MOVEMENT_BASE_SPEED = 1.0f;
    public float TAMA_BASE_SPEED = 1.0f;
    public int TAMA_COST = 1;
    public float DEF = 1;


    [Header("References")]
    public Rigidbody rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public AudioSource audioSource;
    public AudioClip walking;
    public AudioClip swallowing;
    public AudioClip attacking;
    public AudioClip hitting;
    public AudioClip dying;

    [Header("Prefabs")]
    public GameObject slimyTama;
    public ParticleSystem effect;

    private float enterExitY;
    private float exitExitY;
    private bool endOfAiming;
    private float movementSpeedOffset;

    void Awake()
    {
        SlimyEvents.hitEvent.AddListener(TakeDamage);
    }
    // Start is called before the first frame update
    void Start()
    {
        hp = GameManager.instance.playerHp;
        defence = DEF;
        movementSpeedOffset = 1f;
        StateSetup();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        Move();
        ShootWithMouse();
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

        if (Input.GetKeyDown("joystick button 0"))
        {
            ShootWithGamepad(0, -1);
        }
        if (Input.GetKeyDown("joystick button 1"))
        {
            ShootWithGamepad(1, 0);
        }
        if (Input.GetKeyDown("joystick button 2"))
        {
            ShootWithGamepad(-1, 0);
        }
        if (Input.GetKeyDown("joystick button 3"))
        {
            ShootWithGamepad(0, 1);
        }
    }

    public void SetHp(float offset)
    {
        hp += offset;
        StateSetup();
    }

    public IEnumerator SetDefence(float offset, float time)
    {
        defence = DEF + offset;
        StateSetup();

        if(offset > 0)
        {
            effect.Play();
            yield return new WaitForSeconds(time);
            defence = DEF;
        }
    }

    public void SetDef(float offset, float time)
    {
        StartCoroutine(SetDefence(offset, time));
    }
    
    //upgrade the state
    void StateSetup()
    {
        if(hp <= 0) Die();
        else if(hp > 2 * PLAYER_HP_MAX) hp = 2 * PLAYER_HP_MAX;
        float scale = 1.0f*hp / PLAYER_HP_MAX;
        if (scale < 0.5f)
            scale = 0.5f;
        else if (scale > 2.0f)
            scale = 2.0f;
        transform.localScale = new Vector3(scale, scale ,1.0f);

        movementSpeedOffset = 1.0f * PLAYER_HP_MAX / hp;
        if (movementSpeedOffset < 0.75f)
            movementSpeedOffset = 0.75f;
        else if (movementSpeedOffset > 1.25f)
            movementSpeedOffset = 1.25f;

        if(defence > 2.0f) defence = 2.0f;
        else if(defence < 0.5f) defence = 0.5f;
    }

    void Move()
    {
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED * movementSpeedOffset;

        audioSource.clip = walking;
        if(rb.velocity.magnitude != 0 && !audioSource.isPlaying) audioSource.Play();
    }

    void ShootWithMouse()
    {
        if (endOfAiming)
        {
            Vector3 mousePosition3d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 shootingDirection = new Vector3(mousePosition3d.x - transform.position.x, 0.0f,mousePosition3d.z - transform.position.z);
            shootingDirection.Normalize();

            GameObject tama = Instantiate(slimyTama, transform.position, Quaternion.Euler(90, 0, Mathf.Atan2(shootingDirection.z, shootingDirection.x) * Mathf.Rad2Deg));
            //tama.GetComponent<Rigidbody>().velocity = shootingDirection * TAMA_BASE_SPEED;
            Tama tamaScript = tama.GetComponent<Tama>();
            tamaScript.player = gameObject;
            tamaScript.velocity = shootingDirection * TAMA_BASE_SPEED;

            //Debug.Log(Mathf.Atan2(shootingDirection.z, shootingDirection.x) * Mathf.Rad2Deg);

            Destroy(tama, 3.0f);

            audioSource.PlayOneShot(attacking);

            hp -= TAMA_COST;
            StateSetup();
        }
    }

    void ShootWithGamepad(int x, int y)
    {
        Vector3 shootingDirection = new Vector3(x, 0.0f, y);
        shootingDirection.Normalize();

        GameObject tama = Instantiate(slimyTama, transform.position, Quaternion.Euler(90, 0, Mathf.Atan2(shootingDirection.z, shootingDirection.x) * Mathf.Rad2Deg));

        Tama tamaScript = tama.GetComponent<Tama>();
        tamaScript.player = gameObject;
        tamaScript.velocity = shootingDirection * TAMA_BASE_SPEED;

        Destroy(tama, 3.0f);

        audioSource.PlayOneShot(attacking);

        hp -= TAMA_COST;
        StateSetup();
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

            audioSource.PlayOneShot(hitting);

            hp -= data.meleeDamage  / defence;
            
            StateSetup();
        }
    }

    void Die()
    {
        audioSource.PlayOneShot(dying);
        animator.enabled = false;
        spriteRenderer.color = new Color(90f / 255f, 90f / 255f, 90f / 255f);
        this.enabled = false;
        SlimyEvents.dieEvent.Invoke();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Exit")
        {
            enterExitY = transform.position.z;
        }

        if (other.tag == "Rock" || other.tag == "Pot")
        {
            other.gameObject.GetComponent<Item>().Buff();
            other.gameObject.GetComponent<Item>().Gone();
            StateSetup();
        }

        if (other.tag == "Enemy" && !other.GetComponent<Enemy>().isAlive)
        {
            audioSource.PlayOneShot(swallowing);

            other.gameObject.GetComponent<Enemy>().Buff();
            other.gameObject.GetComponent<Enemy>().Gone();
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
