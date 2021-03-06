﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingScript : MonoBehaviour
{







}



/*
public class BuildingScript : GameScript
{
    // General Variables \\
    [Header("Tweekables")]

    public float SF;
    public float GridSize = 40;
    public bool Running = false;

    //

    [Header("Placing Towers")]

    [SerializeField] private GameObject CancelGO = null;
    [SerializeField] private GameObject PlacedTowerStorageGO = null;

    private GameObject PlacedTowersStorage = null;
    private GameObject GameWorld = null;
    public GameObject NewTower = null;

    public bool TowerBeingPlaced = false;
    private bool NewTowerHidden = false;
    private bool CurrentlyDisplayedTowerHidden = true;
    public bool CanBePlaced = false;
    public static bool MenuControllsDisabled = true;

    //

    [Header("General References")]

    [SerializeField] private GameObject Player = null;
    public GameObject Ground;
    public GameObject TowerMenuPos;
    public GameObject RightHandGO;

    private int CurrentPositionPosX;
    private int CurrentPositionPosY;
    private int GridWidth;
    private int GridHeight;

    private Vector3 CurrentPosition;

    //

    [Header("Menu Variables")]

    [SerializeField] private GameObject HandMenuGO = null;
    [SerializeField] private GameObject BuildingMenuGo = null;
    [SerializeField] private GameObject GeneralMenuGO = null;
    [SerializeField] private GameObject Text = null;
    [SerializeField] private GameObject PointsText = null;


    
    private Text NameText;
    private Text PointsTextDisplayer;
    public bool BuildMenuActive = false;
    public bool GeneralMenuActive = false;
    public bool SufficientFunds = false;
    private int _points;



    public void InitatiateBuildingScript()
    {
        Debug.Log("Initiating BuidlingSCript.");

        GameWorld = Player.GetComponent<MovementScript>().GameWorld;
        NameText = Text.GetComponent<Text>();
        PointsTextDisplayer = PointsText.GetComponent<Text>();


        SF = MovementScript.ScaleFactor;

        
        Running = true;
    }


    public void Update()
    {
        if (!Running)
        {
            return;
        }



        if (BuildMenuActive)
        {
            if (Points != _points)
            {
                _points = Points;
                UpdateBuildMenuText();
            }
        }

        SF = MovementScript.ScaleFactor;
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


    public void UpdateBuildMenuText()
    {
        NameText.text = Towers[CurrentlyDisplayedTowerPos].Name;
        int PointsAfter = Points - Towers[CurrentlyDisplayedTowerPos].Cost;
        if (PointsAfter >= 0)
        {
            PointsTextDisplayer.text = "Points: " + _points + " (-" + Towers[CurrentlyDisplayedTowerPos].Cost + ") Purchasable!";
            SufficientFunds = true;
        }

        else
        {
            PointsTextDisplayer.text = "Points: " + _points + " (-" + Towers[CurrentlyDisplayedTowerPos].Cost + ") Insufficient Funds!";
            SufficientFunds = false;
        }
    }

    }


    public void OnPlaceOrCancel(bool Place)
    {
        if (Place)
        {
            Points -= Towers[CurrentlyDisplayedTowerPos].Cost;

            SphereCollider SphereCol = NewTower.AddComponent<SphereCollider>();
            SphereCol.center = new Vector3(0, 0.5f, 0);
            SphereCol.isTrigger = true;

            OnCollisionScript TempColScript = NewTower.AddComponent<OnCollisionScript>();
            TempColScript.CollisionType = 3;

            TowerScript TempTowerScript = NewTower.AddComponent<TowerScript>();
            TempTowerScript.TowerProperties = Towers[CurrentlyDisplayedTowerPos];

            Vector3 TilePosition = GridGenerator.GridStatus[CurrentPositionPosX, CurrentPositionPosY].Tile.transform.position;
            NewTower.transform.position = new Vector3(TilePosition.x, GameWorld.transform.position.y, TilePosition.z);

            if(PlacedTowersStorage == null)
            {
                PlacedTowersStorage = GameObject.Instantiate(PlacedTowerStorageGO, GameWorld.transform);
            }

            NewTower.transform.SetParent(PlacedTowersStorage.transform);
            CanBePlaced = false;
            // Debug.Log(CurrentPositionPosX + " : " + CurrentPositionPosY);
            CircleRadius(new Vector2(CurrentPositionPosX, CurrentPositionPosY), 2);

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

    public void RightTriggerClick()
    {
        if (!TowerBeingPlaced)
        {
            if (TowerMenuPos.GetComponent<OnCollisionScript>().IsColliding && BuildMenuActive) // Move the tower from menu to the users hand.
            {
                SetTowerBeingPlacedTrueFalse(true);
            }

            else if (TowerMenuPos.GetComponent<OnCollisionScript>().IsColliding && GeneralMenuActive && PathGenerator.PathGenerationComplete) // Start Next wave.
            {
                gameObject.GetComponent<EnemySpawner>().StartWave();
                ActivateMenu(false, 1);
                BuildingScript.MenuControllsDisabled = false;
            }
        }

        else if (CanBePlaced) // IF they can place the Tower, Place tower.
        {
            if (SufficientFunds)
            {
                OnPlaceOrCancel(true);
            }
            else
            {
                StartCoroutine(UtilitiesScript.ObjectBlinkColour(NewTower, Color.red, 0.15f)); // Flash red , User cant place the tower.
            }

        }

        else if (TowerBeingPlaced && TowerMenuPos.GetComponent<OnCollisionScript>().IsColliding) // If they click on the X and we know that the tower cant be placed. Cancel.
        {
            OnPlaceOrCancel(false);
        }

        else
        {
            StartCoroutine(UtilitiesScript.ObjectBlinkColour(NewTower, Color.red, 0.15f)); // Flash red , User cant place the tower.
        }

    }

}
*/