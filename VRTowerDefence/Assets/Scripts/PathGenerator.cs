using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Cleaned \\


public class PathTile
{
    public Vector2 Cords;
    public int Direction; // 1, up, 2 Left, 3 Right, 4 Down.
}

public class PathGenerator : MonoBehaviour
{
    private int _gridWidth;
    private int _minWidth;
    private int _maxWidth;

    [Range (0,1)] [SerializeField] private float _minRangeForSplitPercentageAddition = 0.8f;
    [Range(0, 1)] [SerializeField] private float _maxRangeForSplitPercentageAddition = 0.3f;

    [SerializeField] private bool _onlyTheMainPathSplits = false;
    [SerializeField] private int _widthOffsetFromEdge = 2;



    public int MaxAttemptsBeforeAssumingCrashed = 10;
    public int CurrentNumOfAttempts = 0;

    [Range(0, 6)] [SerializeField] private int _numOfPaths = 1;

    public static List<List<PathTile>> Paths = new List<List<PathTile>>();                 // x axis is the different paths created, y  axis is the points the path uses. 


    private Vector2 _currentStartingCords = new Vector2(); // When the pathway splits it will have a new set of starting cords.
    private int _currentStartingDirection = 1;

    private List<PathTile> pathwayToSplit = new List<PathTile>();
    private int _pointInPathwayToSplit = 0;



    /// <summary>
    /// 
    /// --- How is this going to work, ( Making the pathway split ) ---
    /// 
    /// + Run through as usual and create the first pathway. 
    /// + After the first pathway is created, check if we should create another pathway. ( Chance of creating another and have a max number of paths, just in case )
    /// + Add the first pathway (CurrentPathTiles) To the list of all the pathways. 
    /// + Clear the CurrentPathTiles, choose a random path out of ( Paths ) and choose a random point and start the original process but starting cords of the random point selected.
    /// - Now that the next path is generated, add the cords of the original path leading up to when it splits.
    /// + Now we have the entire second pathway so add it to paths, clear the CurrentPath and repeat.
    /// 
    /// </summary>



    // Old 


    public static List<PathTile> CurrentPathTiles = new List<PathTile>();

    [SerializeField] private Vector2 _startingCords = new Vector2 (18,18);

   

    [SerializeField] private int _pathSpawnChanceUp    = 1;
    [SerializeField] private int _pathSpawnChanceLeft  = 1;   // Keep them in this order!
    [SerializeField] private int _pathSpawnChanceRight = 1;
    [SerializeField] private int _pathSpawnChanceDown  = 1 ;

    [SerializeField] private int[] _pathSpawnChanceArray;

    private Vector2 _currentCord = Vector2.zero;
    private int     _lastDirection = 1;
    private int     _maxIterations = 400;
    private int     _counter = 0;
    private bool    _loop = true;
    private int     _failureCount = 0;

 
    public GameObject PathStorage;
    public GameObject StraightPathGo;
    public GameObject CornerPieceGo;

    private GridGenerator _gridGenerator;

    private float _scaleFactor;

    [SerializeField] private int _pathEndNum = 0;

    private void Start()
    {

        _pathSpawnChanceArray = new int[4];
        _pathSpawnChanceArray[0] = _pathSpawnChanceUp;   // This order is important!
        _pathSpawnChanceArray[1] = _pathSpawnChanceLeft;
        _pathSpawnChanceArray[2] = _pathSpawnChanceRight;
        _pathSpawnChanceArray[3] = _pathSpawnChanceDown;



        _pathEndNum = GameObject.Find("Grid").GetComponent<GridGenerator>().GridDiamater - 1;
        _gridGenerator = GameObject.Find("Grid").GetComponent<GridGenerator>();

        _gridWidth = _gridGenerator.GridDiamater;

        _minWidth = _widthOffsetFromEdge;
        _maxWidth = _gridWidth - _widthOffsetFromEdge;

       // Debug.Log("GridGen width" + _gridGenerator._gridWidth);
        //Debug.Log(_pathEndNum);
    }

    public void InitiatePathGeneration()
    {
        _currentStartingCords = _startingCords;

        _scaleFactor = MovementScript.ScaleFactor; // / transform.localScale.x;
        Debug.Log("Generating Virtual Path.... With Scale Factor:" + _scaleFactor);

        ResetCurrentPathGeneration(_startingCords, 1);

        //_running = true;
    }



    // Update is called once per frame
    void Update()
    {

        if (_loop)
        {
            if (_counter >= _maxIterations)
            {
                Debug.Log("More than max iterations");

                if (CurrentNumOfAttempts > 2)
                {
                    Debug.Log("Split failed, going to try another location");
                    SplitPathway();
                }

                if (CurrentNumOfAttempts > MaxAttemptsBeforeAssumingCrashed)
                {
                    _loop = false;
                    Debug.LogError("Path failed to generate too many times!");
                    CompletelyFinished();
                }

                CurrentNumOfAttempts++;

                // They may be stuck in a corner. So flipping the side they start on can help. But obviously only do this if it is not the intial Path

                if (_currentStartingCords != _startingCords)
                {
                    switch (_currentStartingDirection)
                    {
                        default:
                            break;

                        case 1: // Up
                            _currentStartingDirection = 4;
                            break;

                        case 2: // left
                            _currentStartingDirection = 3;
                            break;

                        case 3: // Right
                            _currentStartingDirection = 2;
                            break;

                        case 4: // Down
                            _currentStartingDirection = 1;
                            break;
                    }
                }

                ResetCurrentPathGeneration(_currentStartingCords, _currentStartingDirection);       // Too many attempts, Start again.
            }

            else if (_currentCord.y != _pathEndNum)
            {
                Worm();                             // Calculates the pathway.
            }

            else
            {

                FinishedGeneratingCurrentPathway(); // Saves all of the path data for the current pathway.

                // Check if we need to split the pathway.

                if (Paths.Count != _numOfPaths)
                {
                    SplitPathway();
                  
                }

                else
                {
                    CompletelyFinished();
                }

            }

            _counter++;
        }

    }

    void SplitPathway()
    {
        // Okay so we need to split the pathway. But which pathway and where? 
        // Choose which pathway first.

        if (!_onlyTheMainPathSplits)
        {
            pathwayToSplit = Paths[Random.Range(0, Paths.Count)];
        } 

        else
        {
            pathwayToSplit = Paths[0];
        }


        // Now where on the pathway to split..? 

        if (pathwayToSplit.Count != 0 && pathwayToSplit.Count >= 2)
        {
            PathTile newStartTile = RandomSplitInPathAndCaluculateDirection();
            ResetCurrentPathGeneration(newStartTile.Cords, newStartTile.Direction);
        }

        else
        {
            Debug.LogError("Pathway generated without any pathtiles. List is empty. : " + pathwayToSplit);
        }
    }

    PathTile RandomSplitInPathAndCaluculateDirection()
    {
        int midPointOfPathwayToSplit = pathwayToSplit.Count / 2;

        int minRangeForSplit = midPointOfPathwayToSplit - Mathf.FloorToInt(midPointOfPathwayToSplit * _minRangeForSplitPercentageAddition);
        int maxRangeForSplit = midPointOfPathwayToSplit + Mathf.FloorToInt(midPointOfPathwayToSplit * _maxRangeForSplitPercentageAddition);

        _pointInPathwayToSplit = Random.Range(minRangeForSplit, maxRangeForSplit);

        PathTile pointOfSplit = pathwayToSplit[_pointInPathwayToSplit];

        // Okay, so we have the path to split, and we know where on the path to make the split. But Which direction should we start pointing the new path?

        int newDirectionToSplitOffIn; // Default to up.

        if (pointOfSplit.Direction == 1 || pointOfSplit.Direction == 4)
        {
            // if currently up or down, go left or right.
            int randChoice = Random.Range(0, 1);

            if (randChoice == 0)
            {
                newDirectionToSplitOffIn = 2; // Left
            }
            else
            {
                newDirectionToSplitOffIn = 3; // right
            }
        }
        else
        {
            // if currently left or right, go up or down.
            int randChoice = Random.Range(0, 1);

            if (randChoice == 0)
            {
                newDirectionToSplitOffIn = 1; // up
            }
            else
            {
                newDirectionToSplitOffIn = 4; // Down
            }
        }

        // Now we have the cords of where to split off and the direction to go we should go sooo start the entire process again using these values.

        Debug.Log("New path starting at :" + pointOfSplit.Cords + " " + newDirectionToSplitOffIn);

        PathTile newPathTile = new PathTile();
        newPathTile.Cords = pointOfSplit.Cords;
        newPathTile.Direction = newDirectionToSplitOffIn;

        return newPathTile;
 
    }


    void FinishedGeneratingCurrentPathway()
    {
        Debug.Log("Finished Pathway!" + CurrentPathTiles.Count);

       

        List<PathTile> newCurrentPathTiles = new List<PathTile>();

        // If this is not the first pathway, Add the previous path first!

        if (Paths.Count >= 1) // Basically, if the current path is not the first. Add the last pathway.
        {
            for (int Tile = 0; Tile < pathwayToSplit.Count; Tile++)
            {
                // Excludes the points past the where the last path split
                if (Tile < _pointInPathwayToSplit)
                {
                    newCurrentPathTiles.Add(pathwayToSplit[Tile]);
                }
            }
        }

        // After adding the previous pathway tiles add the current after. So they conect together.

        foreach (PathTile tile in CurrentPathTiles)
        {
            newCurrentPathTiles.Add(tile);
        }

        Paths.Add(newCurrentPathTiles);
        CurrentNumOfAttempts = 0;



    }

    void CompletelyFinished()
    {
        Debug.Log("Finished Generating Path. Now Loading Physical path....");

        _lastDirection = 1;
        _loop = false;

        LoadPhysicalPaths();

        // Debug.Log("Finished Loading Path.");
        GameModeSurvivalScript.GenerationTicker = 2; // Lets the Survival script know that the path is finished generating.
    }


    public void ResetCurrentPathGeneration(Vector2 startingCords, int direction)
    {
        _currentStartingDirection = direction;
        _currentStartingCords = startingCords;

        CurrentPathTiles.Clear();

        PathTile NewTile = new PathTile
        {
            Direction = direction,
            Cords = startingCords
        };

        CurrentPathTiles.Add(NewTile);
        _currentCord = startingCords;
        _counter = 0;

    }

    public void LoadPhysicalPaths()
    {
        

        for (int PathWayToLoad = 0; PathWayToLoad < Paths.Count; PathWayToLoad++)
        {
            CurrentPathTiles = Paths[PathWayToLoad];

            Debug.Log("Loading Patway : " + PathWayToLoad + "It contains : " + CurrentPathTiles.Count + " " + Paths.Count);

            for (int Tick = 0; Tick <= CurrentPathTiles.Count; Tick++)
            {


                if (Tick != 0) // This function is working 1 ahead the current point in the array.
                {

                    GridGenerator.SetGridPointAvailable(false, CurrentPathTiles[Tick - 1].Cords);
                    GameObject newTile = null;

                    if (Tick == CurrentPathTiles.Count) // Last Tile
                    {

                        newTile = GameObject.Instantiate(StraightPathGo); // Defualt is Up

                        UtilitiesScript.AttachObjectToWorld(newTile, GridGenerator.GridStatus[(int)CurrentPathTiles[Tick - 1].Cords.x, (int)CurrentPathTiles[Tick - 1].Cords.y].Position);
                        newTile.transform.SetParent(PathStorage.transform);


                        break;
                    }

                    if (CurrentPathTiles[Tick].Direction != _lastDirection) // Changed Direction
                    {
                        if (Tick > 0 && Tick != CurrentPathTiles.Count)
                        {
                            newTile = SpawnCornerTileWithRotation(CurrentPathTiles[Tick - 1].Direction, CurrentPathTiles[Tick].Direction);
                        }
                    }


                    else
                    {
                        newTile = GameObject.Instantiate(StraightPathGo); // Defualt is Up

                        if (CurrentPathTiles[Tick].Direction == 2) // Left
                        {
                            newTile.transform.eulerAngles = new Vector3(0, -90, 0);
                        }
                        if (CurrentPathTiles[Tick].Direction == 3) // Right
                        {
                            newTile.transform.eulerAngles = new Vector3(0, 90, 0);
                        }
                        if (CurrentPathTiles[Tick].Direction == 4) // Down
                        {
                            newTile.transform.eulerAngles = new Vector3(0, 180, 0);
                        }
                    }

                    if (newTile)
                    {
                        UtilitiesScript.AttachObjectToWorld(newTile, GridGenerator.GridStatus[(int)CurrentPathTiles[Tick - 1].Cords.x, (int)CurrentPathTiles[Tick - 1].Cords.y].Position);
                        newTile.transform.SetParent(PathStorage.transform);

                    }

                    _lastDirection = CurrentPathTiles[Tick].Direction;

                }

            }

        }

      

    }



   

    public GameObject SpawnCornerTileWithRotation(int first, int second)
    {
        GameObject NewTile = GameObject.Instantiate(CornerPieceGo); 

        if (first == 1) // UP
        {
            if (second == 2) // Left
            {
                NewTile.transform.eulerAngles = new Vector3(0, 90, 0);
            }
        }
        else if (first == 2) // Left
        {
            if (second == 1) // UP
            {
                NewTile.transform.eulerAngles = new Vector3(0, -90, 0);
            }
        }
        else if (first == 3)// Right
        {
            if (second == 1) // up
            {
                NewTile.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (second == 4) // down
            {
                NewTile.transform.eulerAngles = new Vector3(0, 90, 0);
            }

        }
        else if (first == 4)// DOwn
        {
            if (second == 2) // Left
            {
                NewTile.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (second == 3) // Right
            {
                NewTile.transform.eulerAngles = new Vector3(0, -90, 0);
            }
        }

        return (NewTile);
    }

    public void Worm()
    {

        int Direction = UtilitiesScript.RandomiseByWeight(_pathSpawnChanceArray);  //Random.Range(0, 100);


        switch (Direction)
        {
            case 0:    // Direction = Up
                _lastDirection = 1;
                AttemptToMove(new Vector2(0, 1));
                return;

            case 1:    // Direction = Left
                _lastDirection = 2;
                AttemptToMove(new Vector2(-1, 0));
                return;

            case 2:    // Direction = Right
                _lastDirection = 3;
                AttemptToMove(new Vector2(1, 0));

                return;

            case 3:    // Direction = Down
                _lastDirection = 4;
                AttemptToMove(new Vector2(0, -1));
                return;

        }

        /*
         * Old Method of randomising Path Generation.
         * New method has much easier controll
         * 
        if (Direction < 20) // Left 35 percent Chance.
        {
            _lastDirection = 2;
            AttemptToMove(new Vector2(-1, 0));
        }

        else if (Direction < 40) // Right 35 Percent Chance.
        {
            _lastDirection = 3;
            AttemptToMove(new Vector2(1, 0));

        }

        else if (Direction < 90) // Up 25 Percent
        {
            _lastDirection = 1;
            AttemptToMove(new Vector2(0, 1));

        }

        else if (Direction < 100) // Down 10 Percent
        {
            _lastDirection = 4;
            AttemptToMove(new Vector2(0, -1));

        }

        */
    }

    public void AttemptToMove(Vector2 offset)
    {
        //Debug.Log("1 : Attempting to move.");

        if (!BackOnSelfChecker(_currentCord + offset))
        {
           //Debug.Log("4 : It did not go back on it self ");

            AddCord(offset);
            _failureCount = 0;
        }
        else
        {
           // Debug.Log("5 : Back on self ");

            //Debug.Log("Back on self");
            if (_failureCount >= 3)
            {
                if (CurrentPathTiles.Count > 2)
                {
                    CurrentPathTiles.RemoveAt(CurrentPathTiles.Count - 1);
                    _currentCord = CurrentPathTiles[CurrentPathTiles.Count - 1].Cords;
                }
                else
                {

                    ResetCurrentPathGeneration(_currentStartingCords, _currentStartingDirection);
                    /*
                    CurrentPathTiles.Clear();

                    PathTile NewTile = new PathTile
                    {
                        Direction = 1,
                        Cords = _startingCords
                    };

                    CurrentPathTiles.Add(NewTile);
                    _currentCord = _startingCords;
                    _counter = 0;
                    */
                }

               // Debug.Log("BackTracking");
            
            }
            else
            {
                _failureCount++;
            }
        }
    }

    public void AddCord(Vector2 offset)
    {
        PathTile NewTile = new PathTile
        {
            Direction = _lastDirection,
            Cords = _currentCord + offset
        };

        _currentCord = (_currentCord + offset);
        CurrentPathTiles.Add(NewTile);

    }

    public bool BackOnSelfChecker(Vector2 newCord)
    {
        //Debug.Log("2 : BackOnSelfChecker");


        //Debug.Log("Is there any shit in the paths list? : " + Paths.Count);

        // We want to check if the current cord is already being used in the current path being generated. 
        // And if this is not the only path. Check the others as well.

        foreach (PathTile tile in CurrentPathTiles) // This is to check that the tile isnt already being used by the current pathway.
        {

            if (tile.Cords == newCord)
            {
                return true;

            }
        }

        if (Paths.Count > 0) // If this is not the only path check the others aswell!
        {
            foreach (List<PathTile> Path in Paths)
            {
              //  Debug.Log("3 : path");

                foreach (PathTile tile in Path)
                {
                 //   Debug.Log("4 : tile" + tile.Cords);

                    if (tile.Cords == newCord)
                    {
                        return true;

                    }
                }
            }

        }

        // Sometimes it can scrape across the edge of the grid this can be annoying.. So This can be used to stop that from happpening. ( Adds an offset to width that path can go.)

        if (newCord.x > _maxWidth || newCord.x < _minWidth)
        {
            return true;
        }


        if (!GridGenerator.GridPointsInUse.Contains(newCord))
        {
            return true;
        }

        return false;



        /*

        for (int CurrentPath = 0; CurrentPath < Paths.Count; CurrentPath++)
        {
            for (int pathTiles = 0; pathTiles < Paths[CurrentPath].Count; pathTiles++)
            {

                Debug.Log("HERE" + Paths[CurrentPath][pathTiles].Cords);
                if (Paths[CurrentPath][pathTiles].Cords == newCord)
                {
                    flag = true;
                }
            }

           
        }
        */




        /*
         * 
        bool flag = false;
        foreach (PathTile Path in CurrentPathTiles)
        {
            if (Path.Cords == newCord)
            {
                flag = true;
            }
        }
        if (!GridGenerator.GridPointsInUse.Contains(newCord))
        {
            flag = true;
        }
        return flag;
        */
    }
}


