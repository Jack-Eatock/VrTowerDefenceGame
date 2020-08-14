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
    public static List<List<PathTile>> FullPathways = new List<List<PathTile>>();                 // x axis is the different paths created, y  axis is the points the path uses. 
    public static List<List<PathTile>> Paths = new List<List<PathTile>>();
    public static List<PathTile> CurrentPathTiles = new List<PathTile>();

    [Header ("Tweak Path Generation!")]

    [SerializeField] private Vector2 _startingCords = new Vector2(18, 18);
    [SerializeField] private int _pathEndNum = 0;                                               // Change it soo they dont stop after getting soo far.. instead if they hit the edge and have got past a limit.
    [Range(0, 6)] [SerializeField] private int _numOfPaths = 1;
    public int MaxAttemptsBeforeAssumingCrashed = 10;

    [SerializeField] private int _pathSpawnChanceUp = 1;
    [SerializeField] private int _pathSpawnChanceLeft = 1;   // Keep them in this order!
    [SerializeField] private int _pathSpawnChanceRight = 1;
    [SerializeField] private int _pathSpawnChanceDown = 1;

    [Range(0, 1)] [SerializeField] private float _minRangeForSplitPercentageAddition = 0.8f;
    [Range(0, 1)] [SerializeField] private float _maxRangeForSplitPercentageAddition = 0.3f;

    [SerializeField] private bool _onlyTheMainPathSplits = false;
    [SerializeField] private int _widthOffsetFromEdge = 2;


    // Private Variables \\

    public int LastDirection { get { return _lastDirection; } set { _lastDirection = value; } }
    private int _lastDirection = 1;

    private List<PathTile> pathwayToSplit = new List<PathTile>();

    private Vector2 _currentStartingCords = new Vector2(); // When the pathway splits it will have a new set of starting cords.
    private Vector2 _currentCord = Vector2.zero;

    private int[] _pathSpawnChanceArray;

    private PathLoader _pathLoader;

    private bool PathGeneratorIsRunning = true;

    private GridGenerator _gridGenerator;

    private float _scaleFactor;

    private int CurrentNumOfAttempts = 0;
    private int _currentStartingDirection = 1;
    private int _pointInPathwayToSplit = 0;
    private int _maxNumofTilePlacementsToReachEnd = 200;
    private int _currentNumOfTilePlacementsToReachEnd = 0;
    private int _failureCount = 0;
    private int _gridWidth;
    private int _minWidth;
    private int _maxWidth;

    /// <summary>
    /// 
    /// --- How is this going to work, ( Making the pathway split ) ---
    /// 
    /// + Run through as usual and create the first pathway. 
    /// + After the first pathway is created, check if we should create another pathway. ( Chance of creating another and have a max number of paths, just in case )
    /// + Add the first pathway (CurrentPathTiles) To the list of all the pathways. 
    /// + Clear the CurrentPathTiles, choose a random path out of ( Paths ) and choose a random point and start the original process but starting cords of the random point selected.
    /// + Now that the next path is generated, add the cords of the original path leading up to when it splits.
    /// + Now we have the entire second pathway so add it to paths, clear the CurrentPath and repeat.
    /// 
    /// ---- What needs to be considered and handlef ----
    /// 
    /// - There is a chance the next "Split" Will split at the same point. ( Stop this from being possible )                              IT DOES NOT FUCKING WORK!
    /// - If the split gets stuck, try to flip it first. if it still gets stuck. Create a new split. Maximum number of attempts.
    /// - Let the paths finish earlier. Maybe if they hit the edge. Check if they are past a certain dist.
    /// </summary>

    private int _totalNumOfFailedFailedAtempts = 0;
    private int _totalNumOfFailedAttemptsAtReachingTheEnd = 0;
    private int _currentAbsoluteIterations = 0;


    [SerializeField] int _maxNumOfTotalFailedAttemptsAtReachingTheEnd = 3;
    [SerializeField] int _aboluteMaxIterations = 3;


    private void Start()
    {

        _pathLoader = transform.GetComponent<PathLoader>();
        _pathSpawnChanceArray = new int[4];
        UpdatePathGenerationVariables();

        _gridGenerator = GameObject.Find("Grid").GetComponent<GridGenerator>();                                             // Clear this shit up

        //_pathEndNum = _gridGenerator.GridDiamater - 1;
        _gridWidth = _gridGenerator.GridDiamater;

        _minWidth = _widthOffsetFromEdge;
        _maxWidth = _gridWidth - _widthOffsetFromEdge;

    }

    // Update is called once per frame
    void Update()
    {
        if (!PathGeneratorIsRunning)
        {
            return;
        }

        if (_currentNumOfTilePlacementsToReachEnd >= _maxNumofTilePlacementsToReachEnd)
        {
            Debug.Log("1 - Failed attempt..");

            // If it tries place tiles for too long, without making progress.

            //Debug.Log("More than max attempts to finish current path");

            // if it goes over max iterations, then it either failed or is stuck.... So try again x amount of times. If it still does not work try to flip it and again attempt x times. If it still fails, generate a new split point. Repeat over a maximum of Y attempts.

            if (_totalNumOfFailedAttemptsAtReachingTheEnd < _maxNumOfTotalFailedAttemptsAtReachingTheEnd)
            {
                // So it just failed a little.. no biggie try again.
                ResetCurrentPathGeneration(_currentStartingCords,_currentStartingDirection);
                _totalNumOfFailedAttemptsAtReachingTheEnd++;
            }

            else
            {
                // Uh ohh... it has failed to generate the path too many times... Time to make some changes.


                _totalNumOfFailedFailedAtempts++;

                if (_totalNumOfFailedFailedAtempts == 1)
                {
                    Debug.Log("2 - Gonna Switch it up!");
                    // Simply flip the path starting direction, and go again.
                    FlipPathStartDirection();                  
                }

                else if (_totalNumOfFailedFailedAtempts == 2)
                {
                    // So no you have tried to wiggle both ways... Not working. So lets try a different pos to split.
                    SplitPathway();
                    _totalNumOfFailedFailedAtempts = 0;

                    if ( _currentAbsoluteIterations >= _aboluteMaxIterations)
                    {
                        Debug.LogError("Failed");
                        CompletelyFinished();
                    }
                    _currentAbsoluteIterations++;
                }
                else
                {
                    Debug.Log("HMMM NOTICE");
                }


                _totalNumOfFailedAttemptsAtReachingTheEnd = 0;
            }
        }

        else if  (_currentCord.y >= _pathEndNum)  // If ti hits the edge and is far enough.
        {
            Debug.Log("End");

           
        
              
            if (GridGenerator.GridPointsOnCircumferance.Contains(_currentCord)) // If it hits the edge.
            {
                Debug.Log("Hit edge");

                FinishedGeneratingCurrentPathway(); // Saves all of the path data for the current pathway.
                                                    // Check if we need to split the pathway.

                if (FullPathways.Count != _numOfPaths)
                {
                    SplitPathway();

                }

                else
                {
                    CompletelyFinished();
                }

            }
            else
            {
                Worm();
            }
              
        }

        else
        {

            Worm();                             // Calculates the pathway.
        }

        _currentNumOfTilePlacementsToReachEnd++;
    }

    public void InitiatePathGeneration()
    {
        _currentStartingCords = _startingCords;

        _scaleFactor = MovementScript.ScaleFactor; // / transform.localScale.x;
        Debug.Log("Generating Virtual Path.... With Scale Factor:" + _scaleFactor);

        ResetCurrentPathGeneration(_startingCords, 1);

        //_running = true;
    }

    void FlipPathStartDirection()
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

    void SplitPathway()
    {
        // Okay so we need to split the pathway. But which pathway and where? 
        // Choose which pathway first.

        if (!_onlyTheMainPathSplits)
        {
            pathwayToSplit = FullPathways[Random.Range(0, FullPathways.Count)];
        }

        else
        {
            pathwayToSplit = FullPathways[0];
        }


        // Now where on the pathway to split..? 

        if (pathwayToSplit.Count != 0 && pathwayToSplit.Count >= 2)
        {
            PathTile newStartTile = CalculateSplitPointAndSplitDirection();
            ResetCurrentPathGeneration(newStartTile.Cords, newStartTile.Direction);
        }

        else
        {
            Debug.LogError("Pathway generated without any pathtiles. List is empty. : " + pathwayToSplit);
        }
    }

    PathTile CalculateSplitPointAndSplitDirection()
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

        UtilitiesScript.CircleRadius(new Vector2(_currentCord.x, _currentCord.y), 2, false);


        

        Debug.Log("Finished Pathway!" + CurrentPathTiles.Count);
        List<PathTile> newCurrentPathTiles = new List<PathTile>();




        // If this is not the first pathway, Add the previous path first!



        if (FullPathways.Count >= 1) // Basically, if the current path is not the first. Add the last pathway.
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
        
        List<PathTile> newPath = new List<PathTile>();

        foreach (PathTile tile in CurrentPathTiles)
        {
            newCurrentPathTiles.Add(tile);
            newPath.Add(tile);
        }

        Paths.Add(newPath);

        FullPathways.Add(newCurrentPathTiles);
        CurrentNumOfAttempts = 0;
    }

    void CompletelyFinished()
    {
        Debug.Log("Finished Generating Path. Now Loading Physical path....");

        _lastDirection = 1;
        PathGeneratorIsRunning = false;

        _pathLoader.LoadPhysicalPaths();

        // Debug.Log("Finished Loading Path.");
        GameModeSurvivalScript.GenerationTicker = 2; // Lets the Survival script know that the path is finished generating.
    }

    void ResetCurrentPathGeneration(Vector2 startingCords, int direction)
    {
        Debug.Log("Resetting Pathway!" + _currentAbsoluteIterations + _currentNumOfTilePlacementsToReachEnd + _totalNumOfFailedFailedAtempts + " " + PathGeneratorIsRunning);

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

        _currentNumOfTilePlacementsToReachEnd = 0;
        //_totalNumOfFailedFailedAtempts = 0;

    }

    void Worm()
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

    }

    void AttemptToMove(Vector2 offset)
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
            Debug.Log("5 : Back on self ");

            //Debug.Log("Back on self");
            if (_failureCount >= 2)
            {
                if (CurrentPathTiles.Count > 2)
                {
                    CurrentPathTiles.RemoveAt(CurrentPathTiles.Count - 1);
                    _currentCord = CurrentPathTiles[CurrentPathTiles.Count - 1].Cords;
                }
                else
                {

                    // If Initial path, try again. Otherwise it needs to start elsewhere.

                    if (Paths.Count > 0)
                    {
                        SplitPathway();
                    }
                    else
                    {
                        ResetCurrentPathGeneration(_currentStartingCords, _currentStartingDirection);
                    }

                   
                }

                // Debug.Log("BackTracking");

            }
            else
            {
                _failureCount++;
            }
        }
    }

    void AddCord(Vector2 offset)
    {
        PathTile NewTile = new PathTile
        {
            Direction = _lastDirection,
            Cords = _currentCord + offset
        };

        _currentCord = (_currentCord + offset);
        CurrentPathTiles.Add(NewTile);

    }

    bool BackOnSelfChecker(Vector2 newCord)
    {




        // We want to check if the current cord is already being used in the current path being generated. 
        // And if this is not the only path. Check the others as well.



        foreach (PathTile tile in CurrentPathTiles) // This is to check that the tile isnt already being used by the current pathway.
        {
            if (tile.Cords == newCord)
            {
                return true;
            }
        }

        if (FullPathways.Count > 0) // If this is not the only path check the others aswell!
        {
            foreach (List<PathTile> Path in FullPathways)
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

    }

    void UpdatePathGenerationVariables()
    {
        _pathSpawnChanceArray[0] = _pathSpawnChanceUp;   // This order is important!
        _pathSpawnChanceArray[1] = _pathSpawnChanceLeft;
        _pathSpawnChanceArray[2] = _pathSpawnChanceRight;
        _pathSpawnChanceArray[3] = _pathSpawnChanceDown;
    }

}

