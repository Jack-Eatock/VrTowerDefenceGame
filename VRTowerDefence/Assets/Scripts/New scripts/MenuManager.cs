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

    private GameObject _currentlyDisplayedObject;
    private bool _inBuildingMode;

    void Start()
    {
        InputScripto.OnLeftMenuPress += LeftMenuButtonPressed;

    }

    // Update is called once per frame
    void Update()
    {  
    }

    public void SetMenuActive(bool setActive)
    {
        if (setActive)
        {
            GridGenerator.GridSwitch(true);
            _menu.SetActive(true);
        }
        else
        {
            GridGenerator.GridSwitch(false);
            _menu.SetActive(false);
        }
           
    }

    public void LeftMenuButtonPressed()
    {
        if (_menu.activeSelf)
        {
            SetMenuActive(false);
        }
        else
        {
            SetMenuActive(true);
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

    public void SetHeaderActive(bool setActive)
    {
        if (setActive)
            _headerText.gameObject.SetActive(true); 

        else
            _headerText.gameObject.SetActive(false);
        
    }

    public void SetSubHeaderActive(bool setActive)
    {
        if (setActive)
            _subHeaderText.gameObject.SetActive(true);

        else
            _subHeaderText.gameObject.SetActive(false);
    }

    public void SwitchOutMenuCurrentlyDisplayedObject(GameObject objectToSwitchIn)
    {
        if (_currentlyDisplayedObject.activeSelf)
        {
            _currentlyDisplayedObject.SetActive(false);
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


