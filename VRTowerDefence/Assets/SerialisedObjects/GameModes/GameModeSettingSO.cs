using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameModeSettings", menuName = "GameModeSettings")]
public class GameModeSettingSO :  ScriptableObject
{

    public string   GameModeName;
    public  float   RoundEnemySpawnRatio = 3f;
    public  float   SpawnRate = 0.5f;  // Time between Spawning
    public  int     MaxPointPool = 0;
    public  int     StartPoints = 30;

}
