using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_PowerUps : MonoBehaviour
{
    [SerializeField]
    private float m_fTimeUntilFire = 3.0f;

    private GameObject m_CollidingPlayer;

    private bool m_bActive = false;

    public void Update()
    {
        if (m_bActive)
        {
            m_fTimeUntilFire -= Time.deltaTime;
            if (m_fTimeUntilFire <= 0)
            {
                CS_Gamemanager.TriggerPowerup(m_CollidingPlayer);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_bActive && other.CompareTag("Player"))
        {
            m_bActive = true;
            m_CollidingPlayer = other.gameObject;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}