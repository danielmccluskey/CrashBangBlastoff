using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_EndGame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<CS_PlayerController>().bCanWin = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<CS_PlayerController>().bCanWin = false;
        }
    }
}
