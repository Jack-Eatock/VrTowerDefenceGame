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
                HitTarget(false);
                return;
            }

            Vector3 TargetPos = Target.GetChild(0).transform.position;

            Vector3 TargetDir = TargetPos - transform.position; // Distance between Bullet and target.
            float DistanceThisFrame = FiringTowerProperties.ProjectileSpeed * Time.deltaTime;  // Calculates how much the bullet can travel in this frame.

            if (TargetDir.magnitude <= DistanceThisFrame) // Hit the object
            {
                HitTarget(true);
                return;
            }

            transform.Translate(TargetDir.normalized * DistanceThisFrame, Space.World); // Normalises the Distance so that it retainains the direction but magnitude of 1. And then moves in that direction based on move per frame.

        }
    }


    void HitTarget(bool Hit)
    {
        if (Hit) // If hits Target
        {
            Target.GetComponent<EnemyScript>().OnHit(FiringTowerProperties);

            // Debug.Log("We hit Something");
            switch (FiringTowerProperties.ProjectileType)
            {
                case TowerSO.ProjectileTypes.Explosive:
                    Debug.Log("Explosion");
                    break;

                case TowerSO.ProjectileTypes.Default:
                    Debug.Log("Default");
                    
                    break;

                case TowerSO.ProjectileTypes.Explosive2:

                    break;

                case TowerSO.ProjectileTypes.Gas:

                    break;
            }

        }
        Destroy(gameObject);
    }
}
