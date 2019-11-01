using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private TowerSO FiringTowerProperties;
    private Transform Target;
    private bool Initiated = false;

    public void SetTarget(Transform _Target, TowerSO _Properties) {
        Target = _Target;
        FiringTowerProperties = _Properties;
        Initiated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Initiated)
        {
            if (Target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 TargetDir = Target.position - transform.position; // Distance between Bullet and target.
            float DistanceThisFrame = FiringTowerProperties.ProjectileSpeed * Time.deltaTime;  // Calculates how much the bullet can travel in this frame.

            if (TargetDir.magnitude <= DistanceThisFrame) // Hit the object
            {
                HitTarget();
                return;
            }

            transform.Translate(TargetDir.normalized * DistanceThisFrame, Space.World); // Normalises the Distance so that it retainains the direction but magnitude of 1. And then moves in that direction based on move per frame.

        }
    }


    void HitTarget()
    {
       // Debug.Log("We hit Something");
        Target.GetComponent<EnemyScript>().Health -= FiringTowerProperties.ProjectileDamagePerEnemyHit;


        Destroy(gameObject);
    }
}
