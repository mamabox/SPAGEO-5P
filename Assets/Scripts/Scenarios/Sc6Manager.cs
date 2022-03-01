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
    private LimiterManager limiterManager;

    int barriersIndex;

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
        limiterManager = FindObjectOfType<GameManager>().GetComponent<LimiterManager>();

        
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
        barriersIndex = gameManager.sessionData.selectedRoute;

        scenarioManager.scenario1Props.SetActive(false);
        routeManager.validationEnabled = false; //No validation possible

        gameManager.sessionData.selectedRouteCoord = new List<string>(); //There is no route
        //gameManager.sessionData.selecedRouteCoordNew = new List<Coordinate>(); //TODO: for new system
        List<Coordinate> routeBarriers = _sc6Data.barrierRoutes[barriersIndex].barriersCoord; //Creates a new list frmo teh selected limiters list
        gameManager.sessionData.routeStart = new List<string> { _sc6Data.barrierRoutes[barriersIndex].startCoord.coord, _sc6Data.barrierRoutes[barriersIndex].startCoord.cardDir }; // routeStart (coord, dir)
        //gameManager.sessionData.routeStartNew =_sc6Data.barrierRoutes[selectedBarriers].startCoord;
        limiterManager.allBarriers = _sc6Data.barrierRoutes[barriersIndex].barriersCoord;
        limiterManager.GenerateBarriers(routeBarriers);
        SaveDataConfig();
    }

    public void StartScenario()
    {
        //StartSavingData();
    }

    private void SaveDataConfig()
    {
        string _barrierCoord;

        List<string> barriersCoordStr = new List<string>();
        for (int x = 0; x < _sc6Data.barrierRoutes[barriersIndex].barriersCoord.Count(); x++) // for each barrier coordinate
        {
            _barrierCoord = _sc6Data.barrierRoutes[barriersIndex].barriersCoord[x].coord + _sc6Data.barrierRoutes[barriersIndex].barriersCoord[x].cardDir;
            barriersCoordStr.Add(_barrierCoord);
        }
        gameManager.sessionData.selectedRouteCoord = barriersCoordStr;
        Debug.Log("barriersCoordStr =" + string.Join((","), barriersCoordStr));
    }

        public void EndScenario()
    {
        //StopSavingData();
        gameManager.sessionEnded = true;
    }
}
