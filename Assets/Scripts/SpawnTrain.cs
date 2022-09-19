using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timers;

public class SpawnTrain : MonoBehaviour
{
    public GameObject trainPrefab;
    void Start() {
        Instantiate(trainPrefab, transform);
        TimersManager.SetLoopableTimer(this, 7f, SpawnTheTrain);
    }

    // Update is called once per frame
    void Update() {
        
    }

    void SpawnTheTrain() {
        Instantiate(trainPrefab, transform);
    }
}
