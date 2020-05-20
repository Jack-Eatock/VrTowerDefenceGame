using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//cleared\\

public class GridPoint
{
    public bool Available = true;
    public Vector3 Position;
    public GameObject Tile;
}

public class GridGenerator : MonoBehaviour
{

    public static List<Vector2> TilesInUseArray = new List<Vector2>();
    public static bool GridCanBeUpdated = false;
    public static GridPoint[,] GridStatus;
    public static float LocalGridSpacing = 0;

    // Generating Grid Placement
    [SerializeField]  private int _gridWidth = 0;
    [SerializeField]  private int _gridHeight = 0;
    [SerializeField]  private GameObject _tileInUseGO = null;
    [SerializeField]  private Transform _inUseTilesStorage = null;

    private float _scaleFactor;


    [Header("Grid Switch Variables")] // Switching grid ON/OFF Visual only \\

     public Material GrassGridMat = null;
     public Material GrassMat = null;


    public void Start()
    {
       
    }

    public void InitiateGridGeneration()
    {
        _scaleFactor = MovementScript.ScaleFactor;
       UpdateGridSpacing(_gridHeight);

        Debug.Log("GridSpacing : " + LocalGridSpacing);
        
        GridStatus = new GridPoint[_gridWidth, _gridHeight];

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                GridStatus[x, y] = new GridPoint();
            }
        }
        GenerateGrid(LocalGridSpacing);
        GenerateTiles();
    }



    public void Update()
    {
        UpdateGridSpacing(_gridHeight);
    }

    public static void UpdateGridSpacing(int gridHeight)
    {
        LocalGridSpacing = ((GameObject.Find("Ground").transform.localScale.x * MovementScript.ScaleFactor) / gridHeight);
    }

    public static void OnLoadInUseTiles(bool loadInUseTiles)
    {
        if (loadInUseTiles)
        {
            foreach (Vector2 tile in TilesInUseArray)
            {
                GridStatus[(int)tile.x, (int)tile.y].Tile.SetActive(true);
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

    public static void SetGridPointAvailable(bool setPointAvailable, Vector2 pointToSet)
    {
        if (setPointAvailable)
        {
            int counter = 0;
            foreach (Vector2 tile in TilesInUseArray)
            {
                if (tile == pointToSet)
                {
                    TilesInUseArray.RemoveAt(counter);
                }
            }

            GridStatus[(int)pointToSet.x, (int)pointToSet.y].Available = true;
            GridStatus[(int)pointToSet.x, (int)pointToSet.y].Tile.SetActive(false);
        }
        else
        {
            bool flag = false;
            foreach (Vector2 tile in TilesInUseArray)
            {
                if (tile == pointToSet)
                {
                    flag = true;
                }
            }

            if (!flag)
            {
                TilesInUseArray.Add(pointToSet);
            }

            GridStatus[(int)pointToSet.x, (int)pointToSet.y].Available = false;
            GridGenerator.OnLoadInUseTiles(true);
            //GridStatus[(int)Point.x, (int)Point.y].Tile.SetActive(true);
        }
    }

    public void GenerateTiles()
    {
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                GameObject newTile = GameObject.Instantiate(_tileInUseGO);
                newTile.transform.SetParent(_inUseTilesStorage);
                newTile.transform.localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);
                newTile.transform.localPosition = GridStatus[x,y].Position;
                newTile.SetActive(false);
                GridStatus[x, y].Tile = newTile;
            }
        }
    }

    public void GenerateGrid(float gridSpacing)
    {   
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                GridStatus[x, y].Position = new Vector3(transform.localPosition.x + (x * gridSpacing), transform.localPosition.y, transform.localPosition.z + (y * gridSpacing));
            }
        }

        OnLoadInUseTiles(true);
    }


    // Function called to switch the ground material from having a grid or not. 
    public static void GridSwitch(bool activateGrid)
    {
        if (activateGrid)
        {
            Material _grassMat = GameObject.Find("Grid").GetComponent<GridGenerator>().GrassMat;
            GridGenerator.OnLoadInUseTiles(false);
            GameObject.Find("Ground").GetComponent<Renderer>().material = _grassMat;
        }
        else
        {
            Material _grassGridMat = GameObject.Find("Grid").GetComponent<GridGenerator>().GrassGridMat;
            GridGenerator.OnLoadInUseTiles(true);
            GameObject.Find("Ground").GetComponent<Renderer>().material = _grassGridMat;
        }
    }


    /*public void OnDrawGizmos()
     {
         Gizmos.color = Color.yellow;

         foreach (Vector3 Point in GridGenerator.GridPoints)
         {
             Gizmos.DrawSphere(Point, 0.1f);
         }

     }
     */
}
