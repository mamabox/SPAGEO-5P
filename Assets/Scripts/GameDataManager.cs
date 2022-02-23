using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class GameDataManager : MonoBehaviour
{
    private string gameDataFile;
    private string importPath;
    private GameManager gameManager;
    private GameData tempGameData = new GameData();



    void Awake()
    {
        //Initialisation
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        importPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Media/Text/");
        gameDataFile = importPath + "gameData.json";
ImportGameData(gameDataFile);

    }


    //Import data from text file using JSON utilitu and save it to gameManager.scenariosData
    [ContextMenu("Import Game Data")]
    public void ImportGameData(string dataFile)
    {
        if (File.Exists(dataFile))
        {
            Debug.Log(gameDataFile + " exists");
            string fileContents = File.ReadAllText(dataFile);                   // Read the entire file and save its contents.
            gameManager.gameData = JsonUtility.FromJson<GameData>(fileContents);                // Deserialize the JSON data into a pattern matching the GameData class.
        }
        else
        {
            Debug.Log(gameDataFile + " does not exist");
        }
    }
}