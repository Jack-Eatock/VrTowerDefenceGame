using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour
{
    public TowerSO TowerProperties;

    public List<GameObject> EnemiesInRange = new List<GameObject>();
    private float TimeSincelastShot = 0;

    private GameObject Target;
    private GameObject Projectile;
    private ProjectileScript TempProjectileScript;
    private Transform BulletStorage;

    private void Start()
    {
        EnemiesInRange = gameObject.GetComponent<OnCollisionScript>().ObjectsWithinCollider;
        gameObject.GetComponent<SphereCollider>().radius = TowerProperties.Range;
        BulletStorage = GameObject.Find("BulletStorage").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (GameScript.WaveIncoming && EnemiesInRange.Count > 0)
        {
            if (EnemiesInRange[0] == null)
            {
                EnemiesInRange.RemoveAt(0);
            }
            else
            {
                Target = EnemiesInRange[0].transform.parent.gameObject;

                // Debug.Log("In Range");

                if (Time.time - TimeSincelastShot > TowerProperties.FireRate)
                {
                    Fire();
                    TimeSincelastShot = Time.time;

                }
            }
        }
    }

    void Fire()
    {
        //Debug.Log("Firing");
        Projectile = GameObject.Instantiate(TowerProperties.ProjectileGO,BulletStorage);
        Projectile.transform.position = gameObject.transform.GetChild(0).position;
        TempProjectileScript = Projectile.AddComponent<ProjectileScript>();
        //Debug.Log("Bullet");

        if (Projectile != null)
        {
            TempProjectileScript.SetTarget(Target.transform, TowerProperties);
        }


    }

}
