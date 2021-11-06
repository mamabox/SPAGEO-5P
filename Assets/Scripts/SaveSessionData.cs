using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class SaveSessionData : MonoBehaviour
{
    private float recordingInterval = 1f; // sample rate in Hz (cycle per second)
    private string fileName;
    private string filePath;
    private string sessionSummaryText;
    private char fileNameDelimiter = '-';
    private List<string> routeCoord;

    private StreamWriter sw;
    private char delimiter = ',';

    private RouteManager routeManager;
    private CheckpointManager checkpointManager;
    private IntersectionManager intersectionManager;
    private GameObject player;
    private PlayerController playerController;
    private GameManager gameManager;
    private HotspotManager hotspotManager;
    private UIManager uIManager;
    private SequenceManager sequenceManager;

    void Awake()
    {
        routeManager = GetComponent<RouteManager>();
        checkpointManager = GetComponent<CheckpointManager>();
        intersectionManager = GetComponent<IntersectionManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        hotspotManager = FindObjectOfType<GameManager>().GetComponent<HotspotManager>();
        uIManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        sequenceManager = FindObjectOfType<GameManager>().GetComponent<SequenceManager>();

        filePath = Path.Combine(Directory.GetCurrentDirectory(), "Exports/SessionsData/"); //For macOS, Linux

        if (!System.IO.Directory.Exists(filePath))    //if save directory does not exist, create it
        {
            System.IO.Directory.CreateDirectory(filePath);
        }

        routeCoord = new List<string>();

    }

    private void Start()
    {

    }

    public void StartSavingData()
    {
        if (gameManager.sessionData.selectedRouteCoord.Any())
        {
            routeCoord = new List<string>(gameManager.sessionData.selectedRouteCoord);
            if (routeCoord.Count >= 2)
                routeCoord.RemoveAt(0);
        }
        


        Debug.Log("Start saving data");
        SetFileName();
        sw = File.AppendText(filePath + fileName);
        sw.WriteLine(HeadersConstructor());
        InvokeRepeating("SaveData", 0, 1 / recordingInterval);


    }

    public void ContinueSavingData()
    {
        sw = File.AppendText(filePath + gameManager.sessionData.fileName);
        InvokeRepeating("SaveData", 0, 1 / recordingInterval);
    }


    private string HeadersConstructor()
    {
        string sessionDataHeader;
        string gameDataHeader;
        string checkpointDataHeader;
        string hotspotDataHeader;
        string playerDataHeader;
        string intersectionDataHeader;
        string routeDataHeader;
        string generalGameDataHeader;

        gameDataHeader =  "sessionTime" + delimiter + "gameIsPause" + delimiter + "sessionDistance" + delimiter + "validationCount" + delimiter + "attemptCount" + delimiter + "attemptTime" + delimiter + "attemptDistance";
        sessionDataHeader = "isGroupSession" + delimiter + "groupID" + delimiter + "studentIDs" + delimiter + "scenario" + delimiter + "selectedRoute" + delimiter + "isSender" + delimiter + "routeStart" + delimiter + "selectedRouteCoord";
        generalGameDataHeader = "time" + delimiter + "scene";
        playerDataHeader = "posX" + delimiter + "posZ" + delimiter + "rotation";
        routeDataHeader = "routeIsValid" + delimiter + "routeEndReached" + delimiter + "routeErrorAt" + delimiter + "routeLength";
        intersectionDataHeader = "routeCoord";
        checkpointDataHeader = "isOnCheckpoint" + delimiter + "lastCheckpoint";
        hotspotDataHeader = "isOnHotspot" + delimiter + "lastHotspot";

        return gameDataHeader + delimiter + sessionDataHeader + delimiter + playerDataHeader + delimiter + generalGameDataHeader + delimiter + routeDataHeader + delimiter + intersectionDataHeader + delimiter + checkpointDataHeader + delimiter + hotspotDataHeader;
    }


    public void SaveData()
    {
        string sessionData;
        string gameData;
        string checkpointData;
        string hotspotData;
        string playerData;
        string intersectionData;
        string routeData;
        string gameGeneralData;


        gameGeneralData = TimeSpan.FromSeconds(Time.unscaledTime).ToString(@"mm\:ss") + delimiter + SceneManager.GetActiveScene().name;
        sessionData = (gameManager.sessionData.isGroupSession.ToString() + delimiter + gameManager.sessionData.groupID + delimiter + string.Join("-",gameManager.sessionData.studentIDs) + delimiter + gameManager.sessionData.selectedScenario + delimiter + (gameManager.sessionData.selectedRoute + 1) + delimiter + gameManager.sessionData.isSender + delimiter + string.Join("", gameManager.sessionData.routeStart) + delimiter + string.Join("-", routeCoord));
        gameData = gameManager.sessionTimeElapsed.ToString(@"mm\:ss") + delimiter + gameManager.gameIsPaused.ToString() + delimiter + gameManager.sessionDistance.ToString("F2") + delimiter + gameManager.sessionData.validationCountNew + delimiter + gameManager.attemptCount + delimiter + gameManager.attemptTimeElapsed.ToString(@"mm\:ss") + delimiter + gameManager.attemptDistance.ToString("F2");
        playerData = player.transform.position.x.ToString("F2") + delimiter + player.transform.position.z.ToString("F2") + delimiter + player.transform.rotation.eulerAngles.y.ToString("F2");
        routeData = routeManager.validationInfo.isValid.ToString() + delimiter + routeManager.validationInfo.endReached + delimiter + routeManager.validationInfo.errorAt + delimiter + routeManager.validationInfo.routeLength;
        intersectionData = string.Join("-",intersectionManager.sessionRoute);
        checkpointData = checkpointManager.onCheckpoint.ToString() + delimiter + checkpointManager.lastCheckpointCollected;
        hotspotData = hotspotManager.onHotspot.ToString() + delimiter + hotspotManager.lastHotspotCollected;

        //sw.WriteLine("Saving data");
       
        sw.WriteLine(gameData + delimiter + sessionData + delimiter + playerData + delimiter + gameGeneralData + delimiter + routeData + delimiter + intersectionData + delimiter + checkpointData + delimiter + hotspotData);
    }

    // Update is called once per frame
    public void stopSavingData()
    {
        Debug.Log("Stop saving data");
        sw.Close();
        CancelInvoke();
    }

    private void SetFileName()
    {
        string studentID;

        if (gameManager.sessionData.isGroupSession)
        {
            sessionSummaryText = "GRP" + gameManager.sessionData.groupID + fileNameDelimiter + "SCN" + gameManager.sessionData.selectedScenario + fileNameDelimiter + "RTE" + (gameManager.sessionData.selectedRoute + 1);
        }
        else
        {
            if (gameManager.sessionData.studentIDs.Count() > 0)
                studentID = gameManager.sessionData.studentIDs[0];
            else
                studentID = "";

            sessionSummaryText = "ELV" + studentID + fileNameDelimiter + "SCN" + gameManager.sessionData.selectedScenario + fileNameDelimiter + "RTE" + (gameManager.sessionData.selectedRoute + 1);
        }

        fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + fileNameDelimiter + sessionSummaryText + ".csv";
        gameManager.sessionData.fileName = fileName;
    }
}
