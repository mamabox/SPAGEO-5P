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

    public List<string> objNamesPronouns;

    private Vector3 startPosition;

    private GameManager gameManager;
    private GameObject player;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");

        currentTrial = 0;
        startPosition = new Vector3(0, 0, 0);
        //Debug.Log("Inside sc9Manager - Awake()");

    }

    // Start is called before the first frame update
    void Start()
    {
        CreateNounsPronouns();
        trialsCount = gameManager.scenariosData.sc9Data.trials.Count();
        AutoSetup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateNounsPronouns()
    {
        //Debug.Log("Inside sc9Manager - CreateNounsPronouns()" + gameManager.scenariosData.sc9Data.propObjs.Count());
        objNamesPronouns = new List<string>();

        for (int x = 0; x < gameManager.scenariosData.sc9Data.propObjs.Count(); x++){
            objNamesPronouns.Add(gameManager.scenariosData.sc9Data.propObjs[x].pronoun + " " + gameManager.scenariosData.sc9Data.propObjs[x].name);
        }
        //Debug.Log("PropObjs: " + string.Join(", ", objNamesPronouns));
    }

    public void AutoSetup()
    {
        // Setup the next trial
        if (currentTrial < trialsCount)
        {
            currentTrial++;
        }
        else
            currentTrial = 1;

        //Set the start and target objects
        int _startObjNum = gameManager.scenariosData.sc9Data.trials[currentTrial - 1].startObj;
        int _targeObjNum = gameManager.scenariosData.sc9Data.trials[currentTrial - 1].targetObj;

        startObj = allObjects[_startObjNum - 1];
        targetObj = allObjects[_targeObjNum - 1];

        //Update dropdown labels
        //startChoice.value = _startObjNum - 1;
        //targetChoice.value = _targeObjNum - 1;

        //Retrieve start coordinates
        Debug.Log("Start coordinate is: " + gameManager.scenariosData.sc9Data.trials[currentTrial - 1].position);
        string startCoordStr = gameManager.scenariosData.sc9Data.trials[currentTrial - 1].position;
        //List<int> startCoord = startCoordStr.Split('_').Select(int.Parse).ToList();
        //Debug.Log("Start coordinate x: " + startCoord[0] + " Start coordiante y: " + startCoord[1]);

        //Move object to new coordiantes
        //Vector3 offSet = new Vector3(startCoord[0], 0, startCoord[1]);
        //transform.position = startPosition + offSet;

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

        Debug.Log("Start rotation: " + startRotation + "\n Angle to target = " + angleToTarget.ToString() + "\n Correct rotation to target = " + correctRotation + "\n End rotation: " + endRotation + "\n Error: " + errorRotation);

        //UItext.text = ("Player's start rotation: " + startRotation + "\n Angle to target = " + angleToTarget.ToString());
        //ResetSPheres();
        //startObj.GetComponent<Renderer>().material = startMat;
        //targetObj.GetComponent<Renderer>().material = targetMat;
    }
}
