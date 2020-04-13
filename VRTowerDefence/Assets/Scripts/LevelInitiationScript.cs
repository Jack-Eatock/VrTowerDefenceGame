using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cleared \\ 

public class LevelInitiationScript : MonoBehaviour
{
    [SerializeField] private GameObject _gameWorld = null;
    [SerializeField] private GameObject _ground = null;
    [SerializeField] private GameObject _grid = null;
    [SerializeField] private GameObject _enemeyStorage = null;

    public enum Levels { Lobby, Survival, Campaign, COOP };

    [SerializeField] private Levels _level;


    // Start is called before the first frame update
    void Start()
    {
        GameObject Gamemanager = GameObject.Find("GAMEMANAGER");
        MovementScript MovementScripto = GameObject.Find("Player").GetComponent<MovementScript>();

       
        MovementScripto.GameWorld = _gameWorld;
        MovementScripto.UpdateSF();
       

        if (_level == Levels.Survival)
        {
            Gamemanager.GetComponent<EnemySpawner>().UnitStorage = _enemeyStorage;

            //BuildingScripto.Ground = _ground;
            //BuildingScripto.InitatiateBuildingScript();

            _grid.GetComponent<GridGenerator>().InitiateGridGeneration();
            _gameWorld.GetComponent<PathGenerator>().InitiatePathGeneration();

            //Gamemanager.GetComponent<GameScript>().InitiateWave();

        }

        else if (_level == Levels.Lobby)
        {

        }

       
        //MovementScript.MovementControllsDisabled = false;
        
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
