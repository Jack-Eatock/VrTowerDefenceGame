using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenuScript : MonoBehaviour
{
    // Required References \\

    [SerializeField] private GameObject _mainMenuCanvas;   // Canvas
    [SerializeField] private GameObject _playerHead; // Camera
    [SerializeField] private GameObject _playerPointer; // pointer

    // Menu Gameobjects \\

    [SerializeField] private GameObject _baseMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _heightCalibrationMenu;

    // Tweakable \\

    [SerializeField] private float _menuDistFromPlayer = 0.5f;

    // Variables \\

    enum Menus {BaseMenu, SettingsMenu, HeightCalibration};
    private Menus _menuState;
    private GameObject _lastMenuObj;
    private GameObject _menuToActivate;

    // Start is called before the first frame update
    void Start()
    {
        InputScripto.OnMainMenuPressed += OnMenuButtonPressed;

        foreach (GameObject child in GameObject.FindGameObjectsWithTag("MenuPanel"))
        {
            child.SetActive(false);
        }

        if (_mainMenuCanvas.activeSelf)
        {
            _mainMenuCanvas.SetActive(false);
        }

    }

    private void SetMenuActive(Menus menuState)
    {
        Debug.Log("Setting Menu");

        // If the menu has just been opened, set the position correctly and Set active. Otherwise the menu is already positioned correctly.
        if (!_mainMenuCanvas.activeSelf)
        {
            Debug.Log("Activating Menu");
            SetPosOfMenu();
            _playerPointer.SetActive(true);
            _mainMenuCanvas.SetActive(true);

        }

        // Get the last menu obj to be active.
        _lastMenuObj = ReturnGameobjectLinkedWithMenuState(_menuState);

        if (_lastMenuObj != null && _lastMenuObj.activeSelf) // Makes sure the last menu is closed.s
        {
            _lastMenuObj.SetActive(false);
        }

        // Get the menu obj to be activated.
        _menuToActivate = ReturnGameobjectLinkedWithMenuState(menuState);

        // Activate the new Menu.
        _menuToActivate.SetActive(true);

        // Make the new menu the "last" menu.
        _menuState = menuState;
 
    }

    private void CloseMenu()
    {
        _mainMenuCanvas.SetActive(false);
        _playerPointer.SetActive(false);
    }


    private void SetPosOfMenu()
    {
        // Grabs the forward direciton
        Vector3 forwardDirection = _playerHead.transform.forward;

        // ignore the y axis. So you just get the horizontal rotation.
        forwardDirection.y = 0;
        forwardDirection.Normalize();

        Vector3 distInDirectionPlayerIsLooking = forwardDirection * _menuDistFromPlayer;
        Vector3 posOffsetForMenu = new Vector3(distInDirectionPlayerIsLooking.x, 0, distInDirectionPlayerIsLooking.z);

        transform.position = _playerHead.transform.position + posOffsetForMenu;
        transform.LookAt(_playerHead.transform.position);

    }


    private GameObject ReturnGameobjectLinkedWithMenuState(Menus menuState)
    {
        GameObject objToReturn = null;

        switch (menuState)
        {
            case Menus.BaseMenu:
                objToReturn = _baseMenu;
                break;

            case Menus.HeightCalibration:
                objToReturn = _heightCalibrationMenu;
                break;

            case Menus.SettingsMenu:
                objToReturn = _settingsMenu;
                break;

        }

        return objToReturn;
    }

    private void OnMenuButtonPressed()
    {
        Debug.Log("MainMenuButtonPressed, MenuState: " + _menuState);

        if (!_mainMenuCanvas.activeSelf)
        {
            SetMenuActive(Menus.BaseMenu);
        }
        else
        {
            CloseMenu();
        }

    }


   
    ////  All of the menu buttons are below  \\\\
  


    // Main Menu Buttons \\

    public void BaseMenu_ReturnBtn()
    {
        Debug.Log("Return Clicked");
        CloseMenu();
    }

    public void BaseMenu_SettingsBtn()
    {
        Debug.Log("Settings Clicked");
        SetMenuActive( Menus.SettingsMenu);
    }

    public void BaseMenu_ReturnToLobbyBtn()
    {
        Debug.Log("ReturnToLobby Clicked");
        CloseMenu();
        LevelManager.SwitchLevel(LevelManager.Levels.Lobby);
    }



    // Settings Menu Buttons \\

    public void Settings_HeightCalBtn()
    {
        Debug.Log("HeightCalibration Clicked");
        SetMenuActive(Menus.HeightCalibration);
    }

    public void Settings_BackBtn()
    {
        SetMenuActive(Menus.BaseMenu);
    }


    // Height Cal Buttons \\

    public void HeightCal_BackBtn()
    {
        SetMenuActive(Menus.SettingsMenu);
    }
















}
