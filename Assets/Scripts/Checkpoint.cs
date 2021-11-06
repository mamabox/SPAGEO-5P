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

    public string coordString;  // Intersection's coordinate in "x_y" format
    //public float[] coordinates;  // Intersection's coordinate in [x,y] format
    public int ID;
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