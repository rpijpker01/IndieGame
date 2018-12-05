using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveText : MonoBehaviour
{

    private static ObjectiveText objectiveText;

    // Use this for initialization
    private void Start()
    {
        objectiveText = this;
    }

    public static void SetObjectiveText(string text)
    {
        objectiveText.GetComponent<Text>().text = text;
    }
}
