using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class ImageSequence : MonoBehaviour
{

    public GameObject imagesSequence;
    public GameObject rawImagePrefab;
    public int maxScrollPos;
    public int currentScrollPos;
    private string importPath;
    Texture2D myTexture;
    string fileName;
    byte[] bytes;
    private int imgSpacing;
    private int offset;
    public List<string> routeList;

    //private routeListNew = new List<string> { "2_4W", "2_4NW", "2_4N", "2_5N", "2_6N" };
    //public List<string> routeListNew = GlobalControl.instance.sessionData.sessionRouteGC;

    private void Awake()
    {
        //gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        importPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Resources/Images/");
        imgSpacing = 80;
        offset = 640;
        currentScrollPos = 0;

        //routeListNew = new List<string> { "2_4W", "2_4NW", "2_4N", "2_5N", "2_6N" };
    }

    // Start is called before the first frame update
    void Start()
    {
        maxScrollPos = routeList.Count - 3;
        //selectedRouteImgSeq
        for (int i = 0; i < routeList.Count; i++)
        {
            instantiateImages(i, imagesSequence, routeList);
        }

    }


    public void OnButtonClick(int direction)
    {

        if (direction > 0 && (currentScrollPos < maxScrollPos) )
        {

            imagesSequence.transform.Translate(offset * -direction, 0, 0);
            currentScrollPos++;

        }
        else if (direction < 0 && currentScrollPos > 0)
        {
            imagesSequence.transform.Translate(offset * -direction, 0, 0);
            currentScrollPos--;
        }
    }



    // Update is called once per frame
    void instantiateImages(int i, GameObject parentObj, List<string> list)
    {
        GameObject newRawImage =
        Instantiate(rawImagePrefab, new Vector3(i * (imgSpacing + rawImagePrefab.GetComponent<RectTransform>().rect.width), 0, 0), rawImagePrefab.transform.rotation);
        newRawImage.transform.SetParent(parentObj.GetComponent<Transform>(), false);   //Set the parent of the prefab
        addTexture(newRawImage, i, list);
        //GameObject textNumber = newRawImage.transform.GetChild(0).gameObject;
        newRawImage.GetComponent<ImageNumber>().routeText.text = (i + 1).ToString();
    }

    void addTexture(GameObject image, int i, List<string> list)
    {
        myTexture = new Texture2D(640, 360);
        fileName = list[i];
        bytes = File.ReadAllBytes(Path.Combine(importPath, fileName + ".png"));
        myTexture.LoadImage(bytes);
        myTexture.name = fileName;
        image.GetComponent<RawImage>().texture = myTexture;
    }


}
