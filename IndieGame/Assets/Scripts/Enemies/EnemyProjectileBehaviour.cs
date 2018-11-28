using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectileBehaviour : MonoBehaviour
{
    [SerializeField]
    [Range(0, 100)]
    private int _projectileSpeed;
    [SerializeField]
    private int _timeTillDestroyInSeconds = 20;
    [SerializeField]
    private float _damage = 10;

    private Rigidbody _rigidBody;
    private DateTime _spawnTime;


    // Use this for initialization
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.AddForce(transform.forward * 100 * _projectileSpeed);

        _spawnTime = DateTime.Now;
    }

    // Update is called once per frame
    private void Update()
    {
        if (DateTime.Now > _spawnTime.AddSeconds(_timeTillDestroyInSeconds))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameController.playerController.TakeDamage(_damage);
            Destroy(this.gameObject);
        }
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }
}
