using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadSceneByName(string name)
    {
        if(name == null)
        {
            throw new System.Exception("enter a scene to load! (Look at StartButton's OnClick() )");
        }
        SceneManager.LoadScene(name);
    }
}
