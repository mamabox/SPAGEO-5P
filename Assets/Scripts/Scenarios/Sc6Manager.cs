using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * Sc6Manager.cs
 * 
 * Manages the Scenario 6 - PTSOT task

 * NOTES ABOUT SAVING DATA 
 **/

public class Sc6Manager : MonoBehaviour
{
    // Game objects and classes
    private GameManager gameManager;
    private UIManager uiManager;
    private GameObject player;
    private Sc6Data _sc6Data;
    private SequenceManager scenarioManager;
    private CheckpointManager checkpointManager;
    private RouteManager routeManager;
    private IntersectionManager intersectionManager;

    private void Awake()
    {
        //Game objects and classes
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        _sc6Data = gameManager.scenariosData.sc6Data;
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
        int selectedBarriers = gameManager.sessionData.selectedRoute;

        scenarioManager.scenario1Props.SetActive(false);
        routeManager.validationEnabled = false; //No validation possible

        gameManager.sessionData.selectedRouteCoord = new List<string>(); //There is no route
        List<string> thisLimiter = _sc6Data.barriers[selectedBarriers].barriersCoord.Split(',').ToList(); //Creates a new list frmo teh selected limiters list
        gameManager.sessionData.routeStart = new List<string> { _sc6Data.barriers[selectedBarriers].startCoord.coord, _sc6Data.barriers[selectedBarriers].startCoord.cardDir };
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
}
