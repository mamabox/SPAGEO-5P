using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StdScManager : MonoBehaviour
{
    // Game objects and classes
    private GameManager gameManager;
    private UIManager uiManager;
    private GameObject player;
    //private Sc6Data _sc6Data;
    private SequenceManager scenarioManager;
    private CheckpointManager checkpointManager;
    private RouteManager routeManager;
    private IntersectionManager intersectionManager;
    private LimiterManager limiterManager;
    private StdScData _scenarioData;

    private int NbStdScenarios; //Number of standard scenarios 

    private void Awake()
    {
        //Game objects and classes
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        //_sc6Data = gameManager.scenariosData.sc6Data;
        scenarioManager = gameManager.GetComponent<SequenceManager>();
        checkpointManager = gameManager.GetComponent<CheckpointManager>();
        routeManager = gameManager.GetComponent<RouteManager>();
        intersectionManager = gameManager.GetComponent<IntersectionManager>();
        limiterManager = FindObjectOfType<GameManager>().GetComponent<LimiterManager>();

        NbStdScenarios = gameManager.scenariosData.stdScData.Count();

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void SetupScenario(int scenarioIndex)
    {
        int _selectedRouteIndex = gameManager.sessionData.selectedRoute;
        _scenarioData = gameManager.scenariosData.stdScData[scenarioIndex];

        scenarioManager.scenario1Props.SetActive(false);
        // TODO Set active scenario to sc7 data
        Debug.Log("Route coordinates = " + _scenarioData.routes[_selectedRouteIndex].routeCoord);
        gameManager.sessionData.selectedRouteCoord = _scenarioData.routes[_selectedRouteIndex].routeCoord.Split(',').ToList(); //Converts string of route coordinates to a list.
        gameManager.sessionData.routeStart = new List<string> { _scenarioData.routes[_selectedRouteIndex].startCoord.coord, _scenarioData.routes[_selectedRouteIndex].startCoord.cardDir };
        //gameManager.sessionData.routeStartNew =_sc6Data.barrierRoutes[selectedBarriers].startCoord;
        gameManager.sessionData.selectedRouteDir = intersectionManager.ConvertRouteToDirection(gameManager.sessionData.selectedRouteCoord);

        SetAttemptsAndValidationLimitsNew();

        //IF image validation is enabled, check the limit status
        if (_scenarioData.imageValidation)
            {
            CheckValidation();
        }
    }

    //TODO: remove referencea to actiescenario since only maxAttempts and maxValidations are used
    public void SetAttemptsAndValidationLimitsNew()
    {
        scenarioManager.activeScenario.maxAttempts = _scenarioData.attemptsNb;
        scenarioManager.activeScenario.maxValidations = _scenarioData.validationNb;

        if (_scenarioData.attemptsNb == 0)
            scenarioManager.attemptsLimited = false;
        else
            scenarioManager.attemptsLimited = true;

        if (_scenarioData.validationNb == 0)
            scenarioManager.validationsLimited = false;
        else
            scenarioManager.validationsLimited = true;

        Debug.Log("Scenario has " + _scenarioData.attemptsNb + "nb of attempts and " + _scenarioData.validationNb + "  nb of validations "); 
    }

    public void StartScenario(int scenarioID)
    {
        //StartSavingData();

    }

    public void CheckValidation()
    {
        if (!gameManager.sessionData.sessionPaused)
        {
            routeManager.validationEnabled = true;
            gameManager.attemptsAllowed = true;
        }

        else if (gameManager.sessionData.sessionPaused)   //if player has already done the validation by image
        {
            //gameManager.validationCount++;
            routeManager.validationEnabled = false;
            gameManager.attemptsAllowed = false;
            //attemptsLimited = false;
        }
    }

    public void EndScenario(int scenarioID)
    {
        //StopSavingData();
        gameManager.sessionEnded = true;
    }

    //Returns the scenario index of gameManger.scenariosDat.stdScData for a given scenario number. Returns -1 if the scenario is not found.
    public int ReturnScenarioIndex(int scenarioID)
    {
        int scenarioIndex = -1;

        for (int x = 0; x < NbStdScenarios; x++)
        {
            if (gameManager.scenariosData.stdScData[x].scenarioID == scenarioID)
                scenarioIndex = x;
        }

        return scenarioIndex;
    }

}
