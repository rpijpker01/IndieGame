using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController gameController;
    public static List<Item> lootPool = new List<Item>();

    public static GameObject player;
    public static PlayerController playerController;
    public static DamageNumbersCanvas damageNumbersCanvas;
    public static UICanvas uiCanvas;

    public static GameObject cameraObj;
    public static Camera camera;

    public static ErrorMessage errorMessage;

    // Use this for initialization
    private void Awake()
    {
        gameController = this;
        player = GameObject.FindGameObjectWithTag("Player");
        if (null != player)
            playerController = player.GetComponent<PlayerController>();

        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        camera = cameraObj.GetComponent<Camera>();

        //Create a loot pool and add the items to a list
        Object[] lootableItems = Resources.LoadAll("LootPool");
        for (int i = 0; i < lootableItems.Length; i++)
        {
            Item item = lootableItems[i] as Item;
            lootPool.Add(item);
        }

        BakeNavMesh();
    }

    private void BakeNavMesh()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.layer == 9)
            {
                NavMeshObstacle obstacle = gameObject.AddComponent<NavMeshObstacle>();
                obstacle.carving = true;
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
