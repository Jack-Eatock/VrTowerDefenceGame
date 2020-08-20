using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Default : ProjectileScript
{
    public override void HitTarget(bool hasHitTarget)
    {
        Debug.Log(" OVERIDE");


        if (hasHitTarget) // If hits Target
        {
            _target.GetComponent<EnemyScript>().OnHit(_firingTowerProperties);
        }

        Destroy(gameObject);
    }
}
