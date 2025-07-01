using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        // Implement singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Optional: Add methods to control music
    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void ChangeMusic(AudioClip newClip)
    {
        audioSource.clip = newClip;
        audioSource.Play();
    }
}