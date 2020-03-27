using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject startUI;
    public GameObject overUI;
    public GameObject clearUI;

    void Awake()
    {
        //Screen.SetResolution(2944, 1664, false);
        Screen.fullScreen = true;
    }

    public void OnClick()
    {
        if (GameManager.instance == null)
        { 
            SlimyEvents.gameStartEvent.Invoke();
            Instantiate(gameManager);
        }
    }

    public void GameRestart()
    {
        GameManager.instance.GetComponent<GameManager>().GameRestart();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 2") && startUI.active) //circlr
        {
            SlimyEvents.gameStartEvent.Invoke();
            Instantiate(gameManager);
        }

        if(Input.GetKeyDown("joystick button 2") && (overUI.active || clearUI.active))
        {
            GameRestart();
        }
    }
}
