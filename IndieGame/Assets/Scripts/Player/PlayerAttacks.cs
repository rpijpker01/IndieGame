using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttacks : MonoBehaviour
{
    [SerializeField]
    [Range(0, 3)]
    private float _attackDuration = 1;
    [SerializeField]
    private GameObject _weaponTrail;
    [Header("Basic Attack")]
    [SerializeField]
    private float _attackDamage;
    [SerializeField]
    private GameObject _attackCollisionBox;
    [Header("Ability 1")]
    [SerializeField]
    private float _abilityOneManaCost;
    [SerializeField]
    private float _abilityOneDamage;
    [SerializeField]
    private GameObject _ability1ProjectilePrefab;
    [SerializeField]
    [Range(0, 100)]
    private int _ability1ManaCost = 25;


    private DateTime _attackStartTime;

    private Collider _collider;
    private GameObject _collisionBox;

    // Use this for initialization
    void Start()
    {
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //Stops the players attack
        StopAttacking();

        if (GameController.playerController.GetHealth() > 0)
        {
            //Checks if the player isn't already attacking
            if (!GameController.playerController.isAttacking && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                BasicAttack();
                Abilities();
            }
        }
    }

    private void BasicAttack()
    {
        //Check for mouse input
        if (Input.GetMouseButton(0))
        {
            PlayerMovement.RotateTowardsMouse();
            GameController.playerController.isAttacking = true;
            _attackStartTime = DateTime.Now.AddMilliseconds(200);
            PlayerController.SetAnimationState(PlayerController.AnimationState.Attacking);

            if (_weaponTrail != null)
            {
                _weaponTrail.SetActive(true);
            }
        }
        else if (_weaponTrail != null)
        {
            _weaponTrail.SetActive(false);
        }
    }

    private void Abilities()
    {
        //Check for mouse input
        if (Input.GetMouseButton(1) && GameController.playerController.GetMana() > _abilityOneManaCost)
        {
            PlayerMovement.RotateTowardsMouse();
            GameController.playerController.isAttacking = true;
            PlayerController.SetAnimationState(PlayerController.AnimationState.Abilitying);
            _attackStartTime = DateTime.Now.AddMilliseconds(600);
        }
    }

    public void SpawnBasicAttackCollisionBox()
    {
        _collisionBox = Instantiate(_attackCollisionBox, transform.position + transform.forward * _collider.bounds.size.z * 1.25f + transform.up * _collider.bounds.extents.y, transform.rotation, transform.parent);
        _collisionBox.GetComponent<BasicAttackBehaviour>().damageValue = _attackDamage;
    }

    public void SpawnAbilityProjectile()
    {
        if (!GameController.playerController.animator.GetBool("IsRunning"))
        {
            //Drain mana from the player
            GameController.playerController.SetMana(GameController.playerController.GetMana() - _abilityOneManaCost);

            //Check what ability is selected (COMING SOON tm)
            GameObject projectile = Instantiate(_ability1ProjectilePrefab, transform.position + transform.forward * _collider.bounds.size.z + transform.up * _collider.bounds.extents.y, transform.rotation, transform.parent);
            projectile.GetComponent<Ability1ProjectileBehaviour>().damageValue = _abilityOneDamage;
        }
    }

    private void StopAttacking()
    {
        //Check how long the player has been attacking
        if (DateTime.Now > _attackStartTime)
        {
            GameController.playerController.isAttacking = false;
            Destroy(_collisionBox);
        }
    }
}
