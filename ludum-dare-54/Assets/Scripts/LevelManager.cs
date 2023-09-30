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

    void Start() {
        Instance = this;
        farmlandTiles = farmlandSprites.Select((sprite, i) => {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = farmlandSprites[i];
            return tile;
        }).ToArray();
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
        farmTiles = farmTilemap.FillWithTile(farmlandTiles[0], -farmSize / 2, farmSize / 2).Select(x => new FarmTile(x)).ToList();
    }

    public TileBase GetFarmlandTile(float moisture) => farmlandTiles[(int)(moisture * (farmlandTiles.Length - 1))];
}

[System.Serializable]
public class FarmTile {
    [HideInInspector] public Vector3Int position;

    protected bool _isUsable = true;
    public bool IsUsable {
        get => _isUsable;
        set { if ((_isUsable = value) && _crop != null) _crop.Destroy(); }
    }

    protected Crop _crop = null;
    public Crop Crop {
        get => _crop;
        set { if ((_crop = value) != null) _crop.Initialize(LevelManager.Instance.farmTilemap, LevelManager.Instance.cropTilemap, position); }
    }

    public FarmTile(Vector3Int pos) => position = pos;
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