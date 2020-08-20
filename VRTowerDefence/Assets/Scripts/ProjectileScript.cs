using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cleared \\

public abstract class ProjectileScript : MonoBehaviour
{
    public TowerSO _firingTowerProperties;
    public Transform _target;
    public bool _initiated = false;

    public void SetTarget(Transform target, TowerSO properties)
    {

        _target = target;
        _firingTowerProperties = properties;
        _initiated = true;
    }

    // Update is called once per frame
    public void Update()
    {
        
        if (_initiated)
        {
            if (_target == null)
            {
                HitTarget(false);
                return;
            }

            Move();

        }
    }


    public virtual void Move()
    {
        Vector3 targetPos = _target.GetChild(0).transform.position;
        Vector3 targetDir = targetPos - transform.position; // Distance between Bullet and target.

        float distanceThisFrame = _firingTowerProperties.ProjectileSpeed * Time.deltaTime * MovementScript.ScaleFactor;  // Calculates how much the bullet can travel in this frame.

        if (targetDir.magnitude <= distanceThisFrame) // Hit the object
        {
            HitTarget(true);
            return;
        }

        transform.Translate(targetDir.normalized * distanceThisFrame, Space.World); // Normalises the Distance so that it retainains the direction but magnitude of 1. And then moves in that direction based on move per frame.
    }

    public virtual void HitTarget(bool hasHitTarget)
    {
        if (hasHitTarget) // If hits Target
        {
            _target.GetComponent<EnemyScript>().OnHit(_firingTowerProperties);
        }

        Destroy(gameObject);
    }
}
