using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wool : MonoBehaviour
{
    GameObject wool;
    bool woolHeld;

    void Start()
    {
        wool = transform.GetChild(0).gameObject;
        wool.SetActive(false);
    }

    void Update()
    {
        woolHeld = GameObject.Find("Wolf").GetComponent<Wolf>().woolHeld;

        if (woolHeld)
        {
            wool.SetActive(true);
        }
        else
        {
            wool.SetActive(false);
        }
    }
}
