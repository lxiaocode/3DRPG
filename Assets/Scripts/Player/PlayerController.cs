using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;


    private GameObject attackTarget;
    private float lastAttackTime;

    private void Awake()
    {
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
    
    private void EventAttack(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }

    }

    private IEnumerator MoveToAttackTarget()
    {
        _agent.isStopped = false;
        transform.LookAt(attackTarget.transform);
        // TODO 可配置攻击距离
        while (Vector3.Distance(transform.position, attackTarget.transform.position) > 1.0f)
        {
            _agent.destination = attackTarget.transform.position;
            yield return null;
        }
        _agent.isStopped = true;

        if (lastAttackTime <= 0.0f)
        {
            _animator.SetTrigger("Attack");
        }
    }
}
