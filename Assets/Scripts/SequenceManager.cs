using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class SequenceManager : MonoBehaviour
{
    public int selectedScenario;        // Sequence selected from Menu
    //public int selectedRoute;
    public RouteManager routeManager;
    private CheckpointManager checkpointManager;
    private IntersectionManager intersectionManager;
    private GameObject player;
    private PlayerController playerController;
    private GameManager gameManager;
    private HotspotManager hotspotManager;
    private UIManager uIManager;
    public GameObject scenario1Props;
    public LimiterManager limiterManager;
    //public GlobalControl.SessionData sessionData;

    //private int allowedAttempts;
    //private int allowedValidations;
    //private int currentAttempt;
    //private int currentValidation;

    //Scenarios managers
    private Sc8Manager sc8Manager;
    private Sc9Manager sc9Manager;

    //Scenario settings
    public bool attemptsLimited = false;
    public bool validationsLimited = false;
   

    //Data files
    private string importPath;  //Data files location
    private char textFilesCommentSeparator = '*';

    // Lists of scenario data
    public List<string> scenario1S0TextFile;
    public List<string> hotspotsS0TextFile;
    public List<string> scenario1S6TextFile;
    public List<string> scenario2TextFile;
    public List<string> scenario3TextFile;
    public List<string> scenario4TextFile;
    public List<string> scenario5TextFile;
    public List<string> scenario6TextFile;
    public List<string> scenario7TextFile;

    // Scenario Data structures
    public Scenario1Data scenario1S0Data = new Scenario1Data();
    public Scenario1Data scenario1S6Data = new Scenario1Data();
    public ScenarioStdData scenario2Data = new ScenarioStdData();
    public ScenarioStdData scenario3Data = new ScenarioStdData();
    public ScenarioStdData scenario4Data = new ScenarioStdData();
    public ScenarioStdData scenario5Data = new ScenarioStdData();
    public ScenarioStdData activeScenario = new ScenarioStdData();  //currently selected scenario (for 2-5)
    public Scenario6Data scenario6Data = new Scenario6Data();
    public ScenarioStdData scenario7Data = new ScenarioStdData();

    private void Awake()
    {
        //Initialise Components
        routeManager = GetComponent<RouteManager>();
        checkpointManager = GetComponent<CheckpointManager>();
        intersectionManager = GetComponent<IntersectionManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        hotspotManager = FindObjectOfType<GameManager>().GetComponent<HotspotManager>();
        uIManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        limiterManager = FindObjectOfType<GameManager>().GetComponent<LimiterManager>();
        sc9Manager = GameObject.Find("ScenariosManager").GetComponent<Sc9Manager>();

        // Import data session from GlobalControl
        //selectedScenario = GlobalControl.instance.activeSequence;
        // selectedRoute = GlobalControl.instance.activeRoute;
        //sessionData = GlobalControl.instance.sessionData;   // isGroupSession, groupID, studentIDs, isSender //TODO: Move to gameManager

        importPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Media/Text/");
        ImportAllTextFiles();   //Import text files containing

        //IMPORT SCENARIOS DATA FROM TEXT FILES
        Debug.Log("Import Scenarios");
        scenario1S0Data = ImportScenario1Data(scenario1S0TextFile, hotspotManager.hotspotTextS0, 2);    //Scenario 1
        scenario1S6Data = ImportScenario1Data(scenario1S6TextFile, hotspotManager.hotspotTextS6, 2);    //Scenario 6
        scenario2Data = ImportScenarioStdData(scenario2TextFile, 2);    //Scenario 2
        scenario3Data = ImportScenarioStdData(scenario3TextFile, 2);    //Scenario 3
        scenario4Data = ImportScenarioStdData(scenario4TextFile, 2);    //Scenario 4
        scenario5Data = ImportScenarioStdData(scenario5TextFile, 2);    // Scenario 5
        scenario6Data = ImportScenario6Data(scenario6TextFile, 2);    // Scenario 6
        scenario7Data = ImportScenarioStdData(scenario7TextFile, 2);    // Scenario 7
    }

    void Start()
    {
        Debug.Log("SCENARIO: " + gameManager.sessionData.selectedScenario + " - ROUTE: " + (gameManager.sessionData.selectedRoute + 1));
        Debug.Log("ROUTE COORD: " + string.Join(",", gameManager.sessionData.selectedRoute));
//        Debug.Log("Has already validated by image: " + sessionData.hasDoneValidationByImage);
        //1. SETUP THE SEQUENCE
        if (gameManager.sessionData.selectedScenario == 1)
        {
            if (gameManager.sessionData.selectedRoute == 0)
            {
                Scenario1(scenario1S0Data);
            }
            else
            {
                Scenario1(scenario1S6Data);
            }
        }
        else if (gameManager.sessionData.selectedScenario == 2)
        {
            Scenario2();
        }
        else if (gameManager.sessionData.selectedScenario == 3)
        {
            Scenario3();
        }
        else if (gameManager.sessionData.selectedScenario == 4)
        {
            Scenario4();
        }
        else if (gameManager.sessionData.selectedScenario == 5)
        {
            Scenario5();
        }
        else if (gameManager.sessionData.selectedScenario == 6)
        {
            Scenario6();
        }
        else if (gameManager.sessionData.selectedScenario == 7)
        {
            Scenario7();
        }
        else if (gameManager.sessionData.selectedScenario == 8)
        {
            sc8Manager.SetupScenario();
        }
        else if (gameManager.sessionData.selectedScenario == 9)
        {
            //Scenario9();
            sc9Manager.SetupScenario();
        }

        //2. SETUP THE PLAYER
        playerController.InitialisePlayer();
    }


    //SCENARIO 1 - HOTSPOTS + VALIDATION BY CHECKPOINTS
    private void Scenario1(Scenario1Data scenarioData)
    {
        scenario1Props.SetActive(true);
        routeManager.validationEnabled = true; //Validation is possibles
        attemptsLimited = false;
        validationsLimited = false;
        gameManager.attemptsAllowed = false;
        gameManager.sessionData.selectedRouteCoord = new List<string>();


        //Read data from routeS1
        gameManager.sessionData.routeStart = scenarioData.startCoord;
        checkpointManager.allCheckpoints = new List<string>(scenarioData.checkpoints);
        hotspotManager.allHotspots = new List<string>(scenarioData.hotspots);

        //Routes import
        for (int i = 0; i < scenarioData.routesCount; i++)
        {
            List<string> tempRoute = scenarioData.routes[i].Split(',').ToList();
            routeManager.SpawnLine(tempRoute, i);
        }

        checkpointManager.GenerateCheckpoints(checkpointManager.allCheckpoints);
        hotspotManager.GenerateHotspots(hotspotManager.allHotspots, scenarioData.hotspotsURL);

        ////Set scenario variables
        //allowedAttempts = 1;
        //allowedValidations = 1;

    }

    //SCENARIO 2 - WORK IN PAIR: SENDER/RECEIVER
    private void Scenario2()
    {
        scenario1Props.SetActive(false);
        activeScenario = scenario2Data;
        gameManager.sessionData.selectedRouteCoord = scenario2Data.routes.ElementAt(gameManager.sessionData.selectedRoute).Split(',').ToList();
        //gameManager.sessionData.selectedRouteCoord = routeManager.selectedRouteCoord;
        gameManager.sessionData.routeStart = routeManager.getRouteStart(gameManager.sessionData.selectedRouteCoord);   //sets at what position the player should start
        if (gameManager.sessionData.isSender)   // if this is the sender, draw the line
        {
            routeManager.validationEnabled = true;
            List<string> lineToDraw = new List<string>(gameManager.sessionData.selectedRouteCoord); //Temportarily displays route (DEUB)
            lineToDraw.RemoveAt(0); //Remove the start coordiante
            routeManager.SpawnLine(lineToDraw, 0);
            uIManager.senderReceiverUI = "(E)"; //for UI display
        }
        else //validation is possible
        {
            routeManager.validationEnabled = true;
            uIManager.senderReceiverUI = "(R)"; //for UI display
        }

        SetAttemptsValidationLimits();

    }

    private void Scenario3()
    {
        scenario1Props.SetActive(false);
        gameManager.attemptsAllowed = false;
        activeScenario = scenario3Data; //Sets as active scenario
        gameManager.sessionData.selectedRouteCoord = scenario3Data.routes.ElementAt(gameManager.sessionData.selectedRoute).Split(',').ToList(); //Sets the route selected in menu as the session's route
        gameManager.sessionData.routeStart = routeManager.getRouteStart(gameManager.sessionData.selectedRouteCoord);   //sets at what position the player should start
        gameManager.sessionData.selectedRouteDir = intersectionManager.ConvertRouteToDirection(gameManager.sessionData.selectedRouteCoord);
        Debug.Log("SCENARIO 3 - selectedRouteDir" + string.Join(",", gameManager.sessionData.selectedRouteDir));

        SetAttemptsValidationLimits();
        //if (!gameManager.sessionData.sessionPaused)
        //{
        //    gameManager.validationCount = -1;
        //}
        gameManager.attemptCount = 0;
   

        //1. display lines and follow them
        routeManager.validationEnabled = false;
        //gameManager.attemptsAllowed = true ;
        List<string> lineToDraw = new List<string>(gameManager.sessionData.selectedRouteCoord); //Temportarily displays route (DEUB)
        lineToDraw.RemoveAt(0); //Remove the start coordiante
        routeManager.SpawnLine(lineToDraw, 0);

      
        if (gameManager.recordRoute)   //if player has 
        {
            //routeManager.lineDrawn.SetActive(false);
        }

        //2. start recording: go to start without lines and try to validate the route
    }


    //SCENARIO 4 - RETRACE AND VALIDATE A ROUTE PREVIOUSLY TAKEN + DISPLAY LINES
    private void Scenario4()
    {
        scenario1Props.SetActive(false);
        activeScenario = scenario4Data; //Sets as active scenario
        gameManager.sessionData.selectedRouteCoord = scenario4Data.routes.ElementAt(gameManager.sessionData.selectedRoute).Split(',').ToList(); //Sets the route selected in menu as the session's route
        gameManager.sessionData.routeStart = routeManager.getRouteStart(gameManager.sessionData.selectedRouteCoord);   //sets at what position the player should start
        gameManager.sessionData.selectedRouteDir = intersectionManager.ConvertRouteToDirection(gameManager.sessionData.selectedRouteCoord);
        Debug.Log(string.Join(",", gameManager.sessionData.selectedRouteDir));

        SetAttemptsValidationLimits();
        //gameManager.attemptCount = 0;

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
            List<string> lineToDraw = new List<string>(gameManager.sessionData.selectedRouteCoord); //Temportarily displays route (DEUB)
            lineToDraw.RemoveAt(0); //Remove the start coordiante
            routeManager.SpawnLine(lineToDraw, 0);
        }
    }


    //SCENARIO 5 - OPEN WORLD WITH OPTION TO DISPLAY ROUTES
    private void Scenario5()
    {
        scenario1Props.SetActive(false);
        routeManager.validationEnabled = false; //No validation possible

        if (gameManager.sessionData.selectedRoute == 0) //First route selected just go to start point and draw nothing
        {
            //Debug.Log("scenario5Data.routesToDraw.ElementAt(0)): " + scenario5Data.routesToDraw.ElementAt(0));
            gameManager.sessionData.routeStart = routeManager.SplitCoordinates(scenario5Data.routes.ElementAt(0));
            gameManager.sessionData.selectedRouteCoord = new List<string>();
        }
        else
        {
            gameManager.sessionData.selectedRouteCoord = scenario5Data.routes.ElementAt(gameManager.sessionData.selectedRoute).Split(',').ToList();
            gameManager.sessionData.routeStart = routeManager.getRouteStart(gameManager.sessionData.selectedRouteCoord);

            //Debug.Log("Selected Route" + string.Join(",", routeManager.selectedRoute));
            List<string> lineToDraw = new List<string>(gameManager.sessionData.selectedRouteCoord); //Temportarily displays route (DEUB)
            lineToDraw.RemoveAt(0); //Remove the start coordiante
            //Debug.Log("route to Draw: " + string.Join(",", lineToDraw));

            routeManager.SpawnLine(lineToDraw, 0);
        }
    }

    //SCENARIO 6 - GENERATE BARRIERS
    private void Scenario6()
    {
        scenario1Props.SetActive(false);
        routeManager.validationEnabled = false; //No validation possible

        //gameManager.sessionData.routeStart = routeManager.SplitCoordinates("5_5W"); //Manual start
        //gameManager.sessionData.routeStart = routeManager.SplitCoordinates(scenario6Data.limiters.ElementAt(0)); //Start point is the first item in the selecte limiters list

        gameManager.sessionData.selectedRouteCoord = new List<string>(); //There is no route

        List<string> thisLimiter = new List<string>(scenario6Data.limiters.ElementAt(gameManager.sessionData.selectedRoute).Split(',').ToList()); //Creates a new list frmo teh selected limiters list
        gameManager.sessionData.routeStart = routeManager.getRouteStart(thisLimiter);

        limiterManager.allLimiters = new List<string>(scenario6Data.limiters);
        Debug.Log("Scenario 6 limiters " + string.Join("+", scenario6Data.limiters));
        //limiterManager.GenerateLimiters(scenario6Data.limiters);
        thisLimiter.RemoveAt(0);
        limiterManager.GenerateLimiters(thisLimiter);
    }

    //SCENARIO 7 - RETRACE AND VALIDATE A ROUTE PREVIOUSLY TAKEN
    private void Scenario7()
    {
        scenario1Props.SetActive(false);
        activeScenario = scenario7Data; //Sets as active scenario
        gameManager.sessionData.selectedRouteCoord = scenario7Data.routes.ElementAt(gameManager.sessionData.selectedRoute).Split(',').ToList(); //Sets the route selected in menu as the session's route
        gameManager.sessionData.routeStart = routeManager.getRouteStart(gameManager.sessionData.selectedRouteCoord);   //sets at what position the player should start
        gameManager.sessionData.selectedRouteDir = intersectionManager.ConvertRouteToDirection(gameManager.sessionData.selectedRouteCoord);
        Debug.Log(string.Join(",", gameManager.sessionData.selectedRouteDir));

        SetAttemptsValidationLimits();
        //gameManager.attemptCount = 0;

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

    //SCENARIO 9 - TACHE DE POINTAGE
    private void Scenario9()
    {
        scenario1Props.SetActive(true); //Make props visible
        routeManager.validationEnabled = true; //Validation is possible
        attemptsLimited = false;
        validationsLimited = false;
        gameManager.attemptsAllowed = false;
        gameManager.sessionData.selectedRouteCoord = new List<string>();
        gameManager.sessionData.routeStart = new List<string> { gameManager.scenariosData.sc9Data.trials[0].position, "" }; //forcing cardinal direciton 
        gameManager.freezeMovement = true;

        sc9Manager.SetupTrial();
    }

    //PULLS DATA FROM LIST<STRING> AND STORES IT IN STRUCTURE (SCENARIO 1)
    private Scenario1Data ImportScenario1Data(List<string> scenarioTextFile, List<string> hotspotTextFile, int routesStartAtLine)
    {
        Scenario1Data scenarioData = new Scenario1Data();

        scenarioData.hotspotsURL = new List<string>();
        scenarioData.routes = new List<string>();

        //Scenario 1 (Routes start at line #2)

        scenarioData.start = scenarioTextFile.ElementAt(0);              //Start string
        scenarioData.startCoord = routeManager.SplitCoordinates(scenarioData.start);  //Start coordinates (coord, dir)
        scenarioData.checkpoints = scenarioTextFile.ElementAt(1).Split(',').ToList();
        scenarioData.hotspots = hotspotTextFile.ElementAt(0).Split(',').ToList();
        scenarioData.routesCount = scenarioTextFile.Count - routesStartAtLine;
        for (int i = 1; i < hotspotTextFile.Count; i++)   // List of all URL
        {
            scenarioData.hotspotsURL.Add(hotspotTextFile.ElementAt(i));
            //Debug.Log("Hotspots URL: " + string.Join("+", scenario1S0Data.hotspotsURL));
        }

        for (int i = routesStartAtLine; i < scenarioTextFile.Count; i++)   // List of all routes
        {
            scenarioData.routes.Add(scenarioTextFile.ElementAt(i));
            //Debug.Log("Scenario 1 - routesToDraw: " + string.Join("+", scenario1S0Data.routesToDraw));
        }

        return scenarioData;
    }

    private Scenario6Data ImportScenario6Data(List<string> textFile, int limitatorsStartAtLine)
    {
        Scenario6Data scenarioData = new Scenario6Data();
        scenarioData.limiters = new List<string>(); //List that will hold the limitator coordinates

        scenarioData.maxAttempts = int.Parse(textFile.ElementAt(0)); // The first element holds the number of attempts
        scenarioData.maxValidations = int.Parse(textFile.ElementAt(1)); //The second element hols the number of validations
        scenarioData.limitatorsCount = textFile.Count - limitatorsStartAtLine; //Calculates the number of limitators configurations
        for (int i = limitatorsStartAtLine; i < textFile.Count; i++) // List of all limiters
        {
            scenarioData.limiters.Add(textFile.ElementAt(i));
        }
        Debug.Log("Inside ImportScenario6Data, " + scenarioData.limitatorsCount + " limitator strings imported");

        return scenarioData;

    }

    //PULLS DATA FROM LIST<STRING> AND STORES IT IN STRUCTURE (SCENARIOS 2 TO 5)
    private ScenarioStdData ImportScenarioStdData(List<string> textFile, int routesStartAtLine)
    {
        ScenarioStdData scenarioData = new ScenarioStdData();
        scenarioData.routes = new List<string>();

        //scenarioData.start = textFile.ElementAt(0); //Start coordinate string
        //scenarioData.startCoord = routeManager.SplitCoordinates(scenarioData.start); //Start coordinates (coord, dir)
        scenarioData.maxAttempts = int.Parse(textFile.ElementAt(0));
        scenarioData.maxValidations = int.Parse(textFile.ElementAt(1));
        scenarioData.routesCount = textFile.Count - routesStartAtLine;   //# of routes in scenario
        for (int i = routesStartAtLine; i < textFile.Count; i++)   // List of all routes
        {
            scenarioData.routes.Add(textFile.ElementAt(i));
        }
        //Debug.Log(textFile + "scenario - routes: " + string.Join("+", scenarioData.routes));

        return scenarioData;
    }

    private void SetAttemptsValidationLimits()
    {
        // Set attempts limit
        if (activeScenario.maxAttempts == 0)
        {
            attemptsLimited = false;
        }
        else
        {
            attemptsLimited = true;
        }

        // Set validation limit
        if (activeScenario.maxValidations == 0)
        {
            validationsLimited = false;
        }
        else
        {
            validationsLimited = true;
        }
//        Debug.Log("Attempts limited: " + attemptsLimited + ". Validations limited: " + validationsLimited);
    }

    //IMPORT TEXT FROM .TXT FILE PER LINE AND REMOVES COMMENTS BEFORE '*' SEPARATOR
    private List<string> ImportText(string fileName)
    {
        List<string> txtImport = new List<string>(System.IO.File.ReadAllLines(importPath + fileName));  //Import the text from the fileName

        //Remove comments in the format #Comment:Data
        List<string> txtImportNoComments = new List<string>();
        for (int i = 0; i < txtImport.Count(); i++)
        {
            List<string> temp = txtImport[i].Split(textFilesCommentSeparator).ToList();
            txtImportNoComments.Add(temp[1]);
        }

        return txtImportNoComments; //Returns the import with comments removed
    }

    // IMPORT SCENARIO DATA FROM TEXT FILES AND STORES THEM IN A LIST<STRING>
    private void ImportAllTextFiles()
    {
        //Scenario 1 - Sequence 0
        scenario1S0TextFile = ImportText("Scenario1-S0.txt"); 
        checkpointManager.checkpointsTextS0 = ImportText("Checkpointstext-S0.txt");
        checkpointManager.checkpointsInstructionsS0 = ImportText("Checkpointsinstructions-S0.txt");
        hotspotManager.hotspotTextS0 = ImportText("Hotspots-S0.txt");

        //Scenario 1 - Sequence 6
        scenario1S6TextFile = ImportText("Scenario1-S6.txt");
        checkpointManager.checkpointsTextS6 = ImportText("Checkpointstext-S6.txt");
        checkpointManager.checkpointsInstructionsS6 = ImportText("Checkpointsinstructions-S6.txt");
        hotspotManager.hotspotTextS6 = ImportText("Hotspots-S6.txt");

        //Scenarion 2 - 7
        scenario2TextFile = ImportText("Scenario2.txt");
        scenario3TextFile = ImportText("Scenario3.txt");
        scenario4TextFile = ImportText("Scenario4.txt");
        scenario5TextFile = ImportText("Scenario5.txt");
        scenario6TextFile = ImportText("Scenario6.txt");
        scenario7TextFile = ImportText("Scenario7.txt");
    }

    // HOLDS DATA FOR SCENARIO 1
    public struct Scenario1Data
    {
        public string start;
        public List<string> startCoord;
        public List<string> checkpoints;
        public List<string> hotspots;
        public List<string> hotspotsURL;
        public int routesCount;
        public List<string> routes;
    }

    // HOLDS DATA FOR SCENARIOS 2 to 5
    public struct ScenarioStdData
    {
        public int maxAttempts;
        public int maxValidations;
        public int routesCount;
        public List<string> routes;
    }

    // HOLDS DATA FOR SCENARIOS 2 to 5
    public struct Scenario6Data
    {
        public int maxAttempts;
        public int maxValidations;
        public int limitatorsCount;
        public List<string> limiters;
    }
}
