using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMenu : MonoBehaviour
{
    GameObject controlsMenu;
    GameObject findYourVoice;
    GameObject escape;

    void Start()
    {
        controlsMenu = transform.GetChild(0).gameObject;
        controlsMenu.SetActive(false);
        findYourVoice = transform.GetChild(3).gameObject;
        findYourVoice.SetActive(false);
        escape = transform.GetChild(4).gameObject;
        escape.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (controlsMenu.activeInHierarchy)
            {
                controlsMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                controlsMenu.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }
}
