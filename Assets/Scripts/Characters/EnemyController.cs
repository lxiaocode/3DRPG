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


    
    private CharacterStats _characterStats;
    private NavMeshAgent _agent;
    private Animator _animator;

    // State
    private EnemyStates _enemyStates;
    private GameObject _attackTarget;
    private float _speed;
    private Vector3 _guardPost;
    private float _remainLookAtTime;
    
    // Attack State
    private float _lastAttackTime;


    // Patrol State
    private Vector3 _wayPoint; 
    
    
    // Animation State
    private bool _isWalk;
    private bool _isChase;
    private bool _isFollow;
    
    private static readonly int Critical = Animator.StringToHash("Critical");
    private static readonly int Attack1 = Animator.StringToHash("Attack");
    private static readonly int Skill = Animator.StringToHash("Skill");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Chase = Animator.StringToHash("Chase");
    private static readonly int Follow = Animator.StringToHash("Follow");


    private void Awake()
    {
        _characterStats = GetComponent<CharacterStats>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _speed = _agent.speed;
    }

    private void Start()
    {
        _guardPost = transform.position;
        if (isGuard)
        {
            _enemyStates = EnemyStates.GUARD;
        }
        else
        {
            _enemyStates = EnemyStates.PATROL;
            _remainLookAtTime = lookAtTime;
            GetNewWayPoint();
        }
    }

    private void Update()
    {
        SwitchState();
        SwitchAnimation();

        _lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        _animator.SetBool(Walk, _isWalk);
        _animator.SetBool(Chase, _isChase);
        _animator.SetBool(Follow, _isFollow);
    }

    private void SwitchState()
    {
        if (FindPlayer())
        {
            _enemyStates = EnemyStates.CHASE;
        }
        
        switch (_enemyStates)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                _isChase = false;
                _agent.speed = _speed * 0.5f;

                if (Vector3.Distance(transform.position, _wayPoint) <= _agent.stoppingDistance)
                {
                    _isWalk = false;
                    if (_remainLookAtTime > 0)
                    {                        
                        _remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        _remainLookAtTime = lookAtTime;
                        GetNewWayPoint();
                    }
                }
                else
                {
                    _isWalk = true;
                    _agent.destination = _wayPoint;
                }
                break;
            case EnemyStates.CHASE:

                _isWalk = false;
                _isChase = true;

                _agent.speed = _speed;
                if (!FindPlayer())
                {
                    _isFollow = false;
                    _agent.destination = _guardPost;
                    if (isGuard)
                        _enemyStates = EnemyStates.GUARD;
                    else
                        _enemyStates = EnemyStates.PATROL;
                }
                else
                {
                    _isFollow = true;
                    _agent.isStopped = false;
                    _agent.destination = _attackTarget.transform.position;
                    
                    // Attack
                    if (TargetInSkillRange() || TargetInAttackRange())
                    {
                        _isFollow = false;
                        _agent.isStopped = true;

                        if (_lastAttackTime < 0)
                        {
                            _lastAttackTime = _characterStats.attackData.coolDown;
                            _characterStats.isCritical = Random.value < _characterStats.attackData.criticalChance;
                            Attack();
                        }
                    }
                }
                break;
            case EnemyStates.DEAD:
                break;
        }
    }

    #region Attack Controller

    /// <summary>
    /// 寻找玩家进行攻击
    /// </summary>
    /// <returns></returns>
    private bool FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && Vector3.Distance(player.transform.position, transform.position) <= sightRadius)
        {
            _attackTarget = player;
            return true;
        }

        _attackTarget = null;
        return false;
    }
    
    /// <summary>
    /// 攻击
    /// </summary>
    private void Attack()
    {
        transform.LookAt(_attackTarget.transform);
        _animator.SetBool(Critical, _characterStats.isCritical);
        if (TargetInAttackRange())
        {
            _animator.SetTrigger(Attack1);
        }
        else if (TargetInSkillRange())
        {
            _animator.SetTrigger(Skill);
        }
    }

    /// <summary>
    /// 判断目标是否在普通攻击范围
    /// </summary>
    /// <returns></returns>
    private bool TargetInAttackRange()
    {
        var distance = Vector3.Distance(transform.position, _attackTarget.transform.position);
        return _characterStats != null && distance < _characterStats.attackData.attackRange;
    }
    
    /// <summary>
    /// 判断目标是否在技能攻击范围
    /// </summary>
    /// <returns></returns>
    private bool TargetInSkillRange()
    {
        var distance = Vector3.Distance(transform.position, _attackTarget.transform.position);
        return _characterStats != null && distance < _characterStats.attackData.skillRange;
    }

    /// <summary>
    /// 动画中触发的事件，执行击中的逻辑
    /// </summary>
    private void Hit()
    {
        if (_attackTarget != null)
        {
            var targetState = _attackTarget.GetComponent<CharacterStats>();
            targetState.TakeDamage(_characterStats, targetState);
        }
    }
    
    #endregion

    #region Move Controller

    /// <summary>
    /// 在巡逻范围寻找新目标点进行移动
    /// </summary>
    private void GetNewWayPoint()
    {
        float x = Random.Range(-patrolRange, patrolRange);
        float z = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(_guardPost.x+ x, transform.position.y, _guardPost.z + z);

        NavMeshHit hit;
        _wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    #endregion
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
