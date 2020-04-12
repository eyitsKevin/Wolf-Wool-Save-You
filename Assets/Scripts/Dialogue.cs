using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Awake()
    {
        textPanelAnimation.SetTrigger("Open");
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(Type());
    }

    private void Update()
    {
        if (textDisplay.text == sentences[index])
        {
            continueButtons.SetActive(true);
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
    }
}
