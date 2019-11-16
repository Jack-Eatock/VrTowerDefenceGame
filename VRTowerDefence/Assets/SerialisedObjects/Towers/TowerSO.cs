using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewTower", menuName = "Tower")]
public class TowerSO :  ScriptableObject
{
    public enum ProjectileTypes
    {
        Default,
        Explosive,
        Explosive2,
        Gas,
        tar,
        Magic1
    };

    public string Name;
    public float FireRate;
    public float Range;
    public float ProjectileSpeed;
    public float ProjectileDamagePerEnemyHit;
    public int Cost;



    public GameObject TowerGO;
    public GameObject MinitureVersion4Menu;
    public GameObject ProjectileGO;

    public ProjectileTypes ProjectileType;



   
}
