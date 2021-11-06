using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;
using System.IO;
//using UnityEngine.InputSystem.Utilities;
using System.Linq;
using UnityEngine.SceneManagement; // for restart scene
using System; //for Math.Round
using UnityEngine.UI;

/**
 * SIMPLE PLAYER CONTROLLER
 * 
 * horizontal input = move
 * vertical input = rotate
 * 
 */

public class PlayerControllerOld : MonoBehaviour
{
    public float speed = 6.0f; //Player's walking speed
    public float lookSpeed = 50.0f; //Player's turning speed
    public float backStepForce = 2;
    public float backStepDuration = .25f;
    public float elapsedTime;

    //public string screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), "Exports/Screenshots/");
    //public string trackMovementPath = Path.Combine(Directory.GetCurrentDirectory(), "Exports/TrackMovements/");

    //private readonly int xRange = 350; // Ground plane size (x-axis) * 10
    //private readonly int yRange = 350; // Ground plane size (y-axis) * 10

    private Camera playerCamera; //Needed?
    private Rigidbody playerRb;
    private GameManager gameManager;
    private UIManager uiManager;
    private IntersectionManager intersectionManager;
    private RouteManager routeManager;
    private ScreenshotManager screenshotManager;
    private CheckpointManager checkpointManager;
    private SequenceManager sequenceManager;
    private HotspotManager hotspotManager;
    private SaveSessionData saveSessionData;

    public Vector3 startPosition; //Used to reset to initial position
    public Vector3 startRotation; //User to reset to initial rotatin
    public Vector3 lastPosition;

    //public Vector3 backstepPosition;

    public Vector2 inputVec;
    public float[] startCoord;
    public float[] lastIntersection; //coordinate of the last intersection the player went through - MOVE TO intersectionmanager.cs
    public string cardinalDirection; //MOVE TO intersectionmanager.cs

    public Vector3 currentRotation; //Player's current rotation
    private Vector3 moveVec;

    public float horizontalInput; //Value of horizontal input
    public float verticalInput; //Value of vertical input

    public bool tookStep = false;

    public bool keyboardShortcutsEnabled = true;    //disable the keyboard when entering data in the goto field

    public bool playerHasMoved = false;
    //public bool cameraTilt = true;

    private void Awake()
    {
        // _controls = new PlayerControl();
        playerCamera = Camera.main; //Set playerCamera to camera with 'main'tag 
        playerRb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        intersectionManager = FindObjectOfType<GameManager>().GetComponent<IntersectionManager>();
        uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        sequenceManager = FindObjectOfType<GameManager>().GetComponent<SequenceManager>();
        routeManager = FindObjectOfType<GameManager>().GetComponent<RouteManager>();
        screenshotManager = FindObjectOfType<GameManager>().GetComponent<ScreenshotManager>();
        checkpointManager = FindObjectOfType<GameManager>().GetComponent<CheckpointManager>();
        hotspotManager = FindObjectOfType<GameManager>().GetComponent<HotspotManager>();
        saveSessionData = FindObjectOfType<GameManager>().GetComponent<SaveSessionData>();
    }

    public void InitialisePlayer()
    {

        intersectionManager.GotoCoord(gameManager.sessionData.routeStart.ElementAt(0), gameManager.sessionData.routeStart.ElementAt(1));  //Place player at start of route

        //Record start position and rotation
        startPosition = transform.position;
        startRotation = currentRotation; //curentRottion is set in GotoCoord
        transform.eulerAngles = startRotation; //Set player to current rotation

        lastPosition = startPosition;   //used to calculate distance travelled
        

        //lastCoord = (transform.position.x / gameManager.blockSize).ToString("F2");
        startCoord = new float[] { (transform.position.x / gameManager.blockSize), (transform.position.z / gameManager.blockSize) };
        //lastIntersection = startCoord; //restore insted of below
        lastIntersection = new float[] { (float)System.Math.Round(startCoord[0], 1), (float)System.Math.Round(startCoord[1], 1) }; //temporay - only used to simplify UI display

    }

    //PLAYER INPUT - MOVEMENT
    void FixedUpdate()
    {

        if (!gameManager.freezePlayer)
        {

            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            if (verticalInput > 0)
            {
                transform.Translate(Vector3.forward * verticalInput * Time.deltaTime * speed);
                tookStep = false;
                playerHasMoved = true;
            }
            else if (verticalInput < 0 && !tookStep)
            {
                StartCoroutine(StepBack());
                tookStep = true;
                //playerRb.AddRelativeForce(Vector3.back * backwardsStepForce, ForceMode.Impulse);
                //playerHasMoved = true;
            }

            currentRotation.y += horizontalInput * Time.deltaTime * lookSpeed;
            transform.eulerAngles = new Vector3(0, currentRotation.y, 0);
        }
        

    }

    IEnumerator StepBack()
    {
        elapsedTime = 0f;
        while (elapsedTime < backStepDuration)
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed * backStepForce);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    //PLAYER INPUT - KEYBOARD SHORTCUTS
    void Update()
    {
        if (keyboardShortcutsEnabled)   // if inputFields are not active
        {
            //Return to start position
            if (Input.GetKeyDown(KeyCode.Alpha0))    
            {
                transform.position = startPosition;
                currentRotation = startRotation;

                //ADD WAIT until you can move again

            }

            //TODO:VALIDEATE SELECTION (joystick)

            //END SESSION / RETURN TO MENU
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    gameManager.GotoMenu();
            //}

            ////Restart session
            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    gameManager.PauseGame();
            //}

            //QUIT APPLICATION
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //if (gameManager.sessionStarted)
                //    saveSessionData.stopSavingData();
                Application.Quit();
            }

            //Backwards force
            if (Input.GetKeyDown(KeyCode.I) && backStepForce > 1)
            {
                backStepForce -= .25f;
            }

            if (Input.GetKeyDown(KeyCode.O) && backStepForce < 5)
            {
                backStepForce += .25f;
            }


            //Location shortcuts - temporary for debugging
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                transform.position = new Vector3(0, 1, 0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                transform.position = new Vector3(0, 1, 245);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                transform.position = new Vector3(245, 1, 245);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                transform.position = new Vector3(245, 1, 105);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                transform.position = new Vector3(385, 1, 105);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                transform.position = new Vector3(385, 1, -140);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                transform.position = new Vector3(140, 1, -140);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                transform.position = new Vector3(140, 1, 0);
            }

            //Hide Canvas
            if (Input.GetKeyDown(KeyCode.M))
            {
                uiManager.debugCanvas.enabled = !uiManager.debugCanvas.enabled;
            }

            //Button active
            if (Input.GetKeyDown(KeyCode.B))
            {
                checkpointManager.ClickCheckpointBtn();
            }

            //Record route
            if (Input.GetKeyDown(KeyCode.C))
            {
                gameManager.RecordRoute();
            }

            //Take screenshot
            if (Input.GetKeyDown(KeyCode.P))
            {
                screenshotManager.TakeScreenshot();
            }

            //VALIDATION
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameManager.CheckValidation();  
            }

            //NEW ATTEMPT
            if (Input.GetKeyDown(KeyCode.X))
            {
                //Debug.Log("Session status changed (Started or Ended)");
                //Debug.Log("session Route cleared");
                gameManager.NewAttempt(true);
            }

            //START SESSION
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (!gameManager.sessionStarted)
                {
                    gameManager.StartSession();
                }
                else
                {
                    gameManager.EndSession(true);
                }
            }
        }
    }

    // WHEN PLAYER ENTERS AN INTERSECTION OR WALKS ON HOTSPOT
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Intersection")
        {
            intersectionManager.OnIntersectionEnter(other);
        }
        else if (other.tag == "Checkpoint")
        {
            checkpointManager.OnCheckpointEnter(other);
        }
        else if (other.tag == "Hotspot")
        {
            hotspotManager.OnHotspotEnter(other);
        }
        //Debug.Log(other.tag + " IN ()");
    }

    // WHEN PLAYER LEAVES AN INTERSECTION OR A HOTSPOT
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Intersection")
        {
            intersectionManager.OnIntersectionExit(other);
        }
        else if (other.tag == "Checkpoint")
        {
            checkpointManager.OnCheckpointExit(other);
        }
        else if (other.tag == "Hotspot")
        {
            hotspotManager.OnHotspotExit(other);
        }
        //Debug.Log(other.tag + " OUT ()");
       // Debug.Log(other.tag + " OUT (" + other.GetComponent<Hotspot>().coordString + ")");
    }
}
