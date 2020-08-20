//using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using Valve.VR;

// Cleared \\

public class LevelManager : MonoBehaviour
{

    public bool useTransitions = true;
    public static bool UseTransitions = true;

    public static GameObject LoadingScreen;
    [SerializeField] private GameObject _loadingScreen;
    
    public void OnValidate()
    {
        UseTransitions = useTransitions;
    }
    


    public enum Levels { Intro ,Lobby, Survival, Campaign, COOP };
    public static Levels CurrentLevel = Levels.Intro;

    public Levels SetLevel = Levels.Survival;

    public GameObject _SceneTransition;
    public static GameObject SceneTransition;


    public void Start()
    {
        SceneTransition = _SceneTransition;
        LoadingScreen = _loadingScreen;

    }

    public void Update()
    {
        if (SetLevel != CurrentLevel)
        {
            SwitchLevel(SetLevel);
        }
    }

    public static void SwitchLevel(Levels NewLevel)
    {
        Debug.Log("Switching Level");

        GameScript.CleanSlate.Invoke();

        MovementScript.MovementControllsDisabled = true;

        CurrentLevel = NewLevel;

        GameObject.Find("GAMEMANAGER").GetComponent<LevelManager>().SetLevel = CurrentLevel;
        
        switch (CurrentLevel)
        {
            case Levels.Campaign:
                Debug.Log("Loading Campaign");
               
                break;

            case Levels.Lobby:

                if (UseTransitions)
                {
                    BeginSceneTransition("Lobby", false);
                }
                else
                {
                   // SceneManager.LoadScene("Lobby");
                }

                Debug.Log("Loading Lobby");
                

                //SceneManager.LoadScene("Lobby");
                break;

               
            case Levels.COOP: 
                Debug.Log("Loading COOP");
                break;

            case Levels.Survival:

                if (UseTransitions)
                {
                    BeginSceneTransition("Survival", true);
          
                }
                else
                {
                   // SceneManager.LoadScene("Survival");
                }
                //Debug.Log("Loading Survival");
               

                //

                break;

        }

       

        
    }


    public static void BeginSceneTransition(string sceneName, bool usepauseFunc)
    {
        LoadingScreenScripts newScript = LoadingScreen.GetComponent<LoadingScreenScripts>();
        if (usepauseFunc)
        {          
            newScript.StartCoroutine(newScript.LoadingScreenFunc(true));
        }
        else
        {
            newScript.StartCoroutine(newScript.LoadingScreenFunc(false));
        }

        GameObject sceneTransition = GameObject.Instantiate(SceneTransition);
        SteamVR_LoadLevel loadLevel = sceneTransition.GetComponent<SteamVR_LoadLevel>();

        loadLevel.usePauseFunc = usepauseFunc;
        loadLevel.levelName = sceneName;
        loadLevel.enabled = true;
    }

 
}
