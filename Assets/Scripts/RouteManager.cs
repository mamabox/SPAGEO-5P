using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class RouteManager : MonoBehaviour
{
    public GameObject routeIndicatorPrefab;
    
    private IntersectionManager intersectionManager;
    private RouteManager routeManager;
    private GameManager gameManager;
    private CheckpointManager checkpointManager;
    private UIManager UIManager;
    public Material tempLineMaterial;
    public List<Material> lineMaterials;
    public GameObject checkpointPrefab;
    public GameObject lineDrawn;
   

    public string xyCoordSeparator = "_"; //TODO: Convert to CHAR

    //VALID ROUTES

    public char textFilesCommentSeparator = '+';

    //public List<string> selectedRouteCoord;
    //public List<string> routeStart;
    public bool validationEnabled;  //TODO: Remove because not used?
    //public bool checkedValidation;  //Has player checked validation for this route
    public ValidationInfo validationInfo = new ValidationInfo();
    //private string importPath;
    //bool isValidationEnabled;

    private void Update()
    {
        validationInfo.routeLength = intersectionManager.sessionRoute.Count();
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        routeManager = FindObjectOfType<GameManager>().GetComponent<RouteManager>();
        UIManager = FindObjectOfType<GameManager>().GetComponent<UIManager>();
        intersectionManager = FindObjectOfType<GameManager>().GetComponent<IntersectionManager>();
        checkpointManager = FindObjectOfType<GameManager>().GetComponent<CheckpointManager>();
    }

    //CHECKS IF myRoute IS THE SAME AS THE ROUTE SELECTED FOR THE SESSION
    public void ValidateRoute(List<string> myRoute)
    {
        UIManager.validationCheck = true;
        List<string> correctRoute = new List<string>(gameManager.sessionData.selectedRouteCoord);

        correctRoute.RemoveAt(0);   //Remove the start point coordinates

        Debug.Log("Checking against route: " + string.Join(xyCoordSeparator, correctRoute));

        bool hasError = false;
        //1. Check if route is correct
        if (correctRoute.Count() != myRoute.Count())     
        {
            hasError = true;
        }
        else //  routes the same lenght
        {
            //Debug.Log("Routes have the same length");
            for (int i = 0; i < myRoute.Count(); i++)   //3. if route not correct, check where the error was
            {
                if (myRoute.ElementAt(i) != correctRoute.ElementAt(i))
                {
                    //Debug.Log("comparaison error found");
                    hasError = true;
                }
            }
        }
   

        if (!hasError)     //PATH IS VALIDATED
        {
            //Debug.Log("there are no errors, the route is valid");
            validationInfo.isValid = true;
            validationInfo.errorAt = 0;
            validationInfo.endReached = true;
            //TODO: Dialog box to return to menu
        }
        else if (myRoute.Count!=0)
        {
            //Debug.Log("there are errors");
            validationInfo.isValid = false;
            if (correctRoute.Last() == myRoute.Last())  //2. check if reached destination
            {
                validationInfo.endReached = true;
                //Debug.Log("Routes ends in the same place");
            }
            else
            {
                validationInfo.endReached = false;
                //Debug.Log("Routes DO NOT in the same place");
            }

            for (int i = 0; i < myRoute.Count(); i++)   //3. if route not correct, check where the error was
            {
                if (correctRoute.Count() >= myRoute.Count()) // myRoute is not longer than the correct route
                {
                    if (myRoute.ElementAt(i) != correctRoute.ElementAt(i))
                    {
                        validationInfo.errorAt = i + 1;
                        //Debug.Log("Error at intersectin #: " + (i+1));
                        break;
                    }
                    else
                    {
                        validationInfo.errorAt = 0;   
                    }
                }
                else
                {
                    validationInfo.errorAt = correctRoute.Count()+1;
                }
            }
        }

        //Debug.Log("Routes are the same= " + isValid);
        validationInfo.routeLength = myRoute.Count();
        //Debug.Log("Valid= " + validationInfo.isValid + "- length: " + validationInfo.routeLength + "- error at #: " + validationInfo.errorAt + "- endReached= " + validationInfo.endReached);
        //return validationInfo;


    }

    public struct ValidationInfo
    {
        public bool isValid;
        public bool endReached;
        public int errorAt;
        public int routeLength;
    }

    public List<string> getRouteStart(List<string> route) // returns the first 
    {

        //Debug.Log("Route count (getRouteStart)= " + selectedRoute.Count());
        //char _startDir = route[0][route[0].Length - 1];    //direcion is the last character of the last coordinate
        char _startDir = route.ElementAt(0).Last();     //direcion is the last character of the last coordinate
        string _startCoord = route[0].Remove(route[0].Length - 1);  //coord is route[0] minus last character
        //string _startCoord = route.ElementAt(0).Remove(route.ElementAt(0).Length - 1); //WHY DOES NOT WORK?

        //Debug.Log("Start Coord from currentRoute =" + _startCoord);
        //Debug.Log("Start Dir from currentRoute =" + _startDir);

        return new List<string> { _startCoord, _startDir.ToString() };
    }

    //IMPORTS TEXT FROM A .TXT FILE AND RETURNS EACH LINE AS A STRING



    public void SpawnLine(List<string> route, int material)
    {
        GameObject newLineGen = Instantiate(routeIndicatorPrefab);

        if (material < lineMaterials.Count)
            newLineGen.GetComponent<Renderer>().material = lineMaterials[material];
        else
            newLineGen.GetComponent<Renderer>().material = lineMaterials[0];

        //Animate the line texture
        newLineGen.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(.5f, .5f);

        LineRenderer lRend = newLineGen.GetComponent<LineRenderer>();

        List<string> lineToDraw = new List<string> (route); 
        //lineToDraw.RemoveAt(0); //remove the starting position
        lRend.positionCount = lineToDraw.Count();    //set length of line renderer to the number of coordinates on the path 

        for (int i = 0; i < lineToDraw.Count(); i++)
        {
            string[] _coord = lineToDraw[i].Split(char.Parse(xyCoordSeparator));
            //Debug.Log("Draw at (" + string.Join(", ", _coord) + ")");
            lRend.SetPosition(i, new Vector3(float.Parse(_coord[0]) * gameManager.blockSize, 0.01f, float.Parse(_coord[1]) * gameManager.blockSize));
        }

        lineDrawn = newLineGen; //Object of the line drawn
    }

    //Split coordinates in the string format 5_5N into a list of string (coordinate, direction)
    public List<string> SplitCoordinates(string coord)
    {
        return new List<string> { coord.Remove(coord.Length - 1), coord.Last().ToString()};
    }
}
