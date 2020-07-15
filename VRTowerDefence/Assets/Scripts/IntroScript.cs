using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting Introduction");

        StartCoroutine(Intro());

    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(1f);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
