﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridPoint
{
    public bool Available = true;
    public Vector3 Position;
    public GameObject Tile;
}

public class GridGenerator : MonoBehaviour
{
    // Generating Grid Placement
    [SerializeField]
    private int GridWidth = 0;
    [SerializeField]
    private int GridHeight = 0;
    [SerializeField]
    private GameObject TileInUseGO = null;
    [SerializeField]
    private Transform InUseTilesStorage = null;

    [SerializeField] private Material GrassGridMat;
    [SerializeField] private Material GrassMat;
    private bool IsGroundGrid = false;

    public static List<Vector2> TilesInUseArray = new List<Vector2>();
    private float SF;

    public static bool GridCanBeUpdated = false;
    public static GridPoint[,] GridStatus;


    public static float GridSpacing = 0.25f;

    public void InitiateGridGeneration()
    {
        Debug.Log("Initiating Grid Generation...");

        TilesInUseArray = new List<Vector2>();
        GridCanBeUpdated = false;

        SF = MovementScript.SF;

        GridStatus = new GridPoint[GridWidth, GridHeight];
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                GridStatus[x, y] = new GridPoint();
            }
        }

        GenerateGrid(GridSpacing);

        Debug.Log("[Completed] Generated Grid!");
        Debug.Log("Generating Tiles...");
        GenerateTiles();

        Debug.Log("[Completed] Generated Tiles");
    }


    public static void OnLoadInUseTiles(bool Load)
    {
        if (Load)
        {
            foreach (Vector2 Tile in TilesInUseArray)
            {
                GridStatus[(int)Tile.x, (int)Tile.y].Tile.SetActive(true);
            }
        }
        else
        {
            foreach (Vector2 Tile in TilesInUseArray)
            {
                GridStatus[(int)Tile.x, (int)Tile.y].Tile.SetActive(false);
            }
        }
    }

    public static void SetGridPointAvailable(bool Available, Vector2 Point)
    {
        if (Available)
        {
            int Counter = 0;
            foreach (Vector2 Tile in TilesInUseArray)
            {
                if (Tile == Point)
                {
                    TilesInUseArray.RemoveAt(Counter);
                }
            }

            GridStatus[(int)Point.x, (int)Point.y].Available = true;
            GridStatus[(int)Point.x, (int)Point.y].Tile.SetActive(false);
        }
        else
        {
            bool Flag = false;
            foreach (Vector2 Tile in TilesInUseArray)
            {
                if (Tile == Point)
                {
                    Flag = true;
                }
            }

            if (!Flag)
            {
                TilesInUseArray.Add(Point);
            }
            GridStatus[(int)Point.x, (int)Point.y].Available = false;
            GridGenerator.OnLoadInUseTiles(true);
            //GridStatus[(int)Point.x, (int)Point.y].Tile.SetActive(true);
        }
    }

    public void GenerateTiles()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                GameObject NewTile = GameObject.Instantiate(TileInUseGO);
                NewTile.transform.SetParent(InUseTilesStorage);
                NewTile.transform.localScale = new Vector3(SF, SF, SF);
                NewTile.transform.localPosition = GridStatus[x,y].Position;
                NewTile.SetActive(false);
                GridStatus[x, y].Tile = NewTile;
            }
        }


    }

    public void GenerateGrid(float GridSpacing)
    {   
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                GridStatus[x, y].Position = new Vector3(transform.position.x + (x * GridSpacing), transform.localPosition.y, transform.position.z + (y * GridSpacing));
            }
        }
        OnLoadInUseTiles(true);
    }



    public void GridSwitch()
    {
        GameObject Ground = GameObject.Find("Ground");

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



    /* public void OnDrawGizmos()
     {
         Gizmos.color = Color.yellow;

         foreach (Vector3 Point in GridGenerator.GridPoints)
         {
             Gizmos.DrawSphere(Point, 0.1f);
         }

     }
     */
}
