using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputScripto : MonoBehaviour
{
    public delegate void RightTriggerClick();
    public delegate void LeftTriggerClick();
    public delegate void DPLeftClick();
    public delegate void DPRightClick();
    public delegate void LeftMenuPress();
    public delegate void MainMenuPress();
    public delegate void SnapTurnLeft();
    public delegate void SnapTurnRight();

    public static event SnapTurnRight       OnSnapTurnRight;
    public static event SnapTurnLeft        OnSnapTurnLeft;
    public static event MainMenuPress       OnMainMenuPressed;
    public static event RightTriggerClick   OnRightTriggerClick;
    public static event LeftTriggerClick    OnLeftTriggerClick;
    public static event DPLeftClick         OnDPLeftClick;
    public static event DPRightClick        OnDPRightClick;
    public static event LeftMenuPress       OnLeftMenuPress;

    [Header("SteamVR References")]

    // Steam VR ACtions
    public SteamVR_Action_Boolean GrabL;
    public SteamVR_Action_Boolean GrabR;
    public SteamVR_Action_Boolean Menu;
    public SteamVR_Action_Boolean DPEast;
    public SteamVR_Action_Boolean DPWest;
    public SteamVR_Action_Boolean GripL;
    public SteamVR_Action_Boolean GripR;
    public SteamVR_Action_Boolean MainMenu;
    public SteamVR_Action_Boolean A_SnapTurnRight;
    public SteamVR_Action_Boolean A_SnapTurnLeft;



    public SteamVR_Input_Sources LeftHand; // Left Controller - Set in Engine.
    public SteamVR_Input_Sources RightHand; // Right Controller - Set in Engine.


    // References

    public static bool RightTriggerDown = false;
    public static bool LeftTriggerDown = false;

    // Start is called before the first frame update
    void Start()
    {
        A_SnapTurnLeft.AddOnStateDownListener(OnSnapTurnLeftPress, RightHand);
        A_SnapTurnRight.AddOnStateDownListener(OnSnapTurnRightPress, RightHand);

        GrabR.AddOnStateDownListener(TriggerDownRight, RightHand);
        GrabR.AddOnStateUpListener(TriggerUpRight, RightHand);

        GrabL.AddOnStateDownListener(TriggerDownLeft, LeftHand);
        GrabL.AddOnStateUpListener(TriggerUpLeft, LeftHand);

        DPWest.AddOnStateDownListener(OnDPLeftPress, LeftHand);
        DPEast.AddOnStateDownListener(OnDPRightPress, LeftHand);

        Menu.AddOnStateDownListener(MenuDown, LeftHand);
        Menu.AddOnStateUpListener(MenuUp, LeftHand);

        GripL.AddOnStateDownListener(GripDownL, LeftHand);
        GripL.AddOnStateUpListener(GripUpL, LeftHand);

        GripR.AddOnStateDownListener(GripDownR, RightHand);
        GripR.AddOnStateUpListener(GripUpR, RightHand);

        MainMenu.AddOnStateDownListener(MainMenuDown, RightHand);
        MainMenu.AddOnStateUpListener(MainMenuUp, RightHand);
    }
    public void MainMenuDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {

        OnMainMenuPressed();

    }


    public void MainMenuUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {

    }



    public void TriggerDownRight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
      
       OnRightTriggerClick();
        
       

    }


    public void TriggerUpRight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        
    }
    public void TriggerDownLeft(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        OnLeftTriggerClick();
    }
    public void TriggerUpLeft(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
    }





    public void OnSnapTurnRightPress(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {

        OnSnapTurnRight();

    }


    public void OnSnapTurnLeftPress(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        OnSnapTurnLeft();
    }








    public void OnDPLeftPress(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        OnDPLeftClick();

        /*
        if (!_buildingScripto.TowerBeingPlaced)
        {
            if (_buildingScripto.BuildMenuActive)
            {
                _buildingScripto.SwitchDisplayedTower(false);
            }
        }
        */
    }
    public void OnDPRightPress(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        OnDPRightClick();
        /*
        if (!_buildingScripto.TowerBeingPlaced)
        {
            if (_buildingScripto.BuildMenuActive)
            {
                _buildingScripto.SwitchDisplayedTower(true);
            }
        }
        */
    }
    public void MenuDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        OnLeftMenuPress();

        /*
        if (!BuildingScript.MenuControllsDisabled)
        {
            if (_buildingScripto.BuildMenuActive)
            {

                _buildingScripto.ActivateMenu(false, 0);
            }
            else
            {
                if (_buildingScripto.GeneralMenuActive)
                {
                    _buildingScripto.ActivateMenu(false, 1);
                }

                _buildingScripto.ActivateMenu(true, 0);
            }
        }
        */

    }

    // Inputs for The Movement Script \\


    public void MenuUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {


    }
    public void GripUpL(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        //Debug.Log("Left Grip released");
        MovementScript.IsGrippingL = false;
    }
    public void GripDownL(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        //Debug.Log("Left Grip Pressed");
        MovementScript.IsGrippingL = true;
    }
    public void GripUpR(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        //Debug.Log("Right Grip released");
        MovementScript.IsGrippingR = false;
    }
    public void GripDownR(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        //Debug.Log("Right Grip Pressed");
        MovementScript.IsGrippingR = true;
    }

}
