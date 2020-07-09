using UnityEngine.SceneManagement;
using UnityEngine;
using Valve.VR;

// Cleared \\

public class LevelManager : MonoBehaviour
{

    public bool useTransitions = false;
    public static bool UseTransitions;

    public void OnValidate()
    {
        UseTransitions = useTransitions;
    }



    public enum Levels { Lobby, Survival, Campaign, COOP };
    public static Levels CurrentLevel = Levels.Lobby;

    public Levels SetLevel;

    public GameObject _SceneTransition;
    public static GameObject SceneTransition;


    public void Start()
    {
        SceneTransition = _SceneTransition;
        SwitchLevel(Levels.Lobby);
       
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
                    SceneManager.LoadScene("Lobby");
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
                    SceneManager.LoadScene("Survival");
                }
                //Debug.Log("Loading Survival");
               

                //

                break;

        }

        
    }


    public static void BeginSceneTransition(string sceneName, bool usepauseFunc)
    {
        GameObject sceneTransition = GameObject.Instantiate(SceneTransition);
        SteamVR_LoadLevel loadLevel = sceneTransition.GetComponent<SteamVR_LoadLevel>();

        loadLevel.usePauseFunc = usepauseFunc;
        loadLevel.levelName = sceneName;
        loadLevel.enabled = true;
    }
}
