using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public enum Levels { Lobby, Survival, Campaign, COOP };

    public static Levels CurrentLevel = Levels.COOP;
    public static Levels LastLevel = Levels.Lobby;

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
        if (CurrentLevel == NewLevel)
        {
            return;
        }

       
        MovementScript.MovementControllsDisabled = true;
        BuildingScript.IsBuidlingModeActive = false;
        GameObject.Find("Player").GetComponent<BuildingScript>().Interupt();

        LastLevel = CurrentLevel;
        CurrentLevel = NewLevel;
        

        GameObject.Find("Player").GetComponent<LevelManager>().SetLevel = CurrentLevel;
        
        switch (CurrentLevel)
        {
            case Levels.Campaign:
                Debug.Log("Loading Campaign");
                break;

            case Levels.Lobby:
                Debug.Log("Loading Lobby");
                //GameObject.Find("Player").GetComponent<BuildingScript>().Running = false;
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


    /*
     * 
     *  Turned out to be a bad idea....

    public static void Restart()
    {
        GameObject Gamemanager =  GameObject.Find("GAMEMANAGER");
        GameObject Player = GameObject.Find("Player");

        Destroy(Player);

        SceneManager.LoadScene("Intro");

        Destroy(Gamemanager);
    }

    */
}
