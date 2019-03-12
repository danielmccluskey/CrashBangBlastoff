using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == CS_Gamemanager.Players[0])
        {
            CS_Gamemanager.Players[0].GetComponent<CS_PlayerController>().SetInvincible(true);
        }
        else if(other.gameObject == CS_Gamemanager.Players[1])
        {
            CS_Gamemanager.Players[1].GetComponent<CS_PlayerController>().SetInvincible(true);
        }
        else
        {
            // Hit neither
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == CS_Gamemanager.Players[0])
        {
            CS_Gamemanager.Players[0].GetComponent<CS_PlayerController>().SetInvincible(false);
        }
        else if (other.gameObject == CS_Gamemanager.Players[1])
        {
            CS_Gamemanager.Players[1].GetComponent<CS_PlayerController>().SetInvincible(false);
        }
        else
        {
            // Hit neither
        }
    }
}
