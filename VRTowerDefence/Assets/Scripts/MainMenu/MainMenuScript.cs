using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputScripto.OnMainMenuPressed += OnMenuButtonPressed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMenuButtonPressed()
    {
        Debug.Log("MainMenuButtonPressed");
    }
}
