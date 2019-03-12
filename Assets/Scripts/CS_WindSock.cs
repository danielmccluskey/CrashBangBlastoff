using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindStrength
{
    NONE,
    WEAK,
    MEDIUM,
    STRONG
}

public class CS_WindSock : MonoBehaviour
{
    [SerializeField]
    private HingeJoint Hinge;

    private JointMotor Motor;

    [SerializeField]
    private Transform m_tObjectToRotateAround;

    [SerializeField]
    private WindStrength m_WindForce;

    [SerializeField]
    [Range(0, 10)]
    public float[] WindSpeeds;

    [SerializeField]
    private GameObject m_gStormRef;

    [SerializeField]
    private float m_fCurrentRotation = 90.0f;

    [SerializeField]
    private float m_fRotationSpeed = 0.5f;

    public void Start()
    {
        Hinge = GetComponent<HingeJoint>();
        Motor = Hinge.motor;
    }

    public void FixedUpdate()
    {
        float f_angle = Vector3.Angle(transform.position, m_gStormRef.transform.position);

        if (f_angle < m_fCurrentRotation)
        {
            transform.Rotate(Vector3.up * m_fRotationSpeed);
            m_fCurrentRotation -= m_fRotationSpeed;
        }
        if (f_angle > m_fCurrentRotation)
        {
            transform.Rotate(Vector3.zero * m_fRotationSpeed);
            m_fCurrentRotation += m_fRotationSpeed;
        }

        m_WindForce = m_gStormRef.GetComponent<CS_SmallStorms>().GetWindStrength();
        if (m_WindForce == WindStrength.NONE)
        {
            Motor.force = WindSpeeds[0];
        }
        if (m_WindForce == WindStrength.WEAK)
        {
            Motor.force = WindSpeeds[1];
        }
        if (m_WindForce == WindStrength.MEDIUM)
        {
            Motor.force = WindSpeeds[2];
        }
        if (m_WindForce == WindStrength.STRONG)
        {
            Motor.force = WindSpeeds[3];
        }

        Hinge.motor = Motor;
    }

    public void SetWindForce(WindStrength a_Strength)
    {
        m_WindForce = a_Strength;
    }

    private float DotProduct(Vector3 a_v1, Vector3 a_v2)
    {
        float fa = a_v1.x * a_v2.x;
        float fb = a_v1.y * a_v2.y;
        float fc = a_v1.z * a_v2.z;

        float cosine = (fa + fb + fc) / (a_v1.magnitude * a_v2.magnitude);

        return Mathf.Acos(cosine);
    }
}

////blueprint for obstacles

////Spawning
//private void SpawnPowerUp()
//{
//    GameObject newItem = Instantiate(PowerupPrefab, new Vector3(Storm.transform.position.x, Storm.transform.position.y + 10.0f, Storm.transform.position.z), Quaternion.identity);

//    GameObject mover = Instantiate(MoverPrefab);

//    Vector3 RandPos = new Vector3(Random.Range(StartOfItemZone.position.x, EndOfItemZone.position.x), ItemPrefabs[0].transform.localScale.y, Random.Range(StartOfItemZone.position.z, EndOfItemZone.position.z));

//    mover.GetComponent<CS_ObjectMover>().LaunchGameObject(newItem, RandPos);
//}

////Timer
//// ITEM
//        if (obstacleTimer <= 0.0f)
//        {
//            bCanSpawnobstacle = true;
//            obstacleTimer = MinBetweenobstacleSpawn;
//        }

//        if (!bCanSpawnobstacle)
//        {
//            obstacleTimer -= Time.deltaTime;
//        }
//        else
//        {
//            if (roundTimer > (fobstacleChanceCap* MaxRoundTime))
//            {
//                fSpawnChanceForobstacle = (MaxRoundTime - roundTimer) / MaxRoundTime;
//            }

//            float fRandVal = Random.Range(0, 1);
//            if (fRandVal <= fSpawnChanceForobstacle)
//            {
//                //Spawn an item
//                Spawnobstacle();
//bCanSpawnobstacle = false;
//            }
//        }

//    //variables
//    [SerializeField] private float MinBetweenobstacle = 6.0f;

//private float obstacleTimer;
//private bool bCanSpawnobstacle = true;
//[SerializeField] private float fobstacleChanceCap = 0.5f;
//private float fSpawnChanceForobstacle = 0;