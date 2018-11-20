using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    Rigidbody rigidbody;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage()
    {
        //take the actual damage ofc lmao (coming soon tm)
    }

    public void TakeDamage(Vector3 knockBackOrigin, float knockBackStrength, float knockBackRadius)
    {
        //Take the actual damage ofc lmao (coming soon tm)

        //Knock the enemy back
        rigidbody.AddExplosionForce(knockBackStrength, knockBackOrigin, knockBackRadius);
    }
}
