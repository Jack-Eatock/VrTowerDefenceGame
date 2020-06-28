using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{


    [SerializeField] private int _entitiesToSpawn = 35;

    [Header("Chance (by Weight) entity spawns.")]

    [SerializeField] private int _chance_1x1_Tile;
    [SerializeField] private int _chance_2x2_Tile;
    [SerializeField] private int _chance_3x3_Tile;

    [SerializeField] private int[] EnvTilesByWeight = new int[4];

    [Header("What percent of left over tiles should have folliage?")]

    [SerializeField, Range(0, 1)] private float _percentFolliageToSpawn = 0.5f;


    enum Direction { UpRight, UpLeft, DownRight, DownLeft };
    private Direction[] _directionsArray = new Direction[] { Direction.UpRight, Direction.UpLeft, Direction.DownLeft, Direction.DownRight };

    // Tiles to load ready. \\

    private GameObject[] env_Foliage_Tiles;
    private GameObject[] _env_1x1_Tiles;
    private GameObject[] _env_2x2_Tiles;
    private GameObject[] _env_3x3_Tiles;

    // Required Variables \\

    private int _numOfEnv_1x1_Tiles = 0;
    private int _numOfEnv_2x2_Tiles = 0;
    private int _numOfEnv_3x3_Tiles = 0;

    private float _scaleFactor = 0;
    private int _gridHeight = 0;
    private int _gridWidth = 0;
    private bool _running = false;

    [Header("Variables needed to be asigned")]

    [SerializeField] private Material _grassMat; // Keeps the grass animation to the correct scale.

    [Header("Environement Tile Storage")]

    [SerializeField] private Transform _folliage_tile_Storage;
    [SerializeField] private Transform _1x1_Tile_Storage;
    [SerializeField] private Transform _2x2_Tile_Storage;
    [SerializeField] private Transform _3x3_Tile_Storage;




    // Multi Tile Variables \\

    private List<Vector2> _pointsMultiTileWillCover = new List<Vector2>();
    private Vector3 _centerPointOfMultiTileOffset;


    private void OnValidate()
    {
        EnvTilesByWeight = new int[] { _chance_1x1_Tile, _chance_2x2_Tile, _chance_3x3_Tile };
    }


    // Start is called before the first frame update
    void Start()
    {


        //EnvironmentMultiTileEntities

        Debug.Log("Loading Environement Tiles.");

        env_Foliage_Tiles = Resources.LoadAll<GameObject>("Env_Folliage_Tiles");
        Debug.Log("Successfully Loaded: (" + env_Foliage_Tiles.Length + ") Env_Folliage_Tiles presets");


        _env_1x1_Tiles = Resources.LoadAll<GameObject>("Env_1x1_Tiles");
        Debug.Log("Successfully Loaded: (" + _env_1x1_Tiles.Length + ") Env_1x1_Tiles presets");


        _env_2x2_Tiles = Resources.LoadAll<GameObject>("Env_2x2_Tiles");
        Debug.Log("Successfully Loaded: (" + _env_2x2_Tiles.Length + ") Env_2x2_Tiles presets");


        _env_3x3_Tiles = Resources.LoadAll<GameObject>("Env_3x3_Tiles");
        Debug.Log("Successfully Loaded: (" + _env_3x3_Tiles.Length + ") Env_3x3_Tiles presets");


        _gridWidth = GameObject.Find("Grid").GetComponent<GridGenerator>()._gridWidth;
        _gridHeight = GameObject.Find("Grid").GetComponent<GridGenerator>()._gridHeight;



        _running = true;
    }

    public void InitiateEnvironmentGeneration()
    {
        _scaleFactor = MovementScript.ScaleFactor / transform.localScale.x;
        Debug.Log("Generating Environment.... With Scale Factor:" + _scaleFactor);
    }


    // Update is called once per frame
    void Update()
    {
        _grassMat.SetFloat("Vector1_95D66403", MovementScript.ScaleFactor);


        if (_running && PathGenerator.PathGenerationComplete)
        {
            //Log("Check 1");

            // Calculate the chances.
            CalculateNumberOfEachTile();

            //Debug.Log("Check 2");

            // Place all the 3x3 tiles if there are any.
            AttemptToPlace3x3Tiles();

            //Debug.Log("Check 3");

            // Place all the 2x2 tiles if there are any.
            AttemptToPlace2x2Tiles();

            //Debug.Log("Check 4");

            // Place all the 1x1 tiles if there are any.
            AttemptToPlace1x1Tiles();

            //Debug.Log("Check 5");

            // Fill in gaps with Folliage.
            AttemptToPlaceFolliage();

            //Debug.Log("Check 6");

            _running = false;



            /*
            
            foreach (GridPoint _point in GridGenerator.GridStatus)
            {
       
                if (_point.Available)
                {
                   // _newGrass = GameObject.Instantiate(_grass);
                   // _newGrass.transform.SetParent(GameObject.Find("World").transform);
                   // _newGrass.transform.localScale = new Vector3(_scaleFactor, _scaleFactor * transform.localScale.x, _scaleFactor);
                   // _newGrass.transform.localPosition =_point.Position;
                }
            }


            //Change this. tTHJIS Big stupidf heqd fucktard!

            for (int i = 0; i < 16; i++)
            {
                SpawnMultiTileEntities();
            }

           


            for (int b = 0; b < _entitiesToSpawn; b++)
            {
                Vector2 point =  GenerateRandomPoint();  // Generates a random point on the  Grid (That is a available)

                GameObject _entityToSpawn = _environmentTilePresets[Random.Range(0,_environmentTilePresets.Length)].gameObject;   // Chooses a random tileset entity to spawn.
                GameObject entityToSpawn = GameObject.Instantiate(_entityToSpawn);                                                // Spawns the Object. Ready to be attached to the Grid.
                Vector3 posToSpawn = GridGenerator.GridStatus[(int)point.x, (int)point.y].Position;                               // Finds 3d position to spawn object. Based on the pos of the grid.

                GridGenerator.SetGridPointAvailable(false, point);                  // prevents the point on the grid having more entities spawned there.
                UtilitiesScript.AttachObjectToWorld(entityToSpawn, posToSpawn);     // Kinda obvious bro.
            }

            

            */

        }

    }




    private void CalculateNumberOfEachTile()
    {
        int randIndex;

        for (int i = 0; i < _entitiesToSpawn; i++)
        {
            randIndex = UtilitiesScript.RandomiseByWeight(EnvTilesByWeight); // for each entity to spawn. It returns the index of the type fo tile that should be spawned.

            switch (randIndex)
            {
                case 0: // 1x1 Tile.
                    _numOfEnv_1x1_Tiles++;
                    break;

                case 1: // 2x2 Tile.
                    _numOfEnv_2x2_Tiles++;
                    break;

                case 2: // 3x3 Tile.
                    _numOfEnv_3x3_Tiles++;
                    break;

            }

        }

        Debug.Log("Spawning 1x1 Tiles: (" + _numOfEnv_1x1_Tiles + ")");
        Debug.Log("Spawning 2x2 Tiles: (" + _numOfEnv_2x2_Tiles + ")");
        Debug.Log("Spawning 3x3 Tiles: (" + _numOfEnv_3x3_Tiles + ")");
    }



    private void AttemptToPlace3x3Tiles()
    {
        AttemptToPlaceAMultiTile(_numOfEnv_3x3_Tiles, _env_3x3_Tiles, 3);
    }

    private void AttemptToPlace2x2Tiles()
    {
        AttemptToPlaceAMultiTile(_numOfEnv_2x2_Tiles, _env_2x2_Tiles, 2);
    }

    private void AttemptToPlace1x1Tiles()
    {
        AttemptToPlaceSingleTile(_numOfEnv_1x1_Tiles, _env_1x1_Tiles, false);
    }

    private void AttemptToPlaceFolliage()
    {
        // Calculate how many tiles are still available.

        List<GridPoint> tilesThatAreAvailable = new List<GridPoint>();
        List<GridPoint> tileThatWeAreGoingToSet = new List<GridPoint>();

        foreach (GridPoint tile in GridGenerator.GridStatus)
        {
            if (tile.Available)
            {
                tilesThatAreAvailable.Add(tile);
            }
        }

        Debug.Log("Tiles that are available : " + tilesThatAreAvailable.Count);

        // Calculate how many of those tiles we actually want to fill.

        int numOfTilesToFill = Mathf.CeilToInt(tilesThatAreAvailable.Count * _percentFolliageToSpawn);

        // Randomly choose points out of the available points.

        for (int tileIndex = 0; tileIndex < numOfTilesToFill; tileIndex++)
        {
            // Choose a random tile to set.

            int randomTile = Random.Range(0, tilesThatAreAvailable.Count - 1);

            // Add that random tile to be used later.

            tileThatWeAreGoingToSet.Add(tilesThatAreAvailable[randomTile]);
            tilesThatAreAvailable.RemoveAt(randomTile);  // Remove the tile we added from the available list. So it is not added again.

        }

        Debug.Log("Tiles to be Created: " + tileThatWeAreGoingToSet.Count);

        // Now we know what tiles to chuck folliage on, create the folliage.

        GameObject newFolliage;

        foreach (GridPoint tile in tileThatWeAreGoingToSet)
        {
            newFolliage = GameObject.Instantiate(env_Foliage_Tiles[Random.Range(0, env_Foliage_Tiles.Length)]);
            UtilitiesScript.AttachObjectToWorld(newFolliage, tile.Position);

            newFolliage.transform.rotation *= (Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
            newFolliage.transform.SetParent(_folliage_tile_Storage);
        }


        // AttemptToPlaceSingleTile(_numOfEnv_DefaultGrass_Tiles, _env_DefaultGrass_Tiles, true);
    }

    private void AttemptToPlaceSingleTile(int numOfTiles, GameObject[] envTiles, bool isGrass)
    {
        Vector2 randomPoint;
        bool posPlaceable = false;

        int placementAttempt;
        int maxPlacementAttempts = 3;
        bool attemptToPlace;

        for (int entity = 0; entity < numOfTiles; entity++)
        {

            placementAttempt = 0;
            attemptToPlace = true;

            while (attemptToPlace) // allows it to attempt to place 3 times before giving up
            {

                randomPoint = GenerateRandomPoint();

                if (GridGenerator.GridStatus[(int)randomPoint.x, (int)randomPoint.y].Available)
                {
                    posPlaceable = true;
                }

                if (posPlaceable == true)
                {
                    attemptToPlace = false; // Dont want to keep placing it if it is placeable.

                    Vector3 posToPlace = GridGenerator.GridStatus[(int)randomPoint.x, (int)randomPoint.y].Position;

                    GameObject entityToPlace = envTiles[Random.Range(0, envTiles.Length)]; // Chooses a random 3x3 tile. POSSIBLY REMOVE - 1

                    entityToPlace = GameObject.Instantiate(entityToPlace);

                    UtilitiesScript.AttachObjectToWorld(entityToPlace, posToPlace);

                    entityToPlace.transform.rotation *= (Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));

                    // Set the points not available.
                    if (isGrass)
                    {
                        entityToPlace.transform.SetParent(_folliage_tile_Storage);
                    }

                    else
                    {
                        entityToPlace.transform.SetParent(_1x1_Tile_Storage);
                    }


                    GridGenerator.SetGridPointAvailable(false, randomPoint);

                }

                else
                {
                    placementAttempt++;

                    if (placementAttempt >= maxPlacementAttempts)
                    {
                        attemptToPlace = false;
                        Debug.LogWarning("Failed to place too many times!!");
                    }

                    else
                    {
                        Debug.Log("Tile Failed to Place");
                    }

                    // Add something to deal with if it cant be placed. (rare occasion.)


                }

            }
        }




    }

    private void AttemptToPlaceAMultiTile(int numOfTiles, GameObject[] envTiles, int multiTileWidth)
    {
        Vector2 randomPoint;
        bool posPlaceable;

        int placementAttempt;
        int maxPlacementAttempts = 3;
        bool attemptToPlace;

        for (int entity = 0; entity < numOfTiles; entity++)
        {

            //Reset the attempt to place loop.
            placementAttempt = 0;
            attemptToPlace = true;

            while (attemptToPlace) // allows it to attempt to place 3 times before giving up
            {

                randomPoint = GenerateRandomPoint();
                posPlaceable = CheckMultiTilePointsAvailable(randomPoint, multiTileWidth);

                if (posPlaceable == true)
                {
                    attemptToPlace = false; // Dont want to keep placing it if it is placeable.

                    //Debug.Log("Pos is placable");

                    Vector3 posToPlace = GridGenerator.GridStatus[(int)randomPoint.x, (int)randomPoint.y].Position;
                    posToPlace += _centerPointOfMultiTileOffset; // center the object.

                    // Debug.Log("Oi twat" + _centerPointOfMultiTileOffset);

                    GameObject entityToPlace = envTiles[Random.Range(0, envTiles.Length)]; // Chooses a random 3x3 tile. POSSIBLY REMOVE - 1

                    entityToPlace = GameObject.Instantiate(entityToPlace);

                    UtilitiesScript.AttachObjectToWorld(entityToPlace, posToPlace);

                    entityToPlace.transform.rotation *= (Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));

                    if (multiTileWidth == 2)
                    {
                        entityToPlace.transform.SetParent(_2x2_Tile_Storage);
                    }

                    else if (multiTileWidth == 3)
                    {
                        entityToPlace.transform.SetParent(_3x3_Tile_Storage);
                    }


                    // Set the points not available.

                    foreach (Vector2 point in _pointsMultiTileWillCover)
                    {
                        GridGenerator.SetGridPointAvailable(false, point);

                    }
                }

                else
                {
                    placementAttempt++;

                    if (placementAttempt >= maxPlacementAttempts)
                    {
                        attemptToPlace = false;
                        Debug.LogWarning("Failed to place too many times!!");
                    }

                    else
                    {
                        Debug.Log("Tile Failed to Place");
                    }

                    // Add something to deal with if it cant be placed. (rare occasion.)


                }

            }

        }



    }

    private bool CheckMultiTilePointsAvailable(Vector2 startingCords, int widthOfMultiTile)
    {
        List<Vector2> pointsMultiTileWillCover = new List<Vector2>();
        Vector3 centerPointOffset = new Vector3();
        bool isPlacable;

        foreach (Direction direction in _directionsArray)
        {
            //Debug.Log(direction);
            isPlacable = true;

            switch (direction)
            {

                case Direction.UpRight:

                    for (int x = (int)startingCords.x; x < startingCords.x + widthOfMultiTile; x++)
                    {
                        for (int y = (int)startingCords.y; y < startingCords.y + widthOfMultiTile; y++)  // loops through every
                        {
                            // Debug.Log(x + " " + y + " " + _gridWidth + " " + _gridHeight);

                            if (x > (_gridWidth - 1) || y >= (_gridHeight - 1) || x < 0 || y < 0)
                            {
                                //  Debug.Log("Out Of Bounds!");
                                isPlacable = false;
                            }

                            else if (!GridGenerator.GridStatus[x, y].Available)
                            {
                                // Debug.Log("Not available");
                                isPlacable = false;
                            }

                            pointsMultiTileWillCover.Add(new Vector2(x, y));
                        }
                    }

                    if (isPlacable) // If it can be placed calculate center point
                    {
                        int offsetAmount = widthOfMultiTile - 1;
                        centerPointOffset = new Vector3(offsetAmount, 0, offsetAmount);
                    }

                    break;

                case Direction.UpLeft:

                    for (int x = (int)startingCords.x; x > startingCords.x - widthOfMultiTile; x--)
                    {
                        for (int y = (int)startingCords.y; y < startingCords.y + widthOfMultiTile; y++)  // loops through every
                        {
                            //  Debug.Log(x + " " + y + " " + _gridWidth + " " + _gridHeight);

                            if (x > (_gridWidth - 1) || y >= (_gridHeight - 1) || x < 0 || y < 0)
                            {
                                //Debug.Log("Out Of Bounds!");
                                isPlacable = false;
                            }

                            else if (!GridGenerator.GridStatus[x, y].Available)
                            {
                                // Debug.Log("Not available");
                                isPlacable = false;
                            }

                            pointsMultiTileWillCover.Add(new Vector2(x, y));
                        }
                    }

                    if (isPlacable) // If it can be placed calculate center point
                    {
                        int offsetAmount = widthOfMultiTile - 1;
                        centerPointOffset = new Vector3(-offsetAmount, 0, offsetAmount);
                    }

                    break;

                case Direction.DownRight:

                    //Debug.Log("DownRight");

                    for (int x = (int)startingCords.x; x < startingCords.x + widthOfMultiTile; x++)
                    {
                        for (int y = (int)startingCords.y; y > startingCords.y - widthOfMultiTile; y--)  // loops through every
                        {
                            // Debug.Log(x + " " + y + " " + _gridWidth + " " + _gridHeight);
                            if (x > (_gridWidth - 1) || y >= (_gridHeight - 1) || x < 0 || y < 0)
                            {
                                //   Debug.Log("Out Of Bounds!");
                                isPlacable = false;
                            }

                            else if (!GridGenerator.GridStatus[x, y].Available)
                            {
                                // Debug.Log("Not available");
                                isPlacable = false;
                            }

                            pointsMultiTileWillCover.Add(new Vector2(x, y));
                        }
                    }

                    if (isPlacable) // If it can be placed calculate center point
                    {
                        int offsetAmount = widthOfMultiTile - 1;
                        centerPointOffset = new Vector3(offsetAmount, 0, -offsetAmount);
                    }

                    break;

                case Direction.DownLeft:

                    for (int x = (int)startingCords.x; x > startingCords.x - widthOfMultiTile; x--)
                    {
                        for (int y = (int)startingCords.y; y > startingCords.y - widthOfMultiTile; y--)  // loops through every
                        {
                            //Debug.Log(x + " " + y + " " + _gridWidth + " " + _gridHeight);
                            if (x > (_gridWidth - 1) || y >= (_gridHeight - 1) || x < 0 || y < 0)
                            {
                                // Debug.Log("Out Of Bounds!");
                                isPlacable = false;
                            }

                            else if (!GridGenerator.GridStatus[x, y].Available)
                            {
                                // Debug.Log("Not available");
                                isPlacable = false;
                            }

                            pointsMultiTileWillCover.Add(new Vector2(x, y));
                        }
                    }

                    if (isPlacable) // If it can be placed calculate center point
                    {
                        int offsetAmount = widthOfMultiTile - 1;
                        centerPointOffset = new Vector3(-offsetAmount, 0, -offsetAmount);
                    }

                    break;
            }


            if (isPlacable)
            {
                //Debug.Log("IsPLacable " + pointsMultiTileWillCover.Count);
                _pointsMultiTileWillCover = pointsMultiTileWillCover;
                _centerPointOfMultiTileOffset = centerPointOffset;

                return true;
            }
            else
            {
                pointsMultiTileWillCover.Clear();
            }


        }



        // No direction works
        // return Unplacable.

        return false;
    }



    private Vector2 GenerateRandomPoint()
    {
        int x = 0;
        int y = 0;

        bool flag = true;

        while (flag)
        {
            x = Random.Range(0, _gridWidth);
            y = Random.Range(0, _gridHeight);

            if (GridGenerator.GridStatus[x, y].Available)
            {
                //Debug.Log("Spawning Entity");
                flag = false;
            }

            else
            {
                // Debug.Log("Unable to spawn entity here.");
            }

        }

        return new Vector2(x, y);
    }







    /*


    private void SpawnMultiTileEntities()
    {
        bool flag = false;

        Vector2 randomGridPoint = Vector2.zero;
        Vector3 posToSpawn = Vector3.zero;
        Direction finalDirection = Direction.UpRight;

        List<Direction> directionsToCheck = new List<Direction>();
        directionsToCheck.Add(Direction.UpRight); directionsToCheck.Add(Direction.UpLeft); directionsToCheck.Add(Direction.DownLeft); directionsToCheck.Add(Direction.DownRight);

        while (!flag)
        {
            randomGridPoint = GenerateRandomPoint();

            foreach (Direction dir in directionsToCheck)
            {
                flag = CheckMultiTilePointsAvailable(randomGridPoint, 2);
                
                if (flag == true)
                {
                    finalDirection = dir;
                    break;
                }
            }
        }

        Debug.Log("Spawn Location Found: " + randomGridPoint + " With Direction: " + finalDirection);

        posToSpawn = GridGenerator.GridStatus[(int) randomGridPoint.x, (int) randomGridPoint.y].Position;
        

        Debug.Log("THIS" + _pointsMultiTileWillCover.Count);

        foreach( Vector2 point in _pointsMultiTileWillCover)
        {
            GridGenerator.SetGridPointAvailable(false, point);
            Debug.Log(point);

            //GridGenerator.GridStatus[(int)point.x, (int)point.y].Available = false;

        }

        GameObject pond =  GameObject.Instantiate(_Pond);
        pond.transform.rotation *= MultiTileRotation(finalDirection);
        UtilitiesScript.AttachObjectToWorld(pond, posToSpawn);
    }

    private Quaternion MultiTileRotation(Direction dir)
    {
        Quaternion valueToReturn = new Quaternion();

        switch (dir)
        {
            case Direction.UpRight:
                valueToReturn = Quaternion.Euler(0, 0, 0);
                break;

            case Direction.UpLeft:
                valueToReturn = Quaternion.Euler(0, -90, 0);
                break;

            case Direction.DownLeft:
                valueToReturn = Quaternion.Euler(0, -180, 0);
                break;

            case Direction.DownRight:
                valueToReturn = Quaternion.Euler(0, -210, 0);
                break;
        }

        return valueToReturn;
    }
    */





}
