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
    private Vector2 StartingCords;
    private Vector2 CurrentCord;
    private int LastDirection = 1;
    private int MaxIterations = 400;
    private int Counter = 0;
    private bool Loop = true;
    private int FailureCount = 0;

    public List<PathTile> PathTiles = new List<PathTile>();

    public GameObject UpGo;
    public GameObject DownGo;
    public GameObject leftGo;
    public GameObject RightGo;

    private float Sf;


    private void Start()
    {
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
                Loop = false;
                foreach (PathTile path in PathTiles)
                {
                    GridGenerator.SetGridPointAvailable(false, path.Cords);
                    

                    GameObject NewTile = GameObject.Instantiate(UpGo);
                    NewTile.transform.SetParent(GameObject.Find("World").transform);
                    NewTile.transform.localScale = new Vector3(Sf, Sf, Sf);
                    NewTile.transform.localPosition = GridGenerator.GridStatus[(int)path.Cords.x, (int)path.Cords.y].Position;

                   /* if (path.Direction == 1) // UP
                    {
                        

                    }
                    if (path.Direction == 2) // Left
                    {

                    }
                    if (path.Direction == 3) // Right
                    {


                    }
                    if (path.Direction == 4) // Down
                    {

                    } */
                }
            }
            Counter++;
        }

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
            Debug.Log("Back on self");
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

                Debug.Log("BackTracking");
            
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


