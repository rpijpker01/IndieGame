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

    public static event Action<GameObject> OnMouseOverGameObjectEvent;
    public static event Action<GameObject> OnMouseAwayFromGameObject;
    public static event Action<GameObject> OnMouseLeftClickGameObject;
    public static event Action<GameObject> OnCtrlButtonHoldEvent;
    public static event Action<GameObject> OnCrtlButtonLetgoEvent;

    public static bool mouseIsOnScreen;

    private GameObject _gameObjectToCompare;
    private bool _mouseOverAnotherObject;
    private Rect _screenSize;
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
        UnityEngine.Object[] lootableItems = UnityEngine.Resources.LoadAll("LootPool");
        for (int i = 0; i < lootableItems.Length; i++)
        {
            Item item = lootableItems[i] as Item;
            lootPool.Add(item);
        }

        _screenSize = new Rect(0, 0, Screen.width, Screen.height);

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

        if (_screenSize.Contains(Input.mousePosition))
        {
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            ManageGameObjectOnMouse(hit.transform.gameObject);
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

    public void ManageGameObjectOnMouse(GameObject pNewGameObject)
    {
        if (_gameObjectToCompare == null || _gameObjectToCompare != pNewGameObject && _mouseOverAnotherObject)
        {
            _gameObjectToCompare = pNewGameObject;
            if (OnMouseOverGameObjectEvent != null)
                OnMouseOverGameObjectEvent(_gameObjectToCompare);

            _mouseOverAnotherObject = false;
        }
        else if (!_mouseOverAnotherObject)
        {
            if (OnMouseAwayFromGameObject != null)
                OnMouseAwayFromGameObject(_gameObjectToCompare);

            _mouseOverAnotherObject = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                if (OnMouseLeftClickGameObject != null)
                    OnMouseLeftClickGameObject(pNewGameObject);
            }
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                if (OnCrtlButtonLetgoEvent != null)
                    OnCrtlButtonLetgoEvent(pNewGameObject);
                if (OnCtrlButtonHoldEvent != null)
                    OnCtrlButtonHoldEvent(pNewGameObject);
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (OnCrtlButtonLetgoEvent != null)
                OnCrtlButtonLetgoEvent(pNewGameObject);
        }
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
        player.transform.position = levelGenerator.playerSpawnPosition + transform.up * 2;
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
