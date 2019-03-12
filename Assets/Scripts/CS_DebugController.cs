using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_DebugController : MonoBehaviour {


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                Debug.LogError("KeyCode down: " + kcode);
        }
    }
}
