using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{

    private GameObject[] _environmentTilePresets;
    private GameObject[] _environmentGrassTilePrests;
    private List<Vector2> _tilesToFill = new List<Vector2>();
 
    private float _scaleFactor = 0;
    private int _gridHeight = 0;
    private int _gridWidth = 0;
    private bool _running = false;
    private GameObject _newGrass = null;
    private GameObject _grass = null;

    [SerializeField] private int _entitiesToSpawn = 35;

    // Start is called before the first frame update
    void Start()
    {
        _environmentTilePresets = Resources.LoadAll<GameObject>("EnvironmentTilePresets");
        Debug.Log("Successfully Loaded:" + _environmentTilePresets.Length + " Environment Preset tiles");

        _environmentGrassTilePrests = Resources.LoadAll<GameObject>("EnvironmentGrassTilePresets");
        Debug.Log("Successfully Loaded:" + _environmentGrassTilePrests.Length + " Grass Preset tiles");

        _gridWidth = GameObject.Find("Grid").GetComponent<GridGenerator>()._gridWidth;
        _gridHeight = GameObject.Find("Grid").GetComponent<GridGenerator>()._gridHeight;

       _grass = _environmentGrassTilePrests[0].gameObject;

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
        if (_running && PathGenerator.PathGenerationComplete)
        {

            
            foreach (GridPoint _point in GridGenerator.GridStatus)
            {
       
                if (_point.Available)
                {
                    _newGrass = GameObject.Instantiate(_grass);
                    _newGrass.transform.SetParent(GameObject.Find("World").transform);
                    _newGrass.transform.localScale = new Vector3(_scaleFactor, _scaleFactor * transform.localScale.x, _scaleFactor);
                    _newGrass.transform.localPosition =_point.Position;
                }
            }
            

            for (int b = 0; b < _entitiesToSpawn; b++)
            {
                Debug.Log(b);
                Vector2 point = GenerateRandomPoint();
                _tilesToFill.Add(point);

            }

            int ap = 0;

            foreach (Vector2 tile in _tilesToFill)
            {
                Debug.Log("Tile:" + ap + tile);
                ap++;
            }
            

            for (int b = 0; b < _entitiesToSpawn; b++)
            {
                Vector2 point =  GenerateRandomPoint();

                GameObject _entityToSpawn = _environmentTilePresets[Random.Range(0,_environmentTilePresets.Length)].gameObject;
                GameObject entityToSpawn = GameObject.Instantiate(_entityToSpawn);

                GridGenerator.SetGridPointAvailable(false, point);
                entityToSpawn.transform.SetParent(GameObject.Find("World").transform);
                entityToSpawn.transform.localScale = new Vector3(_scaleFactor, _scaleFactor * transform.localScale.x, _scaleFactor);
                entityToSpawn.transform.localPosition = GridGenerator.GridStatus[ (int) point.x, (int) point.y].Position;


            }

            _running = false;



        }

    }

    private Vector2 GenerateRandomPoint()
    {
        int x = Random.Range(0, _gridWidth);
        int y = Random.Range(0, _gridHeight);

        if (GridGenerator.GridStatus[x, y].Available)
        {
            Debug.Log("Spawning Entity");
            return new Vector2(x,y);
        }
        else
        {
            Debug.Log("Unable to spawn entity here.");
            GenerateRandomPoint();
        }

        return Vector2.zero;
    }
}
