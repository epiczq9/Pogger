using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue;
    public float spinSpeed = 50f;

    private Vector3 startPos, movedPos;
    public float timeToMoveUp = 1f;
    private float timePassed;

    private void Start() {
        startPos = transform.position;
        movedPos = transform.position + new Vector3(0, 0.2f, 0);
    }
    private void Update() {
        timePassed += Time.deltaTime;
        transform.position = Vector3.Lerp(startPos, movedPos, Mathf.PingPong(timePassed / timeToMoveUp, 1));

        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }
}
