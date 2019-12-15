using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTile
{
    public Vector2 Cords;
    public int Direction; // 1, up, 2 Left, 3 Right, 4 Down.
}



public class PathGenerator : MonoBehaviour
{
    [SerializeField]
    private Vector2 StartingCords = Vector2.zero;
    private Vector2 CurrentCord = Vector2.zero;
    private int LastDirection = 1;
    private int MaxIterations = 400;
    private int Counter = 0;
    private bool Loop = true;
    private int FailureCount = 0;

    public static List<PathTile> PathTiles = new List<PathTile>();
    public GameObject PathStorage;
    public GameObject StraightPathGo;
    public GameObject CornerPieceGo;

    public static bool PathGenerationComplete = false;

    private float Sf;


    private void Start()
    {
        Debug.Log("Generating Path....");
        Sf = GameObject.Find("GAMEMANAGER").GetComponent<BuildingScript>().SF;
        PathTile NewTile = new PathTile
        {
            Direction = 1,
            Cords = StartingCords
        };

        PathTiles.Add(NewTile);
        CurrentCord = StartingCords;
    }

    // Update is called once per frame
    void Update()
    {
        if (Loop)
        {
            if (Counter >= MaxIterations)
            {
                PathTiles.Clear();

                PathTile NewTile = new PathTile
                {
                    Direction = 1,
                    Cords = StartingCords
                };

                PathTiles.Add(NewTile);
                CurrentCord = StartingCords;
                Counter = 0;
            }

            else if (CurrentCord.y != 39)
            {
                Worm();
            }
            else
            {

                Debug.Log("Finished Generating. Now Loading path....");
                LastDirection = 1;
                Loop = false;


                for (int Tick = 0; Tick <= PathTiles.Count; Tick ++)
                {


                    if ( Tick != 0) // This function is working 1 ahead the current point in the array.
                    {

                        GridGenerator.SetGridPointAvailable(false, PathTiles[Tick - 1].Cords);
                        GameObject NewTile = null;

                        if (Tick == PathTiles.Count) // Last Tile
                        {
                        
                            NewTile = GameObject.Instantiate(StraightPathGo); // Defualt is Up
                            
                          

                            NewTile.transform.SetParent(PathStorage.transform);
                            NewTile.transform.localScale = new Vector3(Sf, Sf, Sf);
                            NewTile.transform.localPosition = GridGenerator.GridStatus[(int)PathTiles[Tick - 1].Cords.x, (int)PathTiles[Tick - 1].Cords.y].Position;
                            break;
                        }

                        if (PathTiles[Tick].Direction != LastDirection) // Changed Direction
                        {
                            if (Tick > 0 && Tick != PathTiles.Count)
                            {
                                NewTile = SpawnCornerTileWithRotation(PathTiles[Tick - 1].Direction, PathTiles[Tick].Direction);
                            }
                        }


                        else
                        {
                            NewTile = GameObject.Instantiate(StraightPathGo); // Defualt is Up

                            if (PathTiles[Tick].Direction == 2) // Left
                            {
                                NewTile.transform.eulerAngles = new Vector3(0, -90, 0);
                            }
                            if (PathTiles[Tick].Direction == 3) // Right
                            {
                                NewTile.transform.eulerAngles = new Vector3(0, 90, 0);
                            }
                            if (PathTiles[Tick].Direction == 4) // Down
                            {
                                NewTile.transform.eulerAngles = new Vector3(0, 180, 0);
                            }
                        }

                        if (NewTile)
                        {
                            NewTile.transform.SetParent(PathStorage.transform);
                            NewTile.transform.localScale = new Vector3(Sf, Sf, Sf);
                            NewTile.transform.localPosition = GridGenerator.GridStatus[(int)PathTiles[Tick - 1].Cords.x, (int)PathTiles[Tick - 1].Cords.y].Position;
                        }

                        LastDirection = PathTiles[Tick].Direction;

                    }

                }

                Debug.Log("Finished.");
                PathGenerationComplete = true;
                MovementScript.MovementControllsDisabled = false;
                //BuildingScript.MenuControllsDisabled = false; // Enables Building once the Path is generated.
                this.GetComponent<EnemySpawner>().InitiateEnemySpawner();




            }
            Counter++;
        }

    }

    public GameObject SpawnCornerTileWithRotation(int First, int Second)
    {
        GameObject NewTile = GameObject.Instantiate(CornerPieceGo); 

        if (First == 1) // UP
        {
            if (Second == 2) // Left
            {
                NewTile.transform.eulerAngles = new Vector3(0, 90, 0);
            }
        }
        else if (First == 2) // Left
        {
            if (Second == 1) // UP
            {
                NewTile.transform.eulerAngles = new Vector3(0, -90, 0);
            }
        }
        else if (First == 3)// Right
        {
            if (Second == 1) // up
            {
                NewTile.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (Second == 4) // down
            {
                NewTile.transform.eulerAngles = new Vector3(0, 90, 0);
            }

        }
        else if (First == 4)// DOwn
        {
            if (Second == 2) // Left
            {
                NewTile.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (Second == 3) // Right
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
            LastDirection = 2;
            AttemptToMove(new Vector2(-1, 0));
        }

        else if (Direction < 40) // Right 35 Percent Chance.
        {
            LastDirection = 3;
            AttemptToMove(new Vector2(1, 0));

        }

        else if (Direction < 90) // Up 25 Percent
        {
            LastDirection = 1;
            AttemptToMove(new Vector2(0, 1));

        }

        else if (Direction < 100) // Down 10 Percent
        {
            LastDirection = 4;
            AttemptToMove(new Vector2(0, -1));

        }
    }

    public void AttemptToMove(Vector2 Offset)
    {
        if (!BackOnSelfChecker(CurrentCord + Offset))
        {
            AddCord(Offset);
            FailureCount = 0;
        }
        else
        {
            //Debug.Log("Back on self");
            if (FailureCount >= 3)
            {
                if (PathTiles.Count > 2)
                {
                    PathTiles.RemoveAt(PathTiles.Count - 1);
                    CurrentCord = PathTiles[PathTiles.Count - 1].Cords;
                }
                else
                {

                    PathTiles.Clear();

                    PathTile NewTile = new PathTile
                    {
                        Direction = 1,
                        Cords = StartingCords
                    };

                    PathTiles.Add(NewTile);
                    CurrentCord = StartingCords;
                    Counter = 0;
                }

               // Debug.Log("BackTracking");
            
            }
            else
            {
                FailureCount++;
            }
        }
    }

    public void AddCord(Vector2 Offset)
    {
        PathTile NewTile = new PathTile
        {
            Direction = LastDirection,
            Cords = CurrentCord + Offset
        };
        CurrentCord = (CurrentCord + Offset);
        PathTiles.Add(NewTile);

    }

    public bool BackOnSelfChecker(Vector2 NewCord)
    {
        bool Flag = false;
        foreach (PathTile Path in PathTiles)
        {
            if (Path.Cords == NewCord)
            {
                Flag = true;
            }
        }
        if (NewCord.x >= 40 || NewCord.x < 0 || NewCord.y < 0)
        {
            Flag = true;
        }
        return Flag;

    }
}


