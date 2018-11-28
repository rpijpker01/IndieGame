using System;
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
    public static LevelGenerator levelGenerator;
    public static MainCanvas mainCanvas;
    public static UICanvas uiCanvas;

    public static GameObject cameraObj;
    public static Camera camera;

    public static ErrorMessage errorMessage;

    //Trasition and fading stuff
    public bool isTransitioning { get; set; }
    private bool _loadingLevel;
    private bool _loadingHub;
    private bool _hasToFadeOut;
    private DateTime _fadeTime;
    [SerializeField]
    private GameObject _playerHubSpawnPosition;

    // Use this for initialization
    private void Awake()
    {
        gameController = this;
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.FindGameObjectWithTag("Player");
        if (null != player)
            playerController = player.GetComponent<PlayerController>();
        camera = cameraObj.GetComponent<Camera>();


        //Create a loot pool and add the items to a list
        UnityEngine.Object[] lootableItems = Resources.LoadAll("LootPool");
        for (int i = 0; i < lootableItems.Length; i++)
        {
            Item item = lootableItems[i] as Item;
            lootPool.Add(item);
        }

        BakeNavMesh();
        if (levelGenerator != null)
            levelGenerator.doneWithGenerating += TeleportPlayerToLevel;
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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            mainCanvas.FadeToBlack();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            mainCanvas.FadeOutBlack();
        }

        //Fade out the blackground when neccesary
        if (_hasToFadeOut && DateTime.Now > _fadeTime)
        {
            mainCanvas.FadeOutBlack();
            _hasToFadeOut = false;
            gameController.isTransitioning = false;
        }

        if (_loadingLevel) { LoadLevel(); }
        if (_loadingHub) { LoadHub(); }
    }

    private void LoadLevel()
    {
        if (mainCanvas.GetBlackgroundAlpha() == 1)
        {
            levelGenerator.GenerateFullDungeonLevel(16, 16);
            _loadingLevel = false;
        }
    }

    private void LoadHub()
    {
        if (mainCanvas.GetBlackgroundAlpha() == 1)
        {
            TeleportPlayerToHub();
            _loadingHub = false;
        }
    }

    private void TeleportPlayerToLevel()
    {
        player.transform.position = levelGenerator.playerSpawnPosition;
        _hasToFadeOut = true;
        _fadeTime = DateTime.Now.AddMilliseconds(2000);
    }

    private void TeleportPlayerToHub()
    {
        player.transform.position = _playerHubSpawnPosition.transform.position;
        _hasToFadeOut = true;
        _fadeTime = DateTime.Now.AddMilliseconds(2000);
    }

    public static void GoToLevel()
    {
        if (!gameController.isTransitioning)
        {
            gameController.isTransitioning = true;
            gameController._loadingLevel = true;
            mainCanvas.FadeToBlack();
        }
    }

    public static void GoToHub()
    {
        if (!gameController.isTransitioning)
        {
            gameController.isTransitioning = true;
            gameController._loadingHub = true;
            mainCanvas.FadeToBlack();
        }
    }
}
