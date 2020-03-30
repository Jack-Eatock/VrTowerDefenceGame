using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputScripto : MonoBehaviour
{
    public delegate void RightTriggerClick();
    public delegate void LeftTriggerClick();
    public delegate void DPRightClick();
    public delegate void DPLeftClick();
    public delegate void LeftMenuClick();


    public static event RightTriggerClick  OnRightTriggerClick;
    public static event LeftTriggerClick   OnLeftTriggerClick;
    public static event DPLeftClick        OnLeftDPClick;
    public static event DPRightClick       OnRightDPClick;
    public static event LeftMenuClick      OnLeftMenuClick;


    [Header("SteamVR References")]

    // Steam VR ACtions
    public SteamVR_Action_Boolean GrabL;
    public SteamVR_Action_Boolean GrabR;
    public SteamVR_Action_Boolean Menu;
    public SteamVR_Action_Boolean DPEast;
    public SteamVR_Action_Boolean DPWest;
    public SteamVR_Action_Boolean GripL;
    public SteamVR_Action_Boolean GripR;

    public SteamVR_Input_Sources LeftHand; // Left Controller - Set in Engine.
    public SteamVR_Input_Sources RightHand; // Right Controller - Set in Engine.


    // References

    private BuildingScript BuildingScripto;

    // Start is called before the first frame update
    void Start()
    {

        BuildingScripto = gameObject.GetComponent<BuildingScript>();

        GrabR.AddOnStateDownListener(TriggerDownRight, RightHand);
        GrabR.AddOnStateUpListener(TriggerUpRight, RightHand);

        GrabL.AddOnStateDownListener(TriggerDownLeft, LeftHand);
        GrabL.AddOnStateUpListener(TriggerUpLeft, LeftHand);

        DPWest.AddOnStateDownListener(OnDPLeftClick, LeftHand);
        DPEast.AddOnStateDownListener(OnDPRightClick, LeftHand);

        Menu.AddOnStateDownListener(MenuDown, LeftHand);
        Menu.AddOnStateUpListener(MenuUp, LeftHand);

        GripL.AddOnStateDownListener(GripDownL, LeftHand);
        GripL.AddOnStateUpListener(GripUpL, LeftHand);

        GripR.AddOnStateDownListener(GripDownR, RightHand);
        GripR.AddOnStateUpListener(GripUpR, RightHand);
    }

    // Inputs for the Building Script \\

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

    public void OnDPLeftClick(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        OnLeftDPClick();

      /*  if (!BuildingScripto.TowerBeingPlaced)
        {
            if (BuildingScripto.BuildMenuActive)
            {
                BuildingScripto.SwitchDisplayedTower(false);
            }
        }
        */

    }
    public void OnDPRightClick(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        OnRightDPClick();

        /*
        if (!BuildingScripto.TowerBeingPlaced)
        {
            if (BuildingScripto.BuildMenuActive)
            {
                BuildingScripto.SwitchDisplayedTower(true);
            }
        }
        */
    }
    public void MenuDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        OnLeftMenuClick();

        /*
        if (!BuildingScript.MenuControllsDisabled)
        {
            if (BuildingScripto.BuildMenuActive)
            {

                BuildingScripto.ActivateMenu(false, 0);
            }
            else
            {
                if (BuildingScripto.GeneralMenuActive)
                {
                    BuildingScripto.ActivateMenu(false, 1);
                }

                BuildingScripto.ActivateMenu(true, 0);
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
