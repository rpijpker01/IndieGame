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
    public static event Action<GameObject> OnCtrlKeyHoldEvent;
    public static event Action<GameObject> OnCrtlKeyUpEvent;
    public static event Action OnAltKeyDownEvent;
    public static event Action OnAltKeyUpEvent;
    public static event Action OnEKeyDown;
    public static event Action OnQKeyDown;

    public static float maxHealth;
    public static float maxMana;
    public static float armor;
    public static float strength;
    public static float intelligence;

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

    //Quest stuff
    public static int questProgress = 0;
    public static bool spawnKey = false;

    // Use this for initialization
    private void Awake()
    {
        gameController = this;
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.FindGameObjectWithTag("Player");
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

        Physics.IgnoreLayerCollision(10, 10);
        Physics.IgnoreLayerCollision(10, 11);
    }

    private void BakeNavMesh()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.layer == 9)
            {
                if (gameObject.GetComponent<CapsuleCollider>() != null)
                {
                    if (gameObject.GetComponent<NavMeshObstacle>() == null)
                    {
                        NavMeshObstacle obstacle = gameObject.AddComponent<NavMeshObstacle>();
                        CapsuleCollider capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
                        obstacle.shape = NavMeshObstacleShape.Capsule;
                        obstacle.center = capsuleCollider.center;
                        obstacle.radius = capsuleCollider.radius;
                        obstacle.height = capsuleCollider.height;
                        obstacle.carving = true;
                    }
                }
                else if (gameObject.GetComponent<BoxCollider>() != null)
                {
                    if (gameObject.GetComponent<NavMeshObstacle>() == null)
                    {
                        NavMeshObstacle obstacle = gameObject.AddComponent<NavMeshObstacle>();
                        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
                        obstacle.shape = NavMeshObstacleShape.Box;
                        obstacle.center = boxCollider.center;
                        obstacle.size = boxCollider.size;
                        obstacle.carving = true;
                    }
                }
                else
                {
                    if (gameObject.GetComponent<NavMeshObstacle>() == null)
                    {
                        NavMeshObstacle obstacle = gameObject.AddComponent<NavMeshObstacle>();
                        obstacle.carving = true;
                    }
                }
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

        ManageInputEvents();

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

    private void ManageInputEvents()
    {
        if (Input.GetKeyDown(KeyCode.E))
            if (OnEKeyDown != null)
                OnEKeyDown();

        if (Input.GetKeyDown(KeyCode.Q))
            if (OnQKeyDown != null)
                OnQKeyDown();
    }

    private void ManageGameObjectOnMouse(GameObject pNewGameObject)
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
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() &&
                (pNewGameObject.GetComponent<HighlightGameobject>() != null || pNewGameObject.GetComponent<ItemDrop>() != null))
            {
                Vector3 distToPlayer = player.transform.position - pNewGameObject.transform.position;
                if (distToPlayer.magnitude < 3)
                {
                    if (OnMouseLeftClickGameObject != null)
                        OnMouseLeftClickGameObject(pNewGameObject);
                }
                else
                {
                    errorMessage.AddMessage("Move closer to interact with target");
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                if (OnCtrlKeyHoldEvent != null)
                    OnCtrlKeyHoldEvent(pNewGameObject);
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (OnCrtlKeyUpEvent != null)
                OnCrtlKeyUpEvent(pNewGameObject);
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (OnAltKeyDownEvent != null)
                OnAltKeyDownEvent();
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            if (OnAltKeyUpEvent != null)
                OnAltKeyUpEvent();
        }
    }

    private void LoadLevel()
    {
        if (mainCanvas.GetBlackgroundAlpha() == 1)
        {
            levelGenerator.GenerateFullDungeonLevel(16, 16);
            BakeNavMesh();
            CameraFollowPlayer.InvertCamera();
            PlayerMovement.InvertControls();
            _loadingLevel = false;
        }
    }

    private void LoadHub()
    {
        if (mainCanvas.GetBlackgroundAlpha() == 1)
        {
            TeleportPlayerToHub();
            CameraFollowPlayer.InvertCamera();
            PlayerMovement.InvertControls();
            player.GetComponent<PlayerMovement>().rotation = new Vector3(0, 0, 0);
            _loadingHub = false;
        }
    }

    private void TeleportPlayerToLevel()
    {
        player.transform.position = levelGenerator.playerSpawnPosition + transform.up * 2;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if ((enemy.transform.position - player.transform.position).magnitude < 16f)
            {
                Destroy(enemy.gameObject);
            }
        }
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
