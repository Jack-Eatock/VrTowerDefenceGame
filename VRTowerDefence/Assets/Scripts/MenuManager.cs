using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public  class MenuManager : MonoBehaviour
{



    public Transform MenuDisplayPoint;
    public bool IsMenuTowerPlacementMode = false;
    public GameObject CurrentlyDisplayedObject = null;

    [SerializeField]  private GameObject  _menu = null;
    [SerializeField]  private Text        _headerText = null;
    [SerializeField]  private Text        _subHeaderText = null;
    [SerializeField]  private GameObject  _menuUserPromptButton = null;

    public bool UserPromptRequired = false;
    public bool IsMenuActive = false;

    private GameModeSurvivalScript.TestDelegate _userPromptMethodToCall;
    private string _userPromptHeaderText;
    private string _userPromptSubHeaderText;

    void Start()
    {
        InputScripto.OnLeftMenuPress += LeftMenuButtonPressed;
        
    }

    // Update is called once per frame
    void Update()
    {  
    }

    public void SetUserPrompt(string headerText, string subHeaderText, GameModeSurvivalScript.TestDelegate MethodToCall )
    {
        _userPromptHeaderText = headerText;
        _userPromptSubHeaderText = subHeaderText;
        _userPromptMethodToCall = MethodToCall;

        UserPromptRequired = true;

        LoadUserPromptIntoMenu();
    }


    public void LoadUserPromptIntoMenu()
    {
        SetHeaderText(_userPromptHeaderText);
        SetSubHeaderText(_userPromptSubHeaderText);
        SwitchOutMenuCurrentlyDisplayedObject(_menuUserPromptButton);
    }

    public void UserPromptActivated()
    {
        if (_userPromptMethodToCall != null)
        {
            _userPromptMethodToCall();
        }
        else
        {
            Debug.Log("ERROR : Method caused by the User prompt button being pressed, does not have a refferance to a method.");
        }

        _userPromptSubHeaderText = null;
        _userPromptSubHeaderText = null;
        _userPromptMethodToCall = null;
        UserPromptRequired = false;

        SetMenuActive(false);

    }

    public void SetMenuActive (bool setActive)
    {
        if (setActive)
        {
            _menu.SetActive(true);
            IsMenuActive = true;
            GridGenerator.GridSwitch(false);
        }
        else
        {
            _menu.SetActive(false);
            IsMenuActive = false;
            GetComponent<PlacingTowersScript>().ResetPlacing();
            GridGenerator.GridSwitch(true);
        }
           
    }

    public void LeftMenuButtonPressed()
    {
        if (LevelManager.CurrentLevel != LevelManager.Levels.Survival)
        {
            return;
        }

        if (UserPromptRequired)  // If the user needs to activate something.. such as start wave. Switch between Tower menu and the User Prompt.
        {
            if (!IsMenuActive)  // Ensures that the menu is actually open.
            {
                SetMenuActive(true);
            }

            if (IsMenuTowerPlacementMode)  // If towers are in the menu, then it should load the User prompt menu settings.
            {
                LoadUserPromptIntoMenu();
                IsMenuTowerPlacementMode = false;
            }

            else
            {
                IsMenuTowerPlacementMode = true;
            }
        }

        else
        {
            if (IsMenuActive)
            {
                SetMenuActive(false);
                IsMenuTowerPlacementMode = false;
            }

            else
            {
                SetMenuActive(true);
                IsMenuTowerPlacementMode = true;
            }
        }
    }

    public void SetHeaderText(string newText)
    {
        _headerText.text = newText;
    }

    public void SetSubHeaderText(string newText)
    {
        _subHeaderText.text = newText;
    }

    public void SwitchOutMenuCurrentlyDisplayedObject(GameObject objectToSwitchIn)
    {
        if (CurrentlyDisplayedObject != null)
        {
            if (CurrentlyDisplayedObject.activeSelf)
            {
                CurrentlyDisplayedObject.SetActive(false);
            }
        }
     

        CurrentlyDisplayedObject = objectToSwitchIn;

        SetCurrentlyDisplayedObjectActive(true);
        
    }

    public void SetCurrentlyDisplayedObjectActive(bool setActive )
    {
        if (setActive)
        {
            CurrentlyDisplayedObject.transform.position = MenuDisplayPoint.position;
            CurrentlyDisplayedObject.transform.rotation = MenuDisplayPoint.rotation;
            CurrentlyDisplayedObject.transform.SetParent(MenuDisplayPoint);
            CurrentlyDisplayedObject.SetActive(true);
        }
        else
        {
            CurrentlyDisplayedObject.SetActive(false);
        }
       
    }

    public void SetUpDisplayableObject(GameObject newObject)
    {
        newObject.transform.position = MenuDisplayPoint.position;
        newObject.transform.rotation = MenuDisplayPoint.rotation;
        newObject.transform.SetParent(MenuDisplayPoint);
        newObject.SetActive(false);
    }


}


