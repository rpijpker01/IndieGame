using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    Rigidbody rigidbody;

    //Just testing the item drops btw ^_^
    public GameObject lootDropPrefab;
    private List<Item> _loot = new List<Item>();
    private float _maxHealth = 100;
    private float _currentHealth;
    private int _maxDrops = 7;
    private int _zone = 1;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        _currentHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        //take the actual damage ofc lmao (coming soon tm)
    }

    public void TakeDamage(float damage, Vector3 knockBackOrigin, float knockBackStrength, float knockBackRadius)
    {
        //Take the actual damage ofc lmao (coming soon tm)
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Destroy(this.gameObject);
            DropItems();
            return;
        }

        //Knock the enemy back
        rigidbody.AddExplosionForce(knockBackStrength, knockBackOrigin, knockBackRadius);
    }

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
