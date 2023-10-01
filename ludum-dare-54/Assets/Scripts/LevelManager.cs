using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections;
using UnityEngine.Rendering;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Vector2Int farmSize = new Vector2Int(20, 10);

    [Space]
    public TileBase grassTile;
    public TileBase[] farmlandTiles;

    [Space]
    public Tilemap farmTilemap;
    public Tilemap groundTilemap;
    public Tilemap cropTilemap;

    [Space]
    public List<PlacedCrop> placedCrops;

    [Space]
    public float sellDuration = 0.1f;

    private GameManager gameManager;
    private Vector2 cornerBottomLeft, cornerTopRight;
    private List<Vector3Int> originalFarmPositions;
    private HashSet<PlacedCrop> tempPlacedCrops;

    void Awake() => Instance = this;

    void Start() {
        gameManager = GameManager.Instance;
        cornerBottomLeft = -farmSize / 2;
        cornerTopRight = farmSize / 2;
    }

    public PlacedCrop GetPlacedCrop(Vector3Int position) {
        return placedCrops.Where(x => x.position == position).FirstOrDefault();
    }
    public void SetCrop(Crop crop, Vector3Int position) {
        var farmTile = GetPlacedCrop(position);
        if (farmTile == null || !farmTile.IsUsable || farmTile.Crop != null) return;
        farmTile.Crop = crop;
    }

    public void GenerateLevel() {
        groundTilemap.FillWithTile(grassTile, -farmSize / 2 - Vector2Int.one * 5, farmSize / 2 + Vector2Int.one * 5);
        originalFarmPositions = farmTilemap.FillWithTile(farmlandTiles[0], -farmSize / 2, farmSize / 2);
        placedCrops = originalFarmPositions.Select(x => new PlacedCrop(x)).ToList();
    }

    public TileBase GetFarmlandTile(float moisture) => farmlandTiles[(int)(moisture * (farmlandTiles.Length - 1))];

    public void SellAllCrops() {
        tempPlacedCrops = new HashSet<PlacedCrop>();
        StartCoroutine(SellCropCo(Vector3Int.zero));
    }

    IEnumerator SellCropCo(Vector3Int pos) {
        var crop = GetPlacedCrop(pos);
        if (crop == null || tempPlacedCrops.Contains(crop)) yield break;
        if (crop.Crop != null && crop.IsUsable) {
            gameManager.MoneyAmount += crop.Crop.GetPrice();
            crop.Crop.Destroy();
        }
        tempPlacedCrops.Add(crop);
        yield return new WaitForSecondsRealtime(sellDuration);
        StartCoroutine(SellCropCo(pos + Vector3Int.up));
        StartCoroutine(SellCropCo(pos + Vector3Int.down));
        StartCoroutine(SellCropCo(pos + Vector3Int.left));
        StartCoroutine(SellCropCo(pos + Vector3Int.right));
    }

    public void ShrinkFarm() {
        cornerBottomLeft += Vector2.one;
        cornerTopRight -= Vector2.one;
        var newPlacedCrops = placedCrops.Where(i =>
            i.position.x >= cornerBottomLeft.x
            && i.position.x <= cornerTopRight.x
            && i.position.y >= cornerBottomLeft.y
            && i.position.y <= cornerTopRight.y
        ).ToList();
        var tilesToRemove = placedCrops.Except(newPlacedCrops);
        foreach (var tile in tilesToRemove) {
            tile.IsUsable = false;
            farmTilemap.SetTile(tile.position, null);
        }
        placedCrops = newPlacedCrops;
    }

    public void EnlargeFarm() {
        cornerBottomLeft -= Vector2.one;
        cornerTopRight += Vector2.one;
        var newPlacedCrops = originalFarmPositions.Where(i =>
            i.x >= cornerBottomLeft.x
            && i.x <= cornerTopRight.x
            && i.y >= cornerBottomLeft.y
            && i.y <= cornerTopRight.y
        ).Except(placedCrops.Select(x => x.position))
        .Select(x => new PlacedCrop(x)).ToList();
        placedCrops.AddRange(newPlacedCrops);
        newPlacedCrops.ForEach(i => farmTilemap.SetTile(i.position, farmlandTiles[0]));
    }
}

[System.Serializable]
public class PlacedCrop {
    [HideInInspector] public Vector3Int position;

    protected bool _isUsable = true;
    public bool IsUsable {
        get => _isUsable;
        set {
            _isUsable = value;
            if (!_isUsable && _crop != null) _crop.Destroy();
        }
    }

    protected Crop _crop = null;
    public Crop Crop {
        get => _crop;
        set { if ((_crop = value) != null) _crop.Initialize(LevelManager.Instance.farmTilemap, LevelManager.Instance.cropTilemap, position); }
    }

    public PlacedCrop(Vector3Int pos) => position = pos;
    public override string ToString() => position.ToString();
}

public static class TilemapExtensions {
    public static List<Vector3Int> FillWithTile(this Tilemap tilemap, TileBase tile, Vector2Int bottomLeft, Vector2Int topRight) {
        Vector3Int pos = Vector3Int.zero;
        List<Vector3Int> positions = new List<Vector3Int>();
        for (int x = bottomLeft.x; x <= topRight.x; x++) {
            for (int y = bottomLeft.y; y <= topRight.y; y++) {
                pos.x = x;
                pos.y = y;
                positions.Add(pos);
            }
        }
        tilemap.SetTiles(positions.ToArray(), Enumerable.Repeat(tile, positions.Count).ToArray());
        return positions;
    }
}