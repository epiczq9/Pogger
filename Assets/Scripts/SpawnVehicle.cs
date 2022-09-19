using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timers;

public class SpawnVehicle : MonoBehaviour
{
    public GameObject vehiclePrefab;
    public float spawnInterval;

    public float vehicleSpeed = 20f;
    public float destroyTimer;

    void Start() {
        SpawnTheTrain();
        TimersManager.SetLoopableTimer(this, spawnInterval, SpawnTheTrain);
    }

    void SpawnTheTrain() {
        GameObject go = Instantiate(vehiclePrefab, transform);
        Vehicle vehicleScript = go.GetComponent<Vehicle>();
        vehicleScript.speed = vehicleSpeed;
        vehicleScript.destroyTimer = destroyTimer;

    }
}
