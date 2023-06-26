using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private CharacterStats characterStats;
    private NavMeshAgent _agent;
    private Animator _animator;


    private GameObject attackTarget;
    private float lastAttackTime;

    private void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
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
        lastAttackTime -= Time.deltaTime;
        SwitchAnimation();
    }

    private void SwitchAnimation()
    {
        _animator.SetFloat("Speed", _agent.velocity.sqrMagnitude);
    }

    private void MoveToTarget(Vector3 position)
    {
        StopAllCoroutines();
        _agent.isStopped = false;
        _agent.destination = position;
    }
    
    /// <summary>
    /// 攻击事件（当点击鼠标进行攻击时调用）
    /// </summary>
    /// <param name="target"></param>
    private void EventAttack(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
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
        transform.LookAt(attackTarget.transform);
        while (Vector3.Distance(transform.position, attackTarget.transform.position) > characterStats.attackData.attackRange)
        {
            _agent.destination = attackTarget.transform.position;
            yield return null;
        }
        _agent.isStopped = true;

        // 攻击CD
        if (lastAttackTime <= 0.0f)
        {
            _animator.SetBool("Critical", characterStats.isCritical);
            _animator.SetTrigger("Attack");
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }


    /// <summary>
    /// Animation Event
    /// </summary>
    private void Hit()
    {
        var targetState = attackTarget.GetComponent<CharacterStats>();
        targetState.TakeDamage(characterStats, targetState);
    }
}
