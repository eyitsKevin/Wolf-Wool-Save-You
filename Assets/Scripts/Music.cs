using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    public AudioSource[] audioSources;
    Scene scene;
    public bool MainMenu;
    public bool UpdatedMap;
    public bool Outro;

    static Music prefab;
    Wolf wolf;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (prefab == null)
        {
            prefab = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        MainMenu = false;
        UpdatedMap = false;
    }

    void Update()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "MainMenu" && !MainMenu)
        {
            audioSources[0].Play();
            audioSources[1].Stop();
            audioSources[2].Stop();
            audioSources[3].Stop();
            MainMenu = true;
            UpdatedMap = false;
            Outro = false;
        }
        if (scene.name == "UpdatedMap" && !UpdatedMap)
        {
            wolf = GameObject.Find("Wolf").GetComponent<Wolf>();

            // Play alert music if wolf is caught
            if (!wolf.escaped)
            {
                Debug.Log("Alert Music");
                audioSources[0].Stop();
                audioSources[1].Stop();
                audioSources[2].Play();
                audioSources[3].Stop();
            }
            else
            {
                Debug.Log("Normal Music");
                audioSources[0].Stop();
                audioSources[1].Play();
                audioSources[2].Stop();
                audioSources[3].Stop();
            }

            MainMenu = false;
            UpdatedMap = true;
            Outro = false;
        }

        // Play alert music if wolf is caught
        if (scene.name == "UpdatedMap" && !wolf.escaped)
        {
            audioSources[1].Stop();
            audioSources[2].Play();
        }
        else
        {
            audioSources[1].Play();
            audioSources[2].Stop();
        }

        if (scene.name == "Outro" && !Outro)
        {
            audioSources[0].Stop();
            audioSources[1].Stop();
            audioSources[2].Stop();
            audioSources[3].Play();
            MainMenu = false;
            UpdatedMap = false;
            Outro = true;
        }

    }
}
