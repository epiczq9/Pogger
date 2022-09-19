using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timers;

public class Train : MonoBehaviour
{
    public float speed = 20f;

    void Start() {
        TimersManager.SetTimer(this, 3f, SelfDestruct);
    }

    void Update() {
        transform.position += speed * Time.deltaTime * Vector3.right;
    }

    void SelfDestruct() {
        Destroy(gameObject);
    }
}
