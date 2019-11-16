using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    void Awake()
    {
        Screen.SetResolution(736, 416, false);

        if (GameManager.instance == null)
            Instantiate(gameManager);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
