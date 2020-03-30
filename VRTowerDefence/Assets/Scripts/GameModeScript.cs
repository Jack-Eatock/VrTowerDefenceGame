using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeScript : MonoBehaviour
{

    public  int     CurrentRound = 1;
    public  bool    WaveIncoming = false;
    public  int     PointPool = 0;
    public  static  int     Points = 30;

    [SerializeField] private  float RoundEnemySpawnRatio = 3f;
    [SerializeField] private  float SpawnRate = 0.5f;  // Time between Spawning
    



    [SerializeField] GameObject World = null;
    [SerializeField] EnemySpawner EnemySpawnerScript = null;
    [SerializeField] GridGenerator GridGeneratorScript = null;
    [SerializeField] PathGenerator PathGeneratorScript = null;

    private GameObject Player = null;
    private MenuManagerScript MenuScript;


    private bool Flag = true;
    private bool FlagTwo = true;
    private bool FlagThree = false;


    private void Awake()
    {
        Debug.Log(" Starting Survival GameMode!");

        Player = GameObject.Find("Player");
        MenuScript = Player.GetComponent<MenuManagerScript>();
        Player.GetComponent<MovementScript>().GameWorld = World;    // Sets the world for the Movement script
        Player.GetComponent<MovementScript>().UpdateSF();          // and ensures the SF is updated before initiating the Grid.

        GridGeneratorScript.InitiateGridGeneration();               // Generates Grid and Tiles.
        PathGeneratorScript.InitiatePathGeneration();              // Generates Path. 

    }


    // Update is called once per frame
    void Update()
    {
        if (FlagThree)
        {
            return;
        }

        if (PathGenerator.PathGenerationComplete)           // When the path finishes generating 
        {
            if (Flag)
            {
                Flag = false;                                                                                                              // stops looping
                MovementScript.MovementControllsDisabled = false;                                                                         // When Complete Movement Controls are enabled
                EnemySpawnerScript.InitiateEnemySpawner(SpawnRate, RoundEnemySpawnRatio, this.GetComponent<GameModeScript>());           // Initiates Enemy Spawner Script.
            }
           
            if (EnemySpawner.EnemySpawnerComplete)
            {
                if (FlagTwo)
                {
                    FlagTwo = false;                                        // stops looping

                    
                    BuildingScript.IsBuidlingModeActive = true;

                    //BuildingScripto.InitatiateBuildingScript();
                    //BuildingScripto.PlacedTowerStorageGO = PlacedTowerStorage;

                    //Player.GetComponent<GameScript>().InitiateWave();

                    FlagThree = true;
                }
            }


        }
    }


    public void PrepareWave()
    {

        EnemySpawnerScript.StartWave(CurrentRound);


        //BuildingScript.MenuControllsDisabled = true;
        //gameObject.GetComponent<BuildingScript>().ActivateMenu(false, 0);
       // Player.GetComponent<BuildingScript>().ActivateMenu(true, 1);
        //SetCanvasMesage("Start Wave: " + CurrentRound + "?");
    }

    public void SetCanvasMesage(string Message)
    {
       

    }
}
