using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Animates the arrow map (Detail Albedo x2) of the Route indicator material for the lines to follow. The lines to follow are generated using the line renderer. 
 */

public class ScrollTexture : MonoBehaviour
{
    float scrollSpeedX;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    private void Start()
    {
        scrollSpeedX = -gameManager.gameData.lineScrollSpeed;
        //scrollSpeedX = .5f;
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX = Time.time * scrollSpeedX;
        GetComponent<Renderer>().material.SetTextureOffset("_DetailAlbedoMap", new Vector2(offsetX, 0));
    }
}
