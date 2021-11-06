using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionTracker : MonoBehaviour
{
    private GameManager gameManager;

    public string coordString;  // Intersection's coordinate in "x.y" format
    public int[] coordInt;  // Intersection's coordinate in [x,y] format
    public int instanceID;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        coordInt[0] = (int)(transform.position.x / gameManager.blockSize);
        coordInt[1] = (int)(transform.position.z / gameManager.blockSize);
        coordString = coordInt[0] + "." + coordInt[1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
