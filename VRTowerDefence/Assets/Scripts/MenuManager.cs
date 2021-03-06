﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public  class MenuManager : MonoBehaviour
{


    public InteractionDetection InteractionScript;

    public Transform MenuDisplayPoint;
    public bool IsMenuTowerPlacementMode = false;
    public GameObject CurrentlyDisplayedObject = null;

    public enum TypeOfText {HeaderText, DescriptionText, CostText, DpsText, Rangetext, PointsText };

    [SerializeField]  private GameObject  _menu = null;

    [SerializeField]  private Text        _headerText = null;
    [SerializeField]  private Text        _descriptionText = null;
    [SerializeField]  private Text        _costText = null;
    [SerializeField]  private Text        _dpsText = null;
    [SerializeField]  private Text        _rangeText = null;
    [SerializeField]  private Text        _pointsText = null;


    [SerializeField]  private GameObject  _menuUserPromptButton = null;

    public bool UserPromptRequired = false;
    public bool IsMenuActive = false;

    private GameModeSurvivalScript.TestDelegate _userPromptMethodToCall;

    private string _userPromptHeaderText;
    private string _userPromptSubHeaderText;
    public bool MenuDisabled = false;

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
        SetText(TypeOfText.HeaderText, _userPromptHeaderText);
        SetText(TypeOfText.DescriptionText, _userPromptSubHeaderText);
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
            InteractionScript.DisableInteraction(true, true);
            _menu.SetActive(true);
            IsMenuActive = true;
            GridGenerator.GridSwitch(false);
        }
        else
        {
            InteractionScript.DisableInteraction(true, false);
            _menu.SetActive(false);
            IsMenuActive = false;
            GetComponent<PlacingTowersScript>().ResetPlacing();
            GridGenerator.GridSwitch(true);
        }
           
    }

    public void LeftMenuButtonPressed()
    {
        if (MenuDisabled)
        {
            return;
        }

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


    public void SetText(TypeOfText textType  ,string newText)
    {
        switch (textType)
        {
            case TypeOfText.HeaderText:
                _headerText.text = newText;
                break;

            case TypeOfText.DescriptionText:
                _descriptionText.text = newText;
                break;

            case TypeOfText.DpsText:
                _dpsText.text = newText;
                break;

            case TypeOfText.CostText:
                _costText.text = newText;
                break;

            case TypeOfText.Rangetext:
                _rangeText.text = newText;
                break;

            case TypeOfText.PointsText:
                _pointsText.text = newText;
                break;
        }
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


