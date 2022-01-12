using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sc8Manager : MonoBehaviour
{
    // Game objects and classes
    private GameManager gameManager;
    //private UIManager uiManager;
    private GameObject player;
    private Sc8Data _sc8Data;
    private SequenceManager scenarioManager;
    private CheckpointManager checkpointManager;
    private RouteManager routeManager;

    private int checkpointIndex;


    // Start is called before the first frame update
    void Awake()
    {
        checkpointIndex = 0;

        //Game objects and classes
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        _sc8Data = gameManager.scenariosData.sc8Data;
        scenarioManager = gameManager.GetComponent<SequenceManager>();
        checkpointManager = gameManager.GetComponent<CheckpointManager>();
        routeManager = gameManager.GetComponent<RouteManager>();
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
        scenarioManager.scenario1Props.SetActive(false); //Make props visible
        //scenarioManager.routeManager.validationEnabled = false;
        //scenarioManager.validationsLimited = false;
        gameManager.attemptsAllowed = true;
        gameManager.sessionData.selectedRouteCoord = new List<string>();
        gameManager.sessionData.routeStart = new List<string> { _sc8Data.startCoord.coord, _sc8Data.startCoord.cardDir}; //forcing cardinal direciton 
        gameManager.freezeMovement = false;

        // (1) Read data from scenariosData.json
        // ( ) Generate checkpoints
        //allCheckpoints = new List<string>(_sc8Data.checkpoints);
        checkpointManager.GenerateCheckpoints(_sc8Data.checkpoints);

        // Draw lines
        for (int i = 0; i < _sc8Data.routeSegments.Count(); i++)
        {
            List<string> tempRoute = _sc8Data.routeSegments[i].Split(',').ToList();
            routeManager.SpawnLine(tempRoute, i);
        }
    }

    public void StartScenario()
    {

    }

    public void EndScenario()
    {

    }
}
