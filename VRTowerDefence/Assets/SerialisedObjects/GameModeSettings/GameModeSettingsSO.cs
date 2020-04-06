using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewGameModeSettings", menuName = "GameModeSettings")]

public class GameModeSettingsSO : ScriptableObject
{
    public string   GameModeName;
    public float    EnemySpawnRatio = 3f;
    public float    SpawnRate = 0.5f;  // Time between Spawning
    public int      MaxPointsInPool = 10;
    public int      StartingPoints = 30;
 
    

}
