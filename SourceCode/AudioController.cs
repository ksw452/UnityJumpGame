using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;
    [SerializeField]
    AudioClip hit;
    [SerializeField]
    AudioSource onceAudio;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void PlayOnce()
    {

        onceAudio.PlayOneShot(hit);

    }



}
