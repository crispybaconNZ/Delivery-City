using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlock : MonoBehaviour {

    public BuildingSO building;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = building.sprite;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
