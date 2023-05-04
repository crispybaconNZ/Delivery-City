using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameMode {
    Playing,        // wandering around the city
    Noticeboard,    // looking at the noticeboard
    PauseMenu,      // looking at the pause menu
    Interaction     // interacting with a customer
}


public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public static Color dialogColor = new Color(0.2470588f, 0.2470588f, 0.454902f);

    private void Awake() {
        if (Instance == null) { Instance = this; }
    }

    
    void Start() {
        
    }

    
    void Update() {
        
    }
}
