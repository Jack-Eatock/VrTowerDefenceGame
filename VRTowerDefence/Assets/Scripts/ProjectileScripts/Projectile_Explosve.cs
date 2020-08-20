using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Explosve : ProjectileScript
{
    public override void Move()
    {
        base.Move();
    }

    public override void HitTarget(bool hasHitTarget)
    {
        Debug.Log("EXPLODE!");
        base.HitTarget(hasHitTarget);
    }
}
 