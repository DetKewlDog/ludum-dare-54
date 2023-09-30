using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    public static CursorHandler Instance;
    public Vector2 position;

    public ParticleSystem waterParticles, seedsParticles;

    protected Vector3 mousePos;
    protected Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        mainCamera = Camera.main;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 10;
        position = mainCamera.ScreenToWorldPoint(mousePos);
        transform.position = position;
    }

    void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) Cursor.visible = false;
    }
    void OnApplicationPause(bool pauseStatus) {
        if (!pauseStatus) Cursor.visible = false;
    }
}
