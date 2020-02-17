using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public BoardManager boardScript;
    public float playerHp;
    public List<GameObject> enemies;//added in BoardManager
    public AudioSource audioSource;
    public AudioSource audioSourceBGM;
    public GameObject playerPrefab;

    private int level =1;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();

        playerHp =20;

        SlimyEvents.dieEvent.AddListener(GameOver);

        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("Player").GetComponent<PlayerController>().hp <= 0) audioSourceBGM.Stop();
    }

    private void OnLevelWasLoaded(int index)
    {
        SlimyEvents.levelStartEvent.Invoke();
        GameObject.Find("GameStart").SetActive(false);
        level++;
        InitGame();
    }

    void InitGame()
    {
        //GameObject.Find("UI").SetActive(false);
        GameObject player = Instantiate(playerPrefab);
        player.name = "Player";
        enemies.Clear();
        boardScript.SetupScene(level);
        audioSource.Play();
    }

    void GameOver()
    {
    }

     public void EnemiesCleared()
    {

        if(enemies.Count == 0)
        {
            GameObject exit = GameObject.FindGameObjectWithTag("Exit");
            exit.GetComponent<Collider>().isTrigger = true;
            exit.transform.GetChild(0).localPosition = new Vector3(0f, 0.7f, 0f);
            exit.GetComponent<AudioSource>().Play();

            //shrink the size of boxcollider for huge slimy to pass the exit
            GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>().size = new Vector3(0.4f, 0.8f, 1f);

            audioSource.Stop();
        }
    }
}
