using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatBehaviour : MonoBehaviour
{

    public Transform startPos, endPos;
    public bool toMove = true;
    public float timeToMove;
    private float timePassed;
    void Start() {
        
    }

    void Update() {
        if (toMove) {
            Move();
        }
    }

    private void Move() {
        timePassed += Time.deltaTime;

        transform.position = Vector3.Lerp(startPos.position, endPos.position,  Mathf.PingPong(timePassed / timeToMove, 1));
    }
    /*
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Player IN");
            //other.gameObject.GetComponent<GridMovement>().onFloat = true;
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Player out");
            //other.gameObject.GetComponent<GridMovement>().onFloat = false;
            other.transform.SetParent(null);
        }
    }
    */
}
