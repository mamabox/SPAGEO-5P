using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System;


public class ValidationMenu : MonoBehaviour
{
    private bool returnToMenu;
    public TextMeshProUGUI sessionSummary;
    public TextMeshProUGUI sessionImgSeqText;
    public TextMeshProUGUI routeImgSeqText;
    public GameObject selectedRouteImgSeq;
    public GameObject sessionRouteImgSeq;


    private void Awake()
    {
        returnToMenu = GlobalControl.instance.returnToMenu;
        //Debug.Log("ValidationMenu: " + returnToMenu);
    }

    private void Update()
    {
        if (GlobalControl.instance.sessionData.isGroupSession)
        {
            sessionSummary.text = "Groupe: " + GlobalControl.instance.sessionData.groupID + " - Élève(s): "+ string.Join(",", GlobalControl.instance.sessionData.studentIDs);
        }
        else
        {
            sessionSummary.text = "Élève(s): " + string.Join(",", GlobalControl.instance.sessionData.studentIDs);
        }


            sessionImgSeqText.text = "Session IMGs: " + String.Join("_", sessionRouteImgSeq.GetComponent<ImageSequence>().routeList);


            routeImgSeqText.text = "Route IMGs: " + String.Join("_", selectedRouteImgSeq.GetComponent<ImageSequence>().routeList);

    }

    public void GotoCity()
    {
        //Debug.Log("Go to City");
        GlobalControl.instance.sessionData.validationDone = true;
        //GlobalControl.instance.sessionData.sessionPaused = true;
        Debug.Log("VALIDATIONMENU(pause): " + GlobalControl.instance.sessionData.sessionPaused + "- returnTomenu: "+ returnToMenu);
        if (returnToMenu)
        {
            GlobalControl.instance.sessionData.sessionPaused = false;
            GlobalControl.instance.returnToMenu = false;
            SceneManager.LoadScene("Menu");
        }

        else
        {
            GlobalControl.instance.sessionData.sessionPaused = true;
            SceneManager.LoadScene("5P");
        }
         
 
    }

}
