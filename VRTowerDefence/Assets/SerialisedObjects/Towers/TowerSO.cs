using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewTower", menuName = "Tower")]
public class TowerSO :  ScriptableObject
{

    public string Name;
    public float FireRate;
    public float Range;
    public float ProjectileSpeed;
    public float ProjectileExplosionRadius = 0;
    public float ProjectileDamagePerEnemyHit;


    public GameObject TowerGO;
    public GameObject MinitureVersion4Menu;
    public GameObject ProjectileGO;

}
