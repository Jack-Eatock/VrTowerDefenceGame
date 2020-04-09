using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTowerScript : MonoBehaviour
{

    public TowerSO[] Towers;

    private int              _currentlySelectedTowerPositionInArray = 0;
    private GameObject       _currentlyDisplayedTower = null;
    private GameObject[]     _minitureTowers;

    private MenuManager _menuManager;


    // Start is called before the first frame update
    void Start()
    {
        InputScripto.OnDPLeftClick  += OnDPLeftClick;
        InputScripto.OnDPRightClick += OnDPRightClick;

        _menuManager = GetComponent<MenuManager>();
        _minitureTowers = new GameObject[Towers.Length];

        for (int SObject = 0; SObject < Towers.Length; SObject++)
        {
            _minitureTowers[SObject] = GameObject.Instantiate(Towers[SObject].MinitureVersion4Menu);
            _menuManager.SetUpDisplayableObject(_minitureTowers[SObject]);
        }

        _currentlyDisplayedTower = _minitureTowers[0];

    }

    // Update is called once per frame
    void Update()
    {
        if (_menuManager._isMenuInTowerPlacementMode) // If currently in the tower menu load towers and text etc.
        {
            UpdateTowerMenu();   // Should be optimised.
        }
    }


    public void UpdateTowerMenu(bool overide = false)
    {
        if (overide)
        {
            string towerName = Towers[_currentlySelectedTowerPositionInArray].Name;

            _menuManager.SwitchOutMenuCurrentlyDisplayedObject(_minitureTowers[_currentlySelectedTowerPositionInArray]);
            _menuManager.SetHeaderText("Tower: " + towerName);

        }

        if (!_currentlyDisplayedTower.activeSelf)
        {
            string towerName = Towers[_currentlySelectedTowerPositionInArray].Name;
            _menuManager.SwitchOutMenuCurrentlyDisplayedObject(_minitureTowers[_currentlySelectedTowerPositionInArray]); // Switches current displayed object to the correct tower and sets it active along with correcting its position and rotation.
            _menuManager.SetHeaderText("Tower: " + towerName);
        }
    }

    public void OnDPLeftClick()
    {
        UpdateCurrentlySelectedPos(true);
    }

    public void OnDPRightClick()
    {
        UpdateCurrentlySelectedPos(false);
    }

    public void UpdateCurrentlySelectedPos(bool isLeft)
    {
        if (!_menuManager._isMenuInTowerPlacementMode)
        {
            return;   // User is not selecting a tower, so they should not be able to switch between.
        }

        if (isLeft)
        {
            if (_currentlySelectedTowerPositionInArray > 0)
            {
                _currentlySelectedTowerPositionInArray -= 1;
            }
            else
            {
                _currentlySelectedTowerPositionInArray = Towers.Length - 1;
            }      
        }
        else
        {
            if (_currentlySelectedTowerPositionInArray < Towers.Length - 1)
            {
                _currentlySelectedTowerPositionInArray += 1;
            }
            else
            {
                _currentlySelectedTowerPositionInArray = 0;
            }
        }

        UpdateTowerMenu(true);
    }


}
