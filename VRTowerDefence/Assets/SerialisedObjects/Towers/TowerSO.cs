using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewTower", menuName = "Tower")]
public class TowerSO :  ScriptableObject
{
    public GameObject TowerGO;
    public string Name;
    public float DamagePerShot;
    public float FireRate;
    public float Range;
    public GameObject MinitureVersion4Menu;

}
