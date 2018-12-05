using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteriousDoor : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameController.OnMouseLeftClickGameObject += OpenDoor;
    }

    private void OpenDoor(GameObject gameObject)
    {
        if (GameController.questProgress == 7)
        {
            GameController.mainCanvas.FadeToBlack();
        }
        else
        {
            Debug.Log("Progress futher to unlock");
            //Display message saying the player needs to progress futher to unlock
        }
    }
}
