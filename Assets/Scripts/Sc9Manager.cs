using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sc9Manager : MonoBehaviour
{
    //Props
    private GameObject startObj;
    private GameObject targetObj;
    public GameObject[] allObjects;

    //Calculation
    private float angleToTarget;
    private float rotationValue;
    private float startRotation;
    private float correctRotation;
    private float endRotation;
    private float errorRotation;

    private int trialsCount;
    private int currentTrial;
    private int startObjNum;
    private int targetObjNum;

    public List<string> objNames; // List of all objects names + pronoums

    private Vector3 startPosition;

    private GameManager gameManager;
    //private UIManager uiManager;
    private GameObject player;
    private Sc9Data _sc9Data;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        _sc9Data = gameManager.scenariosData.sc9Data;

        currentTrial = 1;
        startPosition = new Vector3(0, 0, 0);
        //Debug.Log("Inside sc9Manager - Awake()");

    }

    // Start is called before the first frame update
    void Start()
    {
        CreateNounsPronouns();
        trialsCount = gameManager.scenariosData.sc9Data.trials.Count();
        //Debug.Log("Sc9 has this number of trials: " + trialsCount);
        SetupTrial();
    }

    // Display the instructions only if the session has started and not ended
    void Update()
    {
       if (gameManager.sessionStarted && !gameManager.sessionEnded)
            {
            gameManager.uiManager.dialogBoxCheckpoint.gameObject.SetActive(true);
        }
        else
            gameManager.uiManager.dialogBoxCheckpoint.gameObject.SetActive(false);
    }

    private void CreateNounsPronouns()
    {
        //Debug.Log("Inside sc9Manager - CreateNounsPronouns()" + gameManager.scenariosData.sc9Data.propObjs.Count());
        objNames = new List<string>();

        for (int x = 0; x < gameManager.scenariosData.sc9Data.propObjs.Count(); x++){
            objNames.Add(gameManager.scenariosData.sc9Data.propObjs[x].pronoun + " " + gameManager.scenariosData.sc9Data.propObjs[x].name);
        }
        //Debug.Log("PropObjs: " + string.Join(", ", objNamesPronouns));
    }
    //Called when pressing validaiton button
    public void ValidateTrial()
    {
        Calculate();

        if (currentTrial < trialsCount)   // IF there are trials left, setup the next trial
        {
            currentTrial++;
            SetupTrial();
        }
        else // IF no trials left, end the session
        {
            gameManager.sessionEnded = true;
            gameManager.uiManager.dialogBox.GetComponent<DialogBox>().returnToMenu = true;
            gameManager.uiManager.OpenDialogBox(_sc9Data.instructions.end);
        }  
    }

    public void SetupTrial()
    {
        //Retrieve start coordinates
        Debug.Log("Start coordinate is: " + gameManager.scenariosData.sc9Data.trials[currentTrial - 1].position);
        string startCoordStr = gameManager.scenariosData.sc9Data.trials[currentTrial - 1].position;
        //List<string> startCoord = startCoordStr.Split('_').ToList();
        gameManager.GetComponent<IntersectionManager>().GotoCoord(startCoordStr,"X");

        //Set the start and target objects
        startObjNum = gameManager.scenariosData.sc9Data.trials[currentTrial - 1].startObj;
        targetObjNum = gameManager.scenariosData.sc9Data.trials[currentTrial - 1].targetObj;

        startObj = allObjects[startObjNum - 1];
        targetObj = allObjects[targetObjNum - 1];

        //Display instructions
        string attemptInstructionText = "Trial: " + currentTrial + " / " + trialsCount + "|"+ _sc9Data.instructions.attempts[0] + objNames[targetObjNum - 1] + _sc9Data.instructions.attempts[1];
        gameManager.uiManager.OpenCheckpointDialogBox(attemptInstructionText, false);

        //Update dropdown labels
        //startChoice.value = _startObjNum - 1;
        //targetChoice.value = _targeObjNum - 1;

        
        
        //List<int> startCoord = startCoordStr.Split('_').Select(int.Parse).ToList();
        //Debug.Log("Start coordinate x: " + startCoord[0] + " Start coordiante y: " + startCoord[1]);

        //Move object to new coordiantes
        //Vector3 offSet = new Vector3(startCoord[0], 0, startCoord[1]);
        //transform.position = startPosition + offSet;

        //gameManager.sessionData.routeStart = new List<string> { gameManager.scenariosData.sc9Data.trials[0].position, "" }; //forcing cardinal direciton 

        SetupContinued();

    }

    private void SetupContinued()
    {
        //Set and save the correct rotation to the target
        player.transform.LookAt(targetObj.transform);
        correctRotation = player.transform.rotation.eulerAngles.y;

        //Set and save the start rotatin
        player.transform.LookAt(startObj.transform);
        startRotation = player.transform.rotation.eulerAngles.y;
        player.GetComponent<PlayerController>().currentRotation = player.transform.rotation.eulerAngles;
        //gameManager.freezeMovement = true;
        //sliderUI.value = startRotation;

        //Calculate angle to target
        Vector3 targetDir = targetObj.transform.position - player.transform.position;
        angleToTarget = Vector3.Angle(targetDir, player.transform.forward);

        gameManager.uiManager.ptsotText.text = ("Trial #: " + currentTrial + "\nStart obj: " + objNames[startObjNum-1] + "\nTarget obj: " + objNames[targetObjNum - 1] + "\nStart rot: " + startRotation + "\nAngle to target = " + angleToTarget.ToString() + "\nCorrect rot to target = " + correctRotation);

        //UItext.text = ("Player's start rotation: " + startRotation + "\n Angle to target = " + angleToTarget.ToString());
        //ResetSPheres();
        //startObj.GetComponent<Renderer>().material = startMat;
        //targetObj.GetComponent<Renderer>().material = targetMat;
    }

    private void Calculate()
    {
        endRotation = player.transform.rotation.eulerAngles.y;
        errorRotation = Mathf.Abs(endRotation - correctRotation);

        gameManager.uiManager.ptsotText.text = ("Trial #: " + currentTrial + "\nStart obj: " + objNames[startObjNum - 1] + "\nTarget obj: " + objNames[targetObjNum - 1] + "\nStart rot: " + startRotation + "\nAngle to target = " + angleToTarget.ToString() + "\nCorrect rot to target = " + correctRotation + "\n\n Previous trial end rot: " + endRotation + "\n Previous trial error: " + errorRotation);
    }
}
