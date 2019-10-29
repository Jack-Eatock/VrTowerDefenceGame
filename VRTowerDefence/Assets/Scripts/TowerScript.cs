using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour
{
    public float Damage = 2f;
    public float FireRate = 0.5f;
    public string Name;
    public float Range = 0.5f;

    public static List<GameObject> EnemiesInRange = new List<GameObject>();
    private float TimeSincelastShot = 0;

    private GameObject Target;
    private EnemyScript TargetProperties;

    private void Start()
    {
        EnemiesInRange = gameObject.GetComponent<OnCollisionScript>().ObjectsWithinCollider;
        gameObject.GetComponent<SphereCollider>().radius = Range;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameScript.WaveIncoming && EnemiesInRange.Count > 0)
        {

            //Debug.Log("Firing at :" + EnemiesInRange[0]);
            Target = EnemiesInRange[0].transform.parent.gameObject;
            TargetProperties = Target.GetComponent<EnemyScript>();
            Debug.Log("In Range");
            if (Time.time - TimeSincelastShot > FireRate)
            {

                TargetProperties.Health = TargetProperties.Health - Damage;
                Debug.Log("Hit: " + Target.name + " , For : " + Damage);
                Debug.Log("Health of Enemy is : " + TargetProperties.Health);
                TimeSincelastShot = Time.time;

            }

        }
        
    }
}
