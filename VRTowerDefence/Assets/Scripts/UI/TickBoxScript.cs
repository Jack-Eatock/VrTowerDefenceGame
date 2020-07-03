using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickBoxScript : MonoBehaviour
{
    [SerializeField] private GameObject TickBox;

    private bool _active = false;

   public void TickBoxClicked()
    {
        if (_active)
        {
            TickBox.SetActive(false);
            _active = false;
        }

        else
        {
            TickBox.SetActive(true);
            _active = true;
        }
    }
}
