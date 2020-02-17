using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public Transform spriteMask;

    [Range(0.05f, 0.2f)]
    public float flickTime;

    [Range(0.02f, 0.09f)]
    public float addSize;

    private float time;
    private bool isBecomingLarger;
    // Start is called before the first frame update
    void Start()
    {
        time = 0f;
        isBecomingLarger = false;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time > flickTime)
        {
            if(isBecomingLarger)
            {
                spriteMask.localScale = new Vector3(spriteMask.localScale.x + addSize, spriteMask.localScale.y, spriteMask.localScale.z + addSize);
            }
            else
            {
                spriteMask.localScale = new Vector3(spriteMask.localScale.x - addSize, spriteMask.localScale.y, spriteMask.localScale.z - addSize);
            }
            time = 0f;
            isBecomingLarger = !isBecomingLarger;
        }
    }
}
