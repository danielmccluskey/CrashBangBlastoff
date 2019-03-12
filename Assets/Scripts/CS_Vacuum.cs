using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Vacuum : MonoBehaviour
{
    [SerializeField]
    private GameObject MoverPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CS_PlayerController playerRef = other.GetComponent<CS_PlayerController>();
            if (!playerRef.GetInvincible())
            {
                playerRef.RespawnPlayer();
            }
        }
        if (other.CompareTag("Item"))
        {
            if (other.GetComponent<CS_Item>() != null)
            {
                if (other.GetComponent<CS_Item>().m_bHasLanded)
                {
                    GameObject objMover = Instantiate(MoverPrefab);
                    objMover.GetComponent<CS_ObjectMover>().SuckGameObject(other.gameObject, gameObject, new Vector3(transform.position.x, transform.position.y + 30, transform.position.z));
                }
            }
        }
        if (other.CompareTag("Powerup"))
        {
            GameObject objMover = Instantiate(MoverPrefab);
            objMover.GetComponent<CS_ObjectMover>().SuckGameObject(other.gameObject, gameObject, new Vector3(transform.position.x, transform.position.y + 30, transform.position.z));
        }
    }
}