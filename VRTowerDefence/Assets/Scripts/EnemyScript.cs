using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// cleared \\

public class EnemyScript : MonoBehaviour
{
    private GameObject DeathEffect;

    public float Health;
    private float _startHealth;
    private float _speed;
    private int _points;
    private int _mass;

    public List<Vector2> PathPoints = new List<Vector2>();
    public List<Vector3> LocalPathPoints = new List<Vector3>();
    public List<GameObject> TowersTargetingUnit = new List<GameObject>();

    // Healthbar UI Stuff

    private GameObject _healthBarGO;
    private Image _healthBar;

    private int _checkPoint = 0;
    private bool _loop = true;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x <= PathPoints.Count - 1; x++)
        {
            LocalPathPoints.Add(GridGenerator.GridStatus[(int)PathPoints[x].x, (int)PathPoints[x].y].Position);
        }

        _healthBarGO = gameObject.transform.GetChild(0).gameObject;
        _healthBar = _healthBarGO.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();


    }

    public void EnemySetUP(float _Health, float _Speed, int _Points, int Mass_, GameObject DeathEffect_)
    {
        DeathEffect = DeathEffect_;
        Health = _Health;
        _speed = _Speed;
        _points = _Points;
        _mass = Mass_;
        _startHealth = _Health;
    }

    // Update is called once per frame
    void Update()
    {
        if (_loop)
        {
            _healthBarGO.transform.LookAt(Camera.main.transform.position);

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

               
                GameScript.Points += _points;
                EnemySpawner.EnemiesFinished++;

                GameObject deathEffectGO = Instantiate(DeathEffect, transform.position, DeathEffect.transform.rotation);
                deathEffectGO.transform.SetParent(GameObject.Find("World").transform);
                deathEffectGO.transform.localScale = new Vector3(MovementScript.ScaleFactor, MovementScript.ScaleFactor, MovementScript.ScaleFactor);
                
                Destroy(gameObject);
          

            }

            if (_checkPoint < PathPoints.Count - 1)
            {
                if (Vector3.Distance(transform.localPosition, LocalPathPoints[_checkPoint + 1]) > 0.001f)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, LocalPathPoints[_checkPoint + 1], _speed * Time.deltaTime);
                }

                else
                {
                    _checkPoint++;
                }

            }
            else
            {
                Debug.Log("Enemy made it to the Finish");
                EnemySpawner.EnemiesFinished++;
                _loop = false;
                GameScript.PointPool += _mass;
                Destroy(gameObject);
            }
        }
    }

    public void OnHit(TowerSO firingTowerProperties)
    {
        StartCoroutine(UtilitiesScript.ObjectBlinkColour(gameObject, Color.red, 0.1f));

        
        Health -= firingTowerProperties.ProjectileDamagePerEnemyHit;

        if(_healthBar != null)
        {
            _healthBar.fillAmount = Health / _startHealth;
        }
     

        switch (firingTowerProperties.ProjectileType)
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
