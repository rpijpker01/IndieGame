using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float _rotationSpeed = 0.8f;
    [SerializeField]
    [Range(0, 20)]
    private float _movementSpeed = 2f;
    [SerializeField]
    [Range(0, 5)]
    private float _accelerationSpeed = 0.5f;

    private bool _moving;

    private float _currentSpeed;
    private Vector3 _rotation;

    private MeshFilter _meshFilter;

    // Use this for initialization
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Check wether the player is attacking
        if (!GameController.playerController.isAttacking)
        {
            Rotation();
            Movement();
        }
        else
        {
            _rotation.y = transform.rotation.eulerAngles.y;
        }

        StickCharacterToGround();
    }

    //Sets the players rotation
    private void Rotation()
    {
        //Rotation for orthogonal directions
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            _rotation.y = 0;
        }
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            _rotation.y = 90;
        }
        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            _rotation.y = 180;
        }
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            _rotation.y = 270;
        }

        //Rotation for diagonal directions
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            _rotation.y = 315;
        }
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            _rotation.y = 45;
        }
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            _rotation.y = 135;
        }
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            _rotation.y = 225;
        }

        //Slerp for setting rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_rotation), _rotationSpeed);
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
        }
        else
        {
            //Reset player movement speed when he stops moving
            _currentSpeed = 0;
        }

        //Set player position
        Vector2 movementInAxi = new Vector2();
        movementInAxi.x = Mathf.Sin(_rotation.y * Mathf.Deg2Rad);
        movementInAxi.y = Mathf.Sqrt(1f - (movementInAxi.x * movementInAxi.x));
        if (_rotation.y > 90 && _rotation.y < 270)
        {
            movementInAxi.y = -movementInAxi.y;
        }
        transform.position = transform.position + (new Vector3(movementInAxi.x, 0, movementInAxi.y) * _currentSpeed * Time.deltaTime);
    }

    //Move player down to the ground
    private void StickCharacterToGround()
    {
        //Check where the ground is
        RaycastHit raycastHit = new RaycastHit();
        Physics.Raycast(transform.position, -transform.up, out raycastHit);

        //Set position
        transform.position = new Vector3(transform.position.x, raycastHit.point.y + _meshFilter.mesh.bounds.extents.y, transform.position.z);
    }
}
