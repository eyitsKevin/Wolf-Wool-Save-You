using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    public AudioSource[] audioSources;
    Scene scene;
    public bool MainMenu;
    public bool UpdatedMap;
    public bool Alert;
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
        Alert = false;
        Outro = false;
    }

    void Update()
    {
        if (GameObject.Find("Wolf"))
        {
            wolf = GameObject.Find("Wolf").GetComponent<Wolf>();
        }
        scene = SceneManager.GetActiveScene();

        if (scene.name == "MainMenu" && !MainMenu)
        {
            audioSources[0].Play();
            audioSources[1].Stop();
            audioSources[2].Stop();
            audioSources[3].Stop();
            MainMenu = true;
            UpdatedMap = false;
            Alert = false;
            Outro = false;
        }
        if (scene.name == "UpdatedMap" && !UpdatedMap)
        {
            audioSources[0].Stop();
            audioSources[1].Play();
            audioSources[2].Stop();
            audioSources[3].Stop();
            MainMenu = false;
            UpdatedMap = true;
            Alert = false;
            Outro = false;
        }
        // Change music once howl is obtained
        if (scene.name == "UpdatedMap" && UpdatedMap && !Alert && wolf.howl)
        {
            audioSources[0].Stop();
            audioSources[1].Stop();
            audioSources[2].Play();
            audioSources[3].Stop();
            MainMenu = false;
            UpdatedMap = true;
            Alert = true;
            Outro = false;
        }

        if (scene.name == "Outro" && !Outro)
        {
            audioSources[0].Stop();
            audioSources[1].Stop();
            audioSources[2].Stop();
            audioSources[3].Play();
            MainMenu = false;
            UpdatedMap = false;
            Alert = false;
            Outro = true;
        }

    }
}
