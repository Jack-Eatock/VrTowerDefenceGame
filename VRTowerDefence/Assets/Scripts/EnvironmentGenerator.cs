using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{
    enum Direction {UpRight, UpLeft, DownRight, DownLeft}; 

    private GameObject[] _environmentMultiTilePresets;
    private GameObject[] _environmentTilePresets;
    private GameObject[] _environmentGrassTilePrests;
 
    private float _scaleFactor = 0;
    private int _gridHeight = 0;
    private int _gridWidth = 0;
    private bool _running = false;

    private GameObject _newGrass = null;
    private GameObject _grass = null;
    private GameObject _Pond = null;

    [SerializeField] private Material _grassMat;

    [SerializeField] private int _entitiesToSpawn = 35;

    // Start is called before the first frame update
    void Start()
    {
        //EnvironmentMultiTileEntities

        _environmentMultiTilePresets = Resources.LoadAll<GameObject>("EnvironmentMultiTileEntities");
        Debug.Log("Successfully Loaded:" + _environmentMultiTilePresets.Length + " Environment Preset tiles");

        _environmentTilePresets = Resources.LoadAll<GameObject>("EnvironmentTilePresets");
        Debug.Log("Successfully Loaded:" + _environmentTilePresets.Length + " Environment Preset tiles");

        _environmentGrassTilePrests = Resources.LoadAll<GameObject>("EnvironmentGrassTilePresets");
        Debug.Log("Successfully Loaded:" + _environmentGrassTilePrests.Length + " Grass Preset tiles");

        _gridWidth = GameObject.Find("Grid").GetComponent<GridGenerator>()._gridWidth;
        _gridHeight = GameObject.Find("Grid").GetComponent<GridGenerator>()._gridHeight;



        //_grass = _environmentGrassTilePrests[0].gameObject;

        _Pond = _environmentMultiTilePresets[0].gameObject;

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


            SpawnMultiTileEntities();


           




            


            

            for (int b = 0; b < _entitiesToSpawn; b++)
            {
                Vector2 point =  GenerateRandomPoint();  // Generates a random point on the  Grid (That is a available)

                GameObject _entityToSpawn = _environmentTilePresets[Random.Range(0,_environmentTilePresets.Length)].gameObject;   // Chooses a random tileset entity to spawn.
                GameObject entityToSpawn = GameObject.Instantiate(_entityToSpawn);                                                // Spawns the Object. Ready to be attached to the Grid.
                Vector3 posToSpawn = GridGenerator.GridStatus[(int)point.x, (int)point.y].Position;                               // Finds 3d position to spawn object. Based on the pos of the grid.

                GridGenerator.SetGridPointAvailable(false, point);                  // prevents the point on the grid having more entities spawned there.
                UtilitiesScript.AttachObjectToWorld(entityToSpawn, posToSpawn);     // Kinda obvious bro.
            }

            _running = false;



        }

    }

    private void SpawnMultiTileEntities()
    {
        bool flag = false;

        Vector2 randomGridPoint = Vector2.zero;
        Vector3 posToSpawn = Vector3.zero;
        List<Vector2> pointsMultiTileWillCover = new List<Vector2>();
        Direction finalDirection = Direction.UpRight;

        List<Direction> directionsToCheck = new List<Direction>();
        directionsToCheck.Add(Direction.UpRight); directionsToCheck.Add(Direction.UpLeft); directionsToCheck.Add(Direction.DownLeft); directionsToCheck.Add(Direction.DownRight);

        while (!flag)
        {
            randomGridPoint = GenerateRandomPoint();

            foreach (Direction dir in directionsToCheck)
            {
                flag = CheckAreaFromPoint(randomGridPoint, dir, 2);
                
                if (flag == true)
                {
                    finalDirection = dir;
                    break;
                }
            }
        }

        Debug.Log("Spawn Location Found: " + randomGridPoint + " With Direction: " + finalDirection);

        posToSpawn = GridGenerator.GridStatus[(int) randomGridPoint.x, (int) randomGridPoint.y].Position;






        pointsMultiTileWillCover = FindPointsForMultiTile(randomGridPoint, finalDirection, 2);

        Debug.Log("THIS" + pointsMultiTileWillCover.Count);

        foreach( Vector2 point in pointsMultiTileWillCover)
        {
            GridGenerator.SetGridPointAvailable(false, point);
            Debug.Log(point);

            //GridGenerator.GridStatus[(int)point.x, (int)point.y].Available = false;

        }
        GameObject pond =  GameObject.Instantiate(_Pond);

        UtilitiesScript.AttachObjectToWorld(pond, posToSpawn);

        

    }


    private List<Vector2> FindPointsForMultiTile(Vector2 startingCords, Direction direction, int widthOfMultiTile = 2)
    {
        List<Vector2> valueToReturn = new List<Vector2>();

        Debug.Log("DumbDUmbLewis " + direction);

        switch (direction)
        {
            case Direction.UpRight:

                Debug.Log("FUVCKMEDADDYU");


                for (int x = (int)startingCords.x; x < widthOfMultiTile; x++)
                {
                    Debug.Log("eek");
                    for (int y = (int)startingCords.y; y < widthOfMultiTile; y++)  // loops through every
                    {
                        Debug.Log("Gmmm");
                        valueToReturn.Add(new Vector2(x, y));
                        Debug.Log("HMMM");
                    }
                }

                break;

            case Direction.UpLeft:

                for (int x = (int)startingCords.x; x > startingCords.x - widthOfMultiTile; x--)
                {
                    for (int y = (int)startingCords.y; y < widthOfMultiTile; y++)  // loops through every
                    {
                        valueToReturn.Add(new Vector2(x, y));
                    }
                }

                break;

            case Direction.DownRight:

                for (int x = (int)startingCords.x; x < widthOfMultiTile; x++)
                {
                    for (int y = (int)startingCords.y; y > startingCords.y - widthOfMultiTile; y--)  // loops through every
                    {
                        valueToReturn.Add(new Vector2(x, y));
                    }
                }


                break;

            case Direction.DownLeft:

                for (int x = (int)startingCords.x; x > startingCords.x - widthOfMultiTile; x--)
                {
                    for (int y = (int)startingCords.y; y > startingCords.y - widthOfMultiTile; y--)  // loops through every
                    {
                        valueToReturn.Add(new Vector2(x, y));
                    }
                }

                break;
        }

    

        return valueToReturn;
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
                Debug.Log("Spawning Entity");
                flag = false;
            }

            else
            {
                Debug.Log("Unable to spawn entity here.");
            }

        }

        return new Vector2 (x,y);
    }

    private bool CheckAreaFromPoint(Vector2 startingCords, Direction direction, int widthOfMultiTile = 2)
    {
        switch (direction)
        {
            case Direction.UpRight :

                for (int x = (int)startingCords.x; x < widthOfMultiTile; x++)
                {
                    for (int y = (int)startingCords.y; y < widthOfMultiTile; y++)  // loops through every
                    {
                        if (!GridGenerator.GridStatus[x, y].Available)
                        {
                            return false;
                        }
                    }
                }

                break;

            case Direction.UpLeft:

                for (int x = (int)startingCords.x; x > startingCords.x - widthOfMultiTile; x--)
                {
                    for (int y = (int)startingCords.y; y < widthOfMultiTile; y++)  // loops through every
                    {
                        if (!GridGenerator.GridStatus[x, y].Available)
                        {
                            return false;
                        }
                    }
                }

                break;

            case Direction.DownRight:

                for (int x = (int)startingCords.x; x < widthOfMultiTile; x++)
                {
                    for (int y = (int)startingCords.y; y > startingCords.y - widthOfMultiTile; y--)  // loops through every
                    {
                        if (!GridGenerator.GridStatus[x, y].Available)
                        {
                            return false;
                        }
                    }
                }


                break;

            case Direction.DownLeft:

                for (int x = (int)startingCords.x; x > startingCords.x - widthOfMultiTile; x--)
                {
                    for (int y = (int)startingCords.y; y > startingCords.y - widthOfMultiTile; y--)  // loops through every
                    {
                        if (!GridGenerator.GridStatus[x, y].Available)
                        {
                            return false;
                        }
                    }
                }

                break;



        }


        return true;
    }
}
