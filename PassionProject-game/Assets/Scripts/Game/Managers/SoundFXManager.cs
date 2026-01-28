using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioSource loopingSource;

    public bool stopLoop = false;
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

    public void PlayLoop(AudioClip audioClip, Transform spawnTransform, float volume = 1f)
    {
        AudioSource audioSource = Instantiate(loopingSource, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;

        audioSource.volume = volume;
        audioSource.Play();
        audioSource.loop = true;
        
        if (stopLoop)
        {
            Destroy(audioSource.gameObject);
        }
    }

}
