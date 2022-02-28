using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * LiniterManager.cs
 * 
 * Generates limiters
 * 
 * 
 */

public class LimiterManager : MonoBehaviour
{
    private GameManager gameManager;
    private RouteManager routeManager;
    private IntersectionManager intersectionManager;
    private UIManager uiManager;
    private SaveSessionData saveSessionData;

    public GameObject limiterPrefab;
    public List<string> allLimiters;
    public List<Coordinate> allBarriers;

    void Awake()
    {
        // Initialise components
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        routeManager = FindObjectOfType<GameManager>().GetComponent<RouteManager>();
        intersectionManager = FindObjectOfType<GameManager>().GetComponent<IntersectionManager>();
        saveSessionData = GetComponent<SaveSessionData>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //GenerateLimiters(new List<string> { "5_5N", "5_4S", "5_3E","5_5W"});
    }

    public void GenerateLimiters(List<string> limiters)
    {
        string[] coordArray;
        char coordDir;

        Debug.Log("GENERATELIMITERS - " + limiters.Count() + " limiters: " + string.Join(",", limiters));

        for (int i = 0; i < limiters.Count(); i++)
        {
            coordDir = limiters[i].Last();
            string coordNoDir = limiters[i].Remove(limiters[i].Length - 1); //Remove the last character
            coordArray = coordNoDir.Split(char.Parse(routeManager.xyCoordSeparator)); //stores the coordinates x and y in an array
            var newCheckpoint = Instantiate(limiterPrefab, new Vector3(float.Parse(coordArray[0]) * gameManager.blockSize, 0.02f, float.Parse(coordArray[1]) * gameManager.blockSize) + transformOffset(coordDir), Quaternion.Euler(0, rotationOffset(coordDir), 0));

            //Quaternion.Euler(0, rotationOffset(coordDir, 0); // Sets camera to player's rotation + offset

            newCheckpoint.GetComponent<Limiter>().coordString = limiters[i];  //store the coordinates as a string in the instance
            newCheckpoint.GetComponent<Limiter>().ID = i + 1;    //stores the checkpoint ID(int) in the instance
            //newCheckpoint.GetComponent<Limiter>().rotation = newCheckpoint.transform.rotation.y;
        }
    }

    // GenerateBarriers replaces GenerateLimiters by using a list of Coordinates instead of a list of strings
    public void GenerateBarriers(List<Coordinate> barriers)
    {
        string[] coordArray;

        for (int i = 0; i < barriers.Count(); i++)
        {
            coordArray = barriers[i].coord.Split(char.Parse(routeManager.xyCoordSeparator)); //stores the coordinates x and y in an array
            var newCheckpoint = Instantiate(limiterPrefab, new Vector3(float.Parse(coordArray[0]) * gameManager.blockSize, 0.02f, float.Parse(coordArray[1]) * gameManager.blockSize) + transformOffsetStr(barriers[i].cardDir), Quaternion.Euler(0, rotationOffsetStr(barriers[i].cardDir), 0));

            //Quaternion.Euler(0, rotationOffset(coordDir, 0); // Sets camera to player's rotation + offset

            newCheckpoint.GetComponent<Limiter>().coordString = barriers[i].coord;  //store the coordinates as a string in the instance
            newCheckpoint.GetComponent<Limiter>().ID = i + 1;    //stores the checkpoint ID(int) in the instance
        }

    }

    //calculate offset based on cardinal direction;
    private Vector3 transformOffset(char dir)
    {
        Vector3 transformOffset = new Vector3();

       switch(dir)
        {
            case 'N':
                transformOffset = new Vector3(0, 0, 5);
                break;
            case 'S':
                transformOffset = new Vector3(0, 0, -5);
                break;
            case 'W':
                transformOffset = new Vector3(-5, 0, 0);
                break;
            case 'E':
                transformOffset = new Vector3(5, 0, 0);
                break;

        }
        return transformOffset;
    }

    private Vector3 transformOffsetStr(string dir)
    {
        Vector3 transformOffset = new Vector3();

        switch (dir)
        {
            case "N":
                transformOffset = new Vector3(0, 0, 5);
                break;
            case "S":
                transformOffset = new Vector3(0, 0, -5);
                break;
            case "W":
                transformOffset = new Vector3(-5, 0, 0);
                break;
            case "E":
                transformOffset = new Vector3(5, 0, 0);
                break;
        }
        return transformOffset;
    }

    //calculate offset based on cardinal direction;
    private int rotationOffset(char dir)
    {
        int rotationOffset = 0;

        switch (dir)
        {
            case 'N':
                rotationOffset = 180;
                break;
            case 'S':
                rotationOffset = 0;
                break;
            case 'W':
                rotationOffset = 90;
                break;
            case 'E':
                rotationOffset = -90;
                break;

        }
        return rotationOffset;
    }

    //calculate offset based on cardinal direction;
    private int rotationOffsetStr(string dir)
    {
        int rotationOffset = 0;

        switch (dir)
        {
            case "N":
                rotationOffset = 180;
                break;
            case "S":
                rotationOffset = 0;
                break;
            case "W":
                rotationOffset = 90;
                break;
            case "E":
                rotationOffset = -90;
                break;

        }
        return rotationOffset;
    }
}
