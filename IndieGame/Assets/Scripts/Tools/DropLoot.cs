using System.Collections.Generic;
using UnityEngine;

public class DropLoot : MonoBehaviour
{
    [Header("If this is set to 0 it will default to 2")]
    [SerializeField] private int _maxDrops;

    private GameObject _lootDropPrefab;
    private List<Item> _loot = new List<Item>();
    private int _zone = 1;

    private void Start()
    {
        if (_maxDrops == 0)
            _maxDrops = 2;

        _lootDropPrefab = Resources.Load<GameObject>("LootDropParent");

        GetAvailableItems();
    }

    public void DropItems(Transform pObjectTransform)
    {
        List<Item> drops = new List<Item>();

        foreach (Item item in _loot)
        {
            float rnd = UnityEngine.Random.Range(0, 100);

            if (rnd <= item.DropChance)
                drops.Add(item);
        }

        if (drops.Count == 0) return;

        for (int i = 0; i < _maxDrops; i++)
        {
            int rnd = UnityEngine.Random.Range(0, drops.Count);
            GameObject go = null;
            if (this.transform.parent != null)
            {
                go = Instantiate(_lootDropPrefab, this.transform.position + Vector3.up * 2, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), this.transform.parent.transform);
            }
            else
            {
                go = Instantiate(_lootDropPrefab, this.transform.position + Vector3.up * 2, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            }
            if (go == null) continue;

            go.GetComponentInChildren<ItemDrop>().Init(drops[rnd], pObjectTransform);
        }
    }

    private void GetAvailableItems()
    {
        switch (this.transform.tag.ToLower())
        {
            case "enemy":
                foreach (Item i in GameController.lootPool)
                    if (i.DropsFrom == DropsFrom.Enemies || i.DropsFrom == DropsFrom.Everywhere)
                        _loot.Add(i);
                break;
            case "lootchest":
                foreach (Item i in GameController.lootPool)
                    if (i.DropsFrom == DropsFrom.Chests || i.DropsFrom == DropsFrom.Everywhere)
                        _loot.Add(i);
                break;
        }
    }
}
