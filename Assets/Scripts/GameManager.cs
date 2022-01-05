using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //for button
using System.IO; //for Paths
using System.Linq; //for Array Contains
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/**
 * GameManager.cs
 * 
 * Handles all game data (e.g. saving paths, sessionData, 
 * Comment
 * 
 * 
 **/

public class GameManager : MonoBehaviour
{
    private GameObject player;
    private IntersectionManager intersectionManager;
    private RouteManager routeManager;
    private PlayerController playerController;
    public UIManager uiManager;
    private SequenceManager sequenceManager;
    private CheckpointManager checkpointManager;
    public GlobalControl.SessionData sessionData;
    private SaveSessionData saveSessionData;
    private EventSystem eventSystem;
    private LimiterManager limiterMansger;
    private Sc9Manager sc9Manager;
    //public Canvas canvas;

    public int blockSize = 35; //define the city's block size in meters

    public string coordSeparator = ",";    //coordinate separator TODO: change to CHAR and move to RouteManager
    //public bool validationCheck = false;   //whether route is being validated - used for UI display only

    // File save and export paths
    public string screenshotPath;       // screenshots saving path
    public string trackMovementPath;    // path to export player's movement and sequence information

    // Game session element
    public bool sessionStarted;         // Has the session started?
    public bool sessionEnded;           // Has the session ended?
    public int attemptCount;            // attempts # in this session
    public int validationCount;         // # of validation attempts in this session
    //public bool sessionPaused;          // Is the sessions paused?
    public bool gameIsPaused = false;
    public bool freezePlayer;   // Use to freeze the player's movement when a dialog box is opened
    public bool freezeMovement; //Used to freeze the player's movement in Sc9 (PTSOT)
    public bool attemptsAllowed;        //Is the player allowed more attempts?
    public bool recordRoute;        //Player is ready to start recording

    //public float startTime;             // Timestamp of when the session starts
    public float attemptTime;           // Timestamp of when the attempt starts
    public float pauseTime;             // Timestamp of when the session is paused
    public float resumeTime;
    public float pauseDuration;
    public float endTime;               // Timestamp of when the session ends
    public TimeSpan sessionTimeElapsed;   // How much time has elapsed since beginning of session
    public TimeSpan attemptTimeElapsed; // How much time has elapsed since beginning of last attempt

    public float sessionDistance;     // Total distance travelled during session
    public float attemptDistance;   // Total distance travelled during attempt
    //public float pauseDistance;
    //public Vector3 lastPosition;

    //public int selectedSequence;        // Sequence selected from Menu
    public string sessionSummaryText;      //Summary of game session
    public bool overideOnIntersectionExit; //Using when resetting route manually

    public ScenariosData scenariosData;
    public TextData textDataFR;

    // Start is called before the first frame update
    private void Awake()
    {
        // Initialise GameObjects and components
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        intersectionManager = GetComponent<IntersectionManager>();
        routeManager = GetComponent<RouteManager>();
        sequenceManager = GetComponent<SequenceManager>();
        uiManager = GetComponent<UIManager>();
        checkpointManager = GetComponent<CheckpointManager>();
        saveSessionData = GetComponent<SaveSessionData>();
        limiterMansger = GetComponent<LimiterManager>();
        sc9Manager = GameObject.Find("ScenariosManager"). GetComponent<Sc9Manager>();

        overideOnIntersectionExit = false;

        sessionData = GlobalControl.instance.sessionData;          // Import data session from GlobalControl
        scenariosData = new ScenariosData();
        Debug.Log("GAME MANAGER (pause): " + sessionData.sessionPaused);

        SetPaths();                     // Set paths for importing and exporting data
        InitialiseSession();            // Initialise variables for session start

        //Debug.Log("(gamemanager) Distance: " + sessionDistance);
    }

    
    private void Start()
    {
        uiManager.startDialogBox.gameObject.SetActive(true);
        //Debug.Log("Time scale:" + Time.timeScale);
        //Debug.Log("sessionPaused: " + sessionData.sessionPaused);
        if (sessionData.sessionPaused)  //If the session is paused, ... 
        {
            uiManager.sessionStatusText.text = "(S) Session status: Paused";
            sessionTimeElapsed = TimeSpan.FromSeconds(Time.time - sessionData.sessionStartTime);
            sessionDistance = sessionData.pauseDistance;
        }
    }

    private void OnApplicationQuit()
    {
     //   if (!sessionEnded)
       //     saveSessionData.stopSavingData();
    }

    //Update is called once per frame

    // TODO: Create an UpdateSessionMetrics() and call from Update - 
    void Update()
    {
        if (sessionStarted && !gameIsPaused && !sessionEnded) //session is ongoing
            {
            // Timestamp 
            sessionTimeElapsed = TimeSpan.FromSeconds(Time.time - sessionData.sessionStartTime);
            attemptTimeElapsed = TimeSpan.FromSeconds(Time.time - attemptTime);

            // Total distance 
            sessionDistance += Vector3.Distance(playerController.lastPosition, player.transform.position);

            // Session distance
            attemptDistance += Vector3.Distance(playerController.lastPosition, player.transform.position);
            //lastPosition = player.transform.position;
            playerController.lastPosition = player.transform.position;
        }

            sessionSummaryText = ("SCEN:" + sessionData.selectedScenario + uiManager.senderReceiverUI + " - RTE:" + (sessionData.selectedRoute + 1)+ " - VAL#:" + sessionData.validationCountNew + " - TENT#:" + attemptCount + " - GRP:" + sessionData.groupID + " - ELV:" + string.Join(",", sessionData.studentIDs));
    }

    // USED FOR DEBUGGING, DISPLAY GAME TIME INFORMATION
    private void DebugTime()
    {
        Debug.Log("Time.time: " + Time.time);
        Debug.Log("Start time: " + sessionData.sessionStartTime);
        Debug.Log("Pause time: " + sessionData.pauseTime);
        Debug.Log("SessionTimeElapsed: " + sessionTimeElapsed.ToString(@"mm\:ss"));
    }

    // SET VARIABLES FOR SESSION START
    public void InitialiseSession()
    {
        Debug.Log("InitialiseSession()");
        attemptCount = 1;
        attemptDistance = 0;
        //pauseDistance = 0;
        recordRoute = false;
        pauseDuration = 0;
        playerController.keyboardShortcutsEnabled = true;
        Time.timeScale = 1;
        freezeMovement = false; // Movement frozen only within sc9 (PTSOT)
        //Debug.Log("freezeMovement set to false");
        //gameIsPaused = false; - moved inside firs if

        //DebugTime();
        if (!sessionData.sessionPaused) //First time starting session
        {
            Debug.Log("starting session");
            Debug.Log("sessionPaused: " + sessionData.sessionPaused);
            sessionStarted = false;
            sessionEnded = false;
            freezePlayer = true;
            validationCount = 0;
            sessionData.validationCountNew = 0;
            attemptTime = 0;
            //startTime = Time.time;
            sessionDistance = 0f;
            attemptsAllowed = true;
            gameIsPaused = false;
        }
        else //Session is resumed from pause, count as new attempt
        {
            //Debug.Log("resuming session");
            //startTime = sequenceManager.sessionData.sessionStartTime;
            attemptTime = Time.time;
            //sessionData.sessionPaused = false;
            freezePlayer = true;
            gameIsPaused = true;
            
        }


    }

    // ROUTE VALDAITON INFORMATION
    private void InitialiseValidationInfo()
    {
        routeManager.validationInfo.isValid = false;
        routeManager.validationInfo.endReached = false;
        routeManager.validationInfo.errorAt = 0;
        routeManager.validationInfo.routeLength = 0;
    }

    public void StartSession()
    {
        Debug.Log("Start session");
        //Debug.Log("sessionPaused: " + sessionData.sessionPaused + "- freezePlayer: " + freezePlayer+ "- Time scale:" + Time.timeScale);
        //DebugTime();
        //Debug.Log("Time scale:" + Time.timeScale);
        uiManager.startDialogBox.gameObject.SetActive(false);   // Show start dialog box
        sessionStarted = true;  //Start session
        sessionEnded = false;
        freezePlayer = false;
        if (sessionData.selectedScenario != 9)
            freezeMovement = false;
        uiManager.validationCheck = false;
        //Time.timeScale = 1;
        if (!sessionData.sessionPaused) //Session was not paused
        {
            //startTime = Time.time;      //Start time - now
            sessionData.sessionStartTime = Time.time;   //Start time is now
            attemptTime = sessionData.sessionStartTime; //Attempt time is now
            uiManager.sessionStatusText.text = "(S) Session status: Started";   //Update UI 
            
            if (sessionData.selectedScenario != 9)
            { 
saveSessionData.StartSavingData();  //Start saving game session data
                                    }
            else
                {
                sc9Manager.StartSavingData();
            }
        }
        else  //IF the session was paused after a validaton by image
        {
            gameIsPaused = false;   //TODO: check if needed
            //startTime = sessionData.sessionStartTime;   //pull starttime from sessionData
            sessionData.sessionPaused = false;      //unpause session
            //attemptCount++;
            validationCount++;  //
            //Debug.Log("add attempt in StartSession()");
            attemptTime = Time.time;
          
            //PauseGame();
            uiManager.sessionStatusText.text = "(S) Session status: Resumed";
            if (sessionData.selectedScenario != 9)
            {
                saveSessionData.ContinueSavingData();  //Start saving game session data
            }
            //saveSessionData.ContinueSavingData();
        }
        //Debug.Log("Time scale:" + Time.timeScale);
        //Debug.Log("2sessionPaused: " + sessionData.sessionPaused + "- freezePlayer: " + freezePlayer + "- Time scale:" + Time.timeScale);

    }

    //Temporarily stop saving game validation data
    public void PauseSession()
    {
        sessionData.pauseTime = Time.time;
        sessionData.sessionPaused = true;
        freezePlayer = true;
        Time.timeScale = 0;
        gameIsPaused = true;
        if (sessionData.selectedScenario != 9)
        {
            saveSessionData.SaveData();  //Start saving game session data
        }
        //saveSessionData.SaveData();
        //PauseGame();
        sessionData.pauseDistance = sessionDistance;
        //PauseGame();
        //DebugTime();
        uiManager.sessionStatusText.text = "(S) Session status: Paused";
        //Debug.Log("freezeplayer2 = " + freezePlayer);
    }

    //Stop the game session and stop saving data
    public void EndSession(bool displayText)
    {
        Debug.Log("EndSession()");
        endTime = Time.time;
        //TODO: Attempt time
        sessionEnded = true;
        //sessionStarted = false;
        sessionData.sessionPaused = false;
        freezePlayer = true;
        GlobalControl.instance.sessionData = sessionData;
        uiManager.sessionStatusText.text = "(S) Session status: Ended";
        if (displayText)
        {
            uiManager.dialogBox.GetComponent<DialogBox>().returnToMenu = true;
            uiManager.OpenDialogBox("Session terminé.");
        }
        if (sessionData.selectedScenario != 9)
        {
            saveSessionData.SaveData();    //Write data right before closing file
            saveSessionData.stopSavingData();   //Stop saving data
        }
                else
        {
            sc9Manager.StopSavingData();
        }
    }

    // Used in scenario 3, record the route followed by the player
    public void RecordRoute()
    {
        if (sessionStarted)
        {

            if (sessionData.selectedScenario == 3 && !recordRoute) //haven't already started recording
            {
                routeManager.lineDrawn.SetActive(false);    //hide the line
                recordRoute = true;
                attemptsAllowed = true;
                routeManager.validationEnabled = true;
                uiManager.OpenDialogBox("Enregistrement du trajet.");
                ResetRoute();
            }
        }

    }

    public void NewAttempt(bool displayText)
    {
        //saveSessionData.SaveData(); //save data before changing the state
        if (sessionStarted) // IF session has started
        {

            if (!attemptsAllowed)   // Session has started and there are no more attempts
            {
                if (displayText)
                    uiManager.OpenDialogBox("Retour au départ pas autorisé."); //Plus/pas de retour possibles
            }
            if (sessionStarted && !sessionEnded && attemptsAllowed) //Session is ongoing (started and not ended) and there are attempts left
            {
                if (!sequenceManager.attemptsLimited || (sequenceManager.attemptsLimited && attemptCount <= sequenceManager.activeScenario.maxAttempts)) //Attempts are not limited OR are attempts left 
                {

                    if (attemptCount == sequenceManager.activeScenario.maxAttempts) //Last possible attempt
                    {
                        Debug.Log("This is your last attempt");
                        if (displayText)
                            uiManager.OpenDialogBox("Dernière tentative.");
                    }
                    else //More than 1 attempt left
                    {
                        Debug.Log("new atempts" + attemptCount + " /" + sequenceManager.activeScenario.maxAttempts);
                        if (displayText)
                            uiManager.OpenDialogBox("Nouvelle tentative.");
                    }
                    //TODO: Dialog box display
                    //saveSessionData.SaveData();
                    if (intersectionManager.onIntersection && intersectionManager.sessionRoute.Count == 1)    // in an intersection AND one coordinate only AND that is the first one
                    {
                        Debug.Log("test");
                        ResetRoute();
                        intersectionManager.OnIntersectionEnter(intersectionManager.lastIntersectionCollider);
                        //intersectionManager.sessionRoute.Add(intersectionManager.lastIntersectionCollider.GetComponent<Intersection>().coordString);
                    }

                    else 
                    {
                        ResetRoute();
                        
                            
                    }

                }
                else if (sequenceManager.attemptsLimited && attemptCount > sequenceManager.activeScenario.maxAttempts) //Attempts are limited and player is out of attempts
                {
                    Debug.Log("Out of attempts: " + attemptCount + " /" + sequenceManager.activeScenario.maxAttempts);
                    //TODO: Dialog box text + close
                    if (displayText)
                        uiManager.OpenDialogBox("Il ne reste plus de tentative.");
                }
            }
        }
    }

    public void ResetRoute()
    {
        Debug.Log("Inside GameManager.ResetRoute");
        //Reset variables
        //uiManager.validationCheck = false;
        playerController.playerHasMoved = false;
        attemptCount++;
        attemptTime = Time.time;
        attemptDistance = 0;
        playerController.lastPosition = playerController.startPosition;
        intersectionManager.sessionRoute.Clear();
        intersectionManager.sessionRouteDir.Clear();
        //Move player back to start route
        intersectionManager.GotoCoord(sessionData.routeStart.ElementAt(0), sessionData.routeStart.ElementAt(1));
        //player.transform.eulerAngles = new Vector3(0, playerController.currentRotation.y, 0);
        intersectionManager.lastCardinalDirection = "";
        //playerController.playerRb.WakeUp(); //TEST to trigger onTriggerExit before resettings lastIntersection();
        overideOnIntersectionExit = true;   //used to manuall set the lastIntersection information when reseeting the route
        playerController.lastIntersection = playerController.startCoord;
        Debug.Log("Last intersection reset to: " + String.Join(",", playerController.lastIntersection));

        InitialiseValidationInfo();

    }

    //TODO: Move to SequenceManager;
    // Handles the validaton for the different scenarios
    public void CheckValidation()
    {
        Debug.Log("validation: " + sessionData.validationCountNew + " /" + sequenceManager.activeScenario.maxValidations);
        if (!routeManager.validationEnabled)//CANNOT validate
        {
            uiManager.OpenDialogBox("Pas de validation possible.");
        }
        //IF CAN VALIDATE (Validations are not limited or there are validations left)
        else if (!sequenceManager.validationsLimited || (sequenceManager.validationsLimited && sessionData.validationCountNew < sequenceManager.activeScenario.maxValidations))
        {
            validationCount++;  // Counts as one validation
            sessionData.validationCountNew++;
            Debug.Log("validation: " + sessionData.validationCountNew + " /" + sequenceManager.activeScenario.maxValidations);
            uiManager.validationCheck = true; // allow UI display

            //Scenario 1
            if (sessionData.selectedScenario == 1)
            {
                checkpointManager.CheckpointValidationNew();   //Checkpoints validation
            }
            //Scenario 2
            else if (sessionData.selectedScenario == 2)         
            {
                if (sessionData.isSender)   //Player is sender
                {
                    if (sessionData.validationCountNew < sequenceManager.activeScenario.maxValidations)    // there are validations left
                    {
                        uiManager.OpenDialogBox("Attend les instructions.");
                        intersectionManager.GotoCoord(sessionData.routeStart.ElementAt(0), sessionData.routeStart.ElementAt(1));  //Place player at start of route
                        NewAttempt(false);
                        attemptCount = 1;
                    }
                    //else
                    //{
                    //    uiManager.OpenDialogBox("(10) Il ne reste plus de validation possible.");
                    //    EndSession();
                    //    uiManager.dialogBox.GetComponent<DialogBox>().returnToMenu = true;  //return to menu after closing the dialog box
                    //}
                    else
                    {
                        uiManager.OpenDialogBox("C'était la dernière validation. Attend les instructions.");
                        EndSession(false);
                        uiManager.dialogBox.GetComponent<DialogBox>().returnToMenu = true;  //return to menu after closing the dialog box
                    }

                }
                else // Player is receiver
                {
                    routeManager.ValidateRoute(intersectionManager.sessionRoute);
                    uiManager.routeValidationText.text = ("Valid = " + routeManager.validationInfo.isValid + " - errorat #: " + routeManager.validationInfo.errorAt + " - endReached= " + routeManager.validationInfo.endReached + " - length: " + routeManager.validationInfo.routeLength); //UI Display only
                    
                    RouteValidationScenario2();
                }             
            }
            //Scenario 3
            else if (sessionData.selectedScenario == 3)
            {
                routeManager.ValidateRoute(intersectionManager.sessionRoute);
                uiManager.routeValidationText.text = ("Valid = " + routeManager.validationInfo.isValid + " - errorat #: " + routeManager.validationInfo.errorAt + " - endReached= " + routeManager.validationInfo.endReached + " - length: " + routeManager.validationInfo.routeLength); //UI Display only
                //saveSessionData.SaveData();
                PauseSession();
                
                
                if (sessionData.validationCountNew >= sequenceManager.activeScenario.maxValidations)    //if last attempt
                {
                    Debug.Log("last attempt");
                    GlobalControl.instance.returnToMenu = true;
                    endTime = Time.time;
                    sessionEnded = true;
                    sessionData.sessionPaused = false;
                }
                GlobalControl.instance.sessionData = sessionData;
  
                saveSessionData.stopSavingData();
                uiManager.dialogBox.GetComponent<DialogBox>().validateScene = true;  //return to menu after closing the dialog box
                uiManager.OpenDialogBox("VALIDATION PAR IMAGES");
            }
            //Scenario 4
            else if (sessionData.selectedScenario == 4 || sessionData.selectedScenario == 7)
            {
                routeManager.ValidateRoute(intersectionManager.sessionRoute);
                uiManager.routeValidationText.text = ("Valid = " + routeManager.validationInfo.isValid + " - errorat #: " + routeManager.validationInfo.errorAt + " - endReached= " + routeManager.validationInfo.endReached + " - length: " + routeManager.validationInfo.routeLength); //UI Display only
                //saveSessionData.SaveData();
                PauseSession();
                GlobalControl.instance.sessionData = sessionData;
                uiManager.OpenDialogBox("VALIDATION PAR IMAGES");
                saveSessionData.stopSavingData();
                uiManager.dialogBox.GetComponent<DialogBox>().validateScene = true;  //return to menu after closing the dialog box
            }
            else if (sessionData.selectedScenario == 5)  //Scenario 5 (no validation)
            {
                uiManager.routeValidationText.text = "No validation possible";
            }
            //Scenario 9
            else if (sessionData.selectedScenario == 9)  //Scenario 5 (no validation)
            {
                //uiManager.routeValidationText.text = "Validation text";
                sc9Manager.ValidateTrial();
            }
        }
        else
        {
            uiManager.OpenDialogBox("Il ne reste plus de validation possible.");
        }
   

    }

    private void RouteValidationScenario2()
    {
        //IF validation succeeds
        if (routeManager.validationInfo.isValid)
        {
            uiManager.OpenDialogBox("Parcour validé!");
            EndSession(false);
            uiManager.dialogBox.GetComponent<DialogBox>().returnToMenu = true;  //return to menu after closing the dialog box
        }
        //IF validation fails
        else if (!routeManager.validationInfo.isValid)
        {
            Debug.Log("validation: " + sessionData.validationCountNew + " /" + sequenceManager.activeScenario.maxValidations);
            if (sessionData.validationCountNew == sequenceManager.activeScenario.maxValidations - 1)
            {
                Debug.Log("This is your last validation");
                //TODO: Dialog box set text
                uiManager.OpenDialogBox("Parcour non validé.\nIl reste 1 validation");
                //intersectionManager.GotoCoord(routeManager.routeStart.ElementAt(0), routeManager.routeStart.ElementAt(1));
                NewAttempt(false);
                attemptCount = 1;

            }
            else if (sequenceManager.validationsLimited && validationCount >= sequenceManager.activeScenario.maxValidations) //Player is out of attempts
            {
                Debug.Log("Out of validation: " + sessionData.validationCountNew + " /" + sequenceManager.activeScenario.maxValidations);
                //TODO: Dialog box text + close
                uiManager.OpenDialogBox("Parcour non validé.\nIl ne reste plus de validation");
                EndSession(false);
                uiManager.dialogBox.GetComponent<DialogBox>().returnToMenu = true;  //return to menu after closing the dialog box
            }
            else //there are validations left
            {
                Debug.Log("validation: " + validationCount + " /" + sequenceManager.activeScenario.maxValidations + "Try again");
                uiManager.OpenDialogBox("Parcour non validé.");
                //intersectionManager.GotoCoord(routeManager.routeStart.ElementAt(0), routeManager.routeStart.ElementAt(1));
                NewAttempt(false);
                attemptCount = 1;
            }
        }
    }




    // Initialise data import and export paths
    public void SetPaths()
    {
        screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), "Exports/Screenshots/");
        trackMovementPath = Path.Combine(Directory.GetCurrentDirectory(), "Exports/TrackMovements/");
    }

    public void PauseGame()
    {
        
        gameIsPaused = !gameIsPaused;
        Debug.Log("gameIsPaused: " + gameIsPaused);
        //saveSessionData.SaveData();

        if (gameIsPaused)
        {
           // pauseTime = Time.time;
            //pauseDistance = sessionDistance;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            //resumeTime = Time.time;
            //pauseDuration = resumeTime - pauseTime;
            //sessionTimeElapsed -= TimeSpan.FromSeconds(resumeTime - pauseTime);
            //sessionDistance = pauseDistance;
        }
         
    }

    // Saves session data then loads Menu scene
    public void GotoMenu()
    {
        GlobalControl.instance.sessionData = sessionData;
        SceneManager.LoadScene("Menu"); //Load scene called Game
    }

}
