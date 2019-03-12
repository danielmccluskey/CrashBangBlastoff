using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ObjectMover : MonoBehaviour
{
    [Header("Item Launching")]
    [SerializeField]
    [Range(0.0f, 0.4f)]
    private float m_fLaunchSpeed = 0.02f;

    [SerializeField]
    private GameObject m_ObjectToMove;

    private Vector3 m_vItemStartPosition;//The position the player died at
    private Vector3 m_vItemDestination;//Position that the item needs to land at
    private Vector3 m_fCurvePosition;//The third point to generate the curve
    private float m_fDirtyLerpTracker;//Tracker
    private bool m_bIsBeingLaunched;
    private bool m_bDestroyAfter = false;
    public bool debuglaunch;

    public void FixedUpdate()
    {
        if (m_bIsBeingLaunched && m_ObjectToMove != null)
        {
            UpdateLaunchingObject();
        }
    }

    public void UpdateLaunchingObject()
    {
        //If Player is dead and needs to respawn
        if (m_bIsBeingLaunched)
        {
            m_ObjectToMove.transform.position = GetCurvePosition(m_vItemStartPosition, m_vItemDestination, m_fCurvePosition, m_fDirtyLerpTracker);//Get the current position of the storm on the curve
            m_fDirtyLerpTracker += m_fLaunchSpeed;
            if (m_fDirtyLerpTracker >= 1.0f)
            {
                m_bIsBeingLaunched = false;
                m_fDirtyLerpTracker = 0.0f;
                if (m_ObjectToMove.GetComponent<CS_Item>() != null)
                {
                    m_ObjectToMove.GetComponent<CS_Item>().m_bHasLanded = true;
                }
                if (m_bDestroyAfter == true)
                {
                    Destroy(m_ObjectToMove);
                }
                Destroy(gameObject);
            }
        }
    }

    public void LaunchGameObject(GameObject a_gObjectToLaunch, Vector3 a_vDestination)
    {
        m_bIsBeingLaunched = true;
        m_ObjectToMove = a_gObjectToLaunch;
        m_vItemStartPosition = m_ObjectToMove.transform.position;
        m_fCurvePosition = (m_vItemStartPosition + (m_vItemStartPosition - m_vItemDestination) / 2);
        m_fCurvePosition.y += 10;
        m_vItemDestination = a_vDestination;
        m_bDestroyAfter = false;
    }

    public void SuckGameObject(GameObject a_gObjectToLaunch, GameObject a_gStorm, Vector3 a_vDestination)
    {
        m_bIsBeingLaunched = true;
        m_ObjectToMove = a_gObjectToLaunch;
        m_vItemStartPosition = m_ObjectToMove.transform.position;
        m_fCurvePosition = a_gStorm.transform.position;
        m_fCurvePosition.y += 20;
        m_vItemDestination = a_vDestination;
        m_bDestroyAfter = true;
    }

    private Vector3 GetCurvePosition(Vector3 a_v3Pos0, Vector3 a_v3Pos1, Vector3 a_v3Pos2, float a_fTime)//Get cubic point of curve
    {
        return Vector3.Lerp(Vector3.Lerp(a_v3Pos0, a_v3Pos2, a_fTime), Vector3.Lerp(a_v3Pos2, a_v3Pos1, a_fTime), a_fTime);
    }
}