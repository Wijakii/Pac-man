using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStarter : MonoBehaviour
{
    // Start is called before the first frame update
    
    [Header("Audio Clips")]
    public AudioClip normalMusic;
    public AudioClip introClip;
    
    private AudioSource audioSource;
    public float seconds;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayIntroThenMusic());
    }

    private IEnumerator PlayIntroThenMusic()
    {
        audioSource.clip = introClip;
        audioSource.loop = false;
        audioSource.Play();
        
        yield return new WaitForSecondsRealtime(introClip.length);
        audioSource.clip = normalMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
