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
    private GameObject _attackCollisionBox;
    [SerializeField]
    private GameObject _ability1ProjectilePrefab;

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
        transform.LookAt(raycastHit.point);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
    }

    private void BasicAttack()
    {
        //Check for mouse input
        if (Input.GetMouseButton(0))
        {
            RotateTowardsMouse();
            GameController.playerController.isAttacking = true;
            _collisionBox = Instantiate(_attackCollisionBox, transform.position + transform.forward * _meshFilter.mesh.bounds.size.z, transform.rotation, transform.parent);
            _attackStartTime = DateTime.Now;
        }
    }

    private void Abilities()
    {
        //Check for mouse input
        if (Input.GetMouseButton(1))
        {
            RotateTowardsMouse();
            GameController.playerController.isAttacking = true;
            //Check what ability is selected (COMING SOON tm)
            //switch(ability) etc etc
            Instantiate(_ability1ProjectilePrefab, transform.position + transform.forward * _meshFilter.mesh.bounds.size.z, transform.rotation, transform.parent);
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
