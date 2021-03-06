﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private bool _invertControls = false;
    [SerializeField]
    [Range(0, 50)]
    private float _rotationSpeed = 0.8f;
    [SerializeField]
    [Range(0, 1000)]
    private float _movementSpeed = 2f;
    [SerializeField]
    [Range(0, 500)]
    private float _accelerationSpeed = 0.5f;

    private bool _moving;

    private float _currentSpeed;
    public Vector3 rotation;

    private Collider _collider;
    private Rigidbody _rigidBody;
    private Quaternion _surfaceRotation;
    private GameObject _rotationDummy;

    private static PlayerMovement _playerMovement;

    // Use this for initialization
    void Start()
    {
        _collider = GetComponent<Collider>();
        _rigidBody = GetComponent<Rigidbody>();
        _rotationDummy = GameObject.Find("RotationDummy").gameObject;

        _playerMovement = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameController.playerController.GetHealth() > 0)
        {
            //Check wether the player is attacking
            if (!GameController.playerController.isAttacking)
            {
                Rotation();
                Movement();
            }
            else
            {
                _rigidBody.velocity = Vector3.zero;
            }
        }
        else
        {
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.useGravity = false;
            _collider.enabled = false;
        }
        StickCharacterToGround();
    }

    //Sets the players rotation
    private void Rotation()
    {
        int inverted = 0;
        if (_invertControls)
            inverted = 180;

        //Rotation for orthogonal directions
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            rotation.y = 0 - inverted;
        }
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            rotation.y = 90 - inverted;
        }
        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            rotation.y = 180 - inverted;
        }
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rotation.y = 270 - inverted;
        }

        //Rotation for diagonal directions
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            rotation.y = 315 - inverted;
        }
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            rotation.y = 45 - inverted;
        }
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            rotation.y = 135 - inverted;
        }
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            rotation.y = 225 - inverted;
        }

        //Slerp for setting rotation
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, _rotation.y, transform.rotation.z), _rotationSpeed);
    }

    //Moving the player
    private void Movement()
    {
        //Check wether the player should be moving
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            //Accelerate the player
            _currentSpeed += _accelerationSpeed;

            //Clamp player movement speed
            if (_currentSpeed > _movementSpeed) { _currentSpeed = _movementSpeed; }

            //Play running animation
            PlayerController.SetAnimationState(PlayerController.AnimationState.Running);
        }
        else
        {
            //Reset player movement speed when he stops moving
            _currentSpeed = 0;

            //Play idle animation
            PlayerController.SetAnimationState(PlayerController.AnimationState.Idle);
        }

        //Set player position
        Vector2 movementInAxi = new Vector2();
        movementInAxi.x = Mathf.Sin(rotation.y * Mathf.Deg2Rad);
        movementInAxi.y = Mathf.Sqrt(1f - (movementInAxi.x * movementInAxi.x));
        if (rotation.y > 90 && rotation.y < 270)
        {
            movementInAxi.y = -movementInAxi.y;
        }
        _rigidBody.velocity = _rotationDummy.transform.forward * _currentSpeed * Time.deltaTime;
        //_rigidBody.velocity = (new Vector3(movementInAxi.x, 0, movementInAxi.y) * _currentSpeed * Time.deltaTime);
        //transform.position = transform.position + (new Vector3(movementInAxi.x, 0, movementInAxi.y) * _currentSpeed * Time.deltaTime);
    }

    //Move player down to the ground
    private void StickCharacterToGround()
    {
        //Check where the ground is
        RaycastHit raycastHit = new RaycastHit();
        Physics.Raycast(transform.position, -Vector3.up, out raycastHit, 1000, ~(1 << 10));

        if (_rotationDummy != null)
        {
            _surfaceRotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
            _rotationDummy.transform.rotation = Quaternion.Euler(_surfaceRotation.eulerAngles.x, _surfaceRotation.eulerAngles.y, _surfaceRotation.eulerAngles.z);
            _rotationDummy.transform.RotateAround(_rotationDummy.transform.up, rotation.y * Mathf.Deg2Rad);

            transform.rotation = Quaternion.Slerp(this.transform.rotation, _rotationDummy.transform.rotation, Time.deltaTime * _rotationSpeed);
        }
        else
        {
            _surfaceRotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
            transform.rotation = Quaternion.Euler(_surfaceRotation.eulerAngles.x, _surfaceRotation.eulerAngles.y, _surfaceRotation.eulerAngles.z);
            transform.RotateAround(transform.up, rotation.y * Mathf.Deg2Rad);
        }

        //Set position
        transform.position = transform.position - (transform.up * (raycastHit.distance - 0.1f));// + (transform.up * _collider.bounds.extents.y);
    }

    public static void RotateTowardsMouse()
    {
        //Raycasts towards the ground from the mouse position relative to the camera view
        RaycastHit raycastHit = new RaycastHit();
        Physics.Raycast(GameController.camera.ScreenPointToRay(Input.mousePosition), out raycastHit, 50, 9);

        //Rotates the player towards the mouse
        _playerMovement._rotationDummy.transform.position = _playerMovement.gameObject.transform.position;
        _playerMovement._rotationDummy.transform.LookAt(raycastHit.point);
        _playerMovement.rotation = new Vector3(0, _playerMovement._rotationDummy.transform.rotation.eulerAngles.y, 0);
    }

    public static void InvertControls()
    {
        _playerMovement._invertControls = !_playerMovement._invertControls;
    }
}
