using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject gameStartUI;
    public GameObject gameOverUI;
    public GameObject gameClearUI;
    public CanvasGroup modeHUD;
    public Text modeHUD_text;

    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        SlimyEvents.dieEvent.AddListener(GameOver);
        SlimyEvents.gameStartEvent.AddListener(GameStart);
        SlimyEvents.levelStartEvent.AddListener(LevelStart);
        SlimyEvents.gameClearEvent.AddListener(GameClear);
        SlimyEvents.gameRestartEvent.AddListener(GameRestart);
        SlimyEvents.modeSwitchEvent.AddListener(ModeSwitchStart);

        timer = new Timer();
    }

    // Update is called once per frame
    void Update()
    {
        ModeSwitchProcess();
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

    void ModeSwitchStart()
    {
        timer.Start();
        modeHUD.alpha = 1;
        switch(GameManager.instance.GetComponent<GameManager>().controlMod)
        {
            case 0:
                modeHUD_text.text = "MOTION MODE";
                break;
            case 1:
                modeHUD_text.text = "JOYSTICK MODE";
                break;
            case 2:
                modeHUD_text.text = "FINGER MODE";
                break;
        }
    }

    void ModeSwitchProcess()
    {
        if(timer.isStart)
        {
            if(timer.elapasedTime > 1)
            {
                modeHUD.alpha = 1 - (timer.elapasedTime - 1)*2 ;
            }
            else if(timer.elapasedTime > 1.5f)
            {
                modeHUD.alpha = 0;
                timer.Stop();
            }
        }
    }
}
