using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour {
    public static GameOverManager Instance;

    private void Awake() {
        if (Instance == null) { Instance = this; }
    }

    public void Open() {
        transform.LeanMoveLocalY(0.0f, 1.0f).setEaseInBounce();
    }
}
