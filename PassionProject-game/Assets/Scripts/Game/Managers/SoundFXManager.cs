using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioSource loopingSource;
    [SerializeField] private AudioSource scenarioSource;

    [Header("space scenarios")]
    private AudioSource activeScenarioSource;
    Coroutine currentFade;


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


    public void PlayScenarioFXClip(AudioClip audioClip, Transform spawnTransform, float fadeInTime, float volume)
    {
        //spawn in gameobject + assign audioclip
        activeScenarioSource = Instantiate(scenarioSource, spawnTransform.position, Quaternion.identity);
        activeScenarioSource.clip = audioClip;

        //assign volume
        activeScenarioSource.loop = true;
        activeScenarioSource.volume = 0f;

        //play sound
        activeScenarioSource.Play();

        //get length
        float t = fadeInTime;
        StartFade(volume, t, stopAfterFade: false);
    }

    public void StopScenarioSound(float fadeOutTime)
    {
        float t = fadeOutTime;
        StartFade(0f, t, stopAfterFade: true);
    }

    void StartFade(float targetVolume, float duration, bool stopAfterFade = false)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeRoutine(targetVolume, duration, stopAfterFade));
    }

    IEnumerator FadeRoutine(float targetVolume, float duration, bool stopAfterFade)
    {
        float startVol = activeScenarioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            activeScenarioSource.volume = Mathf.Lerp(startVol, targetVolume, t);
            yield return null;
        }

        activeScenarioSource.volume = targetVolume;

        if (stopAfterFade && targetVolume <= 0.001f)
        {
            activeScenarioSource.Stop();
            Destroy(activeScenarioSource.gameObject);
        }
    }

}
