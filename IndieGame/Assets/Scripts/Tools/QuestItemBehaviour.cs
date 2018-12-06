using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItemBehaviour : MonoBehaviour
{

    [Header("Rotation variables")]
    [SerializeField]
    private float _rotationSpeed = 0.5f;
    [Header("Bobbing variables")]
    [SerializeField]
    private float _bobbingSpeed = 0.5f;
    [SerializeField]
    private float _bobbingMagnitude = 1f;

    private Vector3 _startPos;

    // Use this for initialization
    void Start()
    {
        _startPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Rotation
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + _rotationSpeed, transform.rotation.eulerAngles.z);

        //Bobbing up and down
        transform.position = _startPos + (transform.up * Mathf.Sin(Time.time * _bobbingSpeed)) * _bobbingMagnitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameController.questProgress++;
            if (GameController.questProgress >= 5)
            {
                OlChapBehaviour.ContinuePorgression();
                ObjectiveText.SetObjectiveText("- Go back to the old man");
            }
            else
            {
                ObjectiveText.SetObjectiveText("- Find keys (" + GameController.questProgress + "/5)");
            }
            GetComponent<Collider>().enabled = false;
            GetComponent<Renderer>().enabled = false;
            GetComponent<AudioSource>().Play();
        }
    }
}
