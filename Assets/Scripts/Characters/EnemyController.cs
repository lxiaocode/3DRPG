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
    [Header("Base")] 
    public float sightRadius;

    public bool isGuard;

    
    
    private NavMeshAgent _agent;
    private Animator _animator;

    // State
    private EnemyStates enemyStates;
    private GameObject attackTarget;
    private float speed;

    private bool isWalk;
    private bool isChase;
    private bool isFollow;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        speed = _agent.speed;
    }

    private void Update()
    {
        SwitchState();
        SwitchAnimation();
    }

    private void SwitchAnimation()
    {
        _animator.SetBool("Walk", isWalk);
        _animator.SetBool("Chase", isChase);
        _animator.SetBool("Follow", isFollow);
    }

    private void SwitchState()
    {
        if (FindPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                break;
            case EnemyStates.CHASE:

                isWalk = false;
                isChase = true;

                _agent.speed = speed;
                if (!FindPlayer())
                {
                    isFollow = false;
                    _agent.destination = transform.position;
                }
                else
                {
                    isFollow = true;
                    _agent.destination = attackTarget.transform.position;
                }
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
            attackTarget = player;
            return true;
        }

        attackTarget = null;
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
