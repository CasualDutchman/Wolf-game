using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreloadGame : MonoBehaviour {

	void Start () {
        StartCoroutine(LoadScene());
	}

    IEnumerator LoadScene() {
        AsyncOperation op = SceneManager.LoadSceneAsync(1);
        op.allowSceneActivation = false;
        while (!op.isDone) {
            if (op.progress < 0.9f) { } 
            else {
                op.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
