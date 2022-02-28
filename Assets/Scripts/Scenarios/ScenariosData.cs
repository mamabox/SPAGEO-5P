using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class ScenariosData
{
    public int scenariosNb;
    public Sc1Data sc1Data;
    public List<StdScData> stdScData;
    public Sc6Data sc6Data;
    public Sc8Data sc8Data;
    public Sc9Data sc9Data;

}
[System.Serializable]
public class StdScData
{
    public int scenarioID;
    public string description;
    public string dropdownMenuText;
    public int attemptsNb;
    public int validationNb;
    public bool receiverTransmitterOption;
    public bool imageValidation;
    public bool displayLines;
    public List<Route> routes;
}




[System.Serializable]
public class Sc1Data
{
    public int scenarioID;
    public string description;
    public string dropdownMenuText;
}

[System.Serializable]
public class Sc6Data
{
    public int scenarioID;
    public string description;
    public string dropdownMenuText;
    public int attemptsNb;
    public int validationNb;
    public List<BarrierRoute> barrierRoutes;
}

[System.Serializable]
public class Sc8Data
{
    public int scenarioID;
    public string description;
    public string dropdownMenuText;
    public Instructions instructions;
    public List<Sc8Route> routes;

}

[System.Serializable]
public class Sc9Data
{
    public int scenarioID;
    public string description;
    public string dropdownMenuText;
    public Instructions instructions;
    public float visorSize;
    public List<PropObj> propObjs;
    public bool randomTrialsOrder;
    public List<Trial> trials;
}

[System.Serializable]
public class Sc8Route
{
    public Coordinate startCoord;
    public List<Coordinate> checkpoints;
    public List<string> routeSegments;

}
public class ScInfo
{
    public int scenarioID;
    public string description;
    public string dropdownMenuText;
}

[System.Serializable]
public class PropObj
{
    public string name;
    public string pronoun;
    //public string nearbyIntersection;
}

[System.Serializable]
public class Route
{
    //public string startCoord;
    //public string startCardDir;
    public Coordinate startCoord;
    public string routeCoord;
    
}

[System.Serializable]
public class BarrierRoute
{
    public Coordinate startCoord;
    public List<Coordinate> barriersCoord;
}

[System.Serializable]
public class Instructions
{
    public string start;
    public string end;
   public List<string> attempts;
}

[System.Serializable]
public class Trial
{
    public string position;
    public int startObj;
    public int targetObj;

}

[System.Serializable]
public class Coordinate
{
    public string coord;
    public string cardDir;
}