﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float _startingHealth = 100;
    private float _health;
    private bool _destroyEnemy;
    [SerializeField]
    [Range(0, 100)]
    private int _detectRange = 30;
    [SerializeField]
    private bool _isMeleeEnemy = true;
    [Header("Melee Enemy")]
    [SerializeField]
    private float _meleeDamage;
    [SerializeField]
    [Range(0, 3000)]
    private int _attackDelayInMs = 1000;
    [Header("Ranged Enemy")]
    [SerializeField]
    private GameObject _projectilePrefab;
    [SerializeField]
    private int _shootingRange = 8;
    [SerializeField]
    private int _rangedDamage = 10;
    [SerializeField]
    private int _shootCooldownInMs = 1000;


    //Components
    private Rigidbody _rigidbody;
    private Collider _collider;
    private NavMeshAgent _agent;
    private SoundPlayer _soundPlayer;

    //Attacking variables
    private bool _isAttacking;
    private DateTime _lastAttackTime;

    [Header("Loot drops")]
    //Just testing the item drops btw ^_^
    public GameObject lootDropPrefab;
    private List<Item> _loot = new List<Item>();
    private float _maxHealth = 100;
    private int _maxDrops = 2;
    private int _zone = 1;

    // Use this for initialization
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _soundPlayer = GetComponent<SoundPlayer>();
        _health = _startingHealth;

        if (GetComponent<Animation>() != null)
        {
            GetComponent<Animation>().Play();
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Dying();
        if (_isMeleeEnemy)
        {
            MeleeEnemyBehaviour();
        }
        else
        {
            RangedEnemyBehaviour();
        }
    }

    //Play the death animation when enemy dies and afterwards destroy it
    private void Dying()
    {
        if (_health <= 0)
        {
            if (_destroyEnemy)
            {
                DropItems();
                Destroy(this.gameObject);
            }
            else
            {
                PlayDyingAnimation();
            }
        }
    }

    //Melee enemy stuff
    private void MeleeEnemyBehaviour()
    {
        MeleeAttack();
        MeleeMovement();
    }

    //Ranged enemy stuff
    private void RangedEnemyBehaviour()
    {
        RangedAttack();
        RangedMovement();
    }

    //Shoot / spit at the player
    private void RangedAttack()
    {
        if (GameController.player != null)
        {
            transform.LookAt(GameController.player.transform);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            if (_isAttacking && DateTime.Now > _lastAttackTime.AddMilliseconds(_shootCooldownInMs))
            {
                Instantiate(_projectilePrefab, transform.position + new Vector3(_collider.bounds.extents.x, _collider.bounds.extents.y, 0), transform.rotation, transform.parent);
                _isAttacking = false;
                _lastAttackTime = DateTime.Now;
            }
        }
    }

    //Move closer to the player
    private void RangedMovement()
    {
        if (GameController.player != null)
        {
            if (!_isAttacking && (transform.position - GameController.player.transform.position).magnitude < _detectRange)
            {
                if ((transform.position - GameController.player.transform.position).magnitude > _shootingRange)
                {
                    //Move towards player
                    if (_agent != null && _agent.isOnNavMesh)
                        _agent.destination = GameController.player.transform.position;
                }
                else
                {
                    _agent.destination = transform.position;
                    _isAttacking = true;
                }
            }
        }
        else
        {
            _agent.destination = transform.position;
        }
    }

    //Move towards player whenever allowed
    private void MeleeMovement()
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
    private void MeleeAttack()
    {
        if (GameController.player != null)
        {
            //Check if the player is in range
            if ((GameController.player.transform.position - transform.position).magnitude < 1.5f)
            {
                if ((DateTime.Now - _lastAttackTime).TotalMilliseconds > _attackDelayInMs)
                {
                    //Deal damage to the player
                    GameController.playerController.TakeDamage(_meleeDamage);

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

    private void DropItems()
    {
        GetAvailableItems();

        List<Item> drops = new List<Item>();

        foreach (Item item in _loot)
        {
            Equippable eq = item as Equippable;
            if (eq == null) continue;

            float rnd = UnityEngine.Random.Range(0, 100);

            if (rnd <= eq.DropChance)
                drops.Add(eq);
        }

        for (int i = 0; i < drops.Count && i < _maxDrops; i++)
        {
            int rnd = UnityEngine.Random.Range(0, drops.Count - 1);
            GameObject go = Instantiate(lootDropPrefab, this.transform.position, Quaternion.identity);
            if (go == null) continue;
            go.GetComponentInChildren<ItemDrop>().Init(drops[rnd]);
        }
    }

    private void GetAvailableItems()
    {
        foreach (Equippable eq in GameController.lootPool)
        {
            if (eq.AllZone)
            {
                _loot.Add(eq);
                continue;
            }

            switch (_zone)
            {
                case 1:
                    if (eq.ZoneOne)
                        _loot.Add(eq);
                    break;

                case 2:
                    if (eq.ZoneTwo)
                        _loot.Add(eq);
                    break;

                case 3:
                    if (eq.ZoneThree)
                        _loot.Add(eq);
                    break;

                case 4:
                    if (eq.ZoneFour)
                        _loot.Add(eq);
                    break;

                case 5:
                    if (eq.ZoneFive)
                        _loot.Add(eq);
                    break;
            }
        }
    }

    public void PlayStepSound()
    {
        if (null != _soundPlayer)
            _soundPlayer.PlayAudioClip(0);
        Debug.Log("Great step sound");
    }
}
