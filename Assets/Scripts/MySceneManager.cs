using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public int nextLevel;


    private void Update() {
        if(GameObject.FindGameObjectWithTag("Player") == null) {
            RestartScene();
        }
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(nextLevel);
    }
    public void LoadScene(int i) {
        SceneManager.LoadScene(i);
    }
    public void RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
