using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMenu : MonoBehaviour
{
    GameObject controlsMenu;

    void Start()
    {
        controlsMenu = transform.GetChild(0).gameObject;
        controlsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (controlsMenu.activeInHierarchy)
            {
                controlsMenu.SetActive(false);
            }
            else
            {
                controlsMenu.SetActive(true);
            }
        }
    }
}
