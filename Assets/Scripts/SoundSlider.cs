using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    private int isFirstPlayInt;
    public Slider backgroundMusicSlider;
    private float bgFloat;
    private float sfxFloat;
    [SerializeField] AudioSource bgAudio;



    void Start()
    {


        isFirstPlayInt = PlayerPrefs.GetInt("FirstPlay");
        if(isFirstPlayInt == 0)
        {
            bgFloat = .15f;
            backgroundMusicSlider.value = bgFloat;

            PlayerPrefs.SetFloat("BackgroundVolume", bgFloat);
            PlayerPrefs.SetInt("FirstPlay", -1);
        }
        else
        {
            bgFloat = PlayerPrefs.GetFloat("BackgroundVolume");
            backgroundMusicSlider.value = bgFloat;
        }
        
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("BackgroundVolume", backgroundMusicSlider.value);
        UpdateAudio();
    }

    public void UpdateAudio()
    {
        bgAudio.volume = backgroundMusicSlider.value;
    }

   
}
