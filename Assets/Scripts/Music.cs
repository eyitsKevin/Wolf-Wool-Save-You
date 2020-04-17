using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    public AudioSource[] audioSources;
    Scene scene;
    public bool MainMenu;
    public bool UpdatedMap;

    static Music prefab;

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

    // Start is called before the first frame update
    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        MainMenu = false;
        UpdatedMap = false;
    }

    // Update is called once per frame
    void Update()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "MainMenu" && !MainMenu)
        {
            audioSources[0].Play();
            audioSources[1].Stop();
            audioSources[2].Stop();
            MainMenu = true;
            UpdatedMap = false;
        }
        if (scene.name == "UpdatedMap" && !UpdatedMap)
        {
            audioSources[0].Stop();
            audioSources[1].Play();
            UpdatedMap = true;
        }

    }
}
