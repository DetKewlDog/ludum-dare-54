using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class FarmerController : MonoBehaviour
{
    CursorHandler cursorHandler;
    LevelManager levelManager;
    GameManager gameManager;
    Vector2 position;
    Crop crop;

    Vector2Int lastPosition = Vector2Int.zero;

    void Start() {
        cursorHandler = CursorHandler.Instance;
        levelManager = LevelManager.Instance;
        gameManager = GameManager.Instance;
        crop = Resources.Load<Crop>("Crops/Crop");
    }

    // Update is called once per frame
    void Update()
    {
        position = cursorHandler.position;
        if (Time.timeScale == 0) {
            cursorHandler.seedsParticles.Stop();
            cursorHandler.waterParticles.Stop();
            return;
        }
        HandleInput();
        lastPosition = Vector2Int.FloorToInt(position);
    }

    void HandleInput() {
        if (Input.GetKeyDown("mouse 0")) cursorHandler.seedsParticles.Play();
        if (Input.GetKeyDown("mouse 1")) {
            cursorHandler.waterParticles.Play();
            SoundManager.Play("water");
        }

        if (Input.GetKeyUp("mouse 0")) cursorHandler.seedsParticles.Stop();
        if (Input.GetKeyUp("mouse 1")) cursorHandler.waterParticles.Stop();

        if (Input.GetKey("mouse 0") && gameManager.CropAmount > 0) {
            if (levelManager.SetCrop(Instantiate(crop), Vector3Int.FloorToInt(position))) {
                gameManager.CropAmount--;
                if (lastPosition != position) SoundManager.Play("plant");
            }
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
        var farmTile = levelManager.GetPlacedCrop(Vector3Int.FloorToInt(pos));
        if (farmTile == null || !farmTile.IsUsable || farmTile.Crop == null) return;
        farmTile.Crop.AddWater();
    }
}
