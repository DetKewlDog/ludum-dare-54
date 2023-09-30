using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Vector2Int farmSize = new Vector2Int(20, 10);

    [Space]
    public TileBase grassTile;
    public Sprite[] farmlandSprites;

    [Space]
    public Tilemap farmTilemap;
    public Tilemap groundTilemap;
    public Tilemap cropTilemap;

    [Space]
    public List<FarmTile> farmTiles;

    private Tile[] farmlandTiles;
    private List<Vector3Int> originalFarmPositions;

    private Vector2 cornerBottomLeft, cornerTopRight;

    void Start() {
        Instance = this;
        farmlandTiles = farmlandSprites.Select((sprite, i) => {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = farmlandSprites[i];
            return tile;
        }).ToArray();
        cornerBottomLeft = -farmSize / 2;
        cornerTopRight = farmSize / 2;
    }

    public FarmTile GetFarmTile(Vector3Int position) {
        return farmTiles.Where(x => x.position == position).FirstOrDefault();
    }
    public void SetCrop(Crop crop, Vector3Int position) {
        var farmTile = GetFarmTile(position);
        if (farmTile == null || !farmTile.IsUsable || farmTile.Crop != null) return;
        farmTile.Crop = crop;
    }

    public void GenerateLevel() {
        groundTilemap.FillWithTile(grassTile, -farmSize / 2 - Vector2Int.one * 5, farmSize / 2 + Vector2Int.one * 5);
        originalFarmPositions = farmTilemap.FillWithTile(farmlandTiles[0], -farmSize / 2, farmSize / 2);
        farmTiles = originalFarmPositions.Select(x => new FarmTile(x)).ToList();
    }

    public TileBase GetFarmlandTile(float moisture) => farmlandTiles[(int)(moisture * (farmlandTiles.Length - 1))];

    public void ShrinkFarm() {
        cornerBottomLeft += Vector2.one;
        cornerTopRight -= Vector2.one;
        var newFarmTiles = farmTiles.Where(i =>
            i.position.x >= cornerBottomLeft.x
            && i.position.x <= cornerTopRight.x
            && i.position.y >= cornerBottomLeft.y
            && i.position.y <= cornerTopRight.y
        ).ToList();
        var tilesToRemove = farmTiles.Except(newFarmTiles);
        foreach (var tile in tilesToRemove) {
            tile.IsUsable = false;
            farmTilemap.SetTile(tile.position, null);
        }
        farmTiles = newFarmTiles;
    }
    public void EnlargeFarm() {
        cornerBottomLeft -= Vector2.one;
        cornerTopRight += Vector2.one;
        var newFarmTiles = originalFarmPositions.Where(i =>
            i.x >= cornerBottomLeft.x
            && i.x <= cornerTopRight.x
            && i.y >= cornerBottomLeft.y
            && i.y <= cornerTopRight.y
        ).Except(farmTiles.Select(x => x.position))
        .Select(x => new FarmTile(x)).ToList();
        farmTiles.AddRange(newFarmTiles);
        newFarmTiles.ForEach(i => farmTilemap.SetTile(i.position, farmlandTiles[0]));
    }
}

[System.Serializable]
public class FarmTile {
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

    public FarmTile(Vector3Int pos) => position = pos;
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