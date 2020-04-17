using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMenu : MonoBehaviour
{
    public bool visible;

    void Start()
    {
        if (GetComponent<SpriteRenderer>().enabled)
            visible = true;
        else
            visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (visible)
            {
                visible = false;
                GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                visible = true;
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
