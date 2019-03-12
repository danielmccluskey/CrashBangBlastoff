using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_SnatchTrig : MonoBehaviour {

    public bool bTriggerOn = false;
    public int iPlayerNum = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && other.gameObject != CS_Gamemanager.Players[iPlayerNum])
        {
            bTriggerOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.gameObject != CS_Gamemanager.Players[iPlayerNum])
        {
            bTriggerOn = false;
        }
    }
}
