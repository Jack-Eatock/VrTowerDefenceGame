using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewUnit", menuName = "Unit")]
public class UnitSO : ScriptableObject
{
    public GameObject UnitGO;
    public string Name;
    public float Speed;
    public float Health;
    public int Points;

    public string Description;
}
