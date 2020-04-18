using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitRoom : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Wolf"))
        {
            MainMenu m = new MainMenu();
            m.LoadSceneByName("Outro");
        }
    }
}
