using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour {

    public static SceneManagement instance;

    private void Awake() {
        instance = this;

        DontDestroyOnLoad(this);

    }

    public void LoadGameScene() {
        SceneManager.LoadScene(1);
    }

    public void LoadMenuScene() {
        SceneManager.LoadScene(0);
    }

}
