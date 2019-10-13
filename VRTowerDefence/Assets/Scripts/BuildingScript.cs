using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class BuildingScript : MonoBehaviour
{
    // General Variables \\
    [SerializeField]
    public GameObject Ground;
    [SerializeField]
    private GameObject Player;

    public GameObject TowerMenuPos;
    public GameObject RightHandGO;
    public float GridSize = 40;

    // Placing Towers
    [SerializeField]
    private GameObject CancelGO;

    public GameObject PlacedTowersStorage;
    private GameObject GameWorld;
    private bool CanBePlaced = false;

    private  float SF;
    private bool TowerBeingPlaced = false;
    private bool NewTowerHidden = false;
    private bool CurrentlyDisplayedTowerHidden = true;
    private GameObject NewTower = null;
    private Vector3 CurrentPosition;

    //Menu Variables
    private bool MenuActive = false;
    public GameObject MenuGameObject;
    private GameObject[] MinitureTowers;
    public GameObject Text;
    private Text NameText;
    
    //Towers
    public TowerSO[] Towers;
    private GameObject CurrentlyDisplayedTower;
    private int CurrentlyDisplayedTowerPos;

    // Steam VR ACtions
    public SteamVR_Action_Boolean GrabL;
    public SteamVR_Action_Boolean GrabR;
    public SteamVR_Action_Boolean Menu;
    public SteamVR_Action_Boolean DPEast;
    public SteamVR_Action_Boolean DPWest;

    public SteamVR_Input_Sources LeftHand; // Left Controller - Set in Engine.
    public SteamVR_Input_Sources RightHand; // Right Controller - Set in Engine.

    // Switching grid ON/OFF Visual only \\
    [SerializeField]
    private Material GrassGridMat;
    [SerializeField]
    private Material GrassMat;


    private bool IsGroundGrid = false;



    public void Start()
    {
        GameWorld = Player.GetComponent<MovementScript>().GameWorld;
        NameText = Text.GetComponent<Text>();
        SF = Player.GetComponent<MovementScript>().SF;

        MinitureTowers = new GameObject[Towers.Length];
        for (int SObject = 0; SObject < Towers.Length; SObject++)
        {
            MinitureTowers[SObject] = GameObject.Instantiate(Towers[SObject].MinitureVersion4Menu, TowerMenuPos.transform.position, TowerMenuPos.transform.rotation);
            MinitureTowers[SObject].transform.SetParent(TowerMenuPos.transform);
            MinitureTowers[SObject].gameObject.SetActive(false);
        }

        GrabR.AddOnStateDownListener(TriggerDownRight, RightHand);
        GrabR.AddOnStateUpListener(TriggerUpRight, RightHand);

        GrabL.AddOnStateDownListener(TriggerDownLeft, LeftHand);
        GrabL.AddOnStateUpListener(TriggerUpLeft, LeftHand);

        DPWest.AddOnStateDownListener(OnDPLeftClick, LeftHand);
        DPEast.AddOnStateDownListener(OnDPRightClick, LeftHand);

        Menu.AddOnStateDownListener(MenuDown, LeftHand);
        Menu.AddOnStateUpListener(MenuUp, LeftHand);

    }


    public void Update()
    {
        SF = Player.GetComponent<MovementScript>().SF;
        GridGenerator.GridSpacing = ((Ground.transform.localScale.x  * SF) / GridSize);

        if (MenuActive)
        {
        }

        if (TowerBeingPlaced)
        {
            if (!CancelGO.activeSelf)
            {
                CancelGO.SetActive(true);
            }

            CurrentlyDisplayedTower.transform.rotation = Quaternion.LookRotation(new Vector3(1, 0, 0), Vector3.up);
            if (CurrentlyDisplayedTower.transform.position.y - Ground.transform.position.y  < 0.55)
            {
                if (!CurrentlyDisplayedTowerHidden)
                {
                    CurrentlyDisplayedTower.SetActive(false);
                    CurrentlyDisplayedTowerHidden = true;
                }

                // Locking the Tower to the grid.

                float PosX = CurrentlyDisplayedTower.transform.position.x;
                float PosZ = CurrentlyDisplayedTower.transform.position.z;
                float PosY = GameWorld.transform.position.y;
                float GridSpacing = GridGenerator.GridSpacing / 2f;
                int Counter = 0;
                bool Placeable = false;
                foreach (Vector3 Point in GridGenerator.GridPoints)
                {
                    if (Point.x < PosX + GridSpacing && Point.x > PosX - GridSpacing)
                    {
                        if (Point.z < PosZ + GridSpacing && Point.z > PosZ - GridSpacing)
                        {
                            CurrentPosition = new Vector3 (Point.x,PosY,Point.z);
                            Counter++;
                            Placeable = true;
                            break;
                        }
                    }
                }

                if (Placeable)
                {
                    CanBePlaced = true;
                    Placeable = false;
                }

                if (!NewTower)
                {
                    CurrentlyDisplayedTower.SetActive(false);
                    CurrentlyDisplayedTowerHidden = true;
                    NewTower = GameObject.Instantiate(Towers[CurrentlyDisplayedTowerPos].TowerGO);
                    NewTower.transform.position = CurrentPosition;
                    NewTower.transform.localScale = new Vector3 (SF, SF, SF);

                }

                else
                {
                    NewTower.transform.localScale = new Vector3(SF, SF, SF);
                    NewTower.transform.position = CurrentPosition;
                   
                    if (NewTowerHidden)
                    {
                        NewTower.SetActive(true);
                        NewTowerHidden = false;
                    }
                }



            }
            else if (NewTower)
            {
                NewTower.SetActive(false);
                NewTowerHidden = true;
                CurrentlyDisplayedTower.SetActive(true);
                CurrentlyDisplayedTowerHidden = false;
            }

        }
        else
        {
            if (CancelGO.activeSelf)
            {
                CancelGO.SetActive(false);
            }
        }
    }


    public void ActivateMenu(bool Activate)
    {
        if (Activate)
        {
            MenuGameObject.SetActive(true);
            GenerateRemoveMiniTowerFromMenu(CurrentlyDisplayedTowerPos, true);
            MenuActive = true;
            GridSwitch();
        }
        else
        {
            if (TowerBeingPlaced)
            {
                SetTowerBeingPlacedTrueFalse(false);
            }
            MenuGameObject.SetActive(false);
            GenerateRemoveMiniTowerFromMenu(0, false);
            MenuActive = false;
            GridSwitch();
        }

    }

    public void SetCurrentTowerPos(int Active = 0)
    {  
        // Sets tower in the Menu. 
        if (Active == 0)
        {
            CurrentlyDisplayedTower.transform.SetParent(TowerMenuPos.transform);
            CurrentlyDisplayedTower.transform.position = TowerMenuPos.transform.position;
            CurrentlyDisplayedTower.transform.rotation = TowerMenuPos.transform.rotation;
        }

        //Sets tower as child of Hand.
        else if (Active == 1)
        {
            CurrentlyDisplayedTower.gameObject.transform.position = RightHandGO.transform.position;
            CurrentlyDisplayedTower.gameObject.transform.rotation = MinitureTowers[CurrentlyDisplayedTowerPos].transform.rotation;
            CurrentlyDisplayedTower.gameObject.transform.SetParent(RightHandGO.transform);
        }

    }

    public void OnPlaceOrCancel(bool Place)
    {
        if (Place)
        {
            NewTower.transform.SetParent(PlacedTowersStorage.transform);
            CanBePlaced = false;
        }
        NewTower = null;
        TowerBeingPlaced = false;
        SetCurrentTowerPos(0);
        GenerateRemoveMiniTowerFromMenu(CurrentlyDisplayedTowerPos, true);
        //SetPlacingOnOff(true);
    }

    public void SetTowerBeingPlacedTrueFalse(bool Activate)
    {
        if (Activate)
        {
            TowerBeingPlaced = true;
            MenuGameObject.SetActive(true);
            GenerateRemoveMiniTowerFromMenu(CurrentlyDisplayedTowerPos, true);
            MenuActive = true;
            SetCurrentTowerPos(1);
        }
        else
        {
            TowerBeingPlaced = false;
            if (NewTower)
            {
                Destroy(NewTower);
                NewTower = null;
            }
            SetCurrentTowerPos(0);
            GenerateRemoveMiniTowerFromMenu(0, false);
        }
    }
    

    public void SwitchDisplayedTower(bool Right)
    {
        CurrentlyDisplayedTower.SetActive(false);
        if (Right)
        {
            
            if (CurrentlyDisplayedTowerPos + 1 > MinitureTowers.Length -1)
            {
                CurrentlyDisplayedTowerPos = 0;
            }
            else
            {
                CurrentlyDisplayedTowerPos += 1;
            }
        }
        else
        {
            if (CurrentlyDisplayedTowerPos - 1 < 0)
            {
                CurrentlyDisplayedTowerPos = MinitureTowers.Length -1;
            }
            else
            {
                CurrentlyDisplayedTowerPos -= 1;
            }
        }
        GenerateRemoveMiniTowerFromMenu(CurrentlyDisplayedTowerPos, true);

    }

    public void GenerateRemoveMiniTowerFromMenu(int x, bool Generate)
    {   
        if (CurrentlyDisplayedTower != null)
        {
            CurrentlyDisplayedTower.SetActive(false);
            CurrentlyDisplayedTower = null;
        }
        if (Generate)
        {
            MinitureTowers[x].gameObject.SetActive(true);
            CurrentlyDisplayedTower = MinitureTowers[x].gameObject;
            NameText.text = Towers[CurrentlyDisplayedTowerPos].Name;
        }
    }

    // Function called to switch the ground material from having a grid or not. 
    public void GridSwitch()
    {
        if (IsGroundGrid)
        {
            Ground.GetComponent<Renderer>().material = GrassMat;
            IsGroundGrid = false;
        }
        else
        {
            Ground.GetComponent<Renderer>().material = GrassGridMat;
            IsGroundGrid = true;
        }
    }

    // If the ground is made larger use this function to scale the grid along with it. If not the gird will look strange.
    public void UpdateGroundScale()
    {
        float TempGroundScaleX = Ground.transform.localScale.x;
        float TempGroundScaleZ = Ground.transform.localScale.z;
        GrassGridMat.mainTextureScale = new Vector2(TempGroundScaleX, TempGroundScaleZ);
    }



    public void TriggerDownRight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        if (!TowerBeingPlaced)
        {
            if (TowerMenuPos.GetComponent<OnCollisionScript>().IsColliding)
            {
                SetTowerBeingPlacedTrueFalse(true);
            }
        }

        else if (TowerBeingPlaced && TowerMenuPos.GetComponent<OnCollisionScript>().IsColliding)
        {
            OnPlaceOrCancel(false);
        }

        else if (CanBePlaced)
        {
            OnPlaceOrCancel(true);
        }
       
    }
    public void TriggerUpRight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {

    }
    public void TriggerDownLeft(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {

    }
    public void TriggerUpLeft(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {

    }


    public void OnDPLeftClick(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        if (!TowerBeingPlaced)
        {
            if (MenuActive)
            {
                SwitchDisplayedTower(false);
            }
        }

    }
    public void OnDPRightClick(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        if (!TowerBeingPlaced)
        {
            if (MenuActive)
            {
                SwitchDisplayedTower(true);
            }
        }

    }
    public void MenuDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        if (MenuActive)
        {
            ActivateMenu(false);
        }
        else
        {
            ActivateMenu(true);
        }
      
    }
    public void MenuUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
      
        
    }

}
