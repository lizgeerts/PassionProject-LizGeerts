using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioSource loopingSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, float length)
    {
        //spawn in gameobject + assign audioclip
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length
        if (length == 0f)
        {
            length = audioSource.clip.length;
        }

        //destroy clip after done playing   
        Destroy(audioSource.gameObject, length);
    }

    public List<AudioSource> activeLoops = new List<AudioSource>();

    public void PlayLoop(AudioClip audioClip, Transform spawnTransform, float volume = 1f)
    {
        AudioSource audioSource = Instantiate(loopingSource, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;

        audioSource.volume = volume;
        audioSource.Play();
        audioSource.loop = true;

        activeLoops.Add(audioSource);
    }

    public void StopLoop()
    {
        foreach (AudioSource source in activeLoops)
        {
            if (source != null)
                Destroy(source.gameObject);
        }
        activeLoops.Clear();
    }

}
