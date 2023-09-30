using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    public static CursorHandler Instance;
    [HideInInspector] public Vector2 position;
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

        if (Input.GetKey("mouse 0")) {
            LevelManager.Instance.SetCrop(Instantiate(Resources.Load<Crop>("Crops/Crop")), Vector3Int.FloorToInt(position));
            Debug.Log($"planetd crop at {Vector3Int.FloorToInt(position)}");
        }
        if (Input.GetKey("mouse 1")) {
            var farmTile = LevelManager.Instance.GetFarmTile(Vector3Int.FloorToInt(position));
            if (farmTile == null || !farmTile.IsUsable || farmTile.Crop == null) return;
            farmTile.Crop.AddWater();
            Debug.Log($"added water to crop at {Vector3Int.FloorToInt(position)}");
        }
    }

    void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) Cursor.visible = false;
    }
    void OnApplicationPause(bool pauseStatus) {
        if (!pauseStatus) Cursor.visible = false;
    }
}
