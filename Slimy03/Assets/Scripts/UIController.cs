using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject gameStartUI;
    public GameObject gameOverUI;
    public GameObject gameClearUI;
    // Start is called before the first frame update
    void Start()
    {
        SlimyEvents.dieEvent.AddListener(GameOver);
        SlimyEvents.gameStartEvent.AddListener(GameStart);
        SlimyEvents.levelStartEvent.AddListener(LevelStart);
        SlimyEvents.gameClearEvent.AddListener(GameClear);
        SlimyEvents.gameRestartEvent.AddListener(GameRestart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    void GameStart()
    {
        gameStartUI.SetActive(false);
    }

    void GameClear()
    {
        gameClearUI.SetActive(true);
    }

    void LevelStart()
    {
        gameStartUI.SetActive(false);
    }

    void GameRestart()
    {
    }
}
