using System.Collections.Generic;
using UnityEngine;

public class SoundMainMenu : MonoBehaviour
{

    public static SoundMainMenu instance;
    [SerializeField] private AudioSource loopingSource;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public List<AudioSource> activeLoops = new List<AudioSource>();

    public void PlayLoop(AudioClip audioClip, Transform spawnTransform, bool loop, float volume = 1f)
    {
        AudioSource audioSource = Instantiate(loopingSource, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;

        audioSource.volume = volume;
        audioSource.Play();
        audioSource.loop = loop;

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
