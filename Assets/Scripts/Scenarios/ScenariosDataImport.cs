using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class ScenariosDataImport: MonoBehaviour
{
    private string scenarioDataFile;
    private string importPath;
    private GameManager gameManager;
    private ScenariosData tempScenariosData = new ScenariosData();
    private int nonStdScNb;

    private SequenceManager scenarioManager;

    //public ScenariosData scenariosData;


    void Awake()
    {
        //Initialisation

        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        scenarioManager = gameManager.GetComponent<SequenceManager>();
        importPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Media/Text/");
        scenarioDataFile = importPath + "scenariosData.json";
        nonStdScNb = 4; //Number of non standard scenarios: Sc1,Sc6, Sc8, Sc9
        //scenariosData = new ScenariosData();


        ImportScenarioData(scenarioDataFile);

    }


    //Import data from text file using JSON utilitu and save it to gameManager.scenariosData
    [ContextMenu("Import Scenario Data")]
    public void ImportScenarioData(string dataFile)
    {
        if (File.Exists(dataFile))
        {
            Debug.Log(scenarioDataFile + " exists");
            string fileContents = File.ReadAllText(dataFile);                   // Read the entire file and save its contents.
            gameManager.scenariosData = JsonUtility.FromJson<ScenariosData>(fileContents);                // Deserialize the JSON data into a pattern matching the GameData class.
            gameManager.scenariosData.scenariosNb = gameManager.scenariosData.stdScData.Count() + nonStdScNb;
            //int scnNb = Object.gameManager.scenariosData).leng
            BuilDropdownMenuItems();

        }
        else
        {
            Debug.Log(scenarioDataFile + " does not exist");
        }

        //scenarioManager.scenario7Data = scenarioManager.ImportScenarioStdDataJson(7);
    }


    private void BuilDropdownMenuItems()
    {
        string[] dropdownMenuItems = new string[gameManager.scenariosData.scenariosNb];


        dropdownMenuItems[gameManager.scenariosData.sc1Data.scenarioID - 1] = gameManager.scenariosData.sc1Data.dropdownMenuText; //Add description to menu list
        dropdownMenuItems[gameManager.scenariosData.sc6Data.scenarioID - 1] = gameManager.scenariosData.sc6Data.dropdownMenuText; //Add description to menu list
        dropdownMenuItems[gameManager.scenariosData.sc8Data.scenarioID - 1] = gameManager.scenariosData.sc8Data.dropdownMenuText; //Add description to menu list
        dropdownMenuItems[gameManager.scenariosData.sc9Data.scenarioID - 1] = gameManager.scenariosData.sc9Data.dropdownMenuText; //Add description to menu list

        //gameManager.scenariosData.
        for (int x = 0; x < gameManager.scenariosData.stdScData.Count(); x++)
        {
            dropdownMenuItems[gameManager.scenariosData.stdScData[x].scenarioID - 1] = gameManager.scenariosData.stdScData[x].dropdownMenuText;
        }

        Debug.Log("Dropdown menu items: " + String.Join(",",dropdownMenuItems));

    }
}
