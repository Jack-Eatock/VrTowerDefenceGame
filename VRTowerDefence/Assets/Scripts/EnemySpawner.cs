using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

// Cleared \\

public class EnemySpawner : MonoBehaviour
{
    public static int EnemiesFinished = 0; // could have made it to the end or died ..
    public GameObject DeathEffect;

    // Unit Serialised Objects \\
    public UnitSO Soldier;
    public UnitSO Tank;
    public UnitSO Swarmer;
    public UnitSO Charger;


    // General Variables \\
    private int _currentPathToSpawnOn = 0;

    public static List<List<Vector2>> PathwaysAvailable = new List<List<Vector2>>();


    public GameObject UnitStorage = null;

    // Wave Variables
    private List<UnitSO> _unitsInWave = new List<UnitSO>();
    private bool _spawningWaveUnits = false;
    private float _lastRecordedTime;
    private int _counter = 0;

    public int[] UnitSpawnChance;
    private EnemyScript _tempEnemyScript;


    public void Update()
    {
        if (_spawningWaveUnits)
        {
            if (_counter >= _unitsInWave.Count)
            {
                _spawningWaveUnits = false;
                Debug.Log("Finished Spawning Units");
            }

            else if (Time.time - _lastRecordedTime > GameScript.SpawRate)
            {

                SpawnEnemy(_unitsInWave[_counter]);
                _counter++;
                _lastRecordedTime = Time.time;

            }
        }
        
        else if (EnemiesFinished == _unitsInWave.Count && _unitsInWave.Count > 0)
        {
            Debug.Log("Wave Finished");
            GameScript.WaveIncoming = false;
            EnemiesFinished = 0;
            _unitsInWave.Clear();
            _counter = 0;
            GameScript.CurrentRound++;


            StartWave(); // Do some logic instead..
        }

    }


    public void InitiateEnemySpawner()
    {
        Debug.Log("Initiating Spawner.");

        foreach (List<PathTile> pathWay in PathGenerator.FullPathways)
        {
            List<Vector2> newPathway = new List<Vector2>();

            foreach (PathTile tile in pathWay)
            {
                newPathway.Add(tile.Cords);
            }

            newPathway.Reverse(); // Now going the correct direction

            PathwaysAvailable.Add(newPathway);
        }

        /*
        foreach (PathTile Path in PathGenerator.CurrentPathTiles)
        {
            PathPoints.Add(Path.Cords);
        }
        */

        

        //Debug.Log("IamHere?");
        StartWave();

    }

    public void StartWave()
    {
        GameScript.WaveIncoming = true;
        int currentRound = GameScript.CurrentRound;
        int numEnemies = Mathf.FloorToInt(currentRound * GameScript.RoundEnemySpawnRatio);
        int randomValue;

        for (int i = 0; i < numEnemies; i++)
        {
            randomValue = UtilitiesScript.RandomiseByWeight(UnitSpawnChance);

            switch (randomValue)
            { 
                case 0 :
                    _unitsInWave.Add(Soldier);
                    break;

                case 1:
                    _unitsInWave.Add(Charger);
                    break;

                case 2:
                    _unitsInWave.Add(Tank);
                    break;

                case 3:
                    _unitsInWave.Add(Swarmer);
                    break;
            }
        }

        _lastRecordedTime = Time.time;
        _spawningWaveUnits = true;

        Debug.Log("Starting Wave"); // Make Wave thing better. ;)
    }

    public void SpawnEnemy(UnitSO unitType)
    {

        GameObject newUnit = GameObject.Instantiate(unitType.UnitGO);

        newUnit.transform.tag = "Enemy";
        newUnit.AddComponent<EnemyScript>();

        // Setting the Unit to Scale and possition , linked with the World at the current time.
        newUnit.transform.localScale = new Vector3(MovementScript.ScaleFactor, MovementScript.ScaleFactor, MovementScript.ScaleFactor);
        newUnit.transform.SetParent(UnitStorage.transform);



        newUnit.transform.localPosition = GridGenerator.GridStatus[(int)PathwaysAvailable [_currentPathToSpawnOn] [0].x, (int)PathwaysAvailable [_currentPathToSpawnOn] [0].y].Position;

        _tempEnemyScript = newUnit.GetComponent<EnemyScript>();
        _tempEnemyScript.EnemySetUP(unitType.Health, unitType.Speed, unitType.Points,unitType.Mass, DeathEffect, _currentPathToSpawnOn);
        
        if (PathwaysAvailable.Count == 1)
        {
            // Do nothing.
        }

        else if (_currentPathToSpawnOn + 1 < PathwaysAvailable.Count )
        {
            _currentPathToSpawnOn++; // Switch through the pathways.
        }

        else
        {
            _currentPathToSpawnOn = 0;  // Loop back to the start.
        }
   
     
    }
}

