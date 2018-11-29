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

    private MeshFilter _meshFilter;
    private GameObject _collisionBox;

    // Use this for initialization
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        //Stops the players attack
        StopAttacking();

        //Checks if the player isn't already attacking
        if (!GameController.playerController.isAttacking)
        {
            BasicAttack();
            Abilities();
        }
    }

    private void RotateTowardsMouse()
    {
        //Raycasts towards the ground from the mouse position relative to the camera view
        RaycastHit raycastHit = new RaycastHit();
        Physics.Raycast(GameController.camera.ScreenPointToRay(Input.mousePosition), out raycastHit);

        //Rotates the player towards the mouse
        Transform transformCopy = this.transform;
        transformCopy.LookAt(raycastHit.point);
        transformCopy.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));

        GameController.player.GetComponent<PlayerMovement>().rotation = transformCopy.rotation.eulerAngles;
    }

    private void BasicAttack()
    {
        //Check for mouse input
        if (Input.GetMouseButton(0))
        {
            RotateTowardsMouse();
            GameController.playerController.isAttacking = true;
            _collisionBox = Instantiate(_attackCollisionBox, transform.position + transform.forward * _meshFilter.mesh.bounds.size.z, transform.rotation, transform.parent);
            _collisionBox.GetComponent<BasicAttackBehaviour>().damageValue = _attackDamage;
            _attackStartTime = DateTime.Now;
        }
    }

    private void Abilities()
    {
        //Check for mouse input
        if (Input.GetMouseButton(1) && GameController.playerController.GetMana() > _abilityOneManaCost)
        {
            RotateTowardsMouse();
            GameController.playerController.isAttacking = true;

            //Drain mana from the player
            GameController.playerController.SetMana(GameController.playerController.GetMana() - _abilityOneManaCost);

            //Check what ability is selected (COMING SOON tm)
            //switch(ability) etc etc
            GameObject projectile = Instantiate(_ability1ProjectilePrefab, transform.position + transform.forward * _meshFilter.mesh.bounds.size.z, transform.rotation, transform.parent);
            projectile.GetComponent<Ability1ProjectileBehaviour>().damageValue = _abilityOneDamage;
            _attackStartTime = DateTime.Now;
        }
    }

    private void StopAttacking()
    {
        //Check how long the player has been attacking
        if ((DateTime.Now - _attackStartTime).TotalMilliseconds > _attackDuration * 1000)
        {
            GameController.playerController.isAttacking = false;
            Destroy(_collisionBox);
        }
    }
}
