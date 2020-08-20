using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapTurnScript : MonoBehaviour
{

    public bool Enabled = true;
    public int AngleToRotate = 45;
    public Transform Camera;

    private void Start()
    {
        InputScripto.OnSnapTurnLeft += OnSnapTurnLeft;
        InputScripto.OnSnapTurnRight += OnSnapTurnRight;
    }

    public void OnSnapTurnLeft()
    {
        SnapTurn(true);
    }

    public void OnSnapTurnRight()
    {
        SnapTurn(false);
    }

    void SnapTurn(bool isLeft)
    {
        Debug.Log("SNAP");

        Vector3 cameraPosBefore = Camera.position;

        if (isLeft)
        {
           // Quaternion rot = Quaternion.Euler(0, -AngleToRotate, 0);
            transform.Rotate(0, -AngleToRotate, 0);
        }
        else
        {
            Quaternion rot = Quaternion.Euler(0, AngleToRotate, 0);
            //transform.rotation *= rot;
            transform.Rotate(0, AngleToRotate, 0);
        }

        transform.position += (cameraPosBefore - Camera.position);

       
    }
}
