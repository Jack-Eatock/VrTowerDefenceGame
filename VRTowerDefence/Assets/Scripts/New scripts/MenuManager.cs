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
    [SerializeField]  private Canvas      _menuCanvas = null;
    [SerializeField]  private Text        _headerText = null;
    [SerializeField]  private Text        _subHeaderText = null;

    private bool _userPromptRequired = false;
    private bool _isMenuActive = false;


    void Start()
    {
        InputScripto.OnLeftMenuPress += LeftMenuButtonPressed;
        
    }

    // Update is called once per frame
    void Update()
    {  
    }

    public void LoadUserPromptIntoMenu()
    {

    }

    public void SetMenuActive (bool setActive)
    {
        if (setActive)
        {
            _menu.SetActive(true);
            _isMenuActive = true;
            GridGenerator.GridSwitch(false);
        }
        else
        {
            _menu.SetActive(false);
            _isMenuActive = false;
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

        if (_userPromptRequired)  // If the user needs to activate something.. such as start wave. Switch between Tower menu and the User Prompt.
        {
            if (!_isMenuActive)  // Ensures that the menu is actually open.
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
            if (_isMenuActive)
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


