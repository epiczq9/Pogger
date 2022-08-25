using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timers;

public class GridMovement : MonoBehaviour
{
    public bool isMoving = false;
    private Vector3 origPos, targetPos;
    public float timeToMove = 0.5f;
    public Animator animator;
    private bool highJump;
    private float rayLength = 1f;
    // Start is called before the first frame update
    void Start() {
        highJump = false;
    }

    // Update is called once per frame
    void Update()  {
        if (Input.GetAxis("Horizontal") < 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 0, 0);
            //animator.Play("Jump");
            StartCoroutine(MovePlayer(Vector3.left));
            //Move(Vector3.left);
        }
        if (Input.GetAxis("Horizontal") > 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 180, 0);
            //animator.Play("Jump");
            StartCoroutine(MovePlayer(Vector3.right));
            //Move(Vector3.right);
        }
        if (Input.GetAxis("Vertical") < 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 270, 0);
            //animator.Play("Jump");
            StartCoroutine(MovePlayer(Vector3.back));
            //Move(Vector3.down);
        }
        if (Input.GetAxis("Vertical") > 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 90, 0);
            //animator.Play("Jump");
            StartCoroutine(MovePlayer(Vector3.forward));
            //Move(Vector3.up);
        }

        if (Input.GetButton("Jump")) {
            animator.Play("HighJump");
        }
    }

    private IEnumerator MovePlayer(Vector3 direction) {
        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        Vector3 rayPos = origPos + Vector3.up / 6;
        Ray ray = new Ray(rayPos, direction);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        bool rayHit = Physics.Raycast(ray, out RaycastHit hit, rayLength);
        if (!rayHit) {
            animator.Play("Jump");
        } else if (hit.collider.CompareTag("Player")) {
            animator.Play("Jump");
        }

        while (elapsedTime < timeToMove) {
            if (!rayHit) {
                transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            } else if (hit.collider.CompareTag("Player")) {
                transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            } else if (hit.collider.CompareTag("Wall")) {
                transform.position = origPos;
                targetPos = origPos;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        /*
        while (elapsedTime < timeToMove) {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        */

        transform.position = targetPos;

        highJump = false;
        isMoving = false;
    }

    private IEnumerator MovePlayerHigher(Vector3 direction) {
        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        animator.Play("HighJump");

        while (elapsedTime < timeToMove) {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        

        transform.position = targetPos;

        highJump = false;
        isMoving = false;
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Bounce")) {
            if (!isMoving) {
                Debug.Log("BOUNCY");
                transform.eulerAngles = new Vector3(0, 90, 0);
                Vector3 newPos = Vector3.forward + Vector3.up;
                highJump = true;
                StartCoroutine(MovePlayerHigher(newPos));
            }
        }
    }
}
