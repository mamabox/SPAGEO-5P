using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEditor;

[ExecuteInEditMode]
public class ScreenshotManager : MonoBehaviour
{
    private GameManager gameManager;
    private RouteManager routeManager;
    private UIManager uiManager;
    private Canvas canvas;
    private IntersectionManager intersectionManager;
    private GameObject player;
    private string savePath;
    private int i;
    private int j;
    public int iStartValue;
    public int iMaxValue;
    public int jStartValue;
    public int jMaxValue;
    public string screenshotDir;
    public int screenshotOffset;
    public bool takeScreenshot = false;
    List<string> directions = new List<string> { "NW", "NE", "SW", "SE" };


    //private string[] routeDir2 = { "1_1N","1_2N","1_2E","2_2E","2_2N","2_3N" };
    private string[] currentRouteDir;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        uiManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        intersectionManager = FindObjectOfType<GameManager>().GetComponent<IntersectionManager>();
        routeManager = GetComponent<RouteManager>();
        canvas = FindObjectOfType<Canvas>();
        player = GameObject.FindGameObjectWithTag("Player");

        savePath = Path.Combine(gameManager.screenshotPath, "/all/");
        //currentRouteDir = routeDir2;
    }

    private void Start()
    {
        //iStartValue = 4;
        //iMaxValue = 11;
        //jStartValue = -4;
        //jMaxValue = -1;

    }

    // Update is called once per frame
    void Update()
    {
        // X = 4 TO 11, Y = -4 to -1
        if (takeScreenshot)
        {
                if (i <= iMaxValue)
                {
                    TakeScreenshotsAtIntersection(i, j);
                    if (j < jMaxValue)
                    {
                        j++;
                    }
                    else
                    {
                        j = jStartValue;
                        i++;
                    }
                }
                else
                {
                    takeScreenshot = false;
                }
        }
    }

    [ContextMenu("Now")]
    public void TakeScreenshotsNow()
    {
        takeScreenshot = true;

        i = iStartValue;
        j = jStartValue;
    }

    public void TakeScreenshot()
    {

        if (!System.IO.Directory.Exists(gameManager.screenshotPath))    //if save directory does not exist, create it
        {
            System.IO.Directory.CreateDirectory(gameManager.screenshotPath);
        }

        //var screenshotName = "Screenshot_" + System.DateTime.Now.ToString("HH-mm-ss") + ".png";
        var screenshotName = uiManager.inputCoordX + routeManager.xyCoordSeparator + uiManager.inputCoordY + uiManager.inputDir + ".png";

        //canvas.enabled = false;
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(gameManager.screenshotPath, screenshotName));
        //canvas.enabled = true;
        Debug.Log(gameManager.screenshotPath + screenshotName);
    }

    public void screenshotAllIntersections()
    {

    }

    [ContextMenu("Screenshot Intersection")]
    public void TakeScreenshotsAtIntersection(int x, int z)
    {
        // 1. for each element in validCood - for each element in validDir - call goto and take screenshot for each validDir
        //foreach (string coord in intersectionManager.validCoord){

        //}
        //int i = 1;
        //int j = 1;


        string coord = x + routeManager.xyCoordSeparator + z;
        string dir;
        string screenshotName;
        //ALL
        //screenshotName = coord + screenshotDir + ".png";
        //intersectionManager.GotoCoord(coord, screenshotDir);
        //player.transform.Translate(xOffset, 0, zOffset);
        //ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(gameManager.screenshotPath, screenshotName));
        //Debug.Log("Screenshot " + screenshotName);

        // North
        //dir = "N";
        //screenshotName = coord + dir + ".png";
        //intersectionManager.GotoCoord(coord, dir);
        //player.transform.Translate(0, 0, -5);
        //ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(gameManager.screenshotPath, screenshotName));
        //Debug.Log("Screenshot " + screenshotName);
        // South
        //dir = "S";
        //screenshotName = coord + dir + ".png";
        //intersectionManager.GotoCoord(coord, dir);
        //player.transform.Translate(0, 0, 5);
        //ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(gameManager.screenshotPath, screenshotName));
        //Debug.Log("Screenshot " + screenshotName);
        //// East
        //dir = "E";
        //screenshotName = coord + dir + ".png";
        //intersectionManager.GotoCoord(coord, dir);
        //player.transform.Translate(-5, 0, 0);
        //ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(gameManager.screenshotPath, screenshotName));
        //// West
        //dir = "W";
        screenshotName = coord + screenshotDir + ".png";
        intersectionManager.GotoCoord(coord, screenshotDir);
        player.transform.Translate(0, 0, screenshotOffset);
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(gameManager.screenshotPath, screenshotName));

        //List<string> directions = new List<string> { "NW", "NE", "SW", "SE" };
        //foreach (string direction in directions)
        //{
        //    screenshotName = coord + direction + ".png";
        //    intersectionManager.GotoCoord(coord, direction);
        //    ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(gameManager.screenshotPath, screenshotName));
        //}


    }


    //[ContextMenu ("Screenshot Route")]
    //public void ScreenschotCurrentRoute()
    //{
    //    List<string> _route = currentRouteDir.ToList();


    //    if (!System.IO.Directory.Exists(savePath))    //if save directory does not exist, create it
    //    {
    //        System.IO.Directory.CreateDirectory(savePath);
    //    }

    //    for (int i=1; i < _route.Count(); i++)
    //    {

    //    }
    //}

    //public void ScreenshotCoor(string coord, string dir)
    //{
    //    var screenshotName = gameManager.inputCoordX + routeManager.coordSeparator + gameManager.inputCoordY + gameManager.inputDir + ".png";
    //    ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(savePath, screenshotName));
    //}
}