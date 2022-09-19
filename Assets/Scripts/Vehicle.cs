using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timers;

public class Vehicle : MonoBehaviour
{
    public float speed;
    public float destroyTimer;

    void Start() {
        TimersManager.SetTimer(this, destroyTimer, SelfDestruct);
    }

    void Update() {
        transform.position += speed * Time.deltaTime * transform.forward;
    }

    void SelfDestruct() {
        Destroy(gameObject);
    }
}
