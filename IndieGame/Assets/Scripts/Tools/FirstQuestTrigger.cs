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
            if (OlChapBehaviour.GetQuestProgression() == 1)
            {
                OlChapBehaviour.ContinuePorgression();
                ObjectiveText.SetObjectiveText("- Go back to the old man");
                Destroy(this.gameObject);
            }
        }
    }
}
