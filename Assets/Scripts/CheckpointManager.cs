using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/**
 * CheckpointManager.cs
 * 
 * Generates checkpoints
 * Handles interactions between the player and checkpoints
 * 
 */

public class CheckpointManager : MonoBehaviour
{
    private GameManager gameManager;
    private RouteManager routeManager;
    private IntersectionManager intersectionManager;
    private UIManager uiManager;
    private SaveSessionData saveSessionData;

    public GameObject checkpointPrefab;
    public List<string> allCheckpoints;        // List of all checkpoints coordinates
    public int lastCheckpointCollected;                 // ID of the checkpoint that can be validated next. If  = 0 then no checkpoint has been collected

    //Sequence 0
    public List<string> checkpointsTextS0;        // Text import of text to display at each checkpoint
    public List<string> checkpointsInstructionsS0;  // Instructions to the next checkpoint

    //Sequence 6
    public List<string> checkpointsTextS6;        // Text import of text to display at each checkpoint
    public List<string> checkpointsInstructionsS6;  //Instructions to the next checkpoint

    public bool onCheckpoint;           // set to to 
    public GameObject lastCheckpoint;   // Last checkpoint entered

    // Start is called before the first frame update
    void Awake() // BUG if not setting to Awake. Why? Global Control?
    {
        // Initialise components
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        routeManager = FindObjectOfType<GameManager>().GetComponent<RouteManager>();
        intersectionManager = FindObjectOfType<GameManager>().GetComponent<IntersectionManager>();
        saveSessionData = GetComponent<SaveSessionData>();
        lastCheckpointCollected = 0; // No checkpoint has been collected
    }

    private void Start()
    {
        //var newCheckpoint = Instantiate(checkpointPrefab, new Vector3(80, 1f, 140), checkpointPrefab.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //GENERATE CHECKPOINTS PREFABS
    public void GenerateCheckpoints(List<string> checkpoints)
    {
        string[] coordArray;
        Debug.Log("GenerateCheckpoint - checkpoints: " + string.Join(",", checkpoints));
        for (int i = 0; i < checkpoints.Count(); i++)
        {
            string coordNoDirection = checkpoints[i].Remove(checkpoints[i].Length - 1); //Delete the last character which refers to  direction. If players are moved to previous checkpoint, direction is the player's orientation
            coordArray = coordNoDirection.Split(char.Parse(routeManager.xyCoordSeparator));      //stores the coordinates x and y in an array
            //coordArray[1].Remove(coordArray[1].Length-1);   //Delete the last character of the array which is direction
            var newCheckpoint = Instantiate(checkpointPrefab, new Vector3(float.Parse(coordArray[0]) * gameManager.blockSize, 0.02f, float.Parse(coordArray[1]) * gameManager.blockSize), checkpointPrefab.transform.rotation);    //instantiate the checkpoint right above the ground (0.02f)
            newCheckpoint.GetComponent<Checkpoint>().coordString = checkpoints[i];  //store the coordinates as a string in the instance
            newCheckpoint.GetComponent<Checkpoint>().ID = i + 1;    //stores the checkpoint ID(int) in the instance
        }
    }

    // WHEN PLAYER ENTERS WIH CHECKPOINT
    public void OnCheckpointEnter(Collider other)
    {
        onCheckpoint = true;
        lastCheckpoint = other.gameObject;    // Sets this checkpoint as the last checkpoint entered
    }

    // WHEN PLAYER EXITS CHECKPOINT
    public void OnCheckpointExit(Collider other)
    {
        onCheckpoint = false;
    }

    //CHECK IF PLAYER HAS REACHED THE RIGHT CHECKPOINT - NEEDS REFACTORING
    //public void CheckpointValidation()
    //{
    //    //Checkpoint checkpoint = lastCheckpoint.GetComponent<Checkpoint>();

    //    if (onCheckpoint)
    //    {
    //        Checkpoint checkpoint = lastCheckpoint.GetComponent<Checkpoint>();
    //        if (checkpoint.ID == lastCheckpointCollected + 1) // this is the next valid checkpoint
    //        { 
    //            //Debug.Log(checkpointsText.ElementAt(checkpoint.ID - 1));
    //            uiManager.routeValidationText.text = checkpointsTextS0.ElementAt(checkpoint.ID - 1);    //UI info
    //            checkpoint.isCollected = true;
    //            lastCheckpointCollected++;
    //            Debug.Log("lastcheckpointcolleted: " + lastCheckpointCollected + " checkpoint.ID: = " + checkpoint.ID);

    //            //Display information UI
    //            gameManager.freezePlayer = false;
    //            uiManager.dialogBox.GetComponent<DialogBox>().instructionsText.text = checkpointsTextS0.ElementAt(checkpoint.ID - 1); //send the checkpoitn text to the dialog box
    //            uiManager.dialogBox.gameObject.SetActive(true);

    //            if (checkpoint.ID == allCheckpoints.Count)  //if this is the last checkpoint go back to the menu
    //            {
    //                gameManager.EndSession(true);
    //                uiManager.dialogBox.GetComponent<DialogBox>().returnToMenu = true;
    //            }

    //        }
    //        else
    //        {
    //            if (lastCheckpointCollected == 0) // no checkpoint has been collected
    //            {
    //                Debug.Log("Return to start");
    //                uiManager.OpenDialogBox("Incorrect - return to start");

    //                intersectionManager.GotoCoord(routeManager.routeStart.ElementAt(0), routeManager.routeStart.ElementAt(1));  //Place player at start of route
    //                uiManager.routeValidationText.text = "Incorrect - return to start";
                    
    //            }
    //            else
    //            {
    //                Debug.Log("Return to previous checkpoint: " + lastCheckpointCollected);
    //                // go to the previous checkpoint
    //                //Display information UI
    //                uiManager.OpenDialogBox("Incorrect - return to last checkpoint");

    //                List<string> previousCheckpointCoord = routeManager.SplitCoordinates(allCheckpoints[lastCheckpointCollected - 1]);
    //                intersectionManager.GotoCoord(previousCheckpointCoord[0], previousCheckpointCoord[1]);
    //                Debug.Log("Goto called");
    //                //gameManager.freezePlayer = true;
    //                //intersectionManager.GotoCoord(allCheckpoints[lastCheckpointCollected - 1], "e");
    //                uiManager.routeValidationText.text = "Incorrect - return to last checkpoint";

     
    //            }
    //        }
    //    }
    //    else //not on checkpoint
    //    {
    //        if (lastCheckpointCollected == 0) // no checkpoint has been collected
    //        {
    //            Debug.Log("Return to start");
    //            uiManager.OpenDialogBox("Incorrect - return to start");
    //            intersectionManager.GotoCoord(routeManager.routeStart.ElementAt(0), routeManager.routeStart.ElementAt(1));  //Place player at start of route
    //            uiManager.routeValidationText.text = "Incorrect - return to start";        
    //        }
    //        else
    //        {
    //            Debug.Log("Return to previous checkpoint: " + lastCheckpointCollected);
    //            // go to the previous checkpoint
    //            uiManager.OpenDialogBox("Incorrect - return to last checkpoint");
    //            List<string> previousCheckpointCoord = routeManager.SplitCoordinates(allCheckpoints[lastCheckpointCollected - 1]);
    //            intersectionManager.GotoCoord(previousCheckpointCoord[0], previousCheckpointCoord[1]);
    //            //intersectionManager.GotoCoord(allCheckpoints[lastCheckpointCollected - 1], "e");
    //            uiManager.routeValidationText.text = "Incorrect - return to last checkpoint";
              
    //        }
    //    }
    //    gameManager.freezePlayer = false;
    //}

    // IF THIS IS THE NEXT VALID CHECKPOINT, DISPLAY DIALOG BOX, ELSE RETURN TO PREVIOUS CHECKPOINT/START
    //TODO: Merge both IF statements
    public void CheckpointValidationNew()
    {
        if (gameManager.sessionStarted)
        {

            Debug.Log("Inside CheckpointValidationNew");
            //Checkpoint checkpoint = lastCheckpoint.GetComponent<Checkpoint>();

            if (onCheckpoint)
            {
                Checkpoint checkpoint = lastCheckpoint.GetComponent<Checkpoint>();
                if (checkpoint.ID == lastCheckpointCollected + 1) // if this is the next valid checkpoint
                {
                    //Debug.Log(checkpointsText.ElementAt(checkpoint.ID - 1));
                    if (gameManager.sessionData.selectedRoute == 0) // If séance 4P - S0
                    {
                        uiManager.routeValidationText.text = checkpointsTextS0.ElementAt(checkpoint.ID - 1);
                    }
                    else // If séance 4P - S6
                    {
                        uiManager.routeValidationText.text = checkpointsTextS6.ElementAt(checkpoint.ID - 1);
                    }
                       //UI info
                    checkpoint.isCollected = true;
                    lastCheckpointCollected++;
                    Debug.Log("lastcheckpointcolleted: " + lastCheckpointCollected + " checkpoint.ID: = " + checkpoint.ID);

                    //Display information UI
                    gameManager.freezePlayer = false;
                    if (gameManager.sessionData.selectedRoute == 0) // If séance 4P - S0
                    {
                        uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().instructionsText.text = checkpointsTextS0.ElementAt(checkpoint.ID - 1).Replace("|", System.Environment.NewLine); //send the checkpoitn text to the dialog box
                    }
                    else // If séance 4P - S6
                    {
                        uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().instructionsText.text = checkpointsTextS6.ElementAt(checkpoint.ID - 1).Replace("|", System.Environment.NewLine); //send the checkpoitn text to the dialog box
                    }
                    
                    uiManager.dialogBoxCheckpoint.gameObject.SetActive(true);
                    uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.Select();
                    uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.OnSelect(null);

                    if (checkpoint.ID == allCheckpoints.Count)  //if this is the last checkpoint go back to the menu
                    {
                        //gameManager.EndSession(false); - REPLACE WITH BELOW

                        //endTime = Time.time;
                        gameManager.sessionEnded = true;
                        //sessionStarted = false;
                        gameManager.sessionData.sessionPaused = false;
                        //gameManager.freezePlayer = true;
                        GlobalControl.instance.sessionData = gameManager.sessionData;
                        uiManager.sessionStatusText.text = "(S) Session status: Ended";
           
                        saveSessionData.SaveData();    //Write data right before closing file
                        saveSessionData.stopSavingData();   //Stop saving data
                        uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().returnToMenu = true;
                    }
                }
                else // if this is not the next valid checkpoint
                {
                    returnToWhere();
                }
            }
            else //not on checkpoint
            {
                returnToWhere();
            }
        }
        

    }

    private void returnToWhere()
    {
        if (lastCheckpointCollected == 0) // if no checkpoint has been collected, return to start
        {
            Debug.Log("Return to start");
            uiManager.OpenDialogBox("(2) Incorrect - retour au départ");

            intersectionManager.GotoCoord(gameManager.sessionData.routeStart.ElementAt(0), gameManager.sessionData.routeStart.ElementAt(1));  //Place player at start of route
            uiManager.routeValidationText.text = "Incorrect - return to start";

        }
        else // return to previous checkpoint
        {
            Debug.Log("Return to previous checkpoint: " + lastCheckpointCollected);
            // go to the previous checkpoint
            //Display information UI
            // uiManager.OpenDialogBox("(3) Incorrect - retour au dernier checkpoint.\n" + checkpointsInstructionsS0.ElementAt(lastCheckpointCollected - 1));

            //gameManager.freezePlayer = true;
            if (gameManager.sessionData.selectedRoute == 0)
            {
                uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().instructionsText.text = "(3) Incorrect - retour au dernier checkpoint.\n" + checkpointsInstructionsS0.ElementAt(lastCheckpointCollected - 1); //send the checkpoitn text to the dialog box
            }
            else
            {
                uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().instructionsText.text = "(3) Incorrect - retour au dernier checkpoint.\n" + checkpointsInstructionsS6.ElementAt(lastCheckpointCollected - 1); //send the checkpoitn text to the dialog box
            }
            
            uiManager.dialogBoxCheckpoint.gameObject.GetComponent<CheckpointDialogBox>().firstDialog = false;
            uiManager.dialogBoxCheckpoint.gameObject.SetActive(true);
            uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.Select();
            uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.OnSelect(null);

            List<string> previousCheckpointCoord = routeManager.SplitCoordinates(allCheckpoints[lastCheckpointCollected - 1]);
            intersectionManager.GotoCoord(previousCheckpointCoord[0], previousCheckpointCoord[1]);
            Debug.Log("Goto called");
            //gameManager.freezePlayer = true;
            uiManager.routeValidationText.text = "Incorrect - return to last checkpoint";
            gameManager.freezePlayer = false;
        }
    }
    //TODO: Comment out, only called rom PlayerControllerOld.cs
    public void ClickCheckpointBtn()
    {
        Debug.Log("selected object: " + UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);
        if (uiManager.dialogBoxCheckpoint.gameObject.activeInHierarchy && !uiManager.dialogBox.gameObject.activeInHierarchy && !uiManager.hotspotDialogBox.gameObject.activeInHierarchy) //checkpoint hotbox is active but no other dialog box
        {
            if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.gameObject) //if button is selected
            {
                Debug.Log("checkpt button selecte");
            }
            else
            {
                Debug.Log("Checkpt button not selected");
                //uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.Select();
                //uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.OnSelect(null);
                

            }
        }
    }
}
