using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCanvas;   // Canvas
    [SerializeField] private GameObject _playerHead; // Camera
    [SerializeField] private GameObject _baseMenu;

    [SerializeField] private float _menuDistFromPlayer = 0.5f; 


    enum Menus {ClosedMenu ,BaseMenu, SettingsMenu, HeightCalibration};

    private Menus _menuState = Menus.ClosedMenu;

    // Start is called before the first frame update
    void Start()
    {
        InputScripto.OnMainMenuPressed += OnMenuButtonPressed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BtnReturnClicked()
    {
        Debug.Log("Return Clicked");
        _mainMenuCanvas.SetActive(false);
        _menuState = Menus.ClosedMenu;
    }

    public void BtnSettingsClicked()
    {
        Debug.Log("Settings Clicked");
    }

    public void BtnReturnToLobbyClicked()
    {
        Debug.Log("ReturnToLobby Clicked");
        _mainMenuCanvas.SetActive(false);
        _menuState = Menus.ClosedMenu;
        LevelManager.SwitchLevel(LevelManager.Levels.Lobby);
    }

    private void OnMenuButtonPressed()
    {
        Debug.Log("MainMenuButtonPressed, MenuState: " + _menuState);

        if (_menuState == Menus.ClosedMenu)
        {
            SetMenuActive(Menus.BaseMenu);
        }
        else
        {
            _mainMenuCanvas.SetActive(false);
            _menuState = Menus.ClosedMenu;
        }
        

    }

    private void SetMenuActive(Menus menuState)
    {
        Vector3 distInDirectionPlayerIsLooking = _playerHead.transform.forward * _menuDistFromPlayer;
        Vector3 posOffsetForMenu = new Vector3(distInDirectionPlayerIsLooking.x,0, distInDirectionPlayerIsLooking.z);

        transform.position = _playerHead.transform.position + posOffsetForMenu;
        transform.LookAt(_playerHead.transform.position);


        

        switch (menuState)
        {
            case Menus.BaseMenu:
                _mainMenuCanvas.SetActive(true);
                break;

            case Menus.HeightCalibration:
                break;

            case Menus.SettingsMenu:
                break;

        }
        _menuState = menuState;
    }
}
