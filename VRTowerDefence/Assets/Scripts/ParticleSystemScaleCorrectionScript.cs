using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemScaleCorrectionScript : MonoBehaviour
{
    ParticleSystem ps;

    // Start is called before the first frame update
    private void Awake()
    {
        ps = transform.GetComponent<ParticleSystem>();

        var main = ps.main;
        main.gravityModifier = main.gravityModifier.constant * MovementScript.ScaleFactor;
        main.gravityModifier = main.startSpeed.constant * MovementScript.ScaleFactor;

    }

    // Update is called once per frame.
    void Update()
    {
        
    }
}
