using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD, PATROL, CHASE, DEAD
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [Header("Base")] public float sightRadius;
    
    
    private NavMeshAgent _agent;

    // State
    private EnemyStates enemyStates;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        SwitchState();
    }

    private void SwitchState()
    {
        if (FindPlayer()) Debug.Log("Fund Player");
        
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                break;
            case EnemyStates.CHASE:
                break;
            case EnemyStates.DEAD:
                break;
        }
    }

    private bool FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && Vector3.Distance(player.transform.position, transform.position) <= sightRadius)
        {
            return true;
        }

        return false;
    }
    
    // 性能比较差
    // private bool FindPlayer2()
    // {
    //     Collider[] colliders = Physics.OverlapSphere(transform.position, sightRadius);
    //     foreach (var target in colliders)
    //     {
    //         if (target.CompareTag("Player")) return true;
    //     }
    //
    //     return false;
    // }
}
