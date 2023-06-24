using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Data", menuName = "Game Data/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;
    public float coolDown;
    public int minDamge;
    public int maxDamage;
    public float criticalMultiplier;
    public float criticalChance;
}
