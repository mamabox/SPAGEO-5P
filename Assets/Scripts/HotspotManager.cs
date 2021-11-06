using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HotspotManager : MonoBehaviour
{
    private GameManager gameManager;
    private RouteManager routeManager;
    private IntersectionManager intersectionManager;
    private UIManager uiManager;

    public GameObject hotspotPrefab;
    public List<string> allHotspots;        // List of all checkpoints coordinates
    public int lastHotspotCollected;                 // ID of the checkpoint that can be validated next
    public List<string> hotspotTextS0;        // Text import of text to display at each checkpoint
    public List<string> hotspotTextS6;        // Text import of text to display at each checkpoint
    public bool onHotspot;
    public GameObject lastHotspot;

    void Awake()
    {
        // Initialise components
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        routeManager = FindObjectOfType<GameManager>().GetComponent<RouteManager>();
        intersectionManager = FindObjectOfType<GameManager>().GetComponent<IntersectionManager>();
        lastHotspotCollected = 0; //
    }

    //GENERATE CHECKPOINTS PREFABS
    public void GenerateHotspots(List<string> hotspots, List<string> url)
    {
        string[] coordArray;

        for (int i = 0; i < hotspots.Count(); i++)
        {
            coordArray = hotspots[i].Split(char.Parse(routeManager.xyCoordSeparator));      //stores the coordinates x and y in an array
            var newHotspot = Instantiate(hotspotPrefab, new Vector3(float.Parse(coordArray[0]) * gameManager.blockSize, 0.02f, float.Parse(coordArray[1]) * gameManager.blockSize), hotspotPrefab.transform.rotation);    //instantiate the checkpoint right above the ground
            newHotspot.GetComponent<Hotspot>().coordString = hotspots[i];  //store the coordinates as a string in the instance
            newHotspot.GetComponent<Hotspot>().url = url[i];    //stores the hotspot's url
            newHotspot.GetComponent<Hotspot>().ID = i + 1;    //stores the checkpoitn number in the instance
        }
    }

    //PLAYER IS ON HOTSPOT
    public void OnHotspotEnter(Collider other)
    {
        onHotspot = true;
        lastHotspot = other.gameObject;
        lastHotspotCollected = other.gameObject.GetComponent<Hotspot>().ID;
        if (!other.gameObject.GetComponent<Hotspot>().isCollected)  //If the hotspot has not already been collected
        {
            gameManager.freezePlayer = true;
            uiManager.hotspotDialogBox.GetComponent<HotspotDialogBox>().url = other.gameObject.GetComponent<Hotspot>().url; //send the hotspot's url to the dialog box
            uiManager.hotspotDialogBox.gameObject.SetActive(true);

            uiManager.hotspotDialogBox.GetComponent<HotspotDialogBox>().dialogBoxBtn.Select();
            uiManager.hotspotDialogBox.GetComponent<HotspotDialogBox>().dialogBoxBtn.OnSelect(null);
        }
        other.GetComponent<Hotspot>().isCollected = true;   //Mark hotspot as collected
    }

    //PLAYER EXITS HOTSPOT
    public void OnHotspotExit(Collider other)
    {
        onHotspot = false;
    }
}
