
using System.Collections;
using UnityEngine;
using Valve.VR;

public class GameModeSurvivalScript : MonoBehaviour
{
    public static int GenerationTicker = 0;


    public delegate void TestDelegate();
    public static TestDelegate MethodToCall;

    [SerializeField] private MeshCombinerScript _meshCombinerScript;
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

        StartCoroutine(IslandInitialization());

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


    IEnumerator IslandInitialization()
    {

        Debug.Log("Initialising Island.");

        Debug.Log("Initiating Grid Generation...");
        _grid.GetComponent<GridGenerator>().InitiateGridGeneration();
        yield return new WaitUntil(() => GenerationTicker == 1);
        Debug.Log("Completed Grid Generation!");

        Debug.Log("Initiating Path Generation...");
        _gameWorld.GetComponent<PathGenerator>().InitiatePathGeneration();
        yield return new WaitUntil(() => GenerationTicker == 2);
        Debug.Log("Completed Path Generation!");

        Debug.Log("Initiating Environment Generation...");
        _gameWorld.GetComponent<EnvironmentGenerator>().InitiateEnvironmentGeneration();
        yield return new WaitUntil(() => GenerationTicker == 3);
        Debug.Log("Completed Environment Generation!");
        
   
        Debug.Log("Combining Mesh...");
        _meshCombinerScript.MyOwnAdvancedMeshCombinder();
        yield return new WaitUntil(() => GenerationTicker == 4);
        Debug.Log("Completed Combining Meshes!");
 

        Debug.Log("Enabling Controls");

        SteamVR_LoadLevel.PauseFuncFlag = true;
        MovementScript.MovementControllsDisabled = false;

        
    }
}
