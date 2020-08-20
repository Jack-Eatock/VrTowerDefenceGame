using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenuScript : MonoBehaviour
{

    public static bool MainMenuDisabled = false;

    // Required References \\

    [SerializeField] private GameObject _mainMenuCanvas;   // Canvas
    [SerializeField] private GameObject _playerHead; // Camera
    [SerializeField] private GameObject _playerPointer; // pointer

    // Menu Gameobjects \\

    [SerializeField] private GameObject _baseMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _heightCalibrationMenu;
    [SerializeField] private GameObject _audioSettingsMenu;
    [SerializeField] private GameObject _preferencesMenu;
    [SerializeField] private GameObject _videoSettingsMenu;

    // Tweakable \\

    [SerializeField] private float _menuDistFromPlayer = 0.5f;

    // Variables \\


    public enum Menus {BaseMenu, SettingsMenu, HeightCalibration, PreferencesMenu, AudioSettingsMenu, VideoSettingsMenu};

    private Menus _menuState;
    private GameObject _lastMenuObj;
    private GameObject _menuToActivate;

    [SerializeField] private MenuManager _menuManager;

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

            GameScript.PauseGame(true);
            _menuManager.MenuDisabled = true;

            MovementScript.MovementControllsDisabled = true;

            SetPosOfMenu();
            _playerPointer.SetActive(true);
            _mainMenuCanvas.SetActive(true);

            if (_menuManager.IsMenuActive)
            {
                _menuManager.SetMenuActive(false);
            }

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
        GameScript.PauseGame(false);
        MovementScript.MovementControllsDisabled = false;
        _menuManager.MenuDisabled = false;
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

            case Menus.PreferencesMenu:
                objToReturn = _preferencesMenu;
                break;

            case Menus.AudioSettingsMenu:
                objToReturn = _audioSettingsMenu;
                break;

            case Menus.VideoSettingsMenu:
                objToReturn = _videoSettingsMenu;
                break;

        }

        return objToReturn;
    }

    private void OnMenuButtonPressed()
    {
        if (MainMenuDisabled)
        {
            Debug.Log("Menu Disabled");
            return;
        }

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
 

    // Menu Utility Buttons

    public void MenuUtilities_LoadMenu(int menu)
    {
        switch (menu)
        {
            case 0:
                SetMenuActive(Menus.BaseMenu);
                break;

            case 1:
                SetMenuActive(Menus.SettingsMenu);
                break;

            case 2:
                SetMenuActive(Menus.HeightCalibration);
                break;

            case 3:
                SetMenuActive(Menus.PreferencesMenu);
                break;

            case 4:
                SetMenuActive(Menus.AudioSettingsMenu);
                break;

            case 5:
                SetMenuActive(Menus.VideoSettingsMenu);
                break;
    
        }
    }

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










}
