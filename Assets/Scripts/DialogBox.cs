using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

/**
 * DialogBox.cs
 * 
 * 
 * 
 * 
 **/

public class DialogBox : MonoBehaviour
{
    private GameManager gameManager;
    private CheckpointManager checkpointManager;
    public TextMeshProUGUI instructionsText;
    public Button dialogBoxBtn;
    public string text;
    public bool firstDialog = true;
    public bool returnToMenu = false;
    public bool validateScene = false;
    public string nextAction;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        checkpointManager = FindObjectOfType<GameManager>().GetComponent<CheckpointManager>();
    }

    // Start is called before the first frame update
    void Update()
    {

    }

    // Update is called once per frame
    public void OnButtonClick()
    {

        if (firstDialog)    // If this is the first time the dialog box opens, show checkpoint information
        {
            Debug.Log("isFirstDialog, selectedRoute: " + gameManager.sessionData.selectedRoute);
            if (gameManager.sessionData.selectedRoute == 0)
            {
                instructionsText.text = checkpointManager.checkpointsInstructionsS0.ElementAt(checkpointManager.lastCheckpointCollected - 1);
            }
            else
            {
                instructionsText.text = checkpointManager.checkpointsInstructionsS6.ElementAt(checkpointManager.lastCheckpointCollected - 1);
            }
            //Time.timeScale = 0; // Stop the time
            
            //dialogBoxBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Fermer";  //Change the button's text
            firstDialog = false;
        }
        else    // If player clicked to continue
        {
            //Time.timeScale = 1; //Time starts up again
            gameManager.freezePlayer = false;
            gameObject.SetActive(false);
            //dialogBoxBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Continuer";   //Reset the button's text
            firstDialog = true;

            if (returnToMenu)   //On click, player returns to the menu
            {
                returnToMenu = false;
                Debug.Log("returnToMenu: " + returnToMenu);

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