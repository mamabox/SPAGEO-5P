using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Filename.cs
 * 
 * Handles the menu selection
 * Dropdown elements are cleared and updated depending on selection
 * 
 **/

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown routeDropDown;
    public TextMeshProUGUI sessionSummary;
    public GameObject sendReceiveToggleGroup;

    static int menuScenarioSelection;
    static int menuRouteSelection;
    private List<int> allScenarioRouteCounts; //TODO: Initialise

    // SET DEFAULT VALUES FOR MENU SELECTION
    private void Awake()
    {
        InitialiseMenuValues();
    }

    // Update is called once per frame
    private void Update()
    {
        UIUpdate();
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Debug.Log("Sequence selected: " + menuSequenceSelection);
        //Debug.Log("Route selected: " + (menuRouteSelection + 1));

        GlobalControl.instance.sessionData.selectedScenario = menuScenarioSelection; //Set as default if nothing is selected
        GlobalControl.instance.sessionData.selectedRoute = menuRouteSelection;

        PopulateDropdownRoutes(routeDropDown, 1, 0);    //Populate the route selection dropdown for the default scenario (#1)
    }

    private void InitialiseMenuValues()
    {
        sendReceiveToggleGroup.SetActive(false);
        GlobalControl.instance.sessionData.isSender = true;
        menuScenarioSelection = 1;
        menuRouteSelection = 0;
    }

    // Update UI with group or student ID information
    private void UIUpdate()
    {
        if (GlobalControl.instance.sessionData.isGroupSession)
        {
            sessionSummary.text = "Groupe: " + GlobalControl.instance.sessionData.groupID + " - Élève(s): " + string.Join(",", GlobalControl.instance.sessionData.studentIDs);
        }
        else
        {
            sessionSummary.text = "Élève(s): " + string.Join(",", GlobalControl.instance.sessionData.studentIDs);
        }
    }

    // Populate the route selection dropdown for routeCounte # of route for scenario # scenarioID 
    private void PopulateDropdownRoutes (TMP_Dropdown dropdown, int scenarioID, int routeCount)
    {
        List<string> routesToAdd = new List<string>();

        if (scenarioID == 1)
        {
            routesToAdd = new List<string>{ "Séquence 0", "Séquence 6"};
        }

        else if (scenarioID == 5)
        {
            routesToAdd.Add("Pas de route");
            for (int i = 0; i < routeCount; i++)
            {
                routesToAdd.Add("Route" + scenarioID + "-" + (i + 1));
            }
        }
        else if (scenarioID == 6)
        {
            for (int i = 0; i < routeCount; i++)
            {
                routesToAdd.Add("Barrière-" + (i + 1));
            }
        }
        else if (scenarioID == 8 || scenarioID == 9)
        {
            routesToAdd.Add("WIP");
        }
        else // Scenarios 2 - 5
        {
            for (int i = 0; i < routeCount; i++)
            {
                routesToAdd.Add("Route" + scenarioID + "-" + (i + 1));
            }
        }    
        

        routeDropDown.AddOptions(routesToAdd);  //Adds the options to the dropdown

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
    
    // Called from Main Menu > Scenario dropdown on value changed
    public void HandleScenarioSelection (int selection)
    {
        routeDropDown.ClearOptions();   // Clear the options of the dropdown menu
        senderReceiverVisibility(selection);    // Handles sender/receiver toggle button visibility

        if (selection == 0)
        {
            menuScenarioSelection = 1;
            PopulateDropdownRoutes(routeDropDown, 1, 0);

        }
        else if (selection == 1)
        {
            menuScenarioSelection = 2;
            PopulateDropdownRoutes(routeDropDown, 2, 4);
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
        else if (selection == 5)
        {
            menuScenarioSelection = 6;
            PopulateDropdownRoutes(routeDropDown, 6, 4);    // Trigger scenario 6 with no added routes
        }
        else if (selection == 6)
        {
            menuScenarioSelection = 7;
            PopulateDropdownRoutes(routeDropDown, 7, 8);    // Trigger scenario 6 with no added routes
        }
        else if (selection == 7)
        {
            menuScenarioSelection = 8;
            PopulateDropdownRoutes(routeDropDown, 8, 0);    // Trigger scenario 6 with no added routes
        }
        else if (selection == 8)
        {
            menuScenarioSelection = 9;
            PopulateDropdownRoutes(routeDropDown, 9, 0);    // Trigger scenario 6 with no added routes
        }

        //Debug.Log("Sequence selected: " + menuScenarioSelection);
        //Debug.Log("Route selected: " + (menuRouteSelection));
        GlobalControl.instance.sessionData.selectedScenario = menuScenarioSelection;    //TODO: change to selection + 1
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

    // Hide or show the sender/receiver toggle buttons for the specific scenarios
    private void senderReceiverVisibility(int selection)
    {
        if (selection == 1)
            sendReceiveToggleGroup.SetActive(true);
        else
            sendReceiveToggleGroup.SetActive(false);
    }


}
