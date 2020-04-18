using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public AudioSource[] soundEffectAudioSource;
    public int hi;

    public static SoundEffectManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

    }
    private void Start()
    {
        soundEffectAudioSource = GetComponents<AudioSource>();
    }
    public void PlaySheep()
    {
        soundEffectAudioSource[0].Play();
    }

    public void PlayDoorSound()
    {
        soundEffectAudioSource[1].Play();
    }
}
