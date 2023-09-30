using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public TileBase farmTile, grassTile;
    public Vector2Int farmSize = new Vector2Int(20, 10);
    public Tilemap farmTilemap, groundTilemap;

    void Start() => Instance = this;

    public static void GetFarm() => Instance.GenerateLevel();

    private void GenerateLevel() {
        farmTilemap.FillWithTile(farmTile, -farmSize / 2, farmSize / 2);
        groundTilemap.FillWithTile(grassTile, -farmSize / 2 - Vector2Int.one * 5, farmSize / 2 + Vector2Int.one * 5);
    }
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
        var tiles = Enumerable.Repeat(tile, positions.Count).ToArray();
        tilemap.SetTiles(positions.ToArray(), tiles);
        return positions;
    }
}
