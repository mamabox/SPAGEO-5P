using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HotspotDialogBox : MonoBehaviour
{
    private GameManager gameManager;
    public TextMeshProUGUI instructionsText;
    public Button dialogBoxBtn;
    public string url;
    private bool firstDialog = true;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    public void OnButtonClick()
    {
        if (firstDialog)    // If this is the first time the dialog box opens, show quizz information
        {
            Time.timeScale = 0; // Stop the time
            gameManager.gameIsPaused = true;
            //gameManager.PauseGame();
            Application.OpenURL(url);   //Open the hotspot URL
            dialogBoxBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Continuer";  //Change the button's text
            firstDialog = false;
        }
        else    // If player clicked to continue
        {
            Time.timeScale = 1; //Time starts up again
            //gameManager.PauseGame();
            gameManager.gameIsPaused = false;
            gameManager.freezePlayer = false;
            gameObject.SetActive(false);
            dialogBoxBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Quiz";   //Reset the button's text
            firstDialog = true;

        }

    }
}
