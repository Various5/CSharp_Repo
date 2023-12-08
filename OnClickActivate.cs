using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickActivate : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to the cube
        audioSource = GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        // Check if the AudioSource exists
        if (audioSource != null)
        {
            // Toggle the play state of the AudioSource
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            else
            {
                audioSource.Play();
            }
        }
    }
}
