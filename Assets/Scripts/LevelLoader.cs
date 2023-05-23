using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
    private Animator _transition;
    public static LevelLoader Instance;

    private void Awake() {
        if (Instance == null) { Instance = this; }
        _transition = GetComponent<Animator>();
    }

    public void LoadNextLevel(int levelIndex) {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadSpecifiedLevel(int level) {
        StartCoroutine(LoadLevel(level));
    }

    public void EndGame() {
        StartCoroutine(CloseApplication());
    }

    IEnumerator LoadLevel(int level) {
        // play animation
        _transition.SetTrigger("SceneClose");

        // wait for anim to stop playing
        yield return new WaitForSeconds(1f);

        // load scene
        SceneManager.LoadScene(level);
    }

    IEnumerator CloseApplication() {
        _transition.SetTrigger("SceneClose");
        yield return new WaitForSeconds(1f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
