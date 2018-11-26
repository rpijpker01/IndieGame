using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackBehaviour : MonoBehaviour
{
    [SerializeField]
    private LayerMask _collisionLayer = new LayerMask();

    //List for enemies already hit by the attack
    private List<GameObject> _hitEnemies = new List<GameObject>();

    //Amount of damage the attack does to the enemies
    private float _damageValue = 25;

    public float damageValue
    {
        get { return _damageValue; }
        set { _damageValue = value; }
    }

    //Deal damage
    private void OnTriggerEnter(Collider other)
    {
        //Check if already hit
        if (!_hitEnemies.Contains(other.gameObject))
        {
            //Check if in right layer
            if (_collisionLayer.value == (_collisionLayer.value | (1 << other.gameObject.layer)))
            {
                //Damage enemy
                other.GetComponent<EnemyController>().TakeDamage(_damageValue, GameController.player.transform.position, 250, 3);

                //Make sure enemy isn't hit twice by adding it to the list
                _hitEnemies.Add(other.gameObject);
            }
        }
    }
}
