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
    private int GridWidth;
    [SerializeField]
    private int GridHeight;
    [SerializeField]
    private GameObject TileInUseGO;
    [SerializeField]
    private Transform InUseTilesStorage;

    private List<Vector2> TilesInUseArray = new List<Vector2>();
    private List<List<GameObject>> TilesList = new List<List<GameObject>>();
    private float SF;

    public static bool GridCanBeUpdated = false;
    public static GridPoint[,] GridStatus;


    public static float GridSpacing = 0.25f;

  
    public void Start()
    {
        SF = GameObject.Find("GAMEMANAGER").GetComponent<BuildingScript>().SF;
        GridGenerator.GridStatus = new GridPoint[GridWidth, GridHeight];
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y< GridHeight; y++)
            {
                GridStatus[x, y] = new GridPoint();
            }
        }
        GenerateGrid(GridGenerator.GridSpacing);
        GenerateTiles();
    }

    public void Update()
    {
         if (GridGenerator.GridCanBeUpdated) // Updates when the build menu is opn
         {
            GenerateGrid(GridGenerator.GridSpacing);
         }
    }

    public void OnLoadInUseTiles(bool Load)
    {
        if (Load)
        {
            foreach (Vector2 Tile in TilesInUseArray)
            {
                GridGenerator.GridStatus[(int)Tile.x, (int)Tile.y].Tile.SetActive(true);
            }
        }
        else
        {
            foreach (Vector2 Tile in TilesInUseArray)
            {
                GridGenerator.GridStatus[(int)Tile.x, (int)Tile.y].Tile.SetActive(false);
            }
        }
    }

    public void SetGridPointAvailable(bool Available, Vector2 Point)
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

            GridGenerator.GridStatus[(int)Point.x, (int)Point.y].Available = true;
            GridGenerator.GridStatus[(int)Point.x, (int)Point.y].Tile.SetActive(false);
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
            GridGenerator.GridStatus[(int)Point.x, (int)Point.y].Available = false;
            GridGenerator.GridStatus[(int)Point.x, (int)Point.y].Tile.SetActive(true);
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
                NewTile.transform.position = GridGenerator.GridStatus[x,y].Position;
                NewTile.SetActive(false);
                GridGenerator.GridStatus[x, y].Tile = NewTile;
            }
        }
    }

    public void GenerateGrid(float GridSpacing)
    {   
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                GridStatus[x, y].Position = new Vector3(transform.position.x + (x * GridSpacing), transform.position.y, transform.position.z + (y * GridSpacing));
            }
        }
        OnLoadInUseTiles(true);
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
