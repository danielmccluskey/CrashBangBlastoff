using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_TrackObject : MonoBehaviour {

    [SerializeField]
    GameObject TrackObject;
    [SerializeField]
    Vector3 Offset;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Camera.main.WorldToScreenPoint(TrackObject.transform.position) + Offset;
    }
}
