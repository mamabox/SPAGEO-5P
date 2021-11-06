using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
//using UnityEngine.EventSystems;

/**
 * CheckpointDialogBox.cs
 * 
 * Component of Game Canvas > Dialog Box Checkpoint
 * 
 * 
 */

public class CheckpointDialogBox : MonoBehaviour
{
    private GameManager gameManager;
    private CheckpointManager checkpointManager;
    public TextMeshProUGUI instructionsText;
    public Button dialogBoxBtn;
    //private EventSystem eventSystem;
    public string text;
    public bool firstDialog = true; //true if the dialog box has not appeared
    public bool returnToMenu = false;   //if true, return to menu after clicking
    public bool validateScene = false;  // flag to open the Validatio scene when the dialog box is closed
    public string nextAction;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        checkpointManager = FindObjectOfType<GameManager>().GetComponent<CheckpointManager>();
        //eventSystem = EventSystem.current;
    }

    // Start is called before the first frame update
    void Update()
    {

    }

    // Update is called once per frame
    public void OnButtonClick() //Configured in Inspector 
    {

        if (firstDialog)    // If this is the first time the dialog box opens, show checkpoint information
        {
            //Time.timeScale = 0; // Stop the time
            if (gameManager.sessionData.selectedRoute == 0) // if this this séance 4P-S0
            {
                instructionsText.text = checkpointManager.checkpointsInstructionsS0.ElementAt(checkpointManager.lastCheckpointCollected - 1).Replace("|", System.Environment.NewLine);
            }
            else // if this this séance 4P - S6
            {
                instructionsText.text = checkpointManager.checkpointsInstructionsS6.ElementAt(checkpointManager.lastCheckpointCollected - 1).Replace("|", System.Environment.NewLine);
            }
            
            //dialogBoxBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Fermer";  //Change the button's text
            firstDialog = false;   
        }
        else    // If player clicked to continue
        {
            //Time.timeScale = 1; //Time starts up again
            gameManager.freezePlayer = false;   
            gameObject.SetActive(false);    // hide the dialog box
            //dialogBoxBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Continuer";   //Reset the button's text
            firstDialog = true; //reinitialise value

            if (returnToMenu)   //On click, player returns to the menu
            {
                returnToMenu = false;

                SceneManager.LoadScene("Menu"); //Load scene called Game
            }

            if (validateScene) //On click, player goes to validaton screen
            {
                validateScene = false;
                SceneManager.LoadScene("Validation");
            }

        }
    }

    public void OnButtonClickNew()
    {
        //TODO: implement click scenarions
        //if nextAction = "checkpoint", "gotoMenu", "gotoValidation","close";
    }

}