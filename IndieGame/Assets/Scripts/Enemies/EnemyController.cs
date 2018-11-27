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

    //Just testing the item drops btw ^_^
    public GameObject lootDropPrefab;
    private List<Item> _loot = new List<Item>();
    private float _maxHealth = 100;
    private float _currentHealth;
    private int _maxDrops = 7;
    private int _zone = 1;

    // Use this for initialization
    private void Start()
    {
<<<<<<< HEAD
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _health = _startingHealth;
=======
        rigidbody = GetComponent<Rigidbody>();
        _currentHealth = _maxHealth;
>>>>>>> Inventory_and_Equipment
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
<<<<<<< HEAD
        _health -= damage;
=======
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Destroy(this.gameObject);
            DropItems();
            return;
        }
>>>>>>> Inventory_and_Equipment

        //Display damage number on canvas
        GameController.damageNumbersCanvas.DisplayDamageNumber(damage, this.transform.position + this.transform.up * _collider.bounds.extents.y);

<<<<<<< HEAD
        //Knock the enemy back
        _rigidbody.AddExplosionForce(knockBackStrength, knockBackOrigin, knockBackRadius);
=======
    private void DropItems()
    {
        GetAvailableItems();

        List<Item> drops = new List<Item>();

        foreach (Item item in _loot)
        {
            Equippable eq = item as Equippable;
            if (eq == null) continue;

            float rnd = Random.Range(0, 100);

            if (rnd <= eq.DropChance)
                drops.Add(eq);
        }

        for (int i = 0; i < drops.Count && i < _maxDrops; i++)
        {
            int rnd = Random.Range(0, drops.Count - 1);
            GameObject go = Instantiate(lootDropPrefab, this.transform.position, Quaternion.identity);
            if (go == null) continue;
            go.GetComponentInChildren<ItemDrop>().Init(drops[rnd]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CollisionBox")
        {
            TakeDamage(25, GameController.player.transform.position, 500, 3);
        }
>>>>>>> Inventory_and_Equipment
    }

    private void GetAvailableItems()
    {
        foreach (Equippable eq in GameController.lootPool)
        {
            if (eq.AllZone)
            {
                _loot.Add(eq);
                continue;
            }

            switch (_zone)
            {
                case 1:
                    if (eq.ZoneOne)
                        _loot.Add(eq);
                    break;

                case 2:
                    if (eq.ZoneTwo)
                        _loot.Add(eq);
                    break;

                case 3:
                    if (eq.ZoneThree)
                        _loot.Add(eq);
                    break;

                case 4:
                    if (eq.ZoneFour)
                        _loot.Add(eq);
                    break;

                case 5:
                    if (eq.ZoneFive)
                        _loot.Add(eq);
                    break;
            }
        }
    }
}
