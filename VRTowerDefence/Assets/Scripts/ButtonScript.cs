
using UnityEngine;

public class ButtonScript : MonoBehaviour, IInteractable
{
    public int ButtonNum = 0;

    public void Interact(bool isLeftHand)
    {
        Debug.Log("Button Inteacted");










        if (ButtonNum == 0)
        {
            Debug.Log("ERROR - Give this button a purpose (ButtonNum = 0)" + gameObject.transform.name);
        }

        else if (ButtonNum == 1)
        {
            Debug.Log("Clicked Button");
            LevelManager.SwitchLevel(LevelManager.Levels.Survival);
        }
    }

}
