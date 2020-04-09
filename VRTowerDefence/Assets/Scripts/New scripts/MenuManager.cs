using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public  class MenuManager : MonoBehaviour
{

    [SerializeField]  private GameObject  _menu;
    [SerializeField]  private Transform   _menuDisplayPoint;
    [SerializeField]  private Canvas      _menuCanvas;
    [SerializeField]  private Text        _headerText;
    [SerializeField]  private Text        _subHeaderText;

    private GameObject _currentlyDisplayedObject = null;

    public bool _isMenuInTowerPlacementMode = false;
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
            GridGenerator.GridSwitch(true);
        }
           
    }

    public void LeftMenuButtonPressed()
    {
        if (_userPromptRequired)  // If the user needs to activate something.. such as start wave. Switch between Tower menu and the User Prompt.
        {
            if (!_isMenuActive)  // Ensures that the menu is actually open.
            {
                SetMenuActive(true);
            }

            if (_isMenuInTowerPlacementMode)  // If towers are in the menu, then it should load the User prompt menu settings.
            {
                LoadUserPromptIntoMenu();
                _isMenuInTowerPlacementMode = false;
            }

            else
            {
                _isMenuInTowerPlacementMode = true;
            }
        }

        else
        {
            if (_isMenuActive)
            {
                SetMenuActive(false);
                _isMenuInTowerPlacementMode = false;
            }

            else
            {
                SetMenuActive(true);
                _isMenuInTowerPlacementMode = true;
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
        if (_currentlyDisplayedObject != null)
        {
            if (_currentlyDisplayedObject.activeSelf)
            {
                _currentlyDisplayedObject.SetActive(false);
            }
        }
     

        _currentlyDisplayedObject = objectToSwitchIn;

        SetCurrentlyDisplayedObjectActive(true);
        
    }

    public void SetCurrentlyDisplayedObjectActive(bool setActive )
    {
        if (setActive)
        {
            _currentlyDisplayedObject.transform.position = _menuDisplayPoint.position;
            _currentlyDisplayedObject.transform.rotation = _menuDisplayPoint.rotation;
            _currentlyDisplayedObject.transform.SetParent(_menuDisplayPoint);
            _currentlyDisplayedObject.SetActive(true);
        }
        else
        {
            _currentlyDisplayedObject.SetActive(false);
        }
       
    }

    public void SetUpDisplayableObject(GameObject newObject)
    {
        newObject.transform.position = _menuDisplayPoint.position;
        newObject.transform.rotation = _menuDisplayPoint.rotation;
        newObject.transform.SetParent(_menuDisplayPoint);
        newObject.SetActive(false);
    }



}


