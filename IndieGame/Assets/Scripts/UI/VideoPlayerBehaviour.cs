using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _objectsToDisable;

    private bool _playedOnce = false;

    // Use this for initialization
    private void Awake()
    {
        foreach (GameObject obj in _objectsToDisable)
        {
            obj.SetActive(false);
        }
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
            foreach (GameObject obj in _objectsToDisable)
            {
                obj.SetActive(true);
            }
            GameController.mainCanvas.transform.GetChild(0).gameObject.SetActive(true);
            GameController.mainCanvas.FadeOutBlack();
            GameController.gameController.GetComponents<AudioSource>()[1].Play();
            Destroy(this.gameObject);
        }
    }
}
