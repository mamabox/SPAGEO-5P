using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
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

public class PlayerController : MonoBehaviour
{
    public float speed = 6.0f; //Player's walking speed
    public float lookSpeed = 50.0f; //Player's turning speed
    public float backStepForce = 2;
    public float backStepDuration = .25f;
    public float elapsedTime;


    private Camera playerCamera; //Needed?
    public Rigidbody playerRb;
    private GameManager gameManager;
    private UIManager uiManager;
    private IntersectionManager intersectionManager;
    private RouteManager routeManager;
    private ScreenshotManager screenshotManager;
    private CheckpointManager checkpointManager;
    private SequenceManager sequenceManager;
    private HotspotManager hotspotManager;
    private SaveSessionData saveSessionData;
    public Collider lastIntersectionEnteredCollider;

    public Vector3 startPosition; //Used to reset to initial position
    public Vector3 startRotation; //User to reset to initial rotatin
    public Vector3 lastPosition;

    //public Vector3 backstepPosition;
    //public Vector2 inputVec;

    public float[] startCoord;
    public float[] lastIntersection; //TODO:MOVE TO intersectionmanager.cs - coordinate of the last intersection the player went through
    public string cardinalDirection; //TODO:MOVE TO intersectionmanager.cs

    public Vector3 currentRotation; //Player's current rotation

    public float horizontalInput; //Value of horizontal input
    public float verticalInput; //Value of vertical input
    private float moveInput;
    private float rotateInput;

    public bool tookStep = false;   //Has taken backwards step?
    public bool keyboardShortcutsEnabled = true;    //disable the keyboard when entering data in the goto field
    public bool playerHasMoved = false; //Has the player moved since start of the session

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
        lastIntersection = startCoord; //TODO:restore insted of below
        Debug.Log("Last intersection initialized to: " + String.Join(",", lastIntersection));
        //lastIntersection = new float[] { (float)System.Math.Round(startCoord[0], 1), (float)System.Math.Round(startCoord[1], 1) }; //temporay - only used to simplify UI display


    }

    void FixedUpdate()
    {
        if (!gameManager.freezePlayer)
        {
            //MOVEMENT
            //horizontalInput = Input.GetAxis("Horizontal");
            //verticalInput = Input.GetAxis("Vertical");

            if (moveInput > 0)  //Move forward
            {
                transform.Translate(Vector3.forward * moveInput * Time.deltaTime * speed);
                tookStep = false;
                playerHasMoved = true;
            }
            else if (moveInput < 0 && !tookStep)    //Move backwards
            {
                StartCoroutine(StepBack());
                tookStep = true;
                playerHasMoved = true;
            }

            //LOOK AROUND
            currentRotation.y += rotateInput * Time.deltaTime * lookSpeed;
            transform.eulerAngles = new Vector3(0, currentRotation.y, 0);
        }
    }

    IEnumerator StepBack()  //Only one step back allowed before moving forward
    {
        elapsedTime = 0f;
        while (elapsedTime < backStepDuration)  //Move back during a set amount of time
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed * backStepForce);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    //PLAYER INPUT - MOVEMENT AND ROTATION

    void OnMove(InputValue input)
    {
        moveInput = input.Get<float>();
    }
    void OnRotate(InputValue input)
    {
        rotateInput = input.Get<float>();
   
    }

    //PLAYER INPUT - KEYBOARD SHORTCUTS
    void OnValidateRoute()
    {
        if (keyboardShortcutsEnabled)
        {
            Debug.Log("OnValidate");
            gameManager.CheckValidation();
        }
    }

    void OnNewAttempt()
    {
        if (keyboardShortcutsEnabled)
        {
            gameManager.NewAttempt(true);
        }
    }

    void OnStartEndSession()
    {
        //if (keyboardShortcutsEnabled)
        //{
        if (!gameManager.sessionStarted)
        {
            gameManager.StartSession();
        }
        else
        {
            gameManager.EndSession(true);
        }
        //}
    }

    void OnClose()
    {
        if (keyboardShortcutsEnabled)
        {
            //checkpointManager.ClickCheckpointBtn();
            if (uiManager.hotspotDialogBox.gameObject.activeInHierarchy)
            {
                Debug.Log("Hotsport active");
                //uiManager.hotspotDialogBox.GetComponent<HotspotDialogBox>().dialogBoxBtn.Select();
                //uiManager.hotspotDialogBox.GetComponent<HotspotDialogBox>().dialogBoxBtn.OnSelect(null);
                //uiManager.hotspotDialogBox.GetComponent<HotspotDialogBox>().OnButtonClick();
            }
            else if (uiManager.dialogBoxCheckpoint.gameObject.activeInHierarchy)
            {
                Debug.Log("Checkpoint active");
                uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.Select();
                uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.OnSelect(null);
            }
            else if (uiManager.dialogBox.gameObject.activeInHierarchy)
            {
                Debug.Log("Dialog box actice");
                uiManager.dialogBox.GetComponent<DialogBox>().dialogBoxBtn.Select();
                uiManager.dialogBox.GetComponent<DialogBox>().dialogBoxBtn.OnSelect(null);
            }
            //checkpointManager.ClickCheckpointBtn();
        }
    }

    void OnRecordRoute()
    {
        if (keyboardShortcutsEnabled)
        {
            gameManager.RecordRoute();
        }
    }

    void OnShowHideDebug()
    {
        //Debug.Log("ShowHideDebug() " + keyboardShortcutsEnabled);
        if (keyboardShortcutsEnabled)
        {
            //Debug.Log("toggle");
            uiManager.debugCanvas.enabled = !uiManager.debugCanvas.enabled;
        }
    }

    private void OnQuit()
    {
        if (keyboardShortcutsEnabled)
        {
            Application.Quit();
        }
    }

    void OnTakeScreenshot()
    {
        screenshotManager.TakeScreenshot();
    }

    void Update()
    {
        //    if (keyboardShortcutsEnabled)   // Keyboard shorcuts enable only if inputFields are not active
        //    {

        //        //QUIT APPLICATION
        //        if (Input.GetKeyDown(KeyCode.Escape))
        //        {
        //            //if (gameManager.sessionStarted)
        //            //    saveSessionData.stopSavingData();
        //            Application.Quit();
        //        }

        //        //MODIFY STRENGTH OF BACK STEP
        //        //if (Input.GetKeyDown(KeyCode.I) && backStepForce > 1)
        //        //{
        //        //    backStepForce -= .25f;
        //        //}

        //        //if (Input.GetKeyDown(KeyCode.O) && backStepForce < 5)
        //        //{
        //        //    backStepForce += .25f;
        //        //}


        //        //LOCATION SHORTCUTS (DEV ONLY)
        //        //if (Input.GetKeyDown(KeyCode.Alpha0))               //Return to start position
        //        //{
        //        //    transform.position = startPosition;
        //        //    currentRotation = startRotation;
        //        //}
        //        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //        //{
        //        //    transform.position = new Vector3(0, 1, 0);
        //        //}
        //        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //        //{
        //        //    transform.position = new Vector3(0, 1, 245);
        //        //}
        //        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //        //{
        //        //    transform.position = new Vector3(245, 1, 245);
        //        //}
        //        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //        //{
        //        //    transform.position = new Vector3(245, 1, 105);
        //        //}
        //        //if (Input.GetKeyDown(KeyCode.Alpha5))
        //        //{
        //        //    transform.position = new Vector3(385, 1, 105);
        //        //}
        //        //if (Input.GetKeyDown(KeyCode.Alpha6))
        //        //{
        //        //    transform.position = new Vector3(385, 1, -140);
        //        //}
        //        //if (Input.GetKeyDown(KeyCode.Alpha7))
        //        //{
        //        //    transform.position = new Vector3(140, 1, -140);
        //        //}
        //        //if (Input.GetKeyDown(KeyCode.Alpha8))
        //        //{
        //        //    transform.position = new Vector3(140, 1, 0);
        //        //}

        ////SHOW HIDE DEBUG CANVAS
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    uiManager.debugCanvas.enabled = !uiManager.debugCanvas.enabled;
        //}

        //Button active
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            if (uiManager.hotspotDialogBox.gameObject.activeInHierarchy)
            {
                Debug.Log("Pressed in button!");
                uiManager.hotspotDialogBox.GetComponent<HotspotDialogBox>().dialogBoxBtn.Select();
                uiManager.hotspotDialogBox.GetComponent<HotspotDialogBox>().dialogBoxBtn.OnSelect(null);
            }
        }

        //        //RECORD ROUTE
        //        if (Input.GetKeyDown(KeyCode.C))
        //        {
        //            gameManager.RecordRoute();
        //        }

        //        //TAKE SCREENSHOT
        //        if (Input.GetKeyDown(KeyCode.P))
        //        {
        //            screenshotManager.TakeScreenshot();
        //        }

        //        //VALIDATE PATH
        //        if (Input.GetKeyDown(KeyCode.Space))
        //        {
        //            gameManager.CheckValidation();  
        //        }

        //        //NEW ATTEMPT
        //        if (Input.GetKeyDown(KeyCode.X))
        //        {
        //            gameManager.NewAttempt(true);
        //        }

        //        //START SESSION
        //            if (Input.GetKeyDown(KeyCode.S))
        //            {
        //                if (!gameManager.sessionStarted)
        //                {
        //                    gameManager.StartSession();
        //                }
        //                else
        //{
        //    gameManager.EndSession(true);
        //}
        //            }
    }
//    }

    // WHEN PLAYER ENTERS AN INTERSECTION, CHECKPOINT OR HOTSPOT
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Intersection")
        {
            intersectionManager.OnIntersectionEnter(other);
            lastIntersectionEnteredCollider = other;
        }
        else if (other.tag == "Checkpoint")
        {
            checkpointManager.OnCheckpointEnter(other);
        }
        else if (other.tag == "Hotspot")
        {
            hotspotManager.OnHotspotEnter(other);
        }
        //Debug.Log("IN " + other.tag");
    }

    // WHEN PLAYER LEAVES AN INTERSECTION, CHECKPOINT OR HOTSPOT
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
        Debug.Log("OUT " + other.tag);
    }
}
