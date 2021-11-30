using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEditor; 

[ExecuteInEditMode]
public class TakeScreenshot : MonoBehaviour
{
    public bool takeScreenshot = false;
    public string screenshotPath;

    // Start is called before the first frame update
    void Awake()
    {
        screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), "Exports/Screenshots/");

        if (!System.IO.Directory.Exists(screenshotPath))    //if save directory does not exist, create it
        {
            System.IO.Directory.CreateDirectory(screenshotPath);
        }


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
            TakeScreenshotsNow();
        }
    }

    [ContextMenu("Take screenshot")]
    public void TakeScreenshotsNow()
    {
        screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), "Exports/Screenshots/");
        var time = System.DateTime.Now.ToString("yyyyMMd_HHmmss");
        var screenshotName = "SPAGEO-5P_screenshot_" + time + ".png";

        takeScreenshot = true;
       
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(screenshotPath, screenshotName));
        Debug.Log("Screenshot: " + screenshotPath + screenshotName);
        takeScreenshot = false;
    }

}