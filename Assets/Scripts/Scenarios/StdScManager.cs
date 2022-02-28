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
    private Sc6Data _sc6Data;
    private SequenceManager scenarioManager;
    private CheckpointManager checkpointManager;
    private RouteManager routeManager;
    private IntersectionManager intersectionManager;
    private LimiterManager limiterManager;

    private int NbStdScenarios; //Number of standard scenarios 

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

    public void SetupScenario(int scenarioID)
    {

    }

    public void StartScenario(int scenarioID)
    {
        //StartSavingData();
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
