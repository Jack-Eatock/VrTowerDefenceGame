﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public enum Levels { MainMenu, Survival, Campaign, COOP };
    public Levels CurrentLevel = Levels.MainMenu;

    public void SwitchLevel(Levels NewLevel)
    {
        CurrentLevel = NewLevel;
        switch (CurrentLevel)
        {
            case Levels.Campaign:
                Debug.Log("Loading Campaign");
               
                break;

            case Levels.MainMenu:
                Debug.Log("Loading MainMenu");
                SceneManager.LoadScene("MainMenu");
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