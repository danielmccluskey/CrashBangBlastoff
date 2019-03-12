using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemValue
{
    STANDARD,
    HQ,
    LEDGENDARY
}


public class CS_Item : MonoBehaviour
{
    [SerializeField] public Mesh Model; // The model that will be used for this item
    [SerializeField] public int ID; // The ID this item holds
    [SerializeField] public ItemValue Value; // The value of the item

    // Cannot be both true, both can be false
    public bool bHeld = false; // If the item is in the field and held by a player

    public bool bCollected = false; // If this item is held in an item pad

    // cache
    private float distance;

    private GameObject curPlayer;

    [Header("Item Launching")]
    [SerializeField]
    [Range(0.0f, 0.4f)]
    private float m_fLaunchSpeed = 0.02f;

    private Vector3 m_vItemStartPosition;//The position the player died at
    private Vector3 m_vItemDestination;//Position that the item needs to land at
    private Vector3 m_fCurvePosition;//The third point to generate the curve
    private float m_fDirtyLerpTracker;//Tracker
    public bool m_bIsBeingLaunched;
    public bool debuglaunch;
    public bool m_bHasLanded = false;

    public void InitialiseItem(Mesh a_model, int a_ID)
    {
        Model = a_model;
        ID = a_ID;

        GetComponent<MeshFilter>().mesh = a_model;
    }

    private void FixedUpdate()
    {
        if (bCollected == false && bHeld == false)
        {
            for (int i = 0; i < 2; i++)
            {
                curPlayer = CS_Gamemanager.Players[i];
                distance = Vector3.Distance(curPlayer.transform.position, transform.position);

                if (distance <= CS_Gamemanager.InteractionRadius)
                {
                    // Interaction can occur
                    if (curPlayer.GetComponent<CS_PlayerController>().ItemsCloseBy.Contains(gameObject) == false)
                    {
                        curPlayer.GetComponent<CS_PlayerController>().ItemsCloseBy.Add(gameObject);
                    }
                }
                else
                {
                    if (curPlayer.GetComponent<CS_PlayerController>().ItemsCloseBy.Contains(gameObject))
                    {
                        curPlayer.GetComponent<CS_PlayerController>().ItemsCloseBy.Remove(gameObject);
                    }
                }
            }
        }

        if (bCollected == true && GetComponent<MeshFilter>().mesh == null)
        {
            GetComponent<MeshFilter>().mesh = Model;
        }

        if (debuglaunch)
        {
            debuglaunch = false;
            LaunchObjectTo(new Vector3(0, 0, 0), true);
        }

        if (m_bIsBeingLaunched)
        {
            UpdateLaunchingObject();
        }
    }

    public void UpdateLaunchingObject()
    {
        transform.position = GetCurvePosition(m_vItemStartPosition, m_vItemDestination, m_fCurvePosition, m_fDirtyLerpTracker);//Get the current position of the storm on the curve
        m_fDirtyLerpTracker += m_fLaunchSpeed;
        if (m_fDirtyLerpTracker >= 1.0f)
        {
            transform.position = GetCurvePosition(m_vItemStartPosition, m_vItemDestination, m_fCurvePosition, 1.0f);//Get the current position of the storm on the curve
            m_bIsBeingLaunched = false;
            m_fDirtyLerpTracker = 0.0f;
            bHeld = false;
        }
    }

    public void LaunchObjectTo(Vector3 a_vDestination, bool a_bGetHigh)
    {
        m_bIsBeingLaunched = true;
        m_vItemDestination = a_vDestination;//Set the destination
        m_vItemDestination.y += transform.localScale.y * 0.5f;
        m_vItemStartPosition = transform.position;//Set the start position
        m_fCurvePosition = (m_vItemStartPosition + (m_vItemDestination - m_vItemStartPosition) / 2);//Get midpoint of both positions
        if (a_bGetHigh)
        {
            m_fCurvePosition.y += 10.0f;//Get a high curve
        }
    }

    private Vector3 GetCurvePosition(Vector3 a_v3Pos0, Vector3 a_v3Pos1, Vector3 a_v3Pos2, float a_fTime)//Get cubic point of curve
    {
        return Vector3.Lerp(Vector3.Lerp(a_v3Pos0, a_v3Pos2, a_fTime), Vector3.Lerp(a_v3Pos2, a_v3Pos1, a_fTime), a_fTime);
    }
}