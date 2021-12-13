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
    public int attempsNb;
    public int validationNb;
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
    public int attempsNb;
    public int validationNb;
    public List<Barriers> barriers;
}

[System.Serializable]
public class Sc8Data
{
    public int scenarioID;
    public string description;
    public string dropdownMenuText;
}

[System.Serializable]
public class Sc9Data
{
    public int scenarioID;
    public string description;
    public string dropdownMenuText;
    public List<Instruction> instructions = new List<Instruction>();
    public List<PropObj> propObjs;
    //public List<string> objectsTemp;
    public List<Trial> trials = new List<Trial>();
}

[System.Serializable]
public class PropObj
{
    public string name;
    public string pronoun;
}

[System.Serializable]
public class Route
{
    public string startCoord;
    public char startCardDir;
    public string routeCoord;
}

[System.Serializable]
public class Barriers
{
    public string startCoord;
    public char startCardDir;
    public string barriersCoord;
}

[System.Serializable]
public class Instruction
{
    public string description;
    public string text;
}

[System.Serializable]
public class Trial
{
    public string position;
    public int startObj;
    public int targetObj;

}