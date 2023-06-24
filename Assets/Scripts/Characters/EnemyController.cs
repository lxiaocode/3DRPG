using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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

    [Header("Patrol state")] 
    public float patrolRange;
    public float lookAtTime;


    
    private CharacterStats characterStats;
    private NavMeshAgent _agent;
    private Animator _animator;

    // State
    private EnemyStates enemyStates;
    private GameObject attackTarget;
    private float speed;
    private Vector3 guradPost;
    private float remainLookAtTime;
    
    // Attack State
    private float lastAttackTime;


    // Patrol State
    private Vector3 wayPoint; 
    
    
    // Animation State
    private bool isWalk;
    private bool isChase;
    private bool isFollow;
    


    private void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        speed = _agent.speed;
    }

    private void Start()
    {
        guradPost = transform.position;
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            remainLookAtTime = lookAtTime;
            GetNewWayPoint();
        }
    }

    private void Update()
    {
        SwitchState();
        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
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
                isChase = false;
                _agent.speed = speed * 0.5f;

                if (Vector3.Distance(transform.position, wayPoint) <= _agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {                        
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        remainLookAtTime = lookAtTime;
                        GetNewWayPoint();
                    }
                }
                else
                {
                    isWalk = true;
                    _agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:

                isWalk = false;
                isChase = true;

                _agent.speed = speed;
                if (!FindPlayer())
                {
                    isFollow = false;
                    _agent.destination = guradPost;
                    if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                }
                else
                {
                    isFollow = true;
                    _agent.isStopped = false;
                    _agent.destination = attackTarget.transform.position;
                }

                // Attack
                if (TargetInSkillRange() || TargetInAttackRange())
                {
                    isFollow = false;
                    _agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;
                        var isCritical = Random.value < characterStats.attackData.criticalChance;
                        Attack(isCritical);
                    }
                }
                
                break;
            case EnemyStates.DEAD:
                break;
        }
    }

    #region Attack

    private void Attack(bool isCritical)
    {
        transform.LookAt(attackTarget.transform);
        _animator.SetBool("Critical", isCritical);
        if (TargetInAttackRange())
        {
            _animator.SetTrigger("Attack");
        }
        else if (TargetInSkillRange())
        {
            _animator.SetTrigger("Skill");
        }
    }

    private bool TargetInAttackRange()
    {
        var distance = Vector3.Distance(transform.position, attackTarget.transform.position);
        return characterStats != null && distance < characterStats.attackData.attackRange;
    }
    
    private bool TargetInSkillRange()
    {
        var distance = Vector3.Distance(transform.position, attackTarget.transform.position);
        return characterStats != null && distance < characterStats.attackData.skillRange;
    }
    
    #endregion

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

    private void GetNewWayPoint()
    {
        float x = Random.Range(-patrolRange, patrolRange);
        float z = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guradPost.x+ x, transform.position.y, guradPost.z + z);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
