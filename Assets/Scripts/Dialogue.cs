using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;

    public GameObject continueButtons;
    public Animator textDisplayAnimation;
    public Animator textPanelAnimation;
    private AudioSource audioSource;

    GameObject wolf;
    GameObject fyvMessage;
    public bool findYourVoice;
    public float fyvTimer;

    private void Awake()
    {
        textPanelAnimation.SetTrigger("Open");
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(Type());
        wolf = GameObject.Find("Wolf");
        fyvMessage = GameObject.Find("UI").transform.GetChild(3).gameObject;
        fyvTimer = 15;
    }

    private void Update()
    {
        if (textDisplay.text == sentences[index])
        {
            continueButtons.SetActive(true);
        }

        if (index < sentences.Length - 1) // Prevent wolf input during dialogue
        {
            wolf.GetComponent<Wolf>().dialogueActive = true;
        }

        if (findYourVoice && fyvTimer > 0)
        {
            Vector3 chestPosition = new Vector3(212.7f, 1, 0);
            if ((Camera.main.transform.position - chestPosition).magnitude < 1)
            {
                fyvMessage.SetActive(true);
            }
            fyvTimer -= Time.deltaTime;
        }

        if (fyvTimer <= 0)
        {
            fyvMessage.SetActive(false);
            wolf.GetComponent<Wolf>().dialogueActive = false;
        }
    }
    IEnumerator Type()
    {
        foreach(char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence()
    {
        audioSource.Play();
        textDisplayAnimation.SetTrigger("Change");
        continueButtons.SetActive(false);

        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        } 
        else
        {
            textDisplay.text = "";
            continueButtons.SetActive(false);
            textPanelAnimation.SetTrigger("Close");
            textPanelAnimation.ResetTrigger("Open");
        }

        if (index == sentences.Length - 1 && !findYourVoice && SceneManager.GetActiveScene().name == "UpdatedMap")
        {
            CameraPan pan = Camera.main.GetComponent<CameraPan>();
            pan.player = wolf;
            pan.GoTo(new Vector3(212.7f, 1, 0), 2);
            pan.distanceMargin = 0.5f;
            findYourVoice = true;
        }
    }
}
