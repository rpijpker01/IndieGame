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
    }

    private void OnDisable()
    {
        DropItems();
    }

    public void DropItems()
    {
        GetAvailableItems();

        List<Item> drops = new List<Item>();

        foreach (Item item in _loot)
        {
            Equippable eq = item as Equippable;
            if (eq == null) continue;

            float rnd = UnityEngine.Random.Range(0, 100);

            if (rnd <= eq.DropChance)
                drops.Add(eq);
        }

        for (int i = 0; i < _maxDrops; i++)
        {
            int rnd = UnityEngine.Random.Range(0, drops.Count - 1);
            GameObject go = Instantiate(_lootDropPrefab, this.transform.position + Vector3.up * 2, Quaternion.identity);
            if (go == null) continue;

            go.GetComponentInChildren<ItemDrop>().Init(drops[rnd]);
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
