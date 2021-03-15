using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    private static SoundManager instance;

    public AudioSource[] destroyNoise;


    public void PlayRandomSound()
    {
        int randomClip = Random.Range(0, destroyNoise.Length);
        destroyNoise[randomClip].Play();
    }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }

        
    }

   
}
