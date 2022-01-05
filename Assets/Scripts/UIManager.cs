using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;  //for string
using UnityEngine.UI; //for button
using UnityEngine.EventSystems;
using System.Linq;

public class UIManager : MonoBehaviour
{
    // Components
    private GameObject player;
    private GameManager gameManager;
    private IntersectionManager intersectionManager;
    private RouteManager routeManager;
    private PlayerController playerController;
    private EventSystem eventSystem;


    // UI Elements
    public TextMeshProUGUI routeText;
    public TextMeshProUGUI routeDirText;
    public TextMeshProUGUI routeValidationText;
    public TextMeshProUGUI sequenceInformationText;
    public TextMeshProUGUI sessionStatusText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI rotationText;
    public TextMeshProUGUI sessionTimeText;
    public TextMeshProUGUI sessionDistanceText;
    public TextMeshProUGUI attemptCountText;
    public TextMeshProUGUI attemptTimeText;
    public TextMeshProUGUI attemptDistanceText;
    public TextMeshProUGUI backStepForceText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI shorcutsText;
    public TextMeshProUGUI ptsotText;


    public TMP_InputField posXInputField;
    public TMP_InputField posYInputField;
    public TMP_InputField dirInputField;

    public Button goto_Btn;

    public Canvas debugCanvas;
    public Canvas gameCanvas;
    public GameObject hotspotDialogBox;
    public GameObject dialogBox;
    public GameObject dialogBoxCheckpoint;
    public GameObject startDialogBox;

    public string senderReceiverUI ="";


    // UI - Goto
    public string inputCoord;
    public int inputCoordX = 0;
    public int inputCoordY = 0;
    public float inputRot = 0;
    public string inputDir;

    public bool validationCheck = false;   //whether route is being validated - used for UI display only

    private void Awake()
    {
        // Initialise GameObjects and components
        player = GameObject.FindGameObjectWithTag("Player");
        intersectionManager = GetComponent<IntersectionManager>();
        routeManager = GetComponent<RouteManager>();
        playerController = player.GetComponent<PlayerController>();
        gameManager = GetComponent<GameManager>();

        // Initialise UI elements
        goto_Btn.onClick.AddListener(GotoBtnHandler);
        debugCanvas.enabled = false;  //Enable the UI
    }
        // Start is called before the first frame update
        void Start()
    {
        shorcutsText.text = "(START/S) Demarrer / Terminer - (D/X) Nouvelle Tentative - (C/ESPACE) Valider - (A/C) Enregistrer trajet - (B/ENTER) Fermer Fenetre - (M) Menu Debug";
    }

    // Update is called once per frame
    void Update()
    {
        //Postion and rotation
        positionText.text = "POS (X,Z) = (" + player.transform.position.x.ToString("F2") + "," + player.transform.position.z.ToString("F2") + ")";  //Position display as (X,Z)
        rotationText.text = "ROT (Y) = " + player.transform.rotation.eulerAngles.y.ToString("F2"); //Player rotation in euler angles with two digits

        //Route info
        routeText.text = "R : " + String.Join(gameManager.coordSeparator, intersectionManager.sessionRoute); // Recorded intersections
        routeDirText.text = "RD: " + String.Join(gameManager.coordSeparator, intersectionManager.sessionRouteDir);// Recorded intersections with entering direction

        //Session info
        sessionTimeText.text = "Total time: " + gameManager.sessionTimeElapsed.ToString(@"mm\:ss");
        sessionDistanceText.text = "Total distance: " + gameManager.sessionDistance.ToString("F2") + "m";

        //Attempt info
        attemptCountText.text = "Attempt #: "+ gameManager.attemptCount.ToString();
        attemptTimeText.text = "Time: " + gameManager.attemptTimeElapsed.ToString(@"mm\:ss");
        attemptDistanceText.text = "Distance: " + gameManager.attemptDistance.ToString("F2") + "m";

        backStepForceText.text = "Backstep Force (I & O): " + playerController.backStepForce;

        if (!validationCheck)    // If player did not ask for validation show validation shortcut
            routeValidationText.text = "Press (SPACEBAR) for validation";

        sequenceInformationText.text = gameManager.sessionSummaryText;

        timeText.text = Time.unscaledTime.ToString("F2");
    }

    // UI - STORE VALUES ENTERED IN GOTO FIELDS
    public void UpdateCoordinates()
    {
        if ((posXInputField.text != "") && (posYInputField.text != ""))
        {
            inputCoord = posXInputField.text + routeManager.xyCoordSeparator + posYInputField.text;
            inputCoordX = int.Parse(posXInputField.text);
            inputCoordY = int.Parse(posYInputField.text);
            inputDir = dirInputField.text;
        }
    }

    // UI - DISABLE KEYBOARD INPUT IF CURSOR IN INPUT FIELD - setup in Editor
    public void InputFieldActive(bool setting)
    {
        playerController.keyboardShortcutsEnabled = !setting;
    }

    // UI - GOTO BUTTON HANDLER
    void GotoBtnHandler()
    {
        Debug.Log("Inside button handler");
        UpdateCoordinates();
        intersectionManager.GotoCoord(inputCoord, inputDir);
    }

    //TODO: Move to UIManager
    public void OpenDialogBox(string text)
    {

        gameManager.freezePlayer = true;
        dialogBox.GetComponent<DialogBox>().instructionsText.text = text; //send the checkpoitn text to the dialog box
        dialogBox.GetComponent<DialogBox>().firstDialog = false;
        dialogBox.gameObject.SetActive(true);

        dialogBox.GetComponent<DialogBox>().dialogBoxBtn.Select();
        dialogBox.GetComponent<DialogBox>().dialogBoxBtn.OnSelect(null);

    }

    // text = text to display; showHidebutton = hide (false) or show (true) button)
    public void OpenCheckpointDialogBox(string text, bool showHideButton)
    {

        //gameManager.freezePlayer = true;
        dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().instructionsText.text = text.Replace("|", System.Environment.NewLine); //send the checkpoitn text to the dialog box
        dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().firstDialog = false;
        dialogBoxCheckpoint.gameObject.SetActive(true);
        dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.gameObject.SetActive(showHideButton);

        //dialogBox.GetComponent<DialogBox>().dialogBoxBtn.Select();
        //dialogBox.GetComponent<DialogBox>().dialogBoxBtn.OnSelect(null);

        ////Display information UI
        //gameManager.freezePlayer = false;
        //if (gameManager.sessionData.selectedRoute == 0) // If séance 4P - S0
        //{
        //    uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().instructionsText.text = checkpointsTextS0.ElementAt(checkpoint.ID - 1).Replace("|", System.Environment.NewLine); //send the checkpoitn text to the dialog box
        //}
        //else // If séance 4P - S6
        //{
        //    uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().instructionsText.text = checkpointsTextS6.ElementAt(checkpoint.ID - 1).Replace("|", System.Environment.NewLine); //send the checkpoitn text to the dialog box
        //}

        //uiManager.dialogBoxCheckpoint.gameObject.SetActive(true);
        //uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.Select();
        //uiManager.dialogBoxCheckpoint.GetComponent<CheckpointDialogBox>().dialogBoxBtn.OnSelect(null);

    }

}
