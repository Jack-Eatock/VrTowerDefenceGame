using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{

    public static int   CurrentRound = 1;
    public static float RoundEnemySpawnRatio = 3f;
    public static float SpawRate = 0.5f;  // Time between Spawning
    public static bool  WaveIncoming = false;
    public static int   PointPool = 0;
    public static int   Points = 30;


    public bool FirstTime = true;

    // Start is called before the first frame update
    void Start()
    {
        if (FirstTime)
        {
            DontDestroyOnLoad(gameObject);   
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
