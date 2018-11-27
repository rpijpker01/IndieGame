using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour
{
    public static GameController gameController;

    public static GameObject player;
    public static PlayerController playerController;
    public static DamageNumbersCanvas damageNumbersCanvas;
    public static UICanvas uiCanvas;

    public static GameObject cameraObj;
    public static Camera camera;

    // Use this for initialization
    private void Awake()
    {
        gameController = this;
        player = GameObject.FindGameObjectWithTag("Player");
        if (null != player)
            playerController = player.GetComponent<PlayerController>();

        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        camera = cameraObj.GetComponent<Camera>();

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
