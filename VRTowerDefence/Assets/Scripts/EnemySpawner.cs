using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static int EnemiesFinished = 0; // could have made it to the end or died ..
    [SerializeField] private GameObject DeathEffect;
    private GameModeScript GameModeScripto;

    // Unit Serialised Objects \\
    public UnitSO Soldier;
    public UnitSO Tank;
    public UnitSO Swarmer;
    public UnitSO Charger;

    // General Variables \\
    public  List<Vector2> PathPoints = new List<Vector2>();
    public  List<Vector3> ActualPathPoints = new List<Vector3>();
    public  Vector2 StartPoint;
    public  GameObject UnitStorage = null;

    public static bool EnemySpawnerComplete = false;

    // Wave Variables
    private List<UnitSO>    UnitsInWave = new List<UnitSO>();
    private bool            SpawningWaveUnits = false;
    private float           LastRecordedTime;
    private int             Counter = 0;
    private float           SpawnRate;
    private float           RoundEnemySpawnRatio; 

    public int[] UnitSpawnChance;

    private EnemyScript TempEnemyScript;

    public void InitiateEnemySpawner(float TempSpawnRate, float TempRoundEnemySpawnRatio, GameModeScript _GameModeScript)
    {
        Debug.Log("Establishing Enenemy Spawner...");


        foreach (PathTile Path in PathGenerator.PathTiles)
        {
            PathPoints.Add(Path.Cords);
        }

        StartPoint = PathPoints[PathPoints.Count - 1]; // Start point of the enemy.
        PathPoints.Reverse(); // Now going the correct direction

        SpawnRate = TempSpawnRate;
        RoundEnemySpawnRatio = TempRoundEnemySpawnRatio;
        GameModeScripto = _GameModeScript;

        Debug.Log("[Completed] Enemey Spawner has been established!");
        EnemySpawnerComplete = true;

    }

    public void Interupt()
    {

    }

    public void Update()
    {
        if (SpawningWaveUnits)
        {
            if (Counter >= UnitsInWave.Count)
            {
                SpawningWaveUnits = false;
                Debug.Log("Finished Spawning Units");
            }

            else if (Time.time - LastRecordedTime > SpawnRate)
            {
                SpawnEnemy(UnitsInWave[Counter]);
                Counter++;
                LastRecordedTime = Time.time;

            }
        }
        
        else if (EnemiesFinished == UnitsInWave.Count && UnitsInWave.Count > 0)
        {
            Debug.Log("Wave Finished");
            GameModeScripto.WaveIncoming = false;
            EnemiesFinished = 0;
            UnitsInWave.Clear();
            Counter = 0;
            GameModeScripto.CurrentRound++;

            gameObject.GetComponent<GameModeScript>().PrepareWave();
        }

    }

    public void StartWave(int CurrentRound)
    {
        GameModeScripto.WaveIncoming = true;
        int NumEnemies = Mathf.FloorToInt(CurrentRound * RoundEnemySpawnRatio);
        int RandomValue;

        for (int i = 0; i < NumEnemies; i++)
        {
            RandomValue = UtilitiesScript.RandomiseByWeight(UnitSpawnChance);

            switch (RandomValue)
            { 
                case 0 :
                    UnitsInWave.Add(Soldier);
                    break;

                case 1:
                    UnitsInWave.Add(Charger);
                    break;

                case 2:
                    UnitsInWave.Add(Tank);
                    break;

                case 3:
                    UnitsInWave.Add(Swarmer);
                    break;
            }
        }

        LastRecordedTime = Time.time;
        SpawningWaveUnits = true;

        Debug.Log("Starting Wave"); // Make Wave thing better. ;)
    }

    public void SpawnEnemy(UnitSO UnitType)
    {

        GameObject NewUnit = GameObject.Instantiate(UnitType.UnitGO);

        NewUnit.transform.tag = "Enemy";
        NewUnit.AddComponent<EnemyScript>();

        // Setting the Unit to Scale and possition , linked with the World at the current time.
        NewUnit.transform.localScale = new Vector3(MovementScript.SF, MovementScript.SF, MovementScript.SF);
        NewUnit.transform.SetParent(UnitStorage.transform);
        NewUnit.transform.localPosition = GridGenerator.GridStatus[(int)PathPoints[0].x, (int)PathPoints[0].y].Position;

        TempEnemyScript = NewUnit.GetComponent<EnemyScript>();
        TempEnemyScript.EnemySetUP(UnitType.Health, UnitType.Speed, UnitType.Points,UnitType.Mass, DeathEffect, GameModeScripto);

        NewUnit.GetComponent<EnemyScript>().PathPoints = PathPoints;
    }
}

