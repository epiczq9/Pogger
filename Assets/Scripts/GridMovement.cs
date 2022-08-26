using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridMovement : MonoBehaviour
{
    public bool isMoving = false;
    private Vector3 origPos, targetPos;
    public float timeToMove = 0.5f;
    public Animator animator;
    private bool highJump;
    private float rayLength = 1f;

    public Text outputText;

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private Vector2 endTouchPosition;
    private bool stopTouch = false;

    public float swipeRange;
    public float tapRange;
    // Start is called before the first frame update
    void Start() {
        highJump = false;
    }

    // Update is called once per frame
    void Update()  {
        if (Input.GetAxis("Horizontal") < 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 0, 0);
            StartCoroutine(MovePlayer(Vector3.left));
        }
        if (Input.GetAxis("Horizontal") > 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 180, 0);
            StartCoroutine(MovePlayer(Vector3.right));
        }
        if (Input.GetAxis("Vertical") < 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 270, 0);
            StartCoroutine(MovePlayer(Vector3.back));
        }
        if (Input.GetAxis("Vertical") > 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 90, 0);
            StartCoroutine(MovePlayer(Vector3.forward));
        }

        if (Input.GetButton("Jump")) {
            animator.Play("HighJump");
        }

        Swipe();
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
        } else if (hit.collider.CompareTag("Bounce")) {
            targetPos += Vector3.up;
            animator.Play("HighJump");
        } else if (hit.collider.CompareTag("DropDown")) {
            targetPos += Vector3.down;
            animator.Play("HighJump");
        }

        while (elapsedTime < timeToMove) {
            if (!rayHit) {
                transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            } else if (hit.collider.CompareTag("Wall")) {
                transform.position = origPos;
                targetPos = origPos;
            } else {
                transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
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

    public void Swipe() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            startTouchPosition = Input.GetTouch(0).position;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            currentTouchPosition = Input.GetTouch(0).position;
            Vector2 Distance = currentTouchPosition - startTouchPosition;

            if (!stopTouch) {
                if (Distance.x < -swipeRange && Distance.y > swipeRange && !isMoving) {
                    outputText.text = "UP LEFT";
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    StartCoroutine(MovePlayer(Vector3.left));
                    stopTouch = true;
                } else if (Distance.x > swipeRange && Distance.y < -swipeRange && !isMoving) {
                    outputText.text = "DOWN RIGHT";
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    StartCoroutine(MovePlayer(Vector3.right));
                    stopTouch = true;
                } else if (Distance.x < -swipeRange && Distance.y < -swipeRange && !isMoving) {
                    outputText.text = "DOWN LEFT";
                    transform.eulerAngles = new Vector3(0, 270, 0);
                    StartCoroutine(MovePlayer(Vector3.back));
                    stopTouch = true;
                } else if (Distance.x > swipeRange && Distance.y > swipeRange && !isMoving) {
                    outputText.text = "UP RIGHT";
                    transform.eulerAngles = new Vector3(0, 90, 0);
                    StartCoroutine(MovePlayer(Vector3.forward));
                    stopTouch = true;
                }
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
            stopTouch = false;
            endTouchPosition = Input.GetTouch(0).position;
            Vector2 Distance = endTouchPosition - startTouchPosition;

            if (Mathf.Abs(Distance.x) < tapRange && Mathf.Abs(Distance.y) < tapRange) {
                outputText.text = "Tap";
            }
        }
    }

    /*
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
        if (other.gameObject.CompareTag("DropDown")) {
            if (!isMoving) {
                Debug.Log("Drop");
                transform.eulerAngles = new Vector3(0, 90, 0);
                Vector3 newPos = Vector3.forward + Vector3.down;
                highJump = true;
                StartCoroutine(MovePlayerHigher(newPos));
            }
        }
    }
    */
}
