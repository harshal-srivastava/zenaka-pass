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
        audioSource.clip = cardFlippingSoundClip;
        audioSource.Play();
    }
}
