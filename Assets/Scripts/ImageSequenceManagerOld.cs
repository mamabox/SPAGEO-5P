using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class ImageSequenceManagerOld : MonoBehaviour
{
    //private GameManager gameManager;
    public GameObject selectedRouteImgSeq;
    public GameObject sessionRouteImgSeq;
    private string importPath;
    public RawImage testImage;
    public GameObject rawImagePrefab;
    Texture2D myTexture;
    string fileName;
    byte[] bytes;
    private int imgSpacing;

    private List<string> sessionRouteList = new List<string> { "0", "1", "2", "0" };
    private List<string> selectedRouteList = new List<string> { "0", "0", "0", "0", "1"};

    private List<Image> correctRoute = new List<Image>();


    private void Awake()
    {
        //gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        importPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Media/Images/");
        imgSpacing = 100;
    }

    // Start is called before the first frame update
    void Start()
    {
        //selectedRouteImgSeq
        for (int i = 0; i < selectedRouteList.Count; i++)
        {
            instantiateImages(i, selectedRouteImgSeq, selectedRouteList);
        }

        //selectedRouteImgSeq
        for (int i = 0; i < sessionRouteList.Count; i++)
        {
            instantiateImages(i, sessionRouteImgSeq, sessionRouteList);
        }

    }

    // Update is called once per frame
    void instantiateImages(int i, GameObject parentObj, List<string> list)
    {
        GameObject newRawImage = 
        Instantiate(rawImagePrefab, new Vector3(i * (imgSpacing + rawImagePrefab.GetComponent<RectTransform>().rect.width), 0, 0), rawImagePrefab.transform.rotation);
        newRawImage.transform.SetParent(parentObj.GetComponent<Transform>());   //Set the parent of the prefab
        addTexture(newRawImage, i, list);
    }

    void addTexture(GameObject image, int i, List<string> list)
    {
        myTexture = new Texture2D(640, 360);
        fileName = list[i];
        bytes = File.ReadAllBytes(Path.Combine(importPath, fileName + ".png"));
        myTexture.LoadImage(bytes);
        myTexture.name = fileName;
//        image.GetComponent<RawImage>().texture = myTexture;
    }
}