using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Rocket : ProjectileScript
{
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private ParticleSystem Ps;

    private void Start()
    {
        var psMain = Ps.main;
        psMain.customSimulationSpace = GameObject.Find("World").transform;
    }

    public override void Move()
    {
        transform.LookAt(_target.position);
        base.Move();
    }

    public override void HitTarget(bool hasHitTarget)
    {
        GameObject effect = GameObject.Instantiate(_explosionEffect);
  

        Debug.Log("EXPLODE!");

        if (hasHitTarget) // If hits Target
        {
            UtilitiesScript.AttachObjectToWorld(effect, _target.localPosition,true);
            _target.GetComponent<EnemyScript>().OnHit(_firingTowerProperties);
        }

        else
        {
            //UtilitiesScript.AttachObjectToWorld(effect, transform.position, true);
        }

        
        Destroy(gameObject);

        //base.HitTarget(hasHitTarget);
    }
}
