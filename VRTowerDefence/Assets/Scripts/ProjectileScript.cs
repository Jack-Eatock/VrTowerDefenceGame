using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cleared \\

public class ProjectileScript : MonoBehaviour
{
    private TowerSO _firingTowerProperties;
    private Transform _target;
    private bool _initiated = false;

    public void SetTarget(Transform target, TowerSO properties) {

        _target = target;
        _firingTowerProperties = properties;
        _initiated = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_initiated)
        {
            if (_target == null)
            {
                HitTarget(false);
                return;
            }

            Vector3 targetPos = _target.GetChild(0).transform.position;
            Vector3 targetDir = targetPos - transform.position; // Distance between Bullet and target.

            float distanceThisFrame = _firingTowerProperties.ProjectileSpeed * Time.deltaTime;  // Calculates how much the bullet can travel in this frame.

            if (targetDir.magnitude <= distanceThisFrame) // Hit the object
            {
                HitTarget(true);
                return;
            }

            transform.Translate(targetDir.normalized * distanceThisFrame, Space.World); // Normalises the Distance so that it retainains the direction but magnitude of 1. And then moves in that direction based on move per frame.

        }
    }


    void HitTarget(bool hasHitTarget)
    {
        if (hasHitTarget) // If hits Target
        {
            _target.GetComponent<EnemyScript>().OnHit(_firingTowerProperties);

            //Debug.Log("We hit Something");
            switch (_firingTowerProperties.ProjectileType)
            {
                case TowerSO.ProjectileTypes.Explosive:
                    //Debug.Log("Explosion");
                    break;

                case TowerSO.ProjectileTypes.Default:
                    //Debug.Log("Default");
                    
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
