using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlChapBehaviour : MonoBehaviour
{
    private int _playerObjective = 0;

    private SoundPlayer _soundPlayer;

    // Use this for initialization
    void Start()
    {
        GameController.OnMouseLeftClickGameObject += InteractWithOlChap;
        _soundPlayer = GetComponent<SoundPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void InteractWithOlChap(GameObject gameObject)
    {
        if (_playerObjective <= GameController.questProgress)
        {
            _playerObjective++;
            GameController.spawnKey = true;
        }
        DisplayQuestObjective();
    }

    private void DisplayQuestObjective()
    {
        _soundPlayer.PlayAudioClip(_playerObjective - 1);
    }
}
