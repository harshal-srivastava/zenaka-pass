using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class responsible for playing game audio
/// </summary>
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
        //making it singleton so that it would provide easy access to music all over the game
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    /// <summary>
    /// Function to play card flip sound
    /// </summary>
    public void PlayCardFlippedSound()
    {
        SetAndPlayAudioSource(cardFlippingSoundClip);
    }

    /// <summary>
    /// Function to play card match success sound
    /// </summary>
    public void PlayCardMatchSuccessSound()
    {
        SetAndPlayAudioSource(cardMatchSuccessSoundClip);
    }

    /// <summary>
    /// Function to play card match fail sound
    /// </summary>
    public void PlayCardMatchFailSound()
    {
        SetAndPlayAudioSource(cardMatchFailSoundClip);
    }

    /// <summary>
    /// Function to play game won sound
    /// </summary>
    public void PlayGameWonSound()
    {
        SetAndPlayAudioSource(gameWonSoundClip);
    }

    /// <summary>
    /// Function to set the audioclip of the audiosource and play it
    /// </summary>
    /// <param name="clip"></param>
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
