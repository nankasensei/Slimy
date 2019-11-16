using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int PLAYER_HP_MAX = 12;
    public int playerHp = 12;
    public List<GameObject> enemies;//added in BoardManager

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
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }

    void InitGame()
    {
        enemies.Clear();
        boardScript.SetupScene(level);
    }

     public void EnemiesCleared()
    {

        if(enemies.Count == 0)
        {
            GameObject exit = GameObject.FindGameObjectWithTag("Exit");
            exit.GetComponent<Collider>().isTrigger = true;
            exit.transform.GetChild(0).localPosition = new Vector3(0f, 0.7f, 0f);

            //shrink the size of boxcollider for huge slimy to pass the exit
            GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>().size = new Vector3(0.4f, 0.8f, 1f);
        }
    }
}
