using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

/**
 * Sc9Manager.cs
 * 
 * Manages the Scenario 9 - PTSOT task
 * The player goes through a set of trials defined in scenariosData.json
 * For each trial, the player starts at a defined coordinate and looks at a start object. Their movement is locked but they are free to rotate in either direction. The player then points towards a target object by rotating and validates their choice.
 * After each trial, the degree of error is recorded and the total and average error for all trials is calculated.
 * NOTES ABOUT SAVING DATA 
 **/

public class Sc9Manager : MonoBehaviour
{
    // Game objects and classes
    private GameManager gameManager;
    //private UIManager uiManager;
    private GameObject player;
    private Sc9Data _sc9Data;
    private SequenceManager scenarioManager;
    public GameObject visor;

    //Props
    private GameObject startObj;    // Object the player looks at, at start of trial
    private GameObject targetObj;   // Object the player is meant to point towards
    public GameObject[] allObjects; // Array of all objects. Listed in order in scenariosData.json

    //Used for tracking
    private int trialsCount;    // Total number of trials, calculated from scenariosData.json
    private int currentTrial;   // Trial the player is attempting, refers to order in scenariosData.json file. Starts at 0.
    private int trialsListIndex;   // What is the index of trials from the ordered or unordered list of trials
    private int startObjNum;    // Current start object, refers to order in scenariosData.json. Starts at 0.
    private int targetObjNum;   // Current target object, refers to order in scenariosData.json.Starts at 0.
    private List<int> trialsOrder; // Order in which player goes through trials. Is randomized or not based on setting in scenariosData.json

    public List<string> objNames; // List of all objects names with their pronoums. Used for dialog boxes.

    //Calculation variables
    private float angleToTarget;    // Angle betweenthe player's start rotation to correct rotation to point at the target object
    private float startRotation;    // Player's start rotation
    private float correctRotation;  // Correct player rotation is they are pointing at the target object
    private float endRotation;      // Player's validated rotation
    private float rotationError;    // Absolute value of difference between the correct rotation and the validated rotation
    private float totalRotError;    // Sum of rotation error for all trials
    private float avgRotationError; // Avg rotation error, updated after each trial

    //Save Data
    private string filePath;    //Save directory for exports
    private string fileName;    //Name of data file
    private StreamWriter sw;
    private char fileNameDelimiter = '-';
    private char delimiter = ',';
    private string dateTime;



    private void Awake()
    {
        //Game objects and classes
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        _sc9Data = gameManager.scenariosData.sc9Data;
        scenarioManager = gameManager.GetComponent<SequenceManager>();

        trialsCount = gameManager.scenariosData.sc9Data.trials.Count();
        CreateNounsPronouns(); //Creates a list of all objects names with their pronoums

        //Save data
        filePath = Path.Combine(Directory.GetCurrentDirectory(), "Exports/SessionsData/"); //For macOS, Linux
        if (!System.IO.Directory.Exists(filePath))    //if save directory does not exist, create it
        {
            System.IO.Directory.CreateDirectory(filePath);
        }

    }

    public void SetupScenario()
    {
        //Initialise variables
        currentTrial = 0;
        avgRotationError = 0;
        trialsListIndex = 0;
        totalRotError = 0;
        trialsOrder = ConstructTrialIndex();
        int _firstTrial = trialsOrder[trialsListIndex];

        scenarioManager.scenario1Props.SetActive(true); //Make props visible
        scenarioManager.routeManager.validationEnabled = true; //Validation is possible
        scenarioManager.attemptsLimited = false;
        scenarioManager.validationsLimited = false;
        gameManager.attemptsAllowed = false;
        gameManager.sessionData.selectedRouteCoord = new List<string>();
        gameManager.sessionData.routeStart = new List<string> { gameManager.scenariosData.sc9Data.trials[_firstTrial].position, "" }; //forcing cardinal direciton 
        gameManager.freezeMovement = true;



        SetupTrial();
        
    }
    // Display the instructions dialog box only if the session has started and not ended
    public void StartScenario()
    {
        float _visorSize = gameManager.scenariosData.sc9Data.visorSize;
        StartSavingData();
        DisplayInstructions();
        visor.GetComponent<RectTransform>().localScale = new Vector3(_visorSize, _visorSize, _visorSize);
        visor.SetActive(true);
    }

    public void EndScenario()
    {
        StopSavingData();
        gameManager.sessionEnded = true;
        gameManager.uiManager.dialogBoxCheckpoint.gameObject.SetActive(false);
        scenarioManager.scenario1Props.SetActive(false);
        visor.SetActive(false);
    }

    // Constructs a list with the order in which player goes through trials.Is randomized or not based on setting in scenariosData.json
    private List<int> ConstructTrialIndex()
    {
        List<int> _listOrdered = new List<int>();
        for (int x = 0; x < trialsCount; x++)   //Creates ordered list
        {
            _listOrdered.Add(x);
        }
        if (!_sc9Data.randomTrialsOrder)    // IF scenario data is set to NOT random, returns ordered list
        {
            Debug.Log("trials ordered list:" + string.Join("- ", _listOrdered));
            return _listOrdered;
        }
        else // IF scenario data is set to random, returns unordered list
        {
            var rnd = new System.Random();
            List<int> _listUnordered = _listOrdered.OrderBy(ContextMenuItemAttribute => rnd.Next()).ToList();
            Debug.Log("trials unordered:" + string.Join("- ", _listUnordered));
            return _listUnordered;
        }
    }

    //Creates a list of all objects names with their pronoums
    private void CreateNounsPronouns()
    {
        objNames = new List<string>();

        for (int x = 0; x < gameManager.scenariosData.sc9Data.propObjs.Count(); x++){
            objNames.Add(gameManager.scenariosData.sc9Data.propObjs[x].pronoun + " " + gameManager.scenariosData.sc9Data.propObjs[x].name);
        }
        //Debug.Log("PropObjs: " + string.Join(", ", objNamesPronouns));
    }

    // Sets up the next trial in the orderList: 
    public void SetupTrial()
    {
        currentTrial = trialsOrder[trialsListIndex];
        //Retrieve start coordinates and move the player there
        Debug.Log("Start coordinate is: " + gameManager.scenariosData.sc9Data.trials[currentTrial].position);
        string startCoordStr = gameManager.scenariosData.sc9Data.trials[currentTrial].position;
        gameManager.GetComponent<IntersectionManager>().GotoCoord(startCoordStr,"X");

        //Set the start and target objects
        startObjNum = gameManager.scenariosData.sc9Data.trials[currentTrial].startObj;
        targetObjNum = gameManager.scenariosData.sc9Data.trials[currentTrial].targetObj;
        startObj = allObjects[startObjNum];
        targetObj = allObjects[targetObjNum];
        Debug.Log("SETUPTRIAL(): " + "Start obj: " + startObjNum + "-" + objNames[startObjNum] + targetObjNum + "-" + "Target obj: " + objNames[targetObjNum]);

        //Set and save the correct rotation to the target
        player.transform.LookAt(targetObj.transform);
        correctRotation = player.transform.rotation.eulerAngles.y;

        //Set and save the start rotation
        player.transform.LookAt(startObj.transform);
        startRotation = player.transform.rotation.eulerAngles.y;
        player.GetComponent<PlayerController>().currentRotation = player.transform.rotation.eulerAngles;

        //Calculate the correct angle to target
        Vector3 targetDir = targetObj.transform.position - player.transform.position;
        angleToTarget = Vector3.Angle(targetDir, player.transform.forward);

    }
    //Display instructions in dialog box
    private void DisplayInstructions()
    {
        gameManager.uiManager.dialogBoxCheckpoint.gameObject.SetActive(true);

        string attemptInstructionText = _sc9Data.instructions.attempts[0] + (trialsListIndex + 1) + " / " + trialsCount + "|" + _sc9Data.instructions.attempts[1] + objNames[targetObjNum] + _sc9Data.instructions.attempts[2];
        gameManager.uiManager.OpenCheckpointDialogBox(attemptInstructionText, false);

        // Display information on debug canvas
        gameManager.uiManager.ptsotText.text = (_sc9Data.instructions.attempts[0] + (trialsListIndex + 1) + "\nTrials order: " + string.Join(" - ", trialsOrder) + "\nStart obj: " + objNames[startObjNum] + "\nTarget obj: " + objNames[targetObjNum] + "\nStart rot: " + startRotation + "\nAngle to target = " + angleToTarget.ToString() + "\nCorrect rot to target = " + correctRotation);
    }

    // Called when the player validates their rotation. Makes calculation for the trial, then sets up the next trial or ends the session
    public void ValidateTrial()
    {
        CalculateRotation();
        SaveData();

        if (trialsListIndex < trialsCount - 1)   // IF there are trials left, setup the next trial
        {
            trialsListIndex++;
            currentTrial = trialsOrder[trialsListIndex];
            SetupTrial();
            DisplayInstructions();
        }
        else // IF no trials left, end the session
        {
            EndScenario();
            //gameManager.sessionEnded = true;
            gameManager.uiManager.dialogBox.GetComponent<DialogBox>().returnToMenu = true;
            gameManager.uiManager.OpenDialogBox(_sc9Data.instructions.end);

            //StopSavingData();

        }
    }



    // For each trial, calculates the degree of error between the correct rotation and the rotation validated by the player. Also tracks the total and average errors for all trials.
    private void CalculateRotation()
    {
        endRotation = player.transform.rotation.eulerAngles.y;
        rotationError = Mathf.Abs(endRotation - correctRotation);
        totalRotError += rotationError;
        avgRotationError = totalRotError / (trialsListIndex+1);

        // Display information on debug canvas
        gameManager.uiManager.ptsotCalcText.text = ("SAVED:" + (trialsListIndex + 1) + " / " + trialsCount + "\nTot error: " + totalRotError + "\nAvg error: " + avgRotationError + "\n\nPrevious correct rot: " + correctRotation + "\nPrevious end rot: " + endRotation + "\nPrevious error: " + rotationError);
    }
    public void StartSavingData()
    {
        SetFileName();
        sw = File.AppendText(filePath + fileName);
        sw.WriteLine(HeadersConstructor()); //Add Headers to the file
    }
    public void StopSavingData()
    {
        sw.Close();
    }
    private void SetFileName()
    {
        string studentID = gameManager.sessionData.studentIDs[0];
        string sessionSummaryText = "ELV" + studentID + fileNameDelimiter + "SCN" + gameManager.sessionData.selectedScenario;
        dateTime = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        fileName = dateTime + fileNameDelimiter + sessionSummaryText + ".csv";

    }

    private string HeadersConstructor()
    {
        string sessionHeader = "dateTime" + delimiter + "studentID" + delimiter + "scene" + delimiter + "scenario";
        string trialsListHeader = "trialsCount" + delimiter + "trialsOrder" + delimiter + "trialsOrderIndex";
        string trialHeader = "trialNb" + delimiter + "startObjNum" + delimiter + "startObj" + delimiter + "targetObjNb" + delimiter + "targetObj";
        string playerHeader = "startRotation" + delimiter + "correctRotation" + delimiter + "angleToTarget" + delimiter + "endRotation";
        string calculationsHeader = "rotationError" + delimiter + "totalRotationError" + delimiter + "avgRotationError";

        return sessionHeader + delimiter + trialsListHeader + delimiter + trialHeader + delimiter + playerHeader + delimiter + calculationsHeader;
    }

    private void SaveData()
    {
        string sessionData = dateTime + delimiter + gameManager.sessionData.studentIDs[0] + delimiter + SceneManager.GetActiveScene().name + delimiter + gameManager.sessionData.selectedScenario;
        string trialsListData = trialsCount.ToString() + delimiter + string.Join("-", trialsOrder) + delimiter + trialsListIndex; ;
        string trialData = currentTrial.ToString() + delimiter + startObjNum + delimiter + objNames[startObjNum] + delimiter + targetObjNum + delimiter + objNames[targetObjNum];
        string playerData = startRotation.ToString() + delimiter + correctRotation + delimiter + angleToTarget + delimiter + endRotation;
        string calculationsData = rotationError.ToString() + delimiter + totalRotError.ToString() + delimiter + avgRotationError.ToString();

        sw.WriteLine(sessionData + delimiter + trialsListData + delimiter + trialData + delimiter + playerData + delimiter + calculationsData);
    }
}
