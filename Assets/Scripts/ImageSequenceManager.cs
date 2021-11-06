using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class ImageSequenceManager : MonoBehaviour
{
    //private GameManager gameManager;
    public GameObject selectedRouteImgSeq;
    public GameObject sessionRouteImgSeq;
 

    //private List<string> sessionRouteList = new List<string> { "0", "1", "2", "0" };
    //private List<string> selectedRouteList = new List<string> { "0", "0", "0", "0", "1"};

    //private List<Image> correctRoute = new List<Image>();


    private void Awake()
    {
        //selectedRouteImgSeq.GetComponent<ImageSequence>().routeList = new List<string> { "2_4W", "2_4NW", "2_4N", "2_5N", "2_6N" };
        selectedRouteImgSeq.GetComponent<ImageSequence>().routeList = GlobalControl.instance.sessionData.selectedRouteDir;
        sessionRouteImgSeq.GetComponent<ImageSequence>().routeList = GlobalControl.instance.sessionData.sessionRouteGC;
        //sessionRouteImgSeq.transform.Translate(0, -500, 0);
    }

}