using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl instance;

    //public int activeSequence = 0;
    //public int activeRoute = 0;
    public bool returnToMenu;

    public SessionData sessionData = new SessionData();



    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //sessionData.hasDoneValidationByImage = false;
        //sessionData.sessionPaused = false;
        sessionData.selectedScenario = 0;
        sessionData.selectedRoute = 0;


    }

  
        // Start is called before the first frame update
        void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public struct SessionData
    {
        public bool isGroupSession;   //FALSE: student worked alone
        public string groupID;        //IF isGroupSession, what is the groupID
        public List<string> studentIDs;    //IDs of the students working in this session
        public int selectedScenario;
        public int selectedRoute;
        public List<string> selectedRouteCoord;
        public List<string> routeStart;
        public bool isSender;
        public bool validationDone;
        public bool sessionPaused;
        public float sessionStartTime;
        public float pauseTime;
        public string fileName;
        public float pauseDistance;
        public int validationCountNew;
        public List<string> sessionRouteGC;
        public List<string> selectedRouteDir;
    }
}
