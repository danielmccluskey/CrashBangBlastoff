using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_PlayerAudio : MonoBehaviour
{
    private AudioSource MovingSound;

    // Use this for initialization
    private void Start()
    {
        MovingSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (true)//if moving
        {
            MovingSound.volume = 1.0f;
        }
        else
        {
            MovingSound.volume = 0.0f;
        }
    }
}