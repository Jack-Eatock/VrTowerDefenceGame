using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Cleared \\

public class LevelManager : MonoBehaviour
{

    public enum Levels { Lobby, Survival, Campaign, COOP };
    public static Levels CurrentLevel = Levels.Lobby;

    public Levels SetLevel;

    public void Start()
    {
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
                Debug.Log("Loading Lobby");
                SceneManager.LoadScene("Lobby");
                break;

            case Levels.COOP:
                Debug.Log("Loading COOP");
                break;

            case Levels.Survival:
                Debug.Log("Loading Survival");

                
                SceneManager.LoadScene("Survival");
                break;

        }

        
    }
}
