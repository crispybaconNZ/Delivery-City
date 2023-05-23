/** UI Manager for Main menu **/

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;
    [SerializeField] private Text copyright;

    private void Awake() {
        if (Instance == null) { Instance = this; }
    }

    void Start() {
        copyright.text = "Copyright (c) 2023 Digital Bacon. Version " + Application.version + ". Made for Ludum Dare 53 \"Delivery\".";
    }

    public void StartNewGame() {
        LevelLoader.Instance.LoadSpecifiedLevel(1);
    }

    public void ShowCredits() {
        LevelLoader.Instance.LoadSpecifiedLevel(2);
    }

    public void ExitGame() {
        LevelLoader.Instance.EndGame();
    }
}
