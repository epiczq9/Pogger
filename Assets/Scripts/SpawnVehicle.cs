using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timers;

public class SpawnVehicle : MonoBehaviour
{
    //public GameObject vehiclePrefab;
    public GameObject[] vehicles;
    public float spawnInterval;

    public float vehicleSpeed;
    public float destroyTimer;

    void Start() {
        SpawnTheVehicle();
        TimersManager.SetLoopableTimer(this, spawnInterval, SpawnTheVehicle);
    }

    void SpawnTheVehicle() {
        GameObject go = Instantiate(vehicles[Random.Range(0, vehicles.Length-1)], transform);
        Vehicle vehicleScript = go.GetComponent<Vehicle>();
        vehicleScript.speed = vehicleSpeed;
        vehicleScript.destroyTimer = destroyTimer;

    }
}
