using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private GameObject DeathEffect;

    public float Health;
    private float Speed;
    private int Points;
    private int Mass;

    public List<Vector2> PathPoints = new List<Vector2>();
    public List<Vector3> LocalPathPoints = new List<Vector3>();
    public List<GameObject> TowersTargetingUnit = new List<GameObject>();

    private int CheckPoint = 0;
    private Transform Hit;

    private bool Loop = true;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x <= PathPoints.Count - 1; x++)
        {
            LocalPathPoints.Add(GridGenerator.GridStatus[(int)PathPoints[x].x, (int)PathPoints[x].y].Position);
        }
    }

    public void EnemySetUP(float _Health, float _Speed, int _Points, int Mass_, GameObject DeathEffect_)
    {
        DeathEffect = DeathEffect_;
        Health = _Health;
        Speed = _Speed;
        Points = _Points;
        Mass = Mass_;
    }

    // Update is called once per frame
    void Update()
    {
        if (Loop)
        {
            if (Health <= 0)
            {
                Debug.Log("Enemy Killed");
                
                foreach (GameObject GO in UtilitiesScript.ObjectsAffected)
                {
                    if (GO == gameObject)
                    {
                        UtilitiesScript.ObjectsAffected.RemoveAt(UtilitiesScript.ObjectsAffected.IndexOf(gameObject));
                        break;
                    }
                }

               
                GameScript.Points += Points;
                EnemySpawner.EnemiesFinished++;
                GameObject DeathEffectGO = Instantiate(DeathEffect, transform.position, DeathEffect.transform.rotation);
                DeathEffectGO.transform.SetParent(GameObject.Find("World").transform);
                DeathEffectGO.transform.localScale = new Vector3(MovementScript.LocalSF, MovementScript.LocalSF, MovementScript.LocalSF);
                



                Destroy(gameObject);
          

            }

            if (CheckPoint < PathPoints.Count - 1)
            {
                if (Vector3.Distance(transform.localPosition, LocalPathPoints[CheckPoint + 1]) > 0.001f)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, LocalPathPoints[CheckPoint + 1], Speed * Time.deltaTime);
                }

                else
                {
                    CheckPoint++;
                }

            }
            else
            {
                Debug.Log("Enemy made it to the Finish");
                EnemySpawner.EnemiesFinished++;
                Loop = false;
                GameScript.PointPool += Mass;
                Destroy(gameObject);
            }
        }
    }

    public void OnHit(TowerSO FiringTowerProperties, Transform Hit_)
    {
        StartCoroutine(UtilitiesScript.ObjectBlinkColour(gameObject, Color.red, 0.1f));
        Hit = Hit_;
        Health -= FiringTowerProperties.ProjectileDamagePerEnemyHit;

        switch (FiringTowerProperties.ProjectileType)
        {
            case TowerSO.ProjectileTypes.Default:
                
                break;

            case TowerSO.ProjectileTypes.Explosive:

                break;

            case TowerSO.ProjectileTypes.Explosive2:

                break;

            case TowerSO.ProjectileTypes.Gas:

                break;

        }
    }


    public void DisplayhealthBar(bool Activate)
    {

    }

    public void UpdateHealthBar()
    {

    }
} 
