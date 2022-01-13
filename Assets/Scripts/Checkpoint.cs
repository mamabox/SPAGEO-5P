using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * CHECKPOINT.CS
 * 
 * Stores the checkpoint’s coordinates (string - "x_y”), checkpoint ID (int), and collection status (bool)
 * 
 */

public class Checkpoint : MonoBehaviour
{
    private GameManager gameManager;
    private RouteManager routeManager;

    public string coordString;  // Intersection's coordinate in "0_1E" format
    public string coord; //Intersection's coordinate in [x,y] format
    public string cardDir; // Cardinal direction: if the player is moved to this checkpoint, rotate them towards this direction. Using string, not char in case we use NE, NW, etc ... in the future 
    public int ID;
    public int scenario; //Added for Sc8
    public bool isCollected;


    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        routeManager = FindObjectOfType<GameManager>().GetComponent<RouteManager>();

        isCollected = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}