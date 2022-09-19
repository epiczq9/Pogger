using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Timers;

public class GridMovement : MonoBehaviour
{
    public bool isMoving = false;
    private Vector3 origPos, targetPos, origLocalPos, targetLocalPos;
    public float timeToMoveBase = 0.25f;
    private float timeToMove;
    public Animator animator;
    private readonly float rayLength = 1f;
    public bool onFloat = false;

    public Text outputText;

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private Vector2 endTouchPosition;
    private bool stopTouch = false;

    public float swipeRange;
    public float tapRange;

    public int points;

    public CinemachineVirtualCamera vCam;

    void Start() {
        timeToMove = timeToMoveBase;
    }


    void Update()  {
        if (Input.GetAxis("Horizontal") < 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 0, 0);
            StartCoroutine(MovePlayer(Vector3.left));
        } else if (Input.GetAxis("Horizontal") > 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 180, 0);
            StartCoroutine(MovePlayer(Vector3.right));
        } else if (Input.GetAxis("Vertical") < 0 && !isMoving) {
            transform.eulerAngles = new Vector3(0, 270, 0);
            StartCoroutine(MovePlayer(Vector3.back));
        } else if((Input.GetAxis("Vertical") > 0) && !isMoving) {
            transform.eulerAngles = new Vector3(0, 90, 0);
            StartCoroutine(MovePlayer(Vector3.forward));
        }

        if (Input.GetButton("Jump")) {
            animator.Play("HighJump");
        }

        Swipe();
    }

    IEnumerator SimpleMove(Vector3 simpleTarget, float timeToSimpleMove) {
        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;

        Vector3 rayPos = origPos + Vector3.up / 6;
        Ray ray = new Ray(rayPos, simpleTarget);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        bool rayHit = Physics.Raycast(ray, out RaycastHit hit, rayLength);
        

        /*
        while (elapsedTime < timeToMove) {
            if (!rayHit) {
                transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            } else if (hit.collider.CompareTag("Player")) {
                transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToSimpleMove));
            } else if (hit.collider.CompareTag("Wall")) {
                transform.position = origPos;
                targetPos = origPos;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        */

        while (elapsedTime < timeToSimpleMove) {
            transform.position = Vector3.Lerp(origPos, simpleTarget, (elapsedTime / timeToSimpleMove));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(Mathf.RoundToInt(simpleTarget.x), simpleTarget.y, Mathf.RoundToInt(simpleTarget.z));

        isMoving = false;
    }

    private IEnumerator MovePlayer(Vector3 direction) {
        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        RaycastForward(direction);

        while (elapsedTime < timeToMove) {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(Mathf.RoundToInt(targetPos.x), targetPos.y, Mathf.RoundToInt(targetPos.z));

        RaycastDown();
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

            if (Mathf.Abs(Distance.x) < tapRange && Mathf.Abs(Distance.y) < tapRange && !isMoving) {
                outputText.text = "Tap";
                transform.eulerAngles = new Vector3(0, 90, 0);
                StartCoroutine(MovePlayer(Vector3.forward));
            }
        }
    }

    void RaycastForward(Vector3 direction) {
        Vector3 rayPos = origPos + Vector3.up / 6;
        Ray rayForward = new Ray(rayPos, direction);
        Debug.DrawRay(rayForward.origin, rayForward.direction, Color.red);
        bool rayForHit = Physics.Raycast(rayForward, out RaycastHit hitFor, rayLength);
        if (!rayForHit) {
            timeToMove = timeToMoveBase;
            targetPos = new Vector3(Mathf.RoundToInt(targetPos.x), targetPos.y, targetPos.z);
            animator.Play("Jump");
        } else if (hitFor.collider.CompareTag("Player")) {
            timeToMove = timeToMoveBase;
            animator.Play("Jump");
        } else if (hitFor.collider.CompareTag("UpDown")) {
            timeToMove = timeToMoveBase * 1.5f;
            targetPos += Vector3.up * hitFor.collider.gameObject.GetComponent<UpDownValue>().value;
            animator.Play("HighJump");
        } else if (hitFor.collider.CompareTag("Wall")) {
            timeToMove = 0f;
            transform.position = origPos;
            targetPos = origPos;
        } else {
            timeToMove = timeToMoveBase;
            animator.Play("Jump");
        }
    }

    void RaycastDown() {
        Vector3 rayPos = targetPos;
        Ray rayDown = new Ray(rayPos + Vector3.up, Vector3.down);
        Debug.DrawRay(rayDown.origin, rayDown.direction, Color.blue);
        bool rayDownHit = Physics.Raycast(rayDown, out RaycastHit hitDown, rayLength * 2);
        if (!rayDownHit) {
            Debug.Log("NATHING");
        } else if (hitDown.collider.CompareTag("Water")) {
            Debug.Log("DAS IST VATER");
            float timeToSimpleMove = 1f;
            Vector3 simpleTargetPos = targetPos + Vector3.down * 2;
            vCam.Follow = null;
            StartCoroutine(SimpleMove(simpleTargetPos, timeToSimpleMove));
            GetComponent<GridMovement>().enabled = false;
            TimersManager.SetTimer(this, 1.5f, ReloadScene);
        } else if (hitDown.collider.CompareTag("Float")) {
            Debug.Log("FLOAT");
        } else if (hitDown.collider.CompareTag("Launch")) {
            float timeToSimpleMove = timeToMoveBase;
            Launcher launcherScript = hitDown.collider.GetComponent<Launcher>();
            Vector3 simpleTargetPos = targetPos - transform.right * launcherScript.verticalLaunchValue + Vector3.up * launcherScript.horizontalLaunchValue;
            animator.Play("HighJump");
            StartCoroutine(SimpleMove(simpleTargetPos, timeToSimpleMove));
        }
    }

    void ReloadScene() {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<MySceneManager>().RestartScene();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Coin")) {
            Destroy(other.gameObject);
            points += other.gameObject.GetComponent<Coin>().coinValue;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Float")) {
           //onFloat = false;
        }
    }


    /*
>>>>>>> Stashed changes
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
