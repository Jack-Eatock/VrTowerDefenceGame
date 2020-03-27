using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInitiationScript : MonoBehaviour
{
    [SerializeField] private GameObject GameWorld = null;
    [SerializeField] private GameObject Ground = null;
    [SerializeField] private GameObject Grid = null;
    [SerializeField] private GameObject EnemeyStorage = null;

    public enum Levels { Lobby, Survival, Campaign, COOP };

    [SerializeField] private Levels Level;


    // Start is called before the first frame update
    void Start()
    {
        GameObject Gamemanager = GameObject.Find("GAMEMANAGER");
        MovementScript MovementScripto = GameObject.Find("Player").GetComponent<MovementScript>();
        BuildingScript BuildingScripto = Gamemanager.GetComponent<BuildingScript>();

       
        MovementScripto.GameWorld = GameWorld;
        MovementScripto.UpdateSF();
       

        if (Level == Levels.Survival)
        {
            Gamemanager.GetComponent<EnemySpawner>().UnitStorage = EnemeyStorage;
            BuildingScripto.Ground = Ground;
            BuildingScripto.InitatiateBuildingScript();
            Grid.GetComponent<GridGenerator>().InitiateGridGeneration();
            GameWorld.GetComponent<PathGenerator>().InitiatePathGeneration();
            Gamemanager.GetComponent<GameScript>().InitiateWave();

        }

        else if (Level == Levels.Lobby)
        {

        }

       
        MovementScript.MovementControllsDisabled = false;
        
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
