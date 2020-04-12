using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTowerScript : MonoBehaviour
{

    public TowerSO[] Towers;
    public bool DisableTowerSwitch = false;

    public int              CurrentlySelectedTowerPositionInArray = 0;
    public  GameObject      CurrentlyDisplayedTower = null;
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

        CurrentlyDisplayedTower = _minitureTowers[0];

    }

    // Update is called once per frame
    void Update()
    {
        if (_menuManager.IsMenuTowerPlacementMode) // If currently in the tower menu load towers and text etc.
        {
            if (!PlacingTowersScript.IsPlacing) // Checks that the user is not currently placing a tower. Otherwise it would take the tower from their hand and throw it back in the menu.
            {
                UpdateTowerMenu();   // Should be optimised.
            }
           
        }
    }


    public void UpdateTowerMenu(bool overide = false)
    {
        if (overide)
        {
            string towerName = Towers[CurrentlySelectedTowerPositionInArray].Name;

            _menuManager.SwitchOutMenuCurrentlyDisplayedObject(_minitureTowers[CurrentlySelectedTowerPositionInArray]);
            _menuManager.SetHeaderText("Tower: " + towerName);

        }

        if (!CurrentlyDisplayedTower.activeSelf)
        {
            string towerName = Towers[CurrentlySelectedTowerPositionInArray].Name;
            _menuManager.SwitchOutMenuCurrentlyDisplayedObject(_minitureTowers[CurrentlySelectedTowerPositionInArray]); // Switches current displayed object to the correct tower and sets it active along with correcting its position and rotation.
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
        if (DisableTowerSwitch)
        {
            return;
        }

        if (!_menuManager.IsMenuTowerPlacementMode)
        {
            return;   // User is not selecting a tower, so they should not be able to switch between.
        }

        if (isLeft)
        {
            if (CurrentlySelectedTowerPositionInArray > 0)
            {
                CurrentlySelectedTowerPositionInArray -= 1;
            }
            else
            {
                CurrentlySelectedTowerPositionInArray = Towers.Length - 1;
            }      
        }
        else
        {
            if (CurrentlySelectedTowerPositionInArray < Towers.Length - 1)
            {
                CurrentlySelectedTowerPositionInArray += 1;
            }
            else
            {
                CurrentlySelectedTowerPositionInArray = 0;
            }
        }

        CurrentlyDisplayedTower = _minitureTowers[CurrentlySelectedTowerPositionInArray];
        UpdateTowerMenu(true);
    }

  

}
