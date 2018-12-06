using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseDoor : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameController.OnMouseLeftClickGameObject += InteractWithDoor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InteractWithDoor(GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            if (OlChapBehaviour.GetQuestProgression() >= 7)
            {
                GameController.mainCanvas.FadeToBlack();
                OlChapBehaviour.ContinuePorgression();
                GameObject.Find("villageElder").GetComponent<OlChapBehaviour>().DisplayQuestObjective();
                GameObject.Find("villageElder").GetComponent<AudioSource>().spatialBlend = 0;
            }
        }
    }
}
