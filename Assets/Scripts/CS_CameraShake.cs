using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_CameraShake : MonoBehaviour
{

    private static CS_CameraShake instance; //Allows shake to be called outside of class
    public static CS_CameraShake Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<CS_CameraShake>(); //If gameobject is present in scene with script, setting the instance
            }
            return CS_CameraShake.instance; //Else create an instance
        }

    }

    private float fAmplitude = 0.1f;
    private Vector3 v3InitialPosition;
    private bool bIsShaking = false;


    // Use this for initialization
    void Start()
    {
        v3InitialPosition = transform.localPosition;
    }

    public void Shake(float a_fAmplitude, float a_fDuration)
    {
        fAmplitude = a_fAmplitude;
        bIsShaking = true;
        CancelInvoke();
        Invoke("StopShaking", a_fDuration); //Will wait for duration before calling stop shaking
    }

    public void StopShaking()
    {
        bIsShaking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsShaking)
        {
            transform.localPosition = v3InitialPosition + Random.insideUnitSphere * fAmplitude;
        }
    }
}
