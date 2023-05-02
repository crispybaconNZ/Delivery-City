using UnityEngine;
using UnityEngine.UI;

public class StreetSignManager : MonoBehaviour {
    public static StreetSignManager Instance;
    [SerializeField] private Text address;

    private void Awake() {
        if (Instance == null) { Instance = this; }
    }
    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void SetAddress(string new_address) {
        address.text = new_address.Trim();
    }
}
