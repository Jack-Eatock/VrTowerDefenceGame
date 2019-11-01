﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class BuildingScript : MonoBehaviour
{
    // General Variables \\

    public static bool MenuControllsDisabled = true;

    [SerializeField]
    private GameObject Player = null;

    public GameObject Ground;
    public GameObject TowerMenuPos;
    public GameObject RightHandGO;

    public float GridSize = 40;

    // Placing Towers
    [SerializeField]
    private GameObject CancelGO = null;
    [SerializeField]
    private GameObject PlacedTowersStorage = null;

    private GameObject GridGO = null;
    private GameObject GameWorld = null;
    private GameObject NewTower = null;

    private bool TowerBeingPlaced = false;
    private bool NewTowerHidden = false;
    private bool CurrentlyDisplayedTowerHidden = true;
    private bool CanBePlaced = false;

    private int CurrentPositionPosX;
    private int CurrentPositionPosY;
    private int GridWidth;
    private int GridHeight;

    public float SF;

    private Vector3 CurrentPosition;


    //Menu 
    [SerializeField]
    private GameObject HandMenuGO = null;
    [SerializeField]
    private GameObject BuildingMenuGo = null;
    [SerializeField]
    private GameObject GeneralMenuGO = null;
    [SerializeField]
    private GameObject Text = null;

    private GameObject[] MinitureTowers;
    private Text NameText;
    private bool BuildMenuActive = false;
    private bool GeneralMenuActive = false;

    //Towers
    public TowerSO[] Towers;
    private GameObject CurrentlyDisplayedTower = null;
    private int CurrentlyDisplayedTowerPos = 0;

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
    private Material GrassGridMat = null;
    [SerializeField]
    private Material GrassMat = null;

    private bool IsGroundGrid = false;



    public void Start()
    {
        GridGO = GameObject.Find("Grid");
        GameWorld = Player.GetComponent<MovementScript>().GameWorld;
        NameText = Text.GetComponent<Text>();

        SF = MovementScript.LocalSF;

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

        SF = MovementScript.LocalSF;
        GridGenerator.GridSpacing = ((Ground.transform.localScale.x * SF) / GridSize);

        if (TowerBeingPlaced)
        {
            if (!CancelGO.activeSelf)
            {
                CancelGO.SetActive(true);
            }

            CanBePlaced = false;

            CurrentlyDisplayedTower.transform.rotation = Quaternion.LookRotation(new Vector3(1, 0, 0), Vector3.up); // Tower locks rotation looking up.
            if (RightHandGO.GetComponent<OnCollisionScript>().IsColliding)
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



                GridHeight = GridGenerator.GridStatus.GetLength(1);
                GridWidth = GridGenerator.GridStatus.GetLength(0);

                for (int x = 0; x < GridWidth; x++)
                {
                    for (int y = 0; y < GridHeight; y++)
                    {
                        Vector3 Point = GridGenerator.GridStatus[x, y].Tile.transform.position;
                        if (Point.x < PosX + GridSpacing && Point.x > PosX - GridSpacing)
                        {
                            if (Point.z < PosZ + GridSpacing && Point.z > PosZ - GridSpacing)
                            {
                                CurrentPosition = new Vector3(Point.x, PosY, Point.z);

                                if (GridGenerator.GridStatus[x, y].Available)
                                {
                                    CanBePlaced = true;
                                    CurrentPositionPosX = x;
                                    CurrentPositionPosY = y;
                                }
                                else if (CanBePlaced)
                                {
                                    Debug.Log("NotPlaceable");
                                    CanBePlaced = false;
                                }

                                break;
                            }
                        }
                    }
                }

                if (!NewTower)
                {
                    CurrentlyDisplayedTower.SetActive(false);
                    CurrentlyDisplayedTowerHidden = true;
                    NewTower = GameObject.Instantiate(Towers[CurrentlyDisplayedTowerPos].TowerGO);
                    NewTower.transform.position = CurrentPosition;
                    NewTower.transform.localScale = new Vector3(SF, SF, SF);

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
    


    public void ActivateMenu(bool Activate , int MenuType)
    {
        if (Activate)
        {
            if (MenuType == 0) // Building Menu Set Active
            {
                HandMenuGO.SetActive(true);
                BuildingMenuGo.SetActive(true); 
                GenerateRemoveMiniTowerFromMenu(CurrentlyDisplayedTowerPos, true);
                BuildMenuActive = true;
                GridSwitch();
            }

            else if (MenuType == 1) // Start Wave Menu. Active.
            {
                if (BuildMenuActive)
                {
                    ActivateMenu(false, 0);
                }
                HandMenuGO.SetActive(true);
                GeneralMenuGO.SetActive(true);
                GeneralMenuActive = true;
               
            }

        }
        else
        {
            if (MenuType == 0) // Building Menu Set Not active.
            {
                if (TowerBeingPlaced)
                {
                    SetTowerBeingPlacedTrueFalse(false);
                }
                HandMenuGO.SetActive(false);
                BuildingMenuGo.SetActive(false);
                GenerateRemoveMiniTowerFromMenu(0, false);
                BuildMenuActive = false;
                GridSwitch();
            }

            else if (MenuType == 1) // Start Wave Menu. Not Active.
            {
                HandMenuGO.SetActive(false);
                GeneralMenuGO.SetActive(false);
                GeneralMenuActive = false;
            }

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

    public void CircleRadius(Vector2 StartingCords, int Radius)
    {
        List<Vector2> Cords = new List<Vector2>();
        int Offset = 0;
        for (int Counter = 1; Counter <= (Radius + 1); Counter++)
        {
            if (Counter == Radius + 1)
            {
                Offset = 1;
            }

            for (int x = (int)StartingCords.x - Counter + Offset; x <= StartingCords.x + Counter - Offset; x++)
            {
                
                Cords.Add(new Vector2(x, StartingCords.y + ((Radius +1) - Counter)));
                Cords.Add(new Vector2(x, StartingCords.y - ((Radius +1) - Counter)));
            }
        }

        foreach (Vector2 Cord in Cords)
        {
            if (Cord.x < 40 && Cord.x >= 0 && Cord.y < 40 && Cord.y >= 0)
            {
                GridGenerator.SetGridPointAvailable(false, Cord);
            }
      
        }
        
    }


    public void OnPlaceOrCancel(bool Place)
    {
        if (Place)
        {
            SphereCollider SphereCol = NewTower.AddComponent<SphereCollider>();
            SphereCol.center = new Vector3(0, 0.5f, 0);
            SphereCol.isTrigger = true;

            OnCollisionScript TempColScript = NewTower.AddComponent<OnCollisionScript>();
            TempColScript.CollisionType = 3;

            TowerScript TempTowerScript = NewTower.AddComponent<TowerScript>();
            TempTowerScript.TowerProperties = Towers[CurrentlyDisplayedTowerPos];

            Vector3 TilePosition = GridGenerator.GridStatus[CurrentPositionPosX, CurrentPositionPosY].Tile.transform.position;
            NewTower.transform.position = new Vector3( TilePosition.x, GameWorld.transform.position.y, TilePosition.z);
            NewTower.transform.SetParent(PlacedTowersStorage.transform);
            CanBePlaced = false;
            // Debug.Log(CurrentPositionPosX + " : " + CurrentPositionPosY);
            CircleRadius(new Vector2(CurrentPositionPosX,CurrentPositionPosY), 2);

        }
        else
        {
            Destroy(NewTower);
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
            HandMenuGO.SetActive(true);
            GenerateRemoveMiniTowerFromMenu(CurrentlyDisplayedTowerPos, true);
            BuildMenuActive = true;
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
            GridGenerator.OnLoadInUseTiles(false);
            Ground.GetComponent<Renderer>().material = GrassMat;
            IsGroundGrid = false;
        }
        else
        {
            GridGenerator.OnLoadInUseTiles(true);
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
            if (TowerMenuPos.GetComponent<OnCollisionScript>().IsColliding && BuildMenuActive)
            {
                SetTowerBeingPlacedTrueFalse(true);
            }
            else if (TowerMenuPos.GetComponent<OnCollisionScript>().IsColliding && GeneralMenuActive && PathGenerator.PathGenerationComplete)
            {
                gameObject.GetComponent<EnemySpawner>().StartWave();
                ActivateMenu(false, 1);
                BuildingScript.MenuControllsDisabled = false;
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
            if (BuildMenuActive)
            {
                SwitchDisplayedTower(false);
            }
        }

    }
    public void OnDPRightClick(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        if (!TowerBeingPlaced)
        {
            if (BuildMenuActive)
            {
                SwitchDisplayedTower(true);
            }
        }

    }
    public void MenuDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
        if (!MenuControllsDisabled)
        {
            if (BuildMenuActive)
            {

                ActivateMenu(false, 0);
            }
            else
            {
                if (GeneralMenuActive)
                {
                    ActivateMenu(false, 1);
                }

                ActivateMenu(true, 0);
            }
        }
    }
    public void MenuUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources sources)
    {
      
        
    }

}
