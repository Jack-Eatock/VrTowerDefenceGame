using System.Collections;
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

    public static List<Vector2> TilesInUseArray = new List<Vector2>();
    private float SF;

    public static bool GridCanBeUpdated = false;
    public static GridPoint[,] GridStatus;


    public static float GridSpacing = 0.25f;

  
    public void Start()
    {
        SF = GameObject.Find("GAMEMANAGER").GetComponent<BuildingScript>().SF;
        GridStatus = new GridPoint[GridWidth, GridHeight];
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y< GridHeight; y++)
            {
                GridStatus[x, y] = new GridPoint();
            }
        }
        GenerateGrid(GridSpacing);
        GenerateTiles();
    }

    public void Update()
    {
         if (GridCanBeUpdated) // Updates when the build menu is opn
         {
            GenerateGrid(GridSpacing);
         }
    }

    public void OnLoadInUseTiles(bool Load)
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
                NewTile.transform.position = GridStatus[x,y].Position;
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
