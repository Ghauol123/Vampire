using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LoadMusic : MonoBehaviour
{
    public AudioMixer audioMixer;

    void Start()
    {
        LoadMusicVolume();
        LoadSFXVolume();
    }

    public void LoadMusicVolume()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float volume = PlayerPrefs.GetFloat("musicVolume");
            audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        }
        else
        {
            // Set default music volume if not found in PlayerPrefs
            audioMixer.SetFloat("music", Mathf.Log10(1) * 20); // Default volume is 1
        }
    }

    public void LoadSFXVolume()
    {
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            float volume = PlayerPrefs.GetFloat("sfxVolume");
            audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        }
        else
        {
            // Set default SFX volume if not found in PlayerPrefs
            audioMixer.SetFloat("sfx", Mathf.Log10(1) * 20); // Default volume is 1
        }
    }
}
