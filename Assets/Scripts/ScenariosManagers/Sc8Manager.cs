using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sc8Manager : MonoBehaviour
{
    // Game objects and classes
    private GameManager gameManager;
    private UIManager uiManager;
    private GameObject player;
    private Sc8Data _sc8Data;
    private SequenceManager scenarioManager;
    private CheckpointManager checkpointManager;
    private RouteManager routeManager;
    private IntersectionManager intersectionManager;

    //Checkpoints
    public GameObject checkpointPrefab;
    private int checkpointIndex;
    private bool noCheckpointsCollected = true;
    private int numCheckpoints;
    private int checkpointAttempt; // How many times the student has tried to validate this attempt 

    private int lastCheckpointCollected; // ID of the checkpoint that can be validated next. If  = 0 then no checkpoint has been collected
    private bool onCheckpoint;  //Is player on a checkpoint now?
    private Checkpoint lastCheckpointEntered; // Last checkpoint entered

    public List<GameObject> linesDrawn;
    private int selectedRoute;


    // Start is called before the first frame update
    void Awake()
    {


        //Game objects and classes
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        _sc8Data = gameManager.scenariosData.sc8Data;
        scenarioManager = gameManager.GetComponent<SequenceManager>();
        checkpointManager = gameManager.GetComponent<CheckpointManager>();
        routeManager = gameManager.GetComponent<RouteManager>();
        intersectionManager = gameManager.GetComponent<IntersectionManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetupScenario()
    {
        checkpointIndex = 0;
        lastCheckpointCollected = 0; //No checkpoint has been collected
        selectedRoute = gameManager.sessionData.selectedRoute; //TODO: GET FROM SCENARIO DATA
        noCheckpointsCollected = true;
        checkpointAttempt = 0;

        numCheckpoints = _sc8Data.routes[selectedRoute].checkpoints.Count();
        scenarioManager.scenario1Props.SetActive(false); //Make props visible
        scenarioManager.routeManager.validationEnabled = true;
        //scenarioManager.validationsLimited = false;
        gameManager.attemptsAllowed = false;
        gameManager.sessionData.selectedRouteCoord = new List<string>();
        gameManager.sessionData.routeStart = new List<string> { _sc8Data.routes[selectedRoute].startCoord.coord, _sc8Data.routes[selectedRoute].startCoord.cardDir }; //forcing cardinal direciton 
        gameManager.freezeMovement = false;

        // (1) Read data from scenariosData.json
        // ( ) Generate checkpoints
        // allCheckpoints = new List<string>(_sc8Data.checkpoints);

        GenerateCheckpoints();
        GenerateLines();
        HideAllLines();

    }


    public void StartScenario()
    {
        //StartSavingData();
    }

    public void EndScenario()
    {
        //StopSavingData();
        gameManager.sessionEnded = true;
    }
    // WHEN PLAYER ENTERS WIH CHECKPOINT
    public void OnCheckpointEnter(Collider other)
    {
        onCheckpoint = true;
        lastCheckpointEntered = other.gameObject.GetComponent<Checkpoint>();    // Sets this checkpoint as the last checkpoint entered
    }

    // WHEN PLAYER EXITS CHECKPOINT
    public void OnCheckpointExit(Collider other)
    {
        onCheckpoint = false;
    }
    private void GenerateCheckpoints()
    {


        string[] coordArray;
        //string coordCardDir;
        //Debug.Log("GenerateCheckpoint - checkpoints: " + string.Join(",", checkpoints));
        for (int i = 0; i < numCheckpoints; i++)
        {
            //Pul cardinal direction from _sc8Data
            string _checkpointCoord = _sc8Data.routes[selectedRoute].checkpoints[i].coord;
            string _checkpointCardDir = _sc8Data.routes[selectedRoute].checkpoints[i].cardDir;

            //coordCardDir = checkpoints[i].Substring(checkpoints[i].Length - 1); ;
            //string coordNoDirection = checkpoints[i].Remove(checkpoints[i].Length - 1); //Delete the last character which refers to  direction. If players are moved to previous checkpoint, direction is the player's orientation
            coordArray = _checkpointCoord.Split(char.Parse(routeManager.xyCoordSeparator));      //stores the coordinates x and y in an array
            //coordArray[1].Remove(coordArray[1].Length-1);   //Delete the last character of the array which is direction
            var newCheckpoint = Instantiate(checkpointPrefab, new Vector3(float.Parse(coordArray[0]) * gameManager.blockSize, 0.02f, float.Parse(coordArray[1]) * gameManager.blockSize), checkpointPrefab.transform.rotation);    //instantiate the checkpoint right above the ground (0.02f)

            //Populate checkpoint data
            //newCheckpoint.GetComponent<Checkpoint>().coordString = checkpoints[i];  //store the coordinates as a string in the instance
            newCheckpoint.GetComponent<Checkpoint>().ID = i;    //stores the checkpoint ID(int) in the instance
            newCheckpoint.GetComponent<Checkpoint>().scenario = 8;  // sets the scenario ID
            //TODO: Group coordinates under one parent
            newCheckpoint.GetComponent<Checkpoint>().coord = _checkpointCoord;
            newCheckpoint.GetComponent<Checkpoint>().cardDir = _checkpointCardDir;
        }
    }

    private void GenerateLines()
    {
        // Draw lines
        for (int i = 0; i < numCheckpoints; i++)
        {
            List<string> tempRoute = _sc8Data.routes[selectedRoute].routeSegments[i].Split(',').ToList();
            routeManager.SpawnLine(tempRoute, i);
            linesDrawn.Add(routeManager.lineDrawn); //Add the drawn line to the list of lines create for this scenario
        }
    }

    private void HideAllLines()
    {
        for (int i = 0; i < numCheckpoints; i++)
        {
            linesDrawn[i].SetActive(false);
        }
    }

    public void CheckpointValidation()
    {
        if (gameManager.sessionStarted)

        {
            if (onCheckpoint)   //If player presses the validaiton button while on a checkpoint
            {
                if ((noCheckpointsCollected && (lastCheckpointEntered.ID == 0)) || (lastCheckpointEntered.ID == lastCheckpointCollected + 1))// if not checkcpoints have been collected and player is on the first checkpoint OR if this the next valid checkpoint
                {
                    noCheckpointsCollected = false; // Checkpoint is collected
                    lastCheckpointCollected = lastCheckpointEntered.ID;

                    if (lastCheckpointEntered.ID < numCheckpoints - 1)// if this is NOT last checkpoint{
                    {
                        Debug.Log("This is the right checkpoint, keep going!");
                        gameManager.uiManager.OpenDialogBox(_sc8Data.instructions.attempts[0]);
                        checkpointAttempt = 0; //Reset the number of attempts allowed for this section
                    }
                    else // if this is the last checkpoint
                    {
                        Debug.Log("This is the last checkpoint");
                        EndScenario();

                        gameManager.uiManager.dialogBox.GetComponent<DialogBox>().returnToMenu = true;
                        gameManager.uiManager.OpenDialogBox(_sc8Data.instructions.end);

                        //uiManager.routeValidationText.text = "route validation text needed";

                    }

                }
                else // IF this checkpoint is not the next valid checkpoint
                {
                    Debug.Log("This is not the right checkpoint");
                    IncorrectAttemptCheck();
                    MovePlayerBack();
                }
            }
            else // If the player presses the validation button while not on a checkpoint
            {
                IncorrectAttemptCheck();
                MovePlayerBack();
            }
        }
    }
    //Instructions and lines to display when user does not correctly validate a checkpoint
    private void IncorrectAttemptCheck()
    {
        // (1) Display Instructions
        string _instructionsText;
        if (checkpointAttempt == 0)
            _instructionsText = _sc8Data.instructions.attempts[1];
        else if (checkpointAttempt == 1)
            _instructionsText = _sc8Data.instructions.attempts[2];
        else
            _instructionsText = _sc8Data.instructions.attempts[3];

        gameManager.uiManager.OpenDialogBox(_instructionsText);

        // (2) Display lines
        if (checkpointAttempt >= 2)
        {
            HideAllLines();
            if (noCheckpointsCollected) // if
                linesDrawn[0].SetActive(true);
            else
                linesDrawn[lastCheckpointCollected + 1].SetActive(true);
        }


    }

    private void MovePlayerBack()
    {
        string _lastCheckpointCoord = _sc8Data.routes[selectedRoute].checkpoints[lastCheckpointCollected].coord;
        string _lastCheckpointCardDir = _sc8Data.routes[selectedRoute].checkpoints[lastCheckpointCollected].cardDir;


        // (1)  Open dialog box
        //string _attemptInstructionText = "attempt#: " + gameManager.attemptCount + "Incorrect -retour au dernier checkpoint";
        //uiManager.OpenDialogBox(_attemptInstructionText);
        //gameManager.uiManager.OpenCheckpointDialogBox(_attemptInstructionText, false);

        // (2) Move Player back
        if (noCheckpointsCollected) // IF no checkpoint has been collected, return to start
        {
            Debug.Log("Return to start");
            intersectionManager.GotoCoord(_sc8Data.routes[selectedRoute].startCoord.coord, _sc8Data.routes[selectedRoute].startCoord.cardDir);  //Place player at start of route
        }
        else // If at least one checkpoint was previously collected, return the last collected checkpoint
        {
            Debug.Log("Return to previous checkpoint: " + lastCheckpointCollected);
            intersectionManager.GotoCoord(_lastCheckpointCoord, _lastCheckpointCardDir);
        }
        checkpointAttempt++;
    }

}
