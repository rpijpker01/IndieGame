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

    private void Update()
    {
        if (!_soundPlayer.GetAudioSources()[0].isPlaying || Input.GetKeyDown(KeyCode.Escape) || (GameController.player.transform.position - transform.position).magnitude > 4.9999f)
        {
            GameController.uiCanvas.CloseDialogBox();
            GameController.player.GetComponent<PlayerMovement>().enabled = true;
            GameController.player.GetComponent<PlayerAttacks>().enabled = true;
        }
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
        GameController.player.transform.LookAt(this.transform);
        GameController.player.GetComponent<PlayerMovement>().rotation.y = GameController.player.transform.rotation.eulerAngles.y;
    }

    private void DisplayQuestObjective()
    {
        Debug.Log(_playerObjective);
        _soundPlayer.PlayAudioClip(_playerObjective - 1);

        switch (_playerObjective)
        {
            default:
                GameController.uiCanvas.OpenDialogBox("Insert hardcoded string here :-)");
                break;
        }
    }
}
