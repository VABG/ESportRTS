using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip[] songs;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (songs.Length > 0)
        {
            audioSource.clip = songs[0];
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
