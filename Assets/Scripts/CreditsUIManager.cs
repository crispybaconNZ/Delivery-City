using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsUIManager : MonoBehaviour {
    public void ReturnToMainMenu() {
        SceneManager.LoadScene(0);
    }
}
