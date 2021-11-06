using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/**
 * DataInputMenu.cs
 * 
 * Handles Menu that collects session information (group/student IDs)
 * 
 * 
 **/

public class DataInputMenu : MonoBehaviour
{

    public Toggle groupSessionToggle;   // ON if is a group session, OFF if is single user session
    public InputField groupIDText;      // the group's ID
    public GameObject[] studentIDInputFields;   // array of the individual student's ID
    public GameObject nextButton;       // button to next screen or start session
    public GameObject firstStudentIDText;   //used to automatically select first input field


    private bool isGroupSession;
    public string groupID;
    public List<string> enteredIDs;
    private bool dataVerified = false;

    // Start is called before the first frame update
    void Start()
    {
        studentIDInputFields = GameObject.FindGameObjectsWithTag("studentIDField");
        showGroupOptions(false);    //hide group options at start up
        nextButton.SetActive(false); //hide 'Next' button

    }

    // Update is called once per frame
    // IF THE FORM HAS SUFFICIENT DATA, SHOW THE NEXT BUTTON
    void Update()
    {
        dataVerified = VerifyData();
        if (dataVerified)
            nextButton.SetActive(true);
        else
            nextButton.SetActive(false);

    }

    //DATA VALDIDATION (GROUP ID IF GROUP + STUDENT ID IF STUDENT)
    private bool VerifyData()
    {
        if (groupSessionToggle.isOn && string.IsNullOrEmpty(groupIDText.text))       
            return false;
        if (string.IsNullOrEmpty(firstStudentIDText.gameObject.GetComponent<InputField>().text))
            return false;

        return true;
    }

    // HANDLES THE DATA INPUTTED BY THE PLAYER AND SAVES IT TO THE GLOBAL SESSION DATA
    public void HandleInputData()
    {
        isGroupSession = groupSessionToggle.isOn;
        groupID = groupIDText.text;
        enteredIDs.Clear();
        foreach (GameObject inputField in studentIDInputFields)
        {
            //enteredIDs.Add(inputField.GetComponent<InputField>().text);
            if (!string.IsNullOrEmpty(inputField.GetComponent<InputField>().text))
            {
                enteredIDs.Add(inputField.GetComponent<InputField>().text);
            }
        }
        //Debug.Log("EnteredIDS: " + string.Join(",", enteredIDs));
        saveSessionData();
    }

    // SAVE THE SESSION DATA
    private void saveSessionData()
    {
        GlobalControl.instance.sessionData.isGroupSession = isGroupSession;
        GlobalControl.instance.sessionData.groupID = groupID;
        GlobalControl.instance.sessionData.studentIDs = enteredIDs;
    }

    // CALLED FROM INSPECTOR WHEN TOGGLING 'GROUP' CHECKBOX 
    public void showGroupOptions(bool status)
    {
        groupIDText.text = null;
        groupIDText.gameObject.SetActive(status);
        

        for (int i = 0; i < studentIDInputFields.Count(); i++)
        {
            studentIDInputFields[i].GetComponent<InputField>().text = null;
            studentIDInputFields[i].GetComponent<InputField>().gameObject.SetActive(status);
        }
        firstStudentIDText.SetActive(true);
    }



}
