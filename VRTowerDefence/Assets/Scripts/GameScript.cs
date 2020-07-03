using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{

    // Independant Variables \\ 

    public static int   CurrentRound = 1;
    public static bool  WaveIncoming = false;
    public static int   PointPool = 0;
    public static int   Points = 30;

    // Dependant Variables \\

    public static int StartWorldScale = 1;
    public static string GameSettingsName = "";
    public static int StartPoints = 0;
    public static int MaxPointPool = 10;
    public static float RoundEnemySpawnRatio = 3f;
    public static float SpawRate = 0.5f;  // Time between Spawning

    [SerializeField] private GameModeSettingSO _survivalSettings = null;

    

    private bool FirstTime = true;

    // Start is called before the first frame update
    void Start()
    {
        if (FirstTime)
        {
            DontDestroyOnLoad(gameObject);

            //QualitySettings.vSyncCount = 0;
            //Application.targetFrameRate = 300;
            FirstTime = false;
        }

       // InitiateWave();
        // BuildingScript.MenuControllsDisabled = false; // Enables Building once the Path is generated.
    }

    // Update is called once per frame
    void Update()
    {
        if ( PointPool >= 10)
        {
            Debug.Log("GameOver!");
        }
        
    }


    private void OnLevelWasLoaded(int level)
    {


        if (level == 0)  // Intro
        {

        }

        else if (level == 1) // Lobby
        {

        }

        else if (level == 2)// Survival
        {
            LoadSetVariableForLevel(_survivalSettings);
        }
    }

    private void LoadSetVariableForLevel(GameModeSettingSO gameMode)
    {
        StartWorldScale = gameMode.WorldStartScale;
        GameSettingsName = gameMode.GameModeName;
        StartPoints = gameMode.StartPoints;
        MaxPointPool = gameMode.MaxPointPool;
        RoundEnemySpawnRatio = gameMode.RoundEnemySpawnRatio;
        SpawRate = gameMode.SpawnRate;

        Debug.Log("Loaded Game Settings: " + gameMode.GameModeName);
    
    }
    

    /*
        public void InitiateWave()
        {

            BuildingScript.MenuControllsDisabled = true;
            //gameObject.GetComponent<BuildingScript>().ActivateMenu(false, 0);
            gameObject.GetComponent<BuildingScript>().ActivateMenu(true, 1);
            SetCanvasMesage("Start Wave: " + CurrentRound + "?");
        }

        public void SetCanvasMesage(string Message)
        {
            Text nameText = GeneralMenuTextGO.GetComponent<Text>();
            nameText.text = Message;

        }

        */
}
