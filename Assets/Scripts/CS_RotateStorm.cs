using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_RotateStorm : MonoBehaviour
{
    [SerializeField]
    private float fRotateSpeed;

    public void FixedUpdate()
    {
        transform.Rotate(0.0f, fRotateSpeed, 0.0f);
    }

    public enum WindStrength
    {
        NONE,
        WEAK,
        MEDIUM,
        STRONG
    }

    //Wind blueprint
    public void Start()
    {
    }

    public void WindUpdate()
    {
    }

    ////Respawn Blueprint
    //[SerializeField]
    //private Transform m_PlayerSpawnPosition;

    //[SerializeField]
    //private float m_fRespawnSpeed;

    //private bool m_bIsRespawning;
    //private Vector3 m_vPlayerOldPosition;
    //private Vector3 m_fCurvePosition;
    //private float m_fDirtyLerpTracker;

    //public void UpdateRespawningPlayer()
    //{
    //    //If Player is dead and needs to respawn
    //    if (m_bIsRespawning)
    //    {
    //        transform.position = GetCurvePosition(m_vPlayerOldPosition, m_PlayerSpawnPosition.position, m_fCurvePosition, m_fDirtyLerpTracker);//Get the current position of the storm on the curve
    //        m_fDirtyLerpTracker += m_fRespawnSpeed;
    //        if (m_fDirtyLerpTracker >= 1.0f)
    //        {
    //            m_bIsRespawning = false;
    //            m_fDirtyLerpTracker = 0.0f;
    //        }
    //    }
    //}

    //public void RespawnPlayer()
    //{
    //    m_bIsRespawning = true;
    //    m_vPlayerOldPosition = transform.position;
    //    m_fCurvePosition = (transform.position + (m_vPlayerOldPosition - m_PlayerSpawnPosition.position) / 2);
    //    m_fCurvePosition.y += 10;
    //}

    //private Vector3 GetCurvePosition(Vector3 a_v3Pos0, Vector3 a_v3Pos1, Vector3 a_v3Pos2, float a_fTime)//Get cubic point of curve
    //{
    //    return Vector3.Lerp(Vector3.Lerp(a_v3Pos0, a_v3Pos2, a_fTime), Vector3.Lerp(a_v3Pos2, a_v3Pos1, a_fTime), a_fTime);
    //}
}