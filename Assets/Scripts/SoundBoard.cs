using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class SoundBoard
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 4f)]
    public float pitch = 1;

    [HideInInspector]
    public AudioSource audioSource;
    


}