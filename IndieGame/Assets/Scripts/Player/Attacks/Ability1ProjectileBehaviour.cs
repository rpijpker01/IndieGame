using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ability1ProjectileBehaviour : MonoBehaviour
{
    [SerializeField]
    [Range(0, 50)]
    private float _movementSpeed;
    [SerializeField]
    [Range(0, 10)]
    private float _radius;
    [SerializeField]
    [Range(0, 1000)]
    private float _knockBackStrength;

    private float _damageValue = 75;
    public float damageValue
    {
        get { return _damageValue; }
        set { _damageValue = value; }
    }


    private DateTime _spawnTime;

    // Use this for initialization
    void Start()
    {
        //Save the time at which the projectile spawns
        _spawnTime = DateTime.Now;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move the projectile
        transform.position = transform.position + transform.forward * _movementSpeed * Time.deltaTime;

        //Delete the projectile when it's been flying too long
        if ((DateTime.Now - _spawnTime).TotalSeconds > 20)
        {
            DamageEnemiesInRange();
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            DamageEnemiesInRange();

            Destroy(this.gameObject);
        }
    }

    private void DamageEnemiesInRange()
    {
        //Get all enemies in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //Damage all the enemies in range
        foreach (GameObject enemy in enemies)
        {
            if ((enemy.transform.position - transform.position).magnitude < _radius)
            {
                float damageWithFalloff = _damageValue / (enemy.transform.position - transform.position).magnitude;
                damageWithFalloff = Mathf.Clamp(damageWithFalloff, 0, _damageValue * 1.5f);
                enemy.GetComponent<EnemyController>().TakeDamage(damageWithFalloff, transform.position, _knockBackStrength, _radius * 5);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
