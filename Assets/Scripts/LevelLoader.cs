using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

     public void LoadNextLevel() {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadSpecifiedLevel(int level) {
        StartCoroutine(LoadLevel(level));
    }

    IEnumerator LoadLevel(int level) {
        // play animation
        transition.SetTrigger("Start");

        // wait for anim to stop playing
        yield return new WaitForSeconds(1f);

        // load scene
        SceneManager.LoadScene(level);
    }
}
