using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sc9Manager : MonoBehaviour
{
    //Props
    private GameObject startObj;
    private GameObject targetObj;
    public GameObject[] allObjects;

    //Calculation
    private float angleToTarget;
    private float rotationValue;
    private float startRotation;
    private float correctRotation;
    private float endRotation;
    private float errorRotation;

    private int trialsCount;
    private int currentTrial;

    public List<string> objNamesPronouns;

    private Vector3 startPosition;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        currentTrial = 0;
        startPosition = new Vector3(0, 0, 0);

    }

    // Start is called before the first frame update
    void Start()
    {
        CreateNounsPronouns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateNounsPronouns()
    {
        objNamesPronouns = new List<string>();

        for (int x = 0; x < gameManager.scenariosData.sc9Data.propObjs.Count(); x++){
            objNamesPronouns.Add(gameManager.scenariosData.sc9Data.propObjs[x].pronoun + " " + gameManager.scenariosData.sc9Data.propObjs[x].name);
            Debug.Log("PropObjs: " + string.Join(", ", objNamesPronouns));
        }
    }
}
