using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float _startingHealth = 100;
    private float _health;
    private bool _destroyEnemy;
    [SerializeField]
    private float _damage;
    [SerializeField]
    [Range(0, 3000)]
    private int _attackDelayInMs = 1000;
    [SerializeField]
    [Range(0, 100)]
    private int _detectRange = 30;

    //Components
    private Rigidbody _rigidbody;
    private Collider _collider;
    private NavMeshAgent _agent;

    //Attacking variables
    private bool _isAttacking;
    private DateTime _lastAttackTime;

    // Use this for initialization
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _health = _startingHealth;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Dying();
        Attacks();
        Movement();
    }

    //Play the death animation when enemy dies and afterwards destroy it
    private void Dying()
    {
        if (_health <= 0)
        {
            if (_destroyEnemy)
            {
                Destroy(this.gameObject);
            }
            else
            {
                PlayDyingAnimation();
            }
        }
    }


    //Move towards player whenever allowed
    private void Movement()
    {
        if (GameController.player != null)
        {
            if ((transform.position - GameController.player.transform.position).magnitude < _detectRange)
            {
                //Move towards player
                if (_agent != null && _agent.isOnNavMesh)
                    _agent.destination = GameController.player.transform.position;
            }
        }
        else
        {
            _agent.destination = transform.position;
        }
    }

    //Attack the player whenever in range
    private void Attacks()
    {
        if (GameController.player != null)
        {
            //Check if the player is in range
            if ((GameController.player.transform.position - transform.position).magnitude < 1)
            {
                if ((DateTime.Now - _lastAttackTime).TotalMilliseconds > _attackDelayInMs)
                {
                    //Deal damage to the player
                    GameController.playerController.TakeDamage(_damage);

                    //Set last attack time to this
                    _lastAttackTime = DateTime.Now;
                }
            }
        }
        else
        {
            if (_agent.isOnNavMesh)
            {
                _agent.destination = transform.position + transform.up * 50;
            }
        }
    }

    //Death animation
    private void PlayDyingAnimation()
    {
        //Play death animation here lmao


        //Give permission to destroy the enemy
        _destroyEnemy = true;
    }

    public void TakeDamage(float damage)
    {
        //take the actual damage ofc lmao (coming soon tm)
        _health -= damage;

        //Display damage number on canvas
        GameController.damageNumbersCanvas.DisplayDamageNumber(false, damage, this.transform.position + this.transform.up * _collider.bounds.extents.y);
    }

    public void TakeDamage(float damage, Vector3 knockBackOrigin, float knockBackStrength, float knockBackRadius)
    {
        //Take the actual damage ofc lmao (coming soon tm)
        _health -= damage;

        //Display damage number on canvas
        GameController.damageNumbersCanvas.DisplayDamageNumber(false, damage, this.transform.position + this.transform.up * _collider.bounds.extents.y);

        //Knock the enemy back
        _rigidbody.AddExplosionForce(knockBackStrength, knockBackOrigin, knockBackRadius);
    }
}
