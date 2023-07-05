using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private CharacterStats _characterStats;
    private NavMeshAgent _agent;
    private Animator _animator;


    private GameObject _attackTarget;
    private float _lastAttackTime;
    
    private static readonly int Critical = Animator.StringToHash("Critical");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Speed = Animator.StringToHash("Speed");

    private bool _isDeath;
    private static readonly int Death = Animator.StringToHash("Death");

    private void Awake()
    {
        _characterStats = GetComponent<CharacterStats>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
    }

    private void Update()
    {
        _isDeath = _characterStats.CurrentHealth <= 0;
        _lastAttackTime -= Time.deltaTime;
        SwitchAnimation();
    }

    private void SwitchAnimation()
    {
        _animator.SetFloat(Speed, _agent.velocity.sqrMagnitude);
        _animator.SetBool(Death, _isDeath);
    }

    #region Move Controller

    /// <summary>
    /// 移动到目标位置
    /// </summary>
    /// <param name="position"></param>
    private void MoveToTarget(Vector3 position)
    {
        // 这里停止其他用于移动的协程（如：MoveToAttackTarget）
        StopAllCoroutines();
        
        _agent.isStopped = false;
        _agent.destination = position;
    }

    #endregion

    #region Attack Controller

    /// <summary>
    /// 攻击事件（当点击鼠标进行攻击时调用）
    /// </summary>
    /// <param name="target">鼠标选中的攻击目标</param>
    private void EventAttack(GameObject target)
    {
        if (target != null)
        {
            _attackTarget = target;
            _characterStats.isCritical = UnityEngine.Random.value < _characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }

    }

    
    /// <summary>
    /// 移动到攻击目标位置
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveToAttackTarget()
    {
        _agent.isStopped = false;
        // 朝向目标移动
        transform.LookAt(_attackTarget.transform);
        while (Vector3.Distance(transform.position, _attackTarget.transform.position) > _characterStats.attackData.attackRange)
        {
            _agent.destination = _attackTarget.transform.position;
            yield return null;
        }
        _agent.isStopped = true;

        // 攻击动画
        if (_lastAttackTime <= 0.0f)
        {
            _animator.SetBool(Critical, _characterStats.isCritical);
            _animator.SetTrigger(Attack);
            _lastAttackTime = _characterStats.attackData.coolDown;
        }
    }


    /// <summary>
    /// 动画中触发的事件，执行击中的逻辑
    /// </summary>
    private void Hit()
    {
        var targetState = _attackTarget.GetComponent<CharacterStats>();
        targetState.TakeDamage(_characterStats, targetState);
    }

    #endregion
    
}
