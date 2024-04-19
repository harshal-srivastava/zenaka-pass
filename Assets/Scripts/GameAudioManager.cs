using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    private static GameAudioManager instance;

    public static GameAudioManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    private AudioClip cardFlippingSoundClip;

    [SerializeField]
    private AudioClip cardMatchSuccessSoundClip;

    [SerializeField]
    private AudioClip cardMatchFailSoundClip;

    [SerializeField]
    private AudioClip gameWonSoundClip;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void PlayCardFlippedSound()
    {
        SetAndPlayAudioSource(cardFlippingSoundClip);
    }

    public void PlayCardMatchSuccessSound()
    {
        SetAndPlayAudioSource(cardMatchSuccessSoundClip);
    }

    public void PlayCardMatchFailSound()
    {
        SetAndPlayAudioSource(cardMatchFailSoundClip);
    }

    public void PlayGameWonSound()
    {
        SetAndPlayAudioSource(gameWonSoundClip);
    }

    void SetAndPlayAudioSource(AudioClip clip)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.clip = clip;
        audioSource.Play();
    }
}
