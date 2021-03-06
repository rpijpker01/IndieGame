﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumbersCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject _damageTextPrefab;

    // Use this for initialization
    private void Start()
    {
        //Assign this as the official damage number canvas TM
        GameController.damageNumbersCanvas = this;
    }

    //Show damage number above enemies head
    public void DisplayDamageNumber(bool isPlayer, float damageValue, Vector3 topOfEnemyPosition)
    {
        if (isPlayer)
        {
            GameObject damageText = Instantiate(_damageTextPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0), this.transform);
            damageText.GetComponent<DamageTextBehaviour>().SetStartingPosition(topOfEnemyPosition, damageValue);
            damageText.GetComponent<Text>().text = ((int)damageValue).ToString();
            damageText.GetComponent<Text>().color = new Color(255, 255, 255);
        }
        else
        {
            GameObject damageText = Instantiate(_damageTextPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0), this.transform);
            damageText.GetComponent<DamageTextBehaviour>().SetStartingPosition(topOfEnemyPosition, damageValue);
            damageText.GetComponent<Text>().text = ((int)damageValue).ToString();
        }
    }
}
