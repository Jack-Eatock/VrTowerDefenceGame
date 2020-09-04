using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//cleared\\

public class GridPoint
{
    public Vector2 GridPos;
    public bool Inuse = false;

    public bool Available = true;
    public Vector3 Position;
    public GameObject Tile;
}

public class GridGenerator : MonoBehaviour
{

    public GameObject Ground;
    public SphereCollider RadiusDisplayer;

    // new var's
    public int GridDiamater;
    public static List<Vector2> GridPointsInUse = new List<Vector2>();

    public static float InitialScaleFactor = 1;
    public static List<Vector2> TilesInUseArray = new List<Vector2>();
    public static bool GridCanBeUpdated = false;
    public static GridPoint[,] GridStatus;
    public static float LocalGridSpacing = 0;

    public static List<Vector2> GridPointsOnCircumferance = new List<Vector2>();

    /*
    // Generating Grid Placement
    public int _gridWidth = 0;
    public int _gridHeight = 0;
    */

    [SerializeField]  private GameObject _tileInUseGO = null;
    [SerializeField] private GameObject _tileStorage = null;



    [Header("Grid Switch Variables")] // Switching grid ON/OFF Visual only \\

     public Material GrassGridMat = null;


    public void Start()
    {
    }

    void OnValidate()
    {
        float newScale = (GridDiamater * 2f) / 10f; 
        Ground.transform.localScale = new Vector3(newScale, 0, newScale);
        RadiusDisplayer.radius = GridDiamater;
    }

    public void ClearSlateLocal()
    {
        GridPointsInUse.Clear();
        InitialScaleFactor = 1;
        TilesInUseArray.Clear();
        GridCanBeUpdated = false;
        GridStatus = null;
        LocalGridSpacing = 0;
        GridPointsOnCircumferance.Clear();
    }

    public void InitiateGridGeneration()
    {
        /*

        UpdateGridSpacing(_gridHeight);

        //Debug.Log("GridSpacing : " + LocalGridSpacing);
        
        GridStatus = new GridPoint[_gridWidth, _gridHeight];

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                GridStatus[x, y] = new GridPoint();
            }
        }

        */

        ClearSlateLocal();

        UpdateGridSpacing(GridDiamater);

        GridStatus = new GridPoint[GridDiamater, GridDiamater];

        // Now define those grid points.
        Vector2 gridCentrePoint = new Vector2(0.5f * GridDiamater, 0.5f * GridDiamater);
        float circleRadius = (GridDiamater / 2);
        float distFromCentre;

        for (int x = 0; x < GridDiamater; x++)
        {
            for (int y = 0; y < GridDiamater; y++)
            {
                GridStatus[x, y] = new GridPoint();
                GridStatus[x, y].GridPos = new Vector2(x, y);

                distFromCentre = (gridCentrePoint - new Vector2(x, y)).magnitude;

                // If the point is within the circle enable it. Otherwise it is ignored.
                if (distFromCentre < circleRadius)
                {
                    GridStatus[x, y].Inuse = true;
                    GridPointsInUse.Add(new Vector2(x, y));

                    if (circleRadius - distFromCentre <= 1)
                    {
                        GridPointsOnCircumferance.Add(new Vector2(x, y));
                    }
                }
                

            }

        }

       
        GenerateGrid(LocalGridSpacing);
        GenerateTiles();

        GameModeSurvivalScript.GenerationTicker = 1;
    }


    public void Update()
    {
        UpdateGridSpacing(GridDiamater);
    }

    public static void UpdateGridSpacing(int gridHeight)
    {
        LocalGridSpacing = (((GameObject.Find("Ground").transform.localScale.x * 10) * MovementScript.ScaleFactor) / gridHeight);
        //Debug.Log("Local grid spacing" + LocalGridSpacing);
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

    public static void SetGridPointAvailable(bool setPointAvailable, Vector2 pointToSet, bool display = true)
    {
        //Debug.Log("PointToSet" + pointToSet);

        if (!GridStatus[(int) pointToSet.x, (int) pointToSet.y].Inuse)
        {
            return;
        }

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
           

            //GridStatus[(int)Point.x, (int)Point.y].Tile.SetActive(true);
        }
    }

    public static void UpdateTilesLoaded(bool Display)
    {

        if (Display)
        {
            GridGenerator.OnLoadInUseTiles(true);
        }

        else
        {
            GridGenerator.OnLoadInUseTiles(false);
        }

    }


    public void GenerateTiles()
    {
        foreach (GridPoint point in GridStatus)
        {
            if (!point.Inuse)
            {
                continue;
            }

            GameObject newTile = GameObject.Instantiate(_tileInUseGO);


            UtilitiesScript.AttachObjectToWorld(newTile, point.Position);
            newTile.transform.SetParent(_tileStorage.transform);


            newTile.SetActive(false);
            point.Tile = newTile;
        }


        /*
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
               
            }
        }
        */
        /*
        foreach (Vector2 cord in Cords)
        {
            
        }
        */
    }

    public void GenerateGrid(float gridSpacing)
    {   

        foreach (GridPoint point in GridStatus)
        {
            if (point.Inuse) // Point is in the circle.
            {
                point.Position = new Vector3((transform.localPosition.x - GridDiamater * .5f * gridSpacing) + (point.GridPos.x  * gridSpacing), transform.localPosition.y, (transform.localPosition.z - GridDiamater * .5f * gridSpacing) + (point.GridPos.y * gridSpacing));
            }
        }

        /*
        for (int x = 0; x < _gridDiamater; x++)
        {
            for (int y = 0; y < _gridDiamater; y++)
            {
                GridStatus[x, y].Position = new Vector3(transform.localPosition.x + (x * gridSpacing), transform.localPosition.y, transform.localPosition.z + (y * gridSpacing));
            }
        }
        

        foreach (Vector2 cord in Cords)
        {
            GridStatus[(int) cord.x,(int) cord.y].Position = new Vector3(transform.localPosition.x + (cord.x * gridSpacing), transform.localPosition.y, transform.localPosition.z + (cord.y * gridSpacing));
        }

        */

        //OnLoadInUseTiles(false);
    }


    // Function called to switch the ground material from having a grid or not. 
    public static void GridSwitch(bool activateGrid)
    {


        if (activateGrid)
        {
            Material _grassGridMat = GameObject.Find("Grid").GetComponent<GridGenerator>().GrassGridMat;
            _grassGridMat.SetInt("Boolean_35F01A6F", 0);
            UpdateTilesLoaded(false);
        }
        else
        {
            Material _grassGridMat = GameObject.Find("Grid").GetComponent<GridGenerator>().GrassGridMat;
            _grassGridMat.SetInt("Boolean_35F01A6F", 1);
            UpdateTilesLoaded(true);
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
