using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// cleared \\

public class TowerScript : GameScript
{
    public TowerSO TowerProperties;

    public List<GameObject> EnemiesInRange = new List<GameObject>();

    private float               _timeSincelastShot = 0;
    private GameObject          _target;
    private GameObject          _projectile;
    private Transform           _bulletStorage;

    private void Start()
    {

        SphereCollider SphereCol = gameObject.AddComponent<SphereCollider>();
        SphereCol.center = new Vector3(0, 0.5f, 0);
        SphereCol.isTrigger = true;
        SphereCol.radius = TowerProperties.Range;


        OnCollisionScript TempColScript = gameObject.AddComponent<OnCollisionScript>();
        TempColScript.CollisionType = 3;

        EnemiesInRange = gameObject.GetComponent<OnCollisionScript>().ObjectsWithinCollider;     
        _bulletStorage = GameObject.Find("BulletStorage").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (WaveIncoming && EnemiesInRange.Count > 0)
        {
            if (EnemiesInRange[0] == null)
            {
                EnemiesInRange.RemoveAt(0);
            }
            else
            {
                _target = EnemiesInRange[0].transform.parent.gameObject;

                // Debug.Log("In Range");

                if (Time.time - _timeSincelastShot > TowerProperties.FireRate)
                {
                    Fire();
                    _timeSincelastShot = Time.time;

                }
            }
        }
    }

    void Fire()
    {
        //Debug.Log("Firing");
        _projectile = GameObject.Instantiate(TowerProperties.ProjectileGO,_bulletStorage);
        _projectile.transform.position = gameObject.transform.GetChild(0).position;

        //Debug.Log("Bullet");
        ProjectileScript _tempProjectileScript = _projectile.GetComponent<ProjectileScript>();
        //Debug.Log("We hit Something");

        if (_projectile != null)
        {
            _tempProjectileScript.SetTarget(_target.transform, TowerProperties);
        }


    }

}
