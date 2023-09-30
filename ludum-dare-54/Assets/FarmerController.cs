using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class FarmerController : MonoBehaviour
{
    CursorHandler cursorHandler;
    LevelManager levelManager;
    Vector2 position;
    Crop crop;

    void Start() {
        cursorHandler = CursorHandler.Instance;
        levelManager = LevelManager.Instance;
        crop = Resources.Load<Crop>("Crops/Crop");
    }

    // Update is called once per frame
    void Update()
    {
        position = cursorHandler.position;
        HandleInput();
    }

    void HandleInput() {
        if (Input.GetKeyDown("mouse 0")) cursorHandler.seedsParticles.Play();
        if (Input.GetKeyDown("mouse 1")) cursorHandler.waterParticles.Play();

        if (Input.GetKeyUp("mouse 0")) cursorHandler.seedsParticles.Stop();
        if (Input.GetKeyUp("mouse 1")) cursorHandler.waterParticles.Stop();

        if (Input.GetKey("mouse 0")) {
            levelManager.SetCrop(Instantiate(crop), Vector3Int.FloorToInt(position));
        }

        if (Input.GetKey("mouse 1")) {
            AddWaterAtPosition(position);
            AddWaterAtPosition(position + Vector2.up);
            AddWaterAtPosition(position + Vector2.down);
            AddWaterAtPosition(position + Vector2.left);
            AddWaterAtPosition(position + Vector2.right);
        }
    }

    void AddWaterAtPosition(Vector2 pos) {
        var farmTile = levelManager.GetFarmTile(Vector3Int.FloorToInt(pos));
        if (farmTile == null || !farmTile.IsUsable || farmTile.Crop == null) return;
        farmTile.Crop.AddWater();
    }
}
