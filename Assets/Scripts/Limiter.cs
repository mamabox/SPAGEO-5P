using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * LIMITER.CS
 * 
 * Stores the limiter’s coordinates (string - "x_y”), and ID (int)
 * 
 */

public class Limiter : MonoBehaviour
{
    private GameManager gameManager;
    private RouteManager routeManager;

    public string coordString;  // Intersection's coordinate in "x_y" format
    //public float[] coordinates;  // Intersection's coordinate in [x,y] format
    public int cardinalDirection;
    public int ID;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        routeManager = FindObjectOfType<GameManager>().GetComponent<RouteManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}