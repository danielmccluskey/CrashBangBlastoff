using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class CS_Gamemanager : MonoBehaviour
{
    public static CS_Gamemanager instance = null;

    public static int p1RoundsWon = 0;
    public static int p2RoundsWon = 0;

    public bool bEndGameDebug = false;

    public bool bGameActive = false;
    //static bool bRoundActive = false;

    // These will hold references to the players
    [SerializeField] private GameObject[] ScenePlayers; // Set in editor

    [SerializeField]
    private GameObject[] SceneLaserTargets;

    public static GameObject[] LaserTarget;

    public static GameObject[] Players;

    [SerializeField] private GameObject[] SceneItemPads; // Set in editor
    public static GameObject[] ItemPads;

    [SerializeField] private GameObject[] SceneItemPrefabs; // Set in editor
    public static GameObject[] ItemPrefabs;

    [SerializeField] private GameObject ScenePowerupPrefab; // Set in editor
    public static GameObject PowerupPrefab;

    [SerializeField] private GameObject[] SceneRockets; // Set in editor
    public static GameObject[] Rockets;

    [SerializeField] private GameObject SceneMoverPrefab;
    public static GameObject MoverPrefab;

    [SerializeField] private Transform SceneStartOfItemZone;
    public static Transform StartOfItemZone;

    [SerializeField] private Transform SceneEndOfItemZone;
    public static Transform EndOfItemZone;

    [SerializeField] private GameObject SceneStorm;
    public static GameObject Storm;

    [SerializeField] public static float InteractionRadius = 2.0f;

    [SerializeField] private float MaxRoundTime = 90;
    private float roundTimer = 90;

    // Spawning Powerups
    [SerializeField] private float MinBetweenPowerSpawn = 5;

    private float powerTimer;
    private bool bCanSpawnPowerUp = true;
    [SerializeField] private float fPowerChanceCap = 0.3f;
    private float fSpawnChanceForPowerUp = 0;

    public GameObject HIDESKIP;
    // Spawning Items
    [SerializeField] private float MinBetweenItemSpawn = 6.0f;

    private float itemTimer;
    private bool bCanSpawnItem = true;
    [SerializeField] private float fItemChanceCap = 0.5f;
    private float fSpawnChanceForItem = 0;

    public bool bInGame = false;
    private bool bDoOnce = true;
    private bool bPlayersAtRockets = false;
    private bool bRocketsLiftOff = false;
    private float fMaxBlastOffTime = 2.0f;
    private float fBlastOffTimer;
    private GameObject Cam;
    private Transform CamResartPos;
    private bool[] bRocketIsFlying = { false, false };
    private bool[] bCheckOnce = { true, true };
    private float fChanceMaxTime = 1.0f;
    private float fChanceTimer = 0;
    private int iBrokenTime = 0;
    private int iNumOfRocketsInAir = 0;
    private bool bLastRockedFinished = false;
    private float fDistanceTravelled = 0;
    public bool bGameIsEnding = false;
    private List<int> ItemsToDrop = new List<int>();
    private float fTimeOnScreen = 0.5f;

    [SerializeField]
    private GameObject[] DistanceText;

    [SerializeField]
    private GameObject EndGameUI;

    [SerializeField]
    private GameObject[] ButtonPrompt;

    [SerializeField]
    private GameObject Timer;

    [SerializeField]
    private Image[] Animback;

    [SerializeField]
    private Gradient gradient;

    [SerializeField]
    private float duration;

    private float fGradientTimer = 0f;
    public float fFadeTime = 1f;

    [SerializeField]
    private Image[] Animations;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GetPlayersFromInsp();
            GetPadsFromInsp();
            GetRocketsFromInsp();
            GetItemsFromInsp();
            GetPowerupFromInsp();
            GetMoverFromInsp();
            GetZoneFromInsp();
            GetStormFromInsp();
            SetTimers();
            SetUpCamera();
            SetUpText();
            SetUpUI();
            SetTargets();
            EndGameUI.SetActive(false);
            StartCoroutine("FadeAnimation");
            Debug.LogError("Awake");
            fTimeOnScreen = 0.5f;
            // Variables();
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void SetTargets()
    {
        LaserTarget = SceneLaserTargets;
    }

    private void SetUpText()
    {
        for (int i = 0; i < 2; i++)
        {
            DistanceText[i].GetComponent<Text>().text = "0m";
            DistanceText[i].SetActive(false);
        }
    }

    private void SetUpUI()
    {
        for (int i = 0; i < 2; i++)
        {
            ButtonPrompt[i].SetActive(false);
        }
    }

    private void ResetVariables()
    {
        bInGame = false;
        bDoOnce = true;
        bPlayersAtRockets = false;
        bRocketsLiftOff = false;
        fMaxBlastOffTime = 2.0f;
        bRocketIsFlying[0] = false;
        bRocketIsFlying[1] = false;
        bCheckOnce[0] = true;
        bCheckOnce[1] = true;
        fChanceMaxTime = 1.0f;
        fChanceTimer = 0;
        iBrokenTime = 0;
        iNumOfRocketsInAir = 0;
        bLastRockedFinished = false;
        fDistanceTravelled = 0;
        bGameIsEnding = false;
    }

    private void SetUpCamera()
    {
        if (Cam == null)
        {
            Cam = GameObject.FindWithTag("MainCamera");
        }
        CamResartPos = Cam.transform;
    }

    private void SetTimers()
    {
        fBlastOffTimer = fMaxBlastOffTime;
        fChanceTimer = fChanceMaxTime;
    }

    private void GetPowerupFromInsp()
    {
        PowerupPrefab = ScenePowerupPrefab;
    }

    private void GetStormFromInsp()
    {
        Storm = SceneStorm;
    }

    private void GetZoneFromInsp()
    {
        StartOfItemZone = SceneStartOfItemZone;
        EndOfItemZone = SceneEndOfItemZone;

        if (StartOfItemZone == null || EndOfItemZone == null)
        {
            Debug.Log("Lol bad");
        }
    }

    private void Update()
    {
        if (!bInGame)
        {
            return;
        }

        if (bLastRockedFinished && !bGameActive)
        {
            GoBackToMenuCheck();
        }
        if (bGameActive)
        {
            int iInt = (int)roundTimer;
            Timer.GetComponent<Text>().text = iInt.ToString();
            if (roundTimer <= 10)
            {
                Timer.GetComponent<Text>().color = Color.red;
            }
            else
            {
                Timer.GetComponent<Text>().color = Color.black;
            }
            if (bGameIsEnding)
            {
                EndRound();
                Timer.SetActive(false);
            }
            if (bEndGameDebug)
            {
                EndRound();
            }

            for (int i = 0; i < 2; i++)
            {
                if (CheckPartsComplete(i))
                {
                    ButtonPrompt[i].SetActive(true);
                }
                else
                {
                    ButtonPrompt[i].SetActive(false);
                }
            }

            roundTimer -= Time.deltaTime;

            if (roundTimer <= 0.0f)
            {
                // Game Finish
                bGameIsEnding = true;
            }

            if (!Storm.GetComponent<CS_SmallStorms>().GetSafeSpawn())
            {
                return;
            }
            // POWERUP
            if (powerTimer <= 0.0f)
            {
                bCanSpawnPowerUp = true;
                powerTimer = MinBetweenPowerSpawn;
            }

            if (bCanSpawnPowerUp == false)
            {
                powerTimer -= Time.deltaTime;
            }
            else
            {
                if (roundTimer > (fPowerChanceCap * MaxRoundTime))
                {
                    fSpawnChanceForPowerUp = (MaxRoundTime - roundTimer) / MaxRoundTime;
                }

                float fRandVal = Random.Range(0, 1);
                if (fRandVal <= fSpawnChanceForPowerUp)
                {
                    //Spawn a power up
                    SpawnPowerUp();
                    bCanSpawnPowerUp = false;
                }
            }

            // ITEM
            if (itemTimer <= 0.0f)
            {
                bCanSpawnItem = true;
                itemTimer = MinBetweenItemSpawn;
            }

            if (!bCanSpawnItem)
            {
                itemTimer -= Time.deltaTime;
            }
            else
            {
                if (roundTimer > (fItemChanceCap * MaxRoundTime))
                {
                    fSpawnChanceForItem = (MaxRoundTime - roundTimer) / MaxRoundTime;
                }

                float fRandVal = Random.Range(0, 1);
                if (fRandVal <= fSpawnChanceForItem)
                {
                    //Spawn an item
                    SpawnItem();
                    bCanSpawnItem = false;
                }
            }
        }
        else if (Input.GetButtonDown("A1"))
        {
            StopCoroutine("FadeAnimation");
            for (int i = 0; i < Animations.Length; i++)
            {
                Color AnimColour = Animations[i].color;
                AnimColour.a = 0;
                Animations[i].color = AnimColour;
                //Animations[i].enabled = false;
            }
            for (int i = 0; i < Animback.Length; i++)
            {
                Color AnimColour = Animback[i].color;
                AnimColour.a = 0;
                Animback[i].color = AnimColour;
                Animback[i].enabled = false;
                Animback[i].enabled = false;
            }
            HIDESKIP.SetActive(false);
            StartGame();
        }
    }

    private void GetPlayersFromInsp()
    {
        if (ScenePlayers.Length > 0)
        {
            Players = ScenePlayers;
        }
        else
        {
            Players = GameObject.FindGameObjectsWithTag("Player");
        }
    }

    private void GetRocketsFromInsp()
    {
        if (SceneRockets.Length > 0)
        {
            Rockets = SceneRockets;
        }
        else
        {
            Rockets = GameObject.FindGameObjectsWithTag("Rockets");
        }
    }

    private void GetPadsFromInsp()
    {
        if (SceneItemPads.Length > 0)
        {
            ItemPads = SceneItemPads;
        }
        else
        {
            ItemPads = GameObject.FindGameObjectsWithTag("ItemPad");
        }
    }

    private void GetItemsFromInsp()
    {
        if (SceneItemPads.Length > 0)
        {
            ItemPrefabs = SceneItemPrefabs;
        }
        else
        {
            Debug.Log("Set items in inspector");
        }
    }

    private void GetMoverFromInsp()
    {
        MoverPrefab = SceneMoverPrefab;
    }

    public static Vector3 TriggerPowerup(GameObject a_player)
    {
        if (a_player == Players[0]) // If player one picked up the power up
        {
            ItemPads[1].GetComponent<CS_ItemPad>().DropHalfItems();
            a_player.GetComponent<CS_PlayerController>().FireLaser(LaserTarget[1].transform.position);
            return ItemPads[1].gameObject.transform.position;
            // Destroy the powerup
        }
        else if (a_player == Players[1])
        {
            ItemPads[0].GetComponent<CS_ItemPad>().DropHalfItems();
            a_player.GetComponent<CS_PlayerController>().FireLaser(LaserTarget[0].transform.position);

            return ItemPads[0].gameObject.transform.position;
            // Destroy the powerup
        }
        else
        {
            // wtf
            Debug.Log("Something else hit the powerup");
            return new Vector3(6969, 6969, 6969);
        }
    }

    private void SpawnItem()
    {
        int itemsID = -1;

        for (int i = 0; i < 4; i++)
        {
            if (ItemPads[0].GetComponent<CS_ItemPad>().IsSlotFilled[i] == false && ItemPads[1].GetComponent<CS_ItemPad>().IsSlotFilled[i] == false)
            {
                ItemsToDrop.Add(i);
                //itemsID = i;
            }
        }

        int[] PlayerAmount = new int[2];

        if (itemsID == -1)
        {
            // Test who has the least parts
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (ItemPads[j].GetComponent<CS_ItemPad>().IsSlotFilled[i] == false)
                    {
                        PlayerAmount[j]++;
                    }
                }
            }

            if (PlayerAmount[0] > PlayerAmount[1]) //If player one has less parts than player 2
            {
                int iRandVal = Random.Range(0, ItemsToDrop.Count - 1); //Random number between 0 and
                if (ItemPads[0].GetComponent<CS_ItemPad>().IsSlotFilled[ItemsToDrop[iRandVal]] == false)
                {
                    itemsID = ItemsToDrop[iRandVal];
                }
            }
            else
            {
                int iRandVal = Random.Range(0, ItemsToDrop.Count - 1); //Random number between 0 and
                if (ItemPads[1].GetComponent<CS_ItemPad>().IsSlotFilled[ItemsToDrop[iRandVal]] == false)
                {
                    itemsID = ItemsToDrop[iRandVal];
                }
            }
        }
        else
        {
            itemsID = Random.Range(0, 3);
        }

        ItemsToDrop.Clear();

        GameObject newItem = Instantiate(ItemPrefabs[itemsID]);
        newItem.transform.position = Storm.transform.position;
        GameObject mover = Instantiate(MoverPrefab);

        Vector3 RandPos = new Vector3(Random.Range(StartOfItemZone.position.x, EndOfItemZone.position.x), ItemPrefabs[0].transform.localScale.y, Random.Range(StartOfItemZone.position.z, EndOfItemZone.position.z));

        mover.GetComponent<CS_ObjectMover>().LaunchGameObject(newItem, RandPos);
    }

    private void SpawnPowerUp()
    {
        GameObject newItem = Instantiate(PowerupPrefab, new Vector3(Storm.transform.position.x, Storm.transform.position.y + 10.0f, Storm.transform.position.z), Quaternion.identity);

        GameObject mover = Instantiate(MoverPrefab);

        Vector3 RandPos = new Vector3(Random.Range(StartOfItemZone.position.x, EndOfItemZone.position.x), ItemPrefabs[0].transform.localScale.y, Random.Range(StartOfItemZone.position.z, EndOfItemZone.position.z));

        mover.GetComponent<CS_ObjectMover>().LaunchGameObject(newItem, RandPos);
    }

    public static void DropItem(int a_iDItemToSpawn, Transform a_Start)
    {
        //GameObject newItem = Instantiate(ItemPrefabs[a_iDItemToSpawn]);
        GameObject newItem = Instantiate(ItemPrefabs[a_iDItemToSpawn], new Vector3(Storm.transform.position.x, Storm.transform.position.y + 10.0f, Storm.transform.position.z), Quaternion.identity);

        //newItem.transform.position = a_Start.position;
        //newItem.transform.Translate(0.0f, 50.0f, 0.0f);

        GameObject mover = Instantiate(MoverPrefab);

        Vector3 RandPos = new Vector3(Random.Range(StartOfItemZone.position.x, EndOfItemZone.position.x), ItemPrefabs[0].transform.localScale.y, Random.Range(StartOfItemZone.position.z, EndOfItemZone.position.z));

        mover.GetComponent<CS_ObjectMover>().LaunchGameObject(newItem, RandPos); // DEBUG TO ZERO, PICK RANDOM
    }

    private void StartGame()
    {
        if (bGameActive)
        {
            return;
        }

        p1RoundsWon = 0;
        p2RoundsWon = 0;

        bGameActive = true;
    }

    private void StartRound()
    {
        //if(bRoundActive)
        //{
        //    return;
        //}
        SceneManager.LoadScene(1);
    }

    public void EndRound()
    {
        //if(!bRoundActive)
        //{
        //    return;
        //}
        if (bDoOnce)
        {
            // Disable movement on both players
            for (int i = 0; i < 2; i++)
            {
                Players[i].GetComponent<CS_PlayerController>().bMovingAvailable = false;
                // Move the players to their rockets
                Players[i].GetComponent<CS_PlayerController>().RespawnPlayer();
            }
            bDoOnce = false;
        }
        if (bPlayersAtRockets && bRocketsLiftOff && !bLastRockedFinished)
        {
            CS_CameraShake.Instance.StopShaking();
            fDistanceTravelled += (Time.deltaTime * 100);
            // Animate rockets launching
            for (int i = 0; i < 2; i++)
            {
                //if Rocket still going up
                CheckParts(i); //Checks if rocket is complete
                if (bRocketIsFlying[i])
                {
                    Rise(Rockets[i], 0.5f);
                    DistanceText[i].SetActive(true);
                    DistanceText[i].GetComponent<Text>().text = fDistanceTravelled + "m";
                    GetComponent<AudioSource>().volume = 1.0f;
                }
            }
            if (iNumOfRocketsInAir == 0)
            {
                bLastRockedFinished = true;
            }
            // Camera follow
            Rise(Cam, 0.5f);
            RotateCamera();

            // Change background as rockets raise higher
            GradientColours();
            // Determine if the rockets survive
            fChanceTimer -= Time.deltaTime;
            if (fChanceTimer <= 0) //Checks every second
            {
                for (int i = 0; i < 2; i++)
                {
                    if (CheckRocketSurvive(i) == false && bRocketIsFlying[i])
                    {
                        if (iBrokenTime == 1)
                        {
                            bRocketIsFlying[i] = false; //It falls - currently
                            iNumOfRocketsInAir--;
                        }
                        else
                        {
                            // Animate shaking of bad rockets and parts falling off (of both)
                            ShakeRocket(Rockets[i], 5f);
                            BrakeOffBits(Rockets[i]);
                            iBrokenTime++; //Allows three seconds of shaking and broken bits
                        }
                    }
                }
                fChanceTimer = fChanceMaxTime;
            }
            if (bLastRockedFinished) //End Cutscene finished
            {
                //Thats it
                bGameActive = false;
                EndGameUI.SetActive(true);
            }

            // Blow up loosing rockets

            // Pan camera to stary sky and fade out

            // Award a round to a player
            if (p1RoundsWon >= 3 || p2RoundsWon >= 3)
            {
                EndGame();
            }
        }
        else if (bPlayersAtRockets && !bRocketsLiftOff) //BuildUp
        {
            //Play Fire Particles and sound here once
            CS_CameraShake.Instance.Shake(0.15f, fMaxBlastOffTime);
            fBlastOffTimer -= Time.deltaTime;
            if (fBlastOffTimer <= 0)
            {
                fBlastOffTimer = fMaxBlastOffTime;
                bRocketsLiftOff = true;
            }
        }
        else
        {
            if (Players[0].GetComponent<CS_PlayerController>().m_bIsRespawning == false)
            {
                bPlayersAtRockets = true;
            }
            else if (!CheckPartsComplete(0) && !CheckPartsComplete(1))
            {
                Players[0].GetComponent<CS_PlayerController>().bMovingAvailable = true;
                Players[1].GetComponent<CS_PlayerController>().bMovingAvailable = true;
            }
        }
    }

    private void CheckParts(int iPlayerNum)
    {
        if (ItemPads[iPlayerNum].GetComponent<CS_ItemPad>().IsSlotFilled[0] == true
            && ItemPads[iPlayerNum].GetComponent<CS_ItemPad>().IsSlotFilled[1] == true
            && ItemPads[iPlayerNum].GetComponent<CS_ItemPad>().IsSlotFilled[2] == true
            && ItemPads[iPlayerNum].GetComponent<CS_ItemPad>().IsSlotFilled[3] == true) //Item Pads are full
        {
            bRocketIsFlying[iPlayerNum] = true;
            if (bCheckOnce[iPlayerNum])
            {
                iNumOfRocketsInAir++;
                bCheckOnce[iPlayerNum] = false;
                for (int i = 0; i < 4; i++)
                {
                    Players[iPlayerNum].GetComponent<CS_PlayerController>().iPlayerScore += (int)(ItemPads[iPlayerNum].GetComponent<CS_ItemPad>().Slots[i].GetComponent<CS_PadItem>().fProbablitlityMod * 100);
                }
            }
        }
        else
        {
            bRocketIsFlying[iPlayerNum] = false;
        }
    }

    private bool CheckPartsComplete(int iPlayerNum)
    {
        if (ItemPads[iPlayerNum].GetComponent<CS_ItemPad>().IsSlotFilled[0] == true
            && ItemPads[iPlayerNum].GetComponent<CS_ItemPad>().IsSlotFilled[1] == true
            && ItemPads[iPlayerNum].GetComponent<CS_ItemPad>().IsSlotFilled[2] == true
            && ItemPads[iPlayerNum].GetComponent<CS_ItemPad>().IsSlotFilled[3] == true) //Item Pads are full
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Rise(GameObject a_g, float fSpeed)
    {
        a_g.transform.position = new Vector3(a_g.transform.position.x, a_g.transform.position.y + fSpeed, a_g.transform.position.z);
    }

    private void RotateCamera()
    {
        if (Cam.transform.localEulerAngles.x > 10f)
        {
            Cam.transform.localEulerAngles = new Vector3(Cam.transform.localEulerAngles.x - 0.3f, Cam.transform.localEulerAngles.y, Cam.transform.localEulerAngles.z);
        }
        if (Cam.transform.position.z > -12f)
        {
            Cam.transform.position = new Vector3(Cam.transform.position.x + 0.01f, Cam.transform.position.y - 0.3f, Cam.transform.position.z - 0.3f);
        }
    }

    private void GradientColours()
    {
        float fVal = Mathf.Lerp(0f, 1f, fGradientTimer);
        fGradientTimer += Time.deltaTime / duration;
        Color color = gradient.Evaluate(fVal);
        Camera.main.backgroundColor = color;
    }

    private bool CheckRocketSurvive(int a_iPlayerNum)
    {
        float fChance = Random.Range(0, 100);
        if (fChance < Players[a_iPlayerNum].GetComponent<CS_PlayerController>().iPlayerScore)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ShakeRocket(GameObject a_Rocket, float a_fAmp)
    {
        transform.localPosition = a_Rocket.transform.position + Random.insideUnitSphere * a_fAmp;
    }

    public void SetTimer(int iPlayerNum)
    {
        if (CheckPartsComplete(iPlayerNum - 1) && roundTimer > 5)
        {
            roundTimer = 5;
        }
    }

    private void BrakeOffBits(GameObject a_Rocket)
    {
        //Will eventually find the lowest value item and break that off
    }

    private IEnumerator FadeAnimation()
    {
        for (int i = 0; i < Animback.Length; i++)
        {
            Animback[i].enabled = true;
        }
        for (int i = 0; i < Animations.Length; i++)
        {
            Color AnimColour = Animations[i].color;
            AnimColour.a = 0;
            Animations[i].color = AnimColour;
            //Animations[i].enabled = false;
        }
        //_---------ANIMATIONS_--------------
        float fRate = 1.0f / fFadeTime;
        int iStartAlpha = 0;
        int iEndAlpha = 1;
        int j = 0;

        for (int i = 0; i < 18; i++) //Fade in/ Fade Out
        {
            if (i >= 14)
            {
                fTimeOnScreen = 2.5f;
                if (i >= 16)
                {
                    fTimeOnScreen = 12f;
                }
            }

            float fProgress = 0.0f;
            //yield return new WaitForSeconds(2); //How long it stays on the screen
            while (fProgress < 1f) //Whilst not completed
            {
                Color AnimColour = Animations[j].color;
                AnimColour.a = Mathf.Lerp(iStartAlpha, iEndAlpha, fProgress); //Fades in effect
                Animations[j].color = AnimColour;
                fProgress += fRate * Time.deltaTime; //Fades equally in an its dependant on time
                yield return null;
            }
            if ((i + 1) % 2 == 0 && i != 17) // If i is divisible by two
            {
                yield return new WaitForSeconds(0.01f); //Time between frames
                Animations[j].enabled = false;
                j++; //Move on to next Animation
                Animations[j].enabled = true;
            }
            else if (i != 17)
            {
                yield return new WaitForSeconds(fTimeOnScreen); //How long it stays on screen
            }
            int iTemp = iStartAlpha;
            iStartAlpha = iEndAlpha;
            iEndAlpha = iTemp;
        }

        Debug.LogError("ImageComplete");
        StartGame();
        for (int i = 0; i < Animback.Length; i++)
        {
            Animback[i].enabled = false;
        }
        //SceneManager.LoadScene(1); //Second scene
    }

    private void GoBackToMenuCheck()
    {
        if (Input.GetButtonDown("A" + 1))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void EndGame()
    {
        if (!bGameActive)
        {
            return;
        }
        // Display the scores and the winner
        // Display the menu to return to menu or reply
    }
}