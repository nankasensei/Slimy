using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject UI;

    void Awake()
    {
        Screen.SetResolution(2944, 1664, false);
        UI.SetActive(true);
    }

    public void OnClick()
    {
        if (GameManager.instance == null)
        { 
            SlimyEvents.gameStartEvent.Invoke();
            Instantiate(gameManager);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
