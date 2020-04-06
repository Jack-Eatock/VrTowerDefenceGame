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
    public static List<PathTile> PathTiles = new List<PathTile>();
    public static bool PathGenerationComplete = false;

    [SerializeField] private Vector2 _startingCords = Vector2.zero;

    private Vector2 _currentCord = Vector2.zero;
    private int     _lastDirection = 1;
    private int     _maxIterations = 400;
    private int     _counter = 0;
    private bool    _loop = true;
    private int     _failureCount = 0;

 
    public GameObject PathStorage;
    public GameObject StraightPathGo;
    public GameObject CornerPieceGo;

    

    private float _scaleFactor;
    private bool  _running = false;


    private void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (!_running)
        {
            return;
        }


        if (_loop)
        {
            if (_counter >= _maxIterations)
            {
                PathTiles.Clear();

                PathTile NewTile = new PathTile
                {
                    Direction = 1,
                    Cords = _startingCords
                };

                PathTiles.Add(NewTile);
                _currentCord = _startingCords;
                _counter = 0;
            }

            else if (_currentCord.y != 39)
            {
                Worm();
            }
            else
            {

                Debug.Log("Finished Generating. Now Loading path....");
                _lastDirection = 1;
                _loop = false;


                for (int Tick = 0; Tick <= PathTiles.Count; Tick ++)
                {


                    if ( Tick != 0) // This function is working 1 ahead the current point in the array.
                    {

                        GridGenerator.SetGridPointAvailable(false, PathTiles[Tick - 1].Cords);
                        GameObject newTile = null;

                        if (Tick == PathTiles.Count) // Last Tile
                        {
                        
                            newTile = GameObject.Instantiate(StraightPathGo); // Defualt is Up
                            
                          

                            newTile.transform.SetParent(PathStorage.transform);
                            newTile.transform.localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);
                            newTile.transform.localPosition = GridGenerator.GridStatus[(int)PathTiles[Tick - 1].Cords.x, (int)PathTiles[Tick - 1].Cords.y].Position;
                            break;
                        }

                        if (PathTiles[Tick].Direction != _lastDirection) // Changed Direction
                        {
                            if (Tick > 0 && Tick != PathTiles.Count)
                            {
                                newTile = SpawnCornerTileWithRotation(PathTiles[Tick - 1].Direction, PathTiles[Tick].Direction);
                            }
                        }


                        else
                        {
                            newTile = GameObject.Instantiate(StraightPathGo); // Defualt is Up

                            if (PathTiles[Tick].Direction == 2) // Left
                            {
                                newTile.transform.eulerAngles = new Vector3(0, -90, 0);
                            }
                            if (PathTiles[Tick].Direction == 3) // Right
                            {
                                newTile.transform.eulerAngles = new Vector3(0, 90, 0);
                            }
                            if (PathTiles[Tick].Direction == 4) // Down
                            {
                                newTile.transform.eulerAngles = new Vector3(0, 180, 0);
                            }
                        }

                        if (newTile)
                        {
                            newTile.transform.SetParent(PathStorage.transform);
                            newTile.transform.localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);
                            newTile.transform.localPosition = GridGenerator.GridStatus[(int)PathTiles[Tick - 1].Cords.x, (int)PathTiles[Tick - 1].Cords.y].Position;
                        }

                        _lastDirection = PathTiles[Tick].Direction;

                    }

                }

                Debug.Log("Finished.");
                PathGenerationComplete = true;
                MovementScript.MovementControllsDisabled = false;
                //BuildingScript.MenuControllsDisabled = false; // Enables Building once the Path is generated.
                EnemySpawner EnemySpawnero = GameObject.Find("GAMEMANAGER").GetComponent<EnemySpawner>();
                EnemySpawnero.InitiateEnemySpawner();




            }
            _counter++;
        }

    }

    public void InitiatePathGeneration()
    {

        _scaleFactor = MovementScript.ScaleFactor;
        Debug.Log("Generating Path...." + _scaleFactor);
        PathTile NewTile = new PathTile
        {
            Direction = 1,
            Cords = _startingCords
        };

        PathTiles.Add(NewTile);
        _currentCord = _startingCords;
        _running = true;
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
        int Direction = Random.Range(0, 100);

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
    }

    public void AttemptToMove(Vector2 offset)
    {
        if (!BackOnSelfChecker(_currentCord + offset))
        {
            AddCord(offset);
            _failureCount = 0;
        }
        else
        {
            //Debug.Log("Back on self");
            if (_failureCount >= 3)
            {
                if (PathTiles.Count > 2)
                {
                    PathTiles.RemoveAt(PathTiles.Count - 1);
                    _currentCord = PathTiles[PathTiles.Count - 1].Cords;
                }
                else
                {

                    PathTiles.Clear();

                    PathTile NewTile = new PathTile
                    {
                        Direction = 1,
                        Cords = _startingCords
                    };

                    PathTiles.Add(NewTile);
                    _currentCord = _startingCords;
                    _counter = 0;
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
        PathTiles.Add(NewTile);

    }

    public bool BackOnSelfChecker(Vector2 newCord)
    {
        bool flag = false;
        foreach (PathTile Path in PathTiles)
        {
            if (Path.Cords == newCord)
            {
                flag = true;
            }
        }
        if (newCord.x >= 40 || newCord.x < 0 || newCord.y < 0)
        {
            flag = true;
        }
        return flag;

    }
}


