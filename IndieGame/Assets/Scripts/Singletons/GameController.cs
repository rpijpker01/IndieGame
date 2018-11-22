using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController gameController;

    public static GameObject player;
    public static PlayerController playerController;

    public static GameObject cameraObj;
    public static Camera camera;

    // Use this for initialization
    private void Start()
    {
        gameController = this;
        player = GameObject.FindGameObjectWithTag("Player");
        if (null != player)
            playerController = player.GetComponent<PlayerController>();

        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        camera = cameraObj.GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
