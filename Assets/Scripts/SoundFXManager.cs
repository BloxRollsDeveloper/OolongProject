using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    public AudioSource Music;
    public AudioClip background;
    
    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        
        audioSource.clip = clip;
        
        audioSource.volume = volume;
        
        audioSource.Play();
        
        float clipLength = audioSource.clip.length;
        
        Destroy(audioSource.gameObject, clipLength);
    }
    
    public void PlayRandomSoundFXClip(AudioClip[] clip, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, clip.Length);
        
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        
        audioSource.clip = clip[rand];
        
        audioSource.volume = volume;
        
        audioSource.Play();
        
        float clipLength = audioSource.clip.length;
        
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayMusic()
    {
        Music.volume = 0.3f;
        Music.clip = background;
        Music.Play();
    }
}
