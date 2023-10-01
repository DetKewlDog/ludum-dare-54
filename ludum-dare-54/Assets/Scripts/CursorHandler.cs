using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    public static CursorHandler Instance;
    public Vector2 position;

    public Transform particlesT;
    public ParticleSystem waterParticles, seedsParticles;

    protected Vector3 mousePos;
    protected Camera mainCamera;

    void Awake() => Instance = this;

    // Start is called before the first frame update
    void Start() {
        mainCamera = Camera.main;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        transform.position = mousePos = Input.mousePosition;
        mousePos.z = 10;
        particlesT.position = position = mainCamera.ScreenToWorldPoint(mousePos);
    }

    void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) Cursor.visible = false;
    }
    void OnApplicationPause(bool pauseStatus) {
        if (!pauseStatus) Cursor.visible = false;
    }
}
