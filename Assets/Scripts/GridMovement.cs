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

    //public Text outputText;

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private Vector2 endTouchPosition;
    private bool stopTouch = false;

    public float swipeRange;
    public float tapRange;

    public int score;
    public Text scoreText;

    public CinemachineVirtualCamera vCam;
    public GameObject gameController;
    MySceneManager sceneManagerScript;

    void Start() {
        timeToMove = timeToMoveBase;
        sceneManagerScript = gameController.GetComponent<MySceneManager>();
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
    IEnumerator HitPlayer(Vector3 direction) {
        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction * 6;

        while (elapsedTime < timeToMove) {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));

            elapsedTime += Time.deltaTime;
            yield return null;
        }
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
                if (Distance.x < -swipeRange && !isMoving && Mathf.Abs(Distance.x) > Mathf.Abs(Distance.y)) {
                    //outputText.text = "UP LEFT";
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    StartCoroutine(MovePlayer(Vector3.left));
                    stopTouch = true;
                } else if (Distance.x > swipeRange && !isMoving && Mathf.Abs(Distance.x) > Mathf.Abs(Distance.y)) {
                    //outputText.text = "DOWN RIGHT";
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    StartCoroutine(MovePlayer(Vector3.right));
                    stopTouch = true;
                } else if (Distance.y < -swipeRange && !isMoving && Mathf.Abs(Distance.x) < Mathf.Abs(Distance.y)) {
                    //outputText.text = "DOWN LEFT";
                    transform.eulerAngles = new Vector3(0, 270, 0);
                    StartCoroutine(MovePlayer(Vector3.back));
                    stopTouch = true;
                } else if (Distance.y > swipeRange && !isMoving && Mathf.Abs(Distance.x) < Mathf.Abs(Distance.y)) {
                    //outputText.text = "UP RIGHT";
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
                //outputText.text = "Tap";
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
        /*
        if(rayForHit) {
            Debug.Log(hitFor.collider.tag);
        }
        */
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
            //Debug.Log("NATHING");
        } else if (hitDown.collider.CompareTag("Water")) {
            //Debug.Log("DAS IST VATER");
            float timeToSimpleMove = 1f;
            Vector3 simpleTargetPos = targetPos + Vector3.down * 2;
            vCam.Follow = null;
            StartCoroutine(SimpleMove(simpleTargetPos, timeToSimpleMove));
            GetComponent<GridMovement>().enabled = false;
            TimersManager.SetTimer(this, 1.5f, ReloadScene);
        } else if (hitDown.collider.CompareTag("Float")) {
            //Debug.Log("FLOAT");
        }
    }

    void ReloadScene() {
        sceneManagerScript.RestartScene();
    }

    void NextScene() {
        sceneManagerScript.LoadNextScene();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Coin")) {
            Destroy(other.gameObject);
            score += other.gameObject.GetComponent<Coin>().coinValue;
            scoreText.text = score.ToString();
        }
        if (other.gameObject.CompareTag("Launch")) {
            Launcher launcherScript = other.gameObject.GetComponent<Launcher>();
            Vector3 simpleTargetPos = targetPos + launcherScript.direction * launcherScript.horizontalLaunchValue
                + Vector3.up * launcherScript.verticalLaunchValue;
            transform.right = -launcherScript.direction;
            float timeToSimpleMove = timeToMoveBase * 2f;
            float targetDistance = Vector3.Distance(targetPos, simpleTargetPos);
            //Debug.Log(targetDistance);
            animator.Play("LaunchJump");
            StartCoroutine(SimpleMove(simpleTargetPos, timeToSimpleMove));
        } else if (other.gameObject.CompareTag("Vehicle")) {
            //Debug.Log("KNOCKED DOON");
            GameObject vehicle = other.gameObject;
            HitByVehicle(vehicle);
        } else if (other.gameObject.CompareTag("Float")) {
            transform.SetParent(other.gameObject.transform);
        } else if (other.gameObject.CompareTag("Finish")) {
            TimersManager.SetTimer(this, 1f, NextScene);
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Float")) {
            //Debug.Log("FLOAT");
            transform.SetParent(null);
        }
    }

    void HitByVehicle(GameObject vehicle) {
        Vector3 launchDir = transform.position - vehicle.transform.position;
        launchDir = new Vector3(launchDir.x, Mathf.Abs(launchDir.y), 0).normalized;
        Debug.Log(launchDir);
        vCam.Follow = null;
        animator.Play("Hit");
        GetComponent<GridMovement>().enabled = false;
        TimersManager.SetTimer(this, 1.5f, ReloadScene);
        StartCoroutine(HitPlayer(launchDir));
    }
}
