using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    private bool sheared;
    GameObject wolf;
    SpriteRenderer shears;

    void Start()
    {
        wolf = GameObject.Find("Wolf");
        shears = transform.GetChild(4).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Set state according to current tag
        if (this.tag == "Unsheared")
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            this.transform.GetChild(1).gameObject.SetActive(false);
            this.transform.GetChild(2).gameObject.SetActive(false);
        }
        if (this.tag == "Sheared")
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            this.transform.GetChild(1).gameObject.SetActive(true);
            this.transform.GetChild(2).gameObject.SetActive(false);
        }
        if (this.tag == "Clothed")
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            this.transform.GetChild(1).gameObject.SetActive(false);
            this.transform.GetChild(2).gameObject.SetActive(true);
        }

        if ((this.transform.position - wolf.transform.position).magnitude < 3 && this.tag == "Unsheared")
        {
            shears.enabled = true;
        }
        else
        {
            shears.enabled = false;
        }
    }

    public bool IsSheared { get { return sheared; } set { this.sheared = value; } }
}
