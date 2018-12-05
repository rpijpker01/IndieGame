using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerBehaviour : MonoBehaviour
{

    [SerializeField]
    private GameObject _uiCanvas;

    private bool _playedOnce = false;

    // Use this for initialization
    private void Awake()
    {
        _uiCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<VideoPlayer>().isPlaying)
        {
            _playedOnce = true;
            GameController.mainCanvas.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (_playedOnce && !GetComponent<VideoPlayer>().isPlaying)
        {
            _uiCanvas.SetActive(true);
            GameController.mainCanvas.transform.GetChild(0).gameObject.SetActive(true);
            GameController.mainCanvas.FadeOutBlack();
            Destroy(this.gameObject);
        }
    }
}
