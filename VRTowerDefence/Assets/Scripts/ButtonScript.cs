using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public int ButtonNum = 0;
    public bool IsCollidingWithHands = false;
    [SerializeField] private GameModeScript GameScript = null;


    private void Awake()
    {
        Debug.Log("START");
        InputScripto.OnRightTriggerClick += Click;
        InputScripto.OnLeftTriggerClick += Click;
    }

    void Click()
    {
        if (IsCollidingWithHands)
        {
      
            if (ButtonNum == 0)
            {
                Debug.Log("ERROR - Give this button a purpose (ButtonNum = 0)" + gameObject.transform.name);
            }

            else if (ButtonNum == 1)
            {
                Debug.Log("Clicked Button");
                RemoveListeners();
                LevelManager.SwitchLevel(LevelManager.Levels.Survival);
            }

            else if (ButtonNum == 2)
            {
                Debug.Log("Clicked Button");
                RemoveListeners();
                LevelManager.SwitchLevel(LevelManager.Levels.Campaign);
            }

            else if (ButtonNum == 3)
            {
                Debug.Log("Start Wave Button CLicked");
                StartCoroutine(UtilitiesScript.ObjectBlinkColour(gameObject, Color.green, 0.15f)); // Flash Green 
                GameScript.PrepareWave();
            }
        }
    }


    public void RemoveListeners()
    {
        InputScripto.OnRightTriggerClick -= Click;
        InputScripto.OnLeftTriggerClick -= Click;
    }
   


}
