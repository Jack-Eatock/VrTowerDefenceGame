using UnityEngine;

public class PathLoader : MonoBehaviour
{


    [Header("Required Variables")]

    public GameObject CornerPieceGo;
    public GameObject PathStorage;
    public GameObject StraightPathGo;

    private PathGenerator _pathGenerator;

    private void Awake()
    {
        _pathGenerator = transform.GetComponent<PathGenerator>();
    }

    public void LoadPhysicalPaths()
    {

        for (int PathWayToLoad = 0; PathWayToLoad < PathGenerator.Paths.Count; PathWayToLoad++)
        {
            PathGenerator.CurrentPathTiles = PathGenerator.Paths[PathWayToLoad];

            Debug.Log("Loading Patway : " + PathWayToLoad + "It contains : " + PathGenerator.CurrentPathTiles.Count + " " + PathGenerator.Paths.Count);

            for (int Tick = 0; Tick <= PathGenerator.CurrentPathTiles.Count; Tick++)
            {


                if (Tick != 0) // This function is working 1 ahead the current point in the array.
                {

                    GridGenerator.SetGridPointAvailable(false, PathGenerator.CurrentPathTiles[Tick - 1].Cords);
                    GameObject newTile = null;

                    if (Tick == PathGenerator.CurrentPathTiles.Count) // Last Tile
                    {

                        

                        if (PathGenerator.CurrentPathTiles[PathGenerator.CurrentPathTiles.Count - 1].Direction == 2) // Left
                        {
                            newTile = SpawnCornerTileWithRotation(2, 1);
                        }

                        else if (PathGenerator.CurrentPathTiles[PathGenerator.CurrentPathTiles.Count - 1].Direction == 3) // Right
                        {
                            newTile = SpawnCornerTileWithRotation(3, 1);
                        }

                        else
                        {
                            newTile = GameObject.Instantiate(StraightPathGo); // Defualt is Up
                        }


                        UtilitiesScript.AttachObjectToWorld(newTile, GridGenerator.GridStatus[(int)PathGenerator.CurrentPathTiles[Tick - 1].Cords.x, (int)PathGenerator.CurrentPathTiles[Tick - 1].Cords.y].Position);
                        newTile.transform.SetParent(PathStorage.transform);


                        break;
                    }

                    if (PathGenerator.CurrentPathTiles[Tick].Direction != _pathGenerator.LastDirection) // Changed Direction
                    {
                        if (Tick > 0 && Tick != PathGenerator.CurrentPathTiles.Count)
                        {
                            newTile = SpawnCornerTileWithRotation(PathGenerator.CurrentPathTiles[Tick - 1].Direction, PathGenerator.CurrentPathTiles[Tick].Direction);
                        }
                    }


                    else
                    {
                        newTile = GameObject.Instantiate(StraightPathGo); // Defualt is Up

                        if (PathGenerator.CurrentPathTiles[Tick].Direction == 2) // Left
                        {
                            newTile.transform.eulerAngles = new Vector3(0, -90, 0);
                        }
                        if (PathGenerator.CurrentPathTiles[Tick].Direction == 3) // Right
                        {
                            newTile.transform.eulerAngles = new Vector3(0, 90, 0);
                        }
                        if (PathGenerator.CurrentPathTiles[Tick].Direction == 4) // Down
                        {
                            newTile.transform.eulerAngles = new Vector3(0, 180, 0);
                        }
                    }

                    if (newTile)
                    {
                        UtilitiesScript.AttachObjectToWorld(newTile, GridGenerator.GridStatus[(int)PathGenerator.CurrentPathTiles[Tick - 1].Cords.x, (int)PathGenerator.CurrentPathTiles[Tick - 1].Cords.y].Position);
                        newTile.transform.SetParent(PathStorage.transform);

                    }

                    _pathGenerator.LastDirection = PathGenerator.CurrentPathTiles[Tick].Direction;

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





}

