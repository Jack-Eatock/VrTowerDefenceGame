using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    [SerializeField] private int All_X_Y = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (All_X_Y == 0) { transform.LookAt(Camera.main.transform.position); }

        else if (All_X_Y == 1)
        {
            Vector3 targetPos = new Vector3(transform.position.x, Camera.main.transform.position.y , Camera.main.transform.position.z);
            transform.LookAt(targetPos);

        }

        else if (All_X_Y == 2) {

            Vector3 targetPos = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
            transform.LookAt(targetPos);

        }
    }
}
