using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip typingClip;
    [SerializeField] private AudioClip wrongKeyClip;
    [SerializeField] private AudioClip wordCompleteClip;
    [SerializeField] private AudioClip flowerCollectClip;
    [SerializeField] private AudioClip witherClip;
    [SerializeField] private AudioClip gameOverClip;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayTyping()
    {
        sfxSource.volume = 0.2f;
        sfxSource.PlayOneShot(typingClip);
    }
    public void PlayWrongKey()
    {
        sfxSource.volume = 0.2f;
        sfxSource.PlayOneShot(wrongKeyClip);
    }

    public void PlayCompleteWord()
    {
        sfxSource.volume = 0.2f;
        sfxSource.PlayOneShot(wordCompleteClip);
    }

    public void PlayWither()
    {
        sfxSource.volume = 0.1f;
        sfxSource.PlayOneShot(witherClip);
    }

    public void PlayCollect()
    {
        sfxSource.volume = 0.2f;
        sfxSource.PlayOneShot(flowerCollectClip);
    }

    public void PlayGameOver()
    {
        sfxSource.volume = 0.2f;
        sfxSource.PlayOneShot(gameOverClip);
    }

    public void StopMusic() => musicSource.Stop();
}
