using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManagerScript : MonoBehaviour
{
    public GameObject InteractableObject;
    public GameObject CurrentlyDisplayedInteractable;

    [SerializeField] private GameObject MenuGO;
    [SerializeField] private GameObject MenuHeader;
    [SerializeField] private GameObject MenuSubHeader;
    public GameObject RightHandAttatchmentPoint;

    private bool IsMenuActive = false;
    private BuildingScript BuildingScripto;


    private void Update()
    {
        if (BuildingScripto.IsPlacingActive)
        {
            CurrentlyDisplayedInteractable.transform.rotation = Quaternion.LookRotation(new Vector3(1, 0, 0), Vector3.up); // Tower locks rotation looking up.
        }
     
    }


    private void Awake()
    {
        InputScripto.OnLeftMenuClick += LeftMenuClicked;
        BuildingScripto = GetComponent<BuildingScript>();
    }

    public void LeftMenuClicked()
    {
        if (!BuildingScript.IsBuidlingModeActive)
        {
            return;
        }

        
        if (IsMenuActive)
        {
            ActivateMenu(false);
        }

        else
        {
            ActivateMenu(true);
        }
    }

    public void ActivateMenu(bool Activate)
    {
        if (Activate)
        {
            if (LevelManager.CurrentLevel == LevelManager.Levels.Survival)
            {
                GetComponent<MovementScript>().GameWorld.GetComponentInChildren<GridGenerator>().GridSwitch(); // Change Grid
            }

            MenuGO.SetActive(true);
            IsMenuActive = true;
            GetComponent<BuildingScript>().IsBuildingMenuActive = true;
        }

        else
        {
            if (LevelManager.CurrentLevel == LevelManager.Levels.Survival)
            {
                GetComponent<MovementScript>().GameWorld.GetComponentInChildren<GridGenerator>().GridSwitch(); // Change Grid
            }

            MoveMinitureFromHandBackToMenu();
            MenuGO.SetActive(false);
            IsMenuActive = false;
            GetComponent<BuildingScript>().IsBuildingMenuActive = false;
        }
       
    }

    public void ChangeInteractable(GameObject NewInteractable)
    {
        if (CurrentlyDisplayedInteractable != null)
        {
            CurrentlyDisplayedInteractable.SetActive(false);
        }
        
        CurrentlyDisplayedInteractable = NewInteractable;
    }

    public void ChangeHeaderText(string NewText)
    {
        MenuHeader.GetComponent<Text>().text = NewText;
    }

    public void ChangeSubHeaderText(string NewText)
    {
        MenuSubHeader.GetComponent<Text>().text = NewText;
    }

    public void ActivateCurrentDisplayedInteractable(bool Activate)
    {
        if (Activate)
        {
            CurrentlyDisplayedInteractable.SetActive(true);
        }

        else
        {
            CurrentlyDisplayedInteractable.SetActive(false);
        }
    }

    public void ActivateHeader(bool Activate)
    {
        if (Activate)
        {
            MenuHeader.SetActive(true);
        }

        else
        {
            MenuHeader.SetActive(false);
        }
    }

    public void ActivateSubHeader(bool Activate)
    {
        if (Activate)
        {
            MenuSubHeader.SetActive(true);
        }

        else
        {
            MenuSubHeader.SetActive(false);
        }
    }

    public void MoveMinitureFromHandBackToMenu()
    {
        CurrentlyDisplayedInteractable.transform.position = InteractableObject.transform.position;
        CurrentlyDisplayedInteractable.transform.rotation = InteractableObject.transform.rotation;
        CurrentlyDisplayedInteractable.transform.SetParent(InteractableObject.transform);

    }

    public void MoveMinitureTowerFromMenuToHand(GameObject _MinitureTower)
    {
        CurrentlyDisplayedInteractable = _MinitureTower;
        CurrentlyDisplayedInteractable.transform.position = RightHandAttatchmentPoint.transform.position;
        CurrentlyDisplayedInteractable.transform.SetParent(RightHandAttatchmentPoint.transform);

    }

}
