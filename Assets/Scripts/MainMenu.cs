using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown routeDropDown;
    public TextMeshProUGUI sessionSummary;
    public GameObject sendReceiveToggleGroup;
    
    static int menuScenarioSelection;
    static int menuRouteSelection;
    private List<int> allScenarioRouteCounts; //TODO: Initialise

    private void Awake()
    {
        sendReceiveToggleGroup.SetActive(false);
        GlobalControl.instance.sessionData.isSender = true;
        menuScenarioSelection = 1;
        menuRouteSelection = 0;

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
        
    }


    private void Start()
    {
        //Debug.Log("Sequence selected: " + menuSequenceSelection);
        //Debug.Log("Route selected: " + (menuRouteSelection + 1));


        GlobalControl.instance.sessionData.selectedScenario = menuScenarioSelection; //Set as default if nothing is selected
        GlobalControl.instance.sessionData.selectedRoute = menuRouteSelection;

        PopulateDropdownRoutes(routeDropDown, 1, 0);
    
    }

    private void PopulateDropdownRoutes (TMP_Dropdown dropdown, int scenarioID, int routeCount)
    {
        List<string> routesToAdd = new List<string>();

        if (scenarioID == 1)
        {
            routesToAdd = new List<string>{ "Séquence 0", "Séquence 6"};
        }

        if (scenarioID == 5)
        {
            routesToAdd.Add("Pas de route");
        }
        for (int i = 0; i < routeCount; i++)
        {
            routesToAdd.Add("Route Sc" + scenarioID + "-" + (i + 1));
        }

        routeDropDown.AddOptions(routesToAdd);

    }

    public void sendReceiverInput(bool val)
    {
        if (val == false)
        {
            Debug.Log("Sender");
            GlobalControl.instance.sessionData.isSender = true;
        }
        else
        {
            Debug.Log("Receiver");
            GlobalControl.instance.sessionData.isSender = false;
        }
    }

    public void HandleScenarioSelection (int selection)
    {
        routeDropDown.ClearOptions();
        senderReceiverVisibility(selection);

        if (selection == 0)
        {
            menuScenarioSelection = 1;
            PopulateDropdownRoutes(routeDropDown, 1, 0);

        }
        else if (selection == 1)
        {
            menuScenarioSelection = 2;
            PopulateDropdownRoutes(routeDropDown, 2, 2);
        }
        else if (selection == 2)
        {
            menuScenarioSelection = 3;
            PopulateDropdownRoutes(routeDropDown, 3, 6);
        }
        else if (selection == 3)
        {
            menuScenarioSelection = 4;
            PopulateDropdownRoutes(routeDropDown, 4, 6);
        }
        else if (selection == 4)
        {
            menuScenarioSelection = 5;
            PopulateDropdownRoutes(routeDropDown, 5, 8);
        }

        //Debug.Log("Sequence selected: " + menuScenarioSelection);
        //Debug.Log("Route selected: " + (menuRouteSelection));
        GlobalControl.instance.sessionData.selectedScenario = menuScenarioSelection;
    }

    public void RouteInput(int val)
    {
        //Debug.Log("Route selected = " + val);
        GlobalControl.instance.sessionData.selectedRoute = val;
    }

    public void GotoCity()
    {
        SceneManager.LoadScene("5P");
    }

    private void senderReceiverVisibility(int selection)
    {
        if (selection == 1)
            sendReceiveToggleGroup.SetActive(true);
        else
            sendReceiveToggleGroup.SetActive(false);
    }


}
