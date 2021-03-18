using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;




public class SoundManager : MonoBehaviour
{

    private static SoundManager instance;

    [SerializeField] SoundBoard[] soundBoard;
    private void Awake()
    {
       


        foreach (SoundBoard snds in soundBoard)
        {
            snds.audioSource = gameObject.AddComponent<AudioSource>();
            snds.audioSource.clip = snds.clip;

            snds.audioSource.pitch = snds.pitch;
            snds.audioSource.volume = snds.volume;
        }

    }

 
    public void PlayRandomSound()
    {
        int randomSound = Random.Range(0, soundBoard.Length);
        soundBoard[randomSound].audioSource.Play();

    }





}

