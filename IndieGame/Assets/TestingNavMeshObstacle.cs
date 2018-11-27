using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestingNavMeshObstacle : MonoBehaviour
{

    private NavMeshObstacle _obstacle;

    // Use this for initialization
    void Start()
    {
        _obstacle = GetComponent<NavMeshObstacle>();
        _obstacle.carving = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
