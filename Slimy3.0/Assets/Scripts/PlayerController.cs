using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Player statistics")]
    public Vector3 movementDirection;
    public float movementSpeed;
    public int hp;


    [Header("Player attributes")]
    public float MOVEMENT_BASE_SPEED = 1.0f;
    public float TAMA_BASE_SPEED = 1.0f;
    public int TAMA_COST = 1;


    [Header("References")]
    public Rigidbody rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Prefabs")]
    public GameObject slimyTama;

    private float enterExitY;
    private float exitExitY;
    private bool endOfAiming;
    private float movementSpeedOffset;

    // Start is called before the first frame update
    void Start()
    {
        hp = GameManager.instance.playerHp;
        movementSpeedOffset = 1f;
        StateSetup();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        Move();
        Shoot();
        Die();
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

        animator.SetFloat("Horizontal", movementDirection.x);
        animator.SetFloat("Vertical", movementDirection.z);
        animator.SetFloat("Magnitude", movementDirection.magnitude);

        endOfAiming = Input.GetMouseButtonUp(0);
    }

    void StateSetup()
    {
        float scale = 1.0f*hp / GameManager.instance.PLAYER_HP_MAX;
        if (scale < 0.5f)
            scale = 0.5f;
        else if (scale > 2.0f)
            scale = 2.0f;
        transform.localScale = new Vector3(scale, scale ,1.0f);

        movementSpeedOffset = 1.0f * GameManager.instance.PLAYER_HP_MAX / hp;
        if (movementSpeedOffset < 0.75f)
            movementSpeedOffset = 0.75f;
        else if (movementSpeedOffset > 1.25f)
            movementSpeedOffset = 1.25f;
    }

    void Move()
    {
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED * movementSpeedOffset;
    }

    void Shoot()
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

            hp -= TAMA_COST;
            StateSetup();
        }
    }

    void Die()
    {
        if (hp <= 0)
        {
            animator.enabled = false;
            spriteRenderer.color = new Color(90f / 255f, 90f / 255f, 90f / 255f);
            this.enabled = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Exit")
        {
            enterExitY = transform.position.z;
        }

        if (other.tag == "Pot")
        {
            other.gameObject.GetComponent<Item>().Gone();
            StateSetup();
        }

        if (other.tag == "Rock")
        {
            other.gameObject.GetComponent<Item>().Gone();
            StateSetup();
        }

        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyRat>().Gone();
            hp += other.gameObject.GetComponent<EnemyRat>().GetHP_MAX() / 2-1;
            StateSetup();
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
