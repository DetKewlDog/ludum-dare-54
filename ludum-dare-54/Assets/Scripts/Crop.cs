﻿using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Crop", menuName = "ScriptableObjects/Crop")]
public class Crop : ScriptableObject
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField, Range(0, 1)] private float growthChance;
    [SerializeField, Range(0, 1)] private float growthRate;

    private float age, moistureLevel;
    private TileBase[] tiles;
    private Coroutine growCo;
    private Tilemap farmTilemap, cropTilemap;
    private Vector3Int pos;

    public void Initialize(Tilemap farmTilemap, Tilemap cropTilemap, Vector3Int pos) {
        tiles = sprites.Select((sprite, i) => {
            var tile = CreateInstance<Tile>();
            tile.sprite = sprites[i];
            return tile;
        }).ToArray();
        this.farmTilemap = farmTilemap;
        this.cropTilemap = cropTilemap;
        this.pos = pos;
        growCo = LevelManager.Instance.StartCoroutine(GrowCo());
    }

    public void AddWater() {
        moistureLevel = 1;
        UpdateMoisture();
    }

    public void Destroy() {
        LevelManager.Instance.StopCoroutine(growCo);
        cropTilemap.SetTile(pos, null);
        UpdateMoisture();
        Destroy(this);
    }

    private void UpdateCrop() => cropTilemap.SetTile(pos, tiles[(int)(age * (tiles.Length - 1))]);
    private void UpdateMoisture() => farmTilemap.SetTile(pos, LevelManager.Instance.GetFarmlandTile(moistureLevel));

    private IEnumerator GrowCo() {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        age = 0;
        moistureLevel = 0.2f;
        UpdateCrop();
        UpdateMoisture();
        while (age < 1) {
            yield return waitForSeconds;
            moistureLevel -= 0.01f;
            if (moistureLevel <= 0) {
                Destroy();
                yield break;
            }
            UpdateMoisture();
            if (Random.value >= growthChance * moistureLevel) continue;
            age += growthRate;
            if (age > 1) age = 1;
            UpdateCrop();
        }
    }
}