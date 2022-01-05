using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class TextDataImport : MonoBehaviour
{
    private string textDataFileFR;
    private string importPath;
    private GameManager gameManager;
    private TextData _textData = new TextData();
    bool fileExists;

    void Awake()
    {
        fileExists = false;
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        //Set paths
        importPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Media/Text/");
        textDataFileFR = importPath + "textData_FR.json";
        
        //Import French (FR) textData
        ImportTextData(textDataFileFR);
        if (fileExists)
        {
            gameManager.textDataFR = _textData;
            fileExists = false;
        }
            
    }

    private void ImportTextData(string dataFile)
    {
        if (File.Exists(dataFile))
        {
            string fileContents = File.ReadAllText(dataFile); //Read the entire file and save its content
            _textData = JsonUtility.FromJson<TextData>(fileContents); //Deserialize the JSON data into a pattern matching the TextData class
            fileExists = true;
        }
        else
        {
            Debug.Log(textDataFileFR + " does not exist");
        }
    }

}