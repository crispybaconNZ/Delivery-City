using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField] private Text copyright;

    void Start() {
        copyright.text = "Copyright (c) 2023 Digital Bacon. Version " + Application.version;
    }

    public void StartNewGame() {
        SceneManager.LoadScene(1);
    }

    public void ExitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
