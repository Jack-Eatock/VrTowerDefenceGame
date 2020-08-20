
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public int ButtonNum = 0;
    public bool IsCollidingWithHands = false;


    public void Start()
    {
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
                InputScripto.OnRightTriggerClick -= Click;
                InputScripto.OnLeftTriggerClick -= Click;
                LevelManager.SwitchLevel(LevelManager.Levels.Survival);
            }
        }
    }
   


}
