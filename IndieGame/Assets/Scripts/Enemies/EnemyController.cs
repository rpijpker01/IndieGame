using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float _startingHealth = 100;
    private float _health;
    private bool _destroyEnemy;

    private Rigidbody _rigidbody;
    private Collider _collider;

    // Use this for initialization
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _health = _startingHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        Dying();
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
        GameController.damageNumbersCanvas.DisplayDamageNumber(damage, this.transform.position + this.transform.up * _collider.bounds.extents.y);
    }

    public void TakeDamage(float damage, Vector3 knockBackOrigin, float knockBackStrength, float knockBackRadius)
    {
        //Take the actual damage ofc lmao (coming soon tm)
        _health -= damage;

        //Display damage number on canvas
        GameController.damageNumbersCanvas.DisplayDamageNumber(damage, this.transform.position + this.transform.up * _collider.bounds.extents.y);

        //Knock the enemy back
        _rigidbody.AddExplosionForce(knockBackStrength, knockBackOrigin, knockBackRadius);
    }
}
