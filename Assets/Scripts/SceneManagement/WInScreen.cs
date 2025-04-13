using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField]
    private float timeInSeconds;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private AudioSource winVoice;
    [SerializeField]
    private AudioSource explosionSound;

    private void Start()
    {
        canvas.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(WinTimerStart());
        }
    }

    private IEnumerator WinTimerStart()
    {
        yield return new WaitForSeconds(timeInSeconds);
        
        canvas.enabled = true;
        winVoice.Play();
        explosionSound.Play();
    }
}
