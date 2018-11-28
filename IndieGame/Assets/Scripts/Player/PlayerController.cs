﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isAttacking { get; set; }

    [SerializeField]
    private float _startingHealth = 100;
    [SerializeField]
    private float _startingMana = 100;
    private float _health;
    private float _mana;
    private bool _isPlayingDyingAnimation = true;

    private ItemDrop _droppedItem;

    //Components
    private Collider _collider;

    // Use this for initialization
    private void Start()
    {
        _health = _startingHealth;
        _mana = _startingMana;

        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    private void Update()
    {
        Dying();
    }

    private void Dying()
    {
        if (_health <= 0)
        {
            if (_isPlayingDyingAnimation)
            {
                PlayDyingAnimation();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void PlayDyingAnimation()
    {
        _isPlayingDyingAnimation = false;
    }

    public void TakeDamage(float damage)
    {
        //Subtract damage from current health
        _health -= damage;

        //Camera shake
        CameraFollowPlayer.cameraShake(1 + ((damage / 100) * 2), 750);

        //Display damage number
        GameController.damageNumbersCanvas.DisplayDamageNumber(true, damage, new Vector3(transform.position.x, transform.position.y + _collider.bounds.extents.y, transform.position.z));
    }

    public float GetHealth()
    {
        return _health;
    }

    public float GetMana()
    {
        return _mana;
    }
}
