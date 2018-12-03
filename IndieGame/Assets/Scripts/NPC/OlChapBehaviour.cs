using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlChapBehaviour : MonoBehaviour
{
    private int _playerObjective = 0;

    // Use this for initialization
    void Start()
    {
        GameController.OnMouseLeftClickGameObject += InteractWithOlChap;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InteractWithOlChap(GameObject gameObject)
    {
        if (_playerObjective <= GameController.questProgress)
        {
            Debug.Log(_playerObjective);
            _playerObjective++;
            GameController.spawnKey = true;
        }
        DisplayQuestObjective();
    }

    private void DisplayQuestObjective()
    {
        Debug.Log(_playerObjective);
    }
}
