using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_SmallStorms : MonoBehaviour
{
    [Header("Positions/References")]
    [SerializeField]
    private Transform m_tPointA;//Start Point A

    [SerializeField]
    private Transform m_tPointB;//Start Point B

    private Vector2 m_fFloorSize;//Size of the play area

    //Start Positons
    private Vector3 m_fStartPositionA;

    private Vector3 m_fStartPositionB;
    private Vector3 m_fStartPositionC;

    [Header("Storm Customisation")]
    [SerializeField]
    [Range(0.0f, 0.1f)]
    private float m_fStormSpeed;//Speed of the storm

    [SerializeField]
    private float m_fStormSize;//Size of the storm

    [SerializeField]
    private Vector2 m_vSizeRanges;

    [SerializeField]
    private Vector2 m_vRespawnRanges;

    [SerializeField]
    private Color m_TerrainColour;

    private bool m_bLeftToRight = false;//Direction of the storm

    [Header("Storm Timing")]
    [SerializeField]
    private float m_fTimeUntilNextStorm;//Time until the next storm spawns

    private bool m_bStormActive;//If the storm is active or not
    private float m_fDirtyLerpTracker;//Tracks the point of the line the storm is at

    [Header("Wind Force")]
    [SerializeField]
    private WindStrength m_WindForce;

    private float m_fTimerMax;
    private bool m_bSafeToSpawnObjects = false;

    [Header("Sound Settings")]
    private AudioSource StormSound;

    [SerializeField]
    private float m_fVolumeIncreaseSpeed;

    [SerializeField]
    private GameObject m_gChildParticles;

    public void Start()
    {
        SetStormSize(m_fStormSize);
        StormSound = GetComponent<AudioSource>();

        //Sets the initial storm positions
        m_fStartPositionA = m_tPointA.position;
        m_fStartPositionB = m_tPointB.position;

        //Set the floor size for the arena
        m_fFloorSize.x = m_fStartPositionA.x - m_fStartPositionB.x;
        m_fFloorSize.y = m_fStartPositionA.z - m_fStartPositionB.z;

        HideStorm();

        //Dirty IF statements
        if (m_fFloorSize.x < 0)
        {
            m_fFloorSize.x *= -1;
        }
        if (m_fFloorSize.y < 0)
        {
            m_fFloorSize.y *= -1;
        }
    }

    public void FixedUpdate()
    {
        UpdateColour();
        StormAudioHandler();
        if (m_bStormActive == true)
        {
            m_WindForce = WindStrength.STRONG;
            UpdateStorm();//Update the storm
        }
        else
        {
            m_fTimeUntilNextStorm -= Time.deltaTime;
            SetWindForce();
            if (m_fTimeUntilNextStorm <= 0)
            {
                StartRandomStorm();
                m_fTimeUntilNextStorm = Random.Range(m_vRespawnRanges.x, m_vRespawnRanges.y);
                m_fTimerMax = m_fTimeUntilNextStorm;
            }
        }
    }

    private void UpdateColour()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out raycastHit, 3))
        {
            if (raycastHit.transform.gameObject.CompareTag("Terrain"))
            {
                return;
            }
            Texture2D hitTex = (Texture2D)raycastHit.transform.gameObject.GetComponent<MeshRenderer>().material.mainTexture;

            GetComponent<ParticleSystem>().startColor = hitTex.GetPixel((int)Mathf.Floor(raycastHit.textureCoord.x * hitTex.width), (int)Mathf.Floor(raycastHit.textureCoord.y * hitTex.height));
        }
    }

    private void SetWindForce()
    {
        float fTimePerc = (m_fTimerMax - m_fTimeUntilNextStorm) / m_fTimerMax;
        if (fTimePerc >= 0.75f)
        {
            m_WindForce = WindStrength.STRONG;
        }
        else if (fTimePerc >= 0.5f)
        {
            m_WindForce = WindStrength.MEDIUM;
        }
        else if (fTimePerc >= 0.25f)
        {
            m_WindForce = WindStrength.WEAK;
        }
        else
        {
            m_WindForce = WindStrength.NONE;
        }
    }

    private void CalculateNewStorm()
    {
        //Set the positions
        m_tPointA.position = m_fStartPositionA;
        m_tPointB.position = m_fStartPositionB;

        int iRandomDirection = Random.Range(0, 4);

        if (iRandomDirection <= 1)
        {
            //Calculate the third position for the behzier curve
            m_fStartPositionC = m_tPointA.position;
            m_fStartPositionC.x = ((m_tPointA.position.x + m_tPointB.position.x) / 2);//Set to midpoint of A and B

            //Translate all points down by a random number to draw a new path for the curve
            m_tPointA.Translate(0.0f, 0.0f, Random.Range(0.0f, m_fFloorSize.y));
            m_tPointB.Translate(0.0f, 0.0f, -Random.Range(0.0f, m_fFloorSize.y));
            m_fStartPositionC.z = Random.Range(0.0f, m_fFloorSize.y);
        }
        else
        {
            //Calculate the third position for the behzier curve
            m_fStartPositionC = m_tPointA.position;
            m_fStartPositionC.z = ((m_tPointA.position.z + m_tPointB.position.z) / 2);//Set to midpoint of A and B

            //Translate all points down by a random number to draw a new path for the curve
            m_tPointA.Translate(-Random.Range(0.0f, m_fFloorSize.x), 0.0f, 0.0f);
            m_tPointB.Translate(Random.Range(0.0f, m_fFloorSize.x), 0.0f, 0.0f);
            m_fStartPositionC.x = Random.Range(0.0f, m_fFloorSize.x);
        }

        if (iRandomDirection == 0 || iRandomDirection == 2)
        {
            m_bLeftToRight = true;
        }
        else
        {
            m_bLeftToRight = false;
        }
    }

    private void StartStorm(float a_fSize)
    {
        SetStormSize(a_fSize);
        m_bStormActive = true;//Start the storm
        transform.position = m_tPointA.position;//Set the position of the storm to the start point
        m_fDirtyLerpTracker = 0.0f;//Reset the lerp tracker
        m_bSafeToSpawnObjects = false;
    }

    private void UpdateStorm()
    {
        if (m_bLeftToRight)
        {
            transform.position = GetLerpPosition(m_tPointA.position, m_tPointB.position, m_fStartPositionC, m_fDirtyLerpTracker);//Get the current position of the storm on the curve
        }
        else
        {
            transform.position = GetLerpPosition(m_tPointB.position, m_tPointA.position, m_fStartPositionC, m_fDirtyLerpTracker);//Get the current position of the storm on the curve
        }
        m_fDirtyLerpTracker += m_fStormSpeed;//Add the storm speed to the lerp tracker

        if (m_fDirtyLerpTracker >= 0.3f && m_fDirtyLerpTracker <= 0.7f)
        {
            m_bSafeToSpawnObjects = true;
        }
        else
        {
            m_bSafeToSpawnObjects = false;
        }
        if (m_fDirtyLerpTracker >= 1.0f)//If the path is complete
        {
            m_bStormActive = false;//Deactivate the storm
            m_bSafeToSpawnObjects = false;

            HideStorm();
        }
    }

    private Vector3 GetLerpPosition(Vector3 a_v3Pos0, Vector3 a_v3Pos1, Vector3 a_v3Pos2, float a_fTime)//Get cubic point of curve
    {
        return Vector3.Lerp(Vector3.Lerp(a_v3Pos0, a_v3Pos2, a_fTime), Vector3.Lerp(a_v3Pos2, a_v3Pos1, a_fTime), a_fTime);
    }

    public float GetStormSize()
    {
        return m_fStormSize;
    }

    public bool GetSafeSpawn()
    {
        return m_bSafeToSpawnObjects;
    }

    public WindStrength GetWindStrength()
    {
        return m_WindForce;
    }

    public float GetStormSpeed()
    {
        return m_fStormSpeed;
    }

    public float GetTimeUntilNextStorm()
    {
        return m_fTimeUntilNextStorm;
    }

    public bool GetStormActive()
    {
        return m_bStormActive;
    }

    public void StartRandomStorm()
    {
        if (!m_bStormActive)
        {
            CalculateNewStorm();//Calculate a new storm path
            StartStorm(Random.Range(m_vSizeRanges.x, m_vSizeRanges.y));//Start the storm
        }
    }

    public void StartStormWithSize(float a_fSize)
    {
        if (!m_bStormActive)
        {
            CalculateNewStorm();//Calculate a new storm path
            StartStorm(a_fSize);
        }
    }

    public void HideStorm()
    {
        SetStormSize(0.0f);
    }

    public void ShowStorm()
    {
        SetStormSize(1.0f);
    }

    public void SetStormSize(float a_fSize)
    {
        m_fStormSize = a_fSize;
        transform.localScale = new Vector3(a_fSize, a_fSize, a_fSize);

        m_gChildParticles.transform.localScale = new Vector3(a_fSize, a_fSize, a_fSize);
    }

    public void SetStormSpeed(float a_fSpeed)
    {
        m_fStormSpeed = a_fSpeed;
    }

    public void SetStormActive(bool a_bActive)
    {
        m_bStormActive = a_bActive;
    }

    public void SetTimeUntilNextStorm(float a_fSeconds)
    {
        m_fTimeUntilNextStorm = a_fSeconds;
    }

    public void AddTimeUntilNextStorm(float a_fSeconds)
    {
        m_fTimeUntilNextStorm += a_fSeconds;
    }

    private void StormAudioHandler()
    {
        //Fade in
        if (StormSound.volume >= 1.0f && m_bStormActive)
        {
            StormSound.volume = 1.0f;
        }
        if (StormSound.volume < 1.0f && m_bStormActive)
        {
            while (StormSound.volume < 1.0f)
            {
                StormSound.volume += m_fVolumeIncreaseSpeed;
                return;
            }
        }
        //Fade Out
        if (StormSound.volume <= 0.1f && !m_bStormActive)
        {
            StormSound.volume = 0.1f;
        }
        if (StormSound.volume > 0.1f && !m_bStormActive)
        {
            while (StormSound.volume > 0.1f)
            {
                StormSound.volume -= m_fVolumeIncreaseSpeed;
                return;
            }
        }
    }
}