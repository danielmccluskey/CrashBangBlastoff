using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class CS_PlayerController : MonoBehaviour
{
    [SerializeField]
    private int iPlayerNum = 1;

    [SerializeField]
    private CS_SnatchTrig StealTrig;

    [SerializeField] private GameObject GameManager;

    public GameObject Handle;

    public List<GameObject> ItemsCloseBy;
    public int iPlayerScore = 0;
    public GameObject Item; //Set externally
    public bool bPlayerBumpedInto = false;
    public bool bMovingAvailable = true;
    public bool bItemPickedUp = false;

    private float fItemHeight = 0.5f;
    private float fItemThrowModifier = 2.0f;
    private Vector3 v3MovementVec;
    private Vector3 v3PreviousVec;
    private Vector3 v3PreviousPos;
    private Quaternion v3PreviousRot;
    private float fVelocity;
    private CharacterController Controller;
    private float fGravity;
    private float fPlayerHeight = 1.08f;
    private Vector3 knockbackVelocity;
    private bool bIsInvincible = false;

    private Vector3 v3CollisionPos;
    private float fMass = 0.1f;
    private Rigidbody Body;
    private float fClosestDistance = 0f;
    private float fTotalTime = 0.3f;
    private float fTimer = 0;
    private bool bDoOnce = true;
    private bool bParentOnce = true;
    private bool bMoveOnce = true;
    private bool bSetGravOnce = true;
    private bool bDashAvailable = true;
    private float fTotalDashTime = 0.7f;
    private float fDashTimer = 0;
    private bool bDashing = false;

    [Header("Respawn settings")]
    [SerializeField]
    public Transform m_PlayerSpawnPosition;//This players Spawn Position

    [SerializeField]
    private float m_fRespawnSpeed = 0.05f;//The speed the player will respawn at

    public bool debugrespawn;

    public bool bCanWin;
    public bool m_bIsRespawning;//Is the player respawning
    private Vector3 m_vPlayerOldPosition;//The position the player died at
    private Vector3 m_fCurvePosition;//The third point to generate the curve
    private float m_fDirtyLerpTracker;//Tracker

    [Header("Laser System")]
    [SerializeField]
    private float m_fLaserDuration;

    private Vector3 m_vLaserTarget;

    private float m_fLaserTimer;
    private bool m_bFiringLaser;
    public bool debugfire;

    [Header("AudioSettings")]
    private AudioSource m_sMovingSound;

    private AudioSource m_sAudioBank;

    [SerializeField]
    private AudioClip m_sBumpSound;

    [SerializeField]
    private AudioClip m_sAngrySound;

    [SerializeField]
    private AudioClip m_sPickupSound;

    [SerializeField]
    private AudioClip m_sLaserSound;

    [SerializeField]
    private AudioClip m_sDropoffSound;

    // Use this for initialization
    private void Start()
    {
        v3PreviousRot = gameObject.transform.rotation;
        v3PreviousPos = gameObject.transform.position;
        fVelocity = 8f;
        fDashTimer = fTotalDashTime;
        fTimer = fTotalTime;
        fGravity = -2f;
        Controller = gameObject.GetComponent<CharacterController>();
        Body = gameObject.GetComponent<Rigidbody>();
        StealTrig.iPlayerNum = iPlayerNum - 1;

        m_sMovingSound = GetComponent<AudioSource>();
        StopMoveSound();

        m_sAudioBank = gameObject.AddComponent<AudioSource>();
        m_sAudioBank.volume = 1.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        //TestDash();
        if (!bPlayerBumpedInto && bMovingAvailable)
        {
            Movement();
        }
        else if (bPlayerBumpedInto)
        {
            PlayerInteraction();
        }
        IteractWithItems();

        if (debugrespawn)
        {
            debugrespawn = false;
            RespawnPlayer();
        }
        if (m_bIsRespawning)
        {
            UpdateRespawningPlayer();
        }

        if (StealTrig.bTriggerOn)
        {
            StealItem();
        }

        DrawLaserLine();

        if (debugfire)
        {
            debugfire = false;
            FireLaser(Vector3.zero);
        }
    }

    private void TestDash()
    {
        if (Input.GetAxis("Dash" + iPlayerNum) == 1 && bDashAvailable)
        {
            //Do Dash
            bDashing = true;
            bDashAvailable = false;
            bMovingAvailable = false;
        }
        if (bDashing)
        {
            Body.AddRelativeForce(Vector3.forward * 50);
            fDashTimer -= Time.deltaTime;
            if (fDashTimer <= 0)
            {
                bDashing = false;
                bDashAvailable = true;
                bMovingAvailable = true;
            }
        }
    }

    private void Movement()
    {
        if (iPlayerNum == 1)
        {
            v3MovementVec.x = Input.GetAxis("LeftJoystickX1") * fVelocity/* * iTestX*/;
            v3MovementVec.z = Input.GetAxis("LeftJoystickY1") * fVelocity/* * iTestY*/;
        }
        else if (iPlayerNum == 2)
        {
            v3MovementVec.x = Input.GetAxis("LeftJoystickX2") * fVelocity/* * iTestX2*/;
            v3MovementVec.z = Input.GetAxis("LeftJoystickY2") * fVelocity/* * iTestY2*/;
        }

        //if (v3MovementVec.magnitude != 0)
        //{
        //    StartMoveSound();
        //}

        if (v3MovementVec.x > 0
            || v3MovementVec.x < 0
            || v3MovementVec.z > 0
            || v3MovementVec.z < 0)
        {
            StartMoveSound();
        }
        else
        {
            StopMoveSound();
        }

        if (Controller.isGrounded == false)
        {
            v3MovementVec = ApplyGravity(v3MovementVec);
            //if (gameObject.transform.position.y <= 1.1f)
            //{
            //    gameObject.transform.position = new Vector3(gameObject.transform.position.x, fPlayerHeight, gameObject.transform.position.z);
            //}
        }
        if (gameObject.transform.position != v3PreviousPos && Controller.isGrounded)
        {
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.LookRotation(gameObject.transform.position - v3PreviousPos), 50f);
            gameObject.transform.eulerAngles = new Vector3(270f, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z + 180f);
        }
        //if (gameObject.transform.rotation != v3PreviousRot)
        //{
        //    gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.LookRotation(gameObject.transform.position - v3PreviousPos), 50f);
        //}
        v3PreviousRot = gameObject.transform.rotation;
        v3PreviousPos = gameObject.transform.position;
        Controller.Move(v3MovementVec * Time.deltaTime);
    }

    private float DotProduct(Vector3 a_v1, Vector3 a_v2)
    {
        float fa = a_v1.x * a_v2.x;
        float fb = a_v1.y * a_v2.y;
        float fc = a_v1.z * a_v2.z;

        float cosine = (fa + fb + fc) / (a_v1.magnitude * a_v2.magnitude);

        return Mathf.Acos(cosine);
    }

    private void IteractWithItems()
    {
        if (Input.GetButtonDown("A" + iPlayerNum))
        {
            if (bCanWin)
            {
                GameManager.GetComponent<CS_Gamemanager>().SetTimer(iPlayerNum);
            }
            //Makes the closest item in its radius the Item
            int iItemClosest = 0;
            if (!bItemPickedUp)
            {
                for (int i = 0; i < ItemsCloseBy.Count; i++)
                {
                    if (i == 0)
                    {
                        if (ItemsCloseBy[i])
                        {
                            fClosestDistance = Vector3.Distance(gameObject.transform.position, ItemsCloseBy[i].transform.position);
                        }
                        iItemClosest = i;
                    }
                    else
                    {
                        if (Vector3.Distance(gameObject.transform.position, ItemsCloseBy[i].transform.position) < fClosestDistance)
                        {
                            fClosestDistance = Vector3.Distance(gameObject.transform.position, ItemsCloseBy[i].transform.position);
                            iItemClosest = i;
                        }
                    }
                }
                if (ItemsCloseBy.Count != 0 && ItemsCloseBy[iItemClosest] && ItemsCloseBy[iItemClosest].GetComponent<CS_Item>().bHeld == false && ItemsCloseBy[iItemClosest].GetComponent<CS_Item>().m_bIsBeingLaunched == false)
                {
                    Item = ItemsCloseBy[iItemClosest];
                    ItemsCloseBy.RemoveAt(iItemClosest);
                }
            }
        }
        if (Item && (Input.GetButtonDown("A" + iPlayerNum)))
        {
            bItemPickedUp = !bItemPickedUp;
        }

        if (bItemPickedUp)
        {
            if (bDoOnce)
            {
                Item.transform.parent = Handle.transform;
                Item.transform.localPosition = Vector3.zero;
                Item.GetComponent<CS_Item>().bHeld = true;
                bDoOnce = false;
                bParentOnce = true;
                PlayerAudioClip(m_sPickupSound);
            }
        }
        else if (Item)
        {
            if (bParentOnce)
            {
                Item.transform.parent = null;
                bDoOnce = true;
                bParentOnce = false;
                Vector3 v3DestinationToFall = new Vector3((transform.forward.x * fItemThrowModifier) + gameObject.transform.position.x, 0, (transform.forward.z * fItemThrowModifier) + gameObject.transform.position.z);
                Item.GetComponent<CS_Item>().LaunchObjectTo(v3DestinationToFall, false);
                Item = null;
            }
            //Call Item drop function here
            //Item.transform.Translate(new Vector3(0, Time.deltaTime * fGravity, 0));
        }
    }

    public void PutItemOnPad()
    {
        Item.transform.parent = null;
        bItemPickedUp = false;
        bDoOnce = true;
        PlayerAudioClip(m_sDropoffSound);
        Destroy(Item);
    }

    public void SetInvincible(bool a_newState)
    {
        bIsInvincible = a_newState;
    }

    public bool GetInvincible()
    {
        return bIsInvincible;
    }

    private void PlayerInteraction()
    {
        //F = MA
        //For certain amount of time
        if (bMoveOnce)
        {
            knockbackVelocity = -v3MovementVec;
            knockbackVelocity = new Vector3(knockbackVelocity.x / 20, Mathf.Sin(knockbackVelocity.y + (Time.deltaTime * fGravity * -1)), knockbackVelocity.z / 20);
            PlayerAudioClip(m_sBumpSound);
            bMoveOnce = false;
        }
        if (fTimer >= 0)
        {
            fTimer -= Time.deltaTime;
            //Vector3 knockbackVelocity = new Vector3((transform.position.x - v3CollisionPos.x) * fBumpModifier, fBumpModifier, (transform.position.z - v3CollisionPos.z) * fBumpModifier);
            //Vector2 knockbackVelocity = new Vector3((1 / v3MovementVec.x), (1 / v3MovementVec.y), (1 / v3MovementVec.z));
            if (fTimer > fTotalTime / 2)
            {
                //knockbackVelocity = new Vector3(knockbackVelocity.x, knockbackVelocity.y + (Time.deltaTime * fGravity * -1), knockbackVelocity.z);
                knockbackVelocity = ApplyGravity(knockbackVelocity);
            }
            else
            {
                knockbackVelocity = ApplyGravity(knockbackVelocity);
            }
            Vector3 v3Temp = knockbackVelocity.normalized;
            v3Temp = new Vector3(v3Temp.x, v3Temp.y * 2, v3Temp.z);
            Controller.Move(v3Temp * Time.deltaTime * 40);
        }
        else if (fTimer < 0)
        {
            if (Controller.isGrounded)
            {
                bPlayerBumpedInto = false;
                fTimer = fTotalTime;
                bMoveOnce = true;
            }
            else
            {
                fTimer = fTotalTime / 2;
            }
        }
    }

    private void StealItem()
    {
        if (Input.GetButtonDown("B" + iPlayerNum)) //If Player presses B
        {
            Debug.LogError("B Pressed");
            int iTempPlayerNum = 1;
            if (iPlayerNum == 2)
            {
                iTempPlayerNum = 0;
            }
            if (CS_Gamemanager.Players[iTempPlayerNum].GetComponent<CS_PlayerController>().bItemPickedUp) //If the other player is holding an item
            {
                CS_Gamemanager.Players[iTempPlayerNum].GetComponent<CS_PlayerController>().Item.transform.parent = null;
                CS_Gamemanager.Players[iTempPlayerNum].GetComponent<CS_PlayerController>().bDoOnce = true;
                CS_Gamemanager.Players[iTempPlayerNum].GetComponent<CS_PlayerController>().bParentOnce = false;

                bItemPickedUp = true;
                Item = CS_Gamemanager.Players[iTempPlayerNum].GetComponent<CS_PlayerController>().Item;
                CS_Gamemanager.Players[iTempPlayerNum].GetComponent<CS_PlayerController>().Item = null;
                CS_Gamemanager.Players[iTempPlayerNum].GetComponent<CS_PlayerController>().bItemPickedUp = false; //They drop the item

                Item.transform.parent = Handle.transform;
                Item.transform.localPosition = Vector3.zero;
                Item.GetComponent<CS_Item>().bHeld = true;
                bDoOnce = false;
                bParentOnce = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.name != gameObject.name)
        {
            //other.GetComponent<CS_PlayerController>().bPlayerBumpedInto = true;
            bPlayerBumpedInto = true;
            v3CollisionPos = other.GetComponent<CS_PlayerController>().v3MovementVec;
        }
    }

    private Vector3 ApplyGravity(Vector3 a_v)
    {
        //f=ma
        a_v = new Vector3(a_v.x, a_v.y + (fGravity * 40 * Time.deltaTime), a_v.z);
        return a_v;
    }

    public void UpdateRespawningPlayer()
    {
        //If Player is dead and needs to respawn
        if (m_bIsRespawning)
        {
            transform.position = GetCurvePosition(m_vPlayerOldPosition, m_PlayerSpawnPosition.position, m_fCurvePosition, m_fDirtyLerpTracker);//Get the current position of the storm on the curve
            m_fDirtyLerpTracker += m_fRespawnSpeed;
            if (m_fDirtyLerpTracker >= 1.0f)
            {
                m_bIsRespawning = false;
                m_fDirtyLerpTracker = 0.0f;
            }
        }
    }

    public void RespawnPlayer()
    {
        if (bItemPickedUp == true)
        {
            bItemPickedUp = false;
            Item.transform.parent = null;
            Item.GetComponent<CS_Item>().bHeld = false;
        }
        m_bIsRespawning = true;
        m_vPlayerOldPosition = transform.position;
        m_fCurvePosition = (transform.position + (m_vPlayerOldPosition - m_PlayerSpawnPosition.position) / 2);
        m_fCurvePosition.y += 10f;
        PlayerAudioClip(m_sAngrySound);
    }

    private Vector3 GetCurvePosition(Vector3 a_v3Pos0, Vector3 a_v3Pos1, Vector3 a_v3Pos2, float a_fTime)//Get cubic point of curve
    {
        return Vector3.Lerp(Vector3.Lerp(a_v3Pos0, a_v3Pos2, a_fTime), Vector3.Lerp(a_v3Pos2, a_v3Pos1, a_fTime), a_fTime);
    }

    public void FireLaser(Vector3 a_v3Target)
    {
        m_fLaserTimer = m_fLaserDuration;
        m_bFiringLaser = true;
        m_vLaserTarget = a_v3Target;
        GetComponent<LineRenderer>().enabled = true;
        PlayerAudioClip(m_sLaserSound);
    }

    public void DrawLaserLine()
    {
        if (m_bFiringLaser)
        {
            LineRenderer linerend = GetComponent<LineRenderer>();
            linerend.SetPosition(0, transform.position);
            linerend.SetPosition(1, m_vLaserTarget);
            m_fLaserTimer -= Time.deltaTime;
            if (m_fLaserTimer <= 0)
            {
                m_bFiringLaser = false;
                GetComponent<LineRenderer>().enabled = false;
                m_fLaserTimer = m_fLaserDuration;
            }
        }
    }

    public void StartMoveSound()
    {
        m_sMovingSound.volume = 1.0f;
    }

    public void StopMoveSound()
    {
        m_sMovingSound.volume = 0.0f;
    }

    public void PlayerAudioClip(AudioClip a_sAudioClip)
    {
        m_sAudioBank.PlayOneShot(a_sAudioClip);
    }
}