using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Tooltip("Farm size has to be not divisible by 2, since (0, 0) is also included in the farm")]
    public Vector2Int farmSize = new Vector2Int(21, 11);
    public bool isMainMenu = false;

    [Space]
    public TileBase fenceTile;
    public TileBase[] farmlandTiles;

    [Space]
    public SpriteRenderer ground;
    public Tilemap farmTilemap;
    public Tilemap fenceTilemap;
    public Tilemap cropTilemap;

    [Space]
    public List<PlacedCrop> placedCrops;

    [Space]
    public float sellDuration = 0.1f;

    private GameManager gameManager;
    private StatsManager statsManager;
    private Vector2Int cornerBottomLeft, cornerTopRight;
    private List<Vector3Int> originalFarmPositions;
    private HashSet<PlacedCrop> tempPlacedCrops;
    private static GameObject puffParticles;

    void Awake() => Instance = this;

    void Start() {
        gameManager = GameManager.Instance;
        statsManager = StatsManager.Instance;
        cornerBottomLeft = -farmSize / 2;
        cornerTopRight = farmSize / 2;
        puffParticles = Resources.Load<GameObject>("PuffParticles");
    }

    public PlacedCrop GetPlacedCrop(Vector3Int position) => placedCrops.Where(x => x.position == position).FirstOrDefault();
    public bool SetCrop(Crop crop, Vector3Int position) {
        var farmTile = GetPlacedCrop(position);
        if (farmTile == null || !farmTile.IsUsable || farmTile.Crop != null) return false;
        farmTile.Crop = crop;
        if (statsManager != null) statsManager.CropsPlanted++;
        return true;
    }

    public void GenerateLevel() {
        statsManager = statsManager ?? StatsManager.Instance;
        ground.size = farmSize + new Vector2Int(40, 20);
        UpdateFence();
        originalFarmPositions = farmTilemap.FillWithTile(farmlandTiles[0], cornerBottomLeft, cornerTopRight);
        placedCrops = originalFarmPositions.Select(x => new PlacedCrop(x)).ToList();

        if (!isMainMenu) return;
        var crop = Resources.Load<Crop>("Crops/Crop");
        placedCrops.ForEach(x => {
            x.Crop = crop;
            x.Crop.AddWater();
            x.Crop.GrowRandomly();
        });
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
            statsManager.TotalEarnings += crop.Crop.GetPrice();
            crop.Crop.Destroy();
            farmTilemap.SetTile(crop.position, farmlandTiles[0]);
        }
        tempPlacedCrops.Add(crop);
        yield return new WaitForSecondsRealtime(sellDuration);
        StartCoroutine(SellCropCo(pos + Vector3Int.up));
        StartCoroutine(SellCropCo(pos + Vector3Int.down));
        StartCoroutine(SellCropCo(pos + Vector3Int.left));
        StartCoroutine(SellCropCo(pos + Vector3Int.right));
    }

    public void ShrinkFarm() {
        cornerBottomLeft += Vector2Int.one;
        cornerTopRight -= Vector2Int.one;
        var newPlacedCrops = placedCrops.Where(i =>
            i.position.x >= cornerBottomLeft.x
            && i.position.x <= cornerTopRight.x
            && i.position.y >= cornerBottomLeft.y
            && i.position.y <= cornerTopRight.y
        ).ToList();
        foreach (var tile in placedCrops.Except(newPlacedCrops)) {
            tile.IsUsable = false;
            farmTilemap.SetTile(tile.position, null);
            Instantiate(puffParticles, tile.position, Quaternion.identity);
        }
        placedCrops = newPlacedCrops;
        placedCrops.ForEach(i => farmTilemap.SetTile(i.position, farmlandTiles[0]));

        UpdateFarmSize();
        SoundManager.Play("destroy", "plant");
    }

    public void EnlargeFarm() {
        cornerBottomLeft -= Vector2Int.one;
        cornerTopRight += Vector2Int.one;
        var newPlacedCrops = originalFarmPositions.Where(i =>
            i.x >= cornerBottomLeft.x
            && i.x <= cornerTopRight.x
            && i.y >= cornerBottomLeft.y
            && i.y <= cornerTopRight.y
        ).Except(placedCrops.Select(x => x.position))
        .Select(x => new PlacedCrop(x)).ToList();
        placedCrops.AddRange(newPlacedCrops);
        placedCrops.ForEach(i => farmTilemap.SetTile(i.position, farmlandTiles[0]));

        UpdateFarmSize();
    }

    public void UpdateFarmSize() => farmSize = Vector2Int.FloorToInt(cornerTopRight - cornerBottomLeft + Vector2.one);

    public void UpdateFence() {
        fenceTilemap.ClearAllTiles();
        fenceTilemap.FillWithTile(fenceTile, cornerBottomLeft - Vector2Int.one * 3, cornerTopRight + Vector2Int.one * 3, true);
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
    public static List<Vector3Int> FillWithTile(this Tilemap tilemap, TileBase tile, Vector2Int bottomLeft, Vector2Int topRight, bool outline = false) {
        Vector3Int pos = Vector3Int.zero;
        List<Vector3Int> positions = new List<Vector3Int>();
        for (int x = bottomLeft.x; x <= topRight.x; x++) {
            for (int y = bottomLeft.y; y <= topRight.y; y++) {
                if (outline
                    && x != bottomLeft.x
                    && x != topRight.x
                    && y != bottomLeft.y
                    && y != topRight.y) continue;
                pos.x = x;
                pos.y = y;
                positions.Add(pos);
            }
        }
        tilemap.SetTiles(positions.ToArray(), Enumerable.Repeat(tile, positions.Count).ToArray());
        return positions;
    }
}