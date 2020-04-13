using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    private bool sheared;

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
    }

    public bool IsSheared { get { return sheared; } set { this.sheared = value; } }
}
