using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstQuestTrigger : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (GameController.questProgress == 0 && GameController.spawnKey == true)
            {
                GameController.questProgress++;
                GameController.spawnKey = false;
                Destroy(this.gameObject);
            }
        }
    }
}
