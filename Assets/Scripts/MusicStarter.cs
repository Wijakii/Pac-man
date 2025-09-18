using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStarter : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip normalMusic;
    public AudioSource audioSource;
    public float seconds;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        seconds = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying == false || seconds > 3)
        {
            audioSource.clip = normalMusic;
            audioSource.Play();
        }
    }
}
