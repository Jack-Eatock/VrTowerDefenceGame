using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeSurvivalScript : MonoBehaviour
{

    public delegate void TestDelegate();
    public static TestDelegate MethodToCall;

    [SerializeField] private GameObject _gameWorld = null;
    [SerializeField] private GameObject _grid = null;
    [SerializeField] private GameObject _unitStorage = null;

    private EnemySpawner _enemySpawner;
    private MenuManager _menuManager;
    private GameObject _gameManager;
    private GameObject _player;

    // Start is called before the first frame updates
    void Start()
    {
        MethodToCall = OnStartWave;

        _gameManager = GameObject.Find("GAMEMANAGER");
        _player = GameObject.Find("Player");
        _enemySpawner = _gameManager.GetComponent<EnemySpawner>();


        _player.GetComponent<MovementScript>().UpdateSF();
        _gameManager.GetComponent<EnemySpawner>().UnitStorage = _unitStorage;

        _grid.GetComponent<GridGenerator>().InitiateGridGeneration();
        _gameWorld.GetComponent<PathGenerator>().InitiatePathGeneration();

        MovementScript.MovementControllsDisabled = false;

        _menuManager = _player.GetComponent<MenuManager>();
        _menuManager.SetUserPrompt("Start Wave?", "", MethodToCall );
    }

    private void Update()
    {

    }


    public void OnStartWave()
    {
        Debug.Log("StartWave");
        _enemySpawner.InitiateEnemySpawner();
    }

}
