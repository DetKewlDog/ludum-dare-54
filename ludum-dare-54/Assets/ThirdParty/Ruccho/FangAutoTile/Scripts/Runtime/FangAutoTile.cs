﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ruccho.Fang
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Fang/Tile", fileName = "New Fang Auto Tile", order = 1)]
#endif
    public class FangAutoTile : TileBase
    {
        #region Parameters

#pragma warning disable CS0414
        [SerializeField, Header("Layout"), Tooltip("Extrude pixels on contours to avoid weird borders on tiles.")] private bool enablePadding = true;
        [SerializeField, Header("Sprite / Texture"), Tooltip("Tile will be justified into one unit.")] private bool oneTilePerUnit = true;
        [SerializeField] private int pixelsPerUnit = 16;
        [SerializeField] private TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        [SerializeField] private FilterMode filterMode = FilterMode.Point;

        [SerializeField, Header("Sources")] private Texture2D mainChannel = default;
        [SerializeField] private Texture2D[] subChannels = default;
#pragma warning restore CS0414

        [SerializeField, Header("Animation")] public float animationMinSpeed = 1f;
        [SerializeField] public float animationMaxSpeed = 1f;
        [SerializeField] public float animationStartTime;

        [SerializeField, Header("Collision")] private Tile.ColliderType colliderType = Tile.ColliderType.None;

        [SerializeField, Header("Auto Tiling"), Tooltip("These tiles recognized to be connected with this tile.")]
        private TileBase[] connectableTiles = default;
        [SerializeField] public WeightedSprite[] Sprites;

        [Serializable]
        public struct WeightedSprite {
            public Sprite Sprite;
            public int Weight;
        }

        #endregion

        #region Generated

#pragma warning disable CS0414
        [SerializeField] private Texture2D[] compiledChannels = default;
        [SerializeField] private FangAutoTilePacker packer = default;
#pragma warning restore CS0414

        [SerializeField] private TileCombination[] combinations = default;

        [SerializeField] private int[] combinationTable = new int[256];

        #endregion

        #region Internal Memners

        public IEnumerable<Sprite> GetAllSprites()
        {
            return combinations.SelectMany(c => c.Frames);
        }

        #endregion

        public override void RefreshTile(Vector3Int location, ITilemap tileMap)
        {
            for (int yd = -1; yd <= 1; yd++)
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (TileValue(tileMap, position))
                    tileMap.RefreshTile(position);
            }
        }


        private TileCombination currentCombination = default;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;

            byte neighborCombination = 0;
            for (byte i = 0; i < 8; i++)
            {
                int x = Mathf.Clamp(Mathf.Abs(((i + 5) % 8) - 4) - 2, -1, 1);
                int y = Mathf.Clamp(Mathf.Abs(((i + 3) % 8) - 4) - 2, -1, 1);
                Vector3Int d = new Vector3Int(x, y, 0);

                //Debug.Log($"({x}, {y})");

                neighborCombination += (byte) (TileValue(tilemap, position + d) ? 1 << i : 0);
            }

            int combinationIndex = combinationTable[neighborCombination];

            //Debug.Log(combinationIndex);

            if (combinations == null || combinations.Length <= combinationIndex) return;

            var combination = combinations[combinationIndex];
            if (combination == null) return;

            if (TileValue(tilemap, position))
            {
                if (combinationIndex == 46 && Sprites != null && Sprites.Length > 0)
				{
					// Get the cumulative weight of the sprites
					var cumulativeWeight = 0;
					foreach (var spriteInfo in Sprites) cumulativeWeight += spriteInfo.Weight;

					// Pick a random weight and choose a sprite depending on it
					var randomWeight = UnityEngine.Random.Range(0, cumulativeWeight);
					foreach (var spriteInfo in Sprites) {
						randomWeight -= spriteInfo.Weight;
						if (randomWeight < 0) {
							tileData.sprite = spriteInfo.Sprite;
							break;
						}
					}
				}
                else {
                    tileData.sprite = combination.Frames[0];
                }
                tileData.color = Color.white;
                tileData.flags = (TileFlags.LockTransform | TileFlags.LockColor);
                tileData.colliderType = colliderType;
            }

            currentCombination = combination;
        }

        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap,
            ref TileAnimationData tileAnimationData)
        {
            if (currentCombination == null) return false;
            if (currentCombination.Frames.Length == 1 || (currentCombination == combinations.Last() && Sprites != null && Sprites.Length > 0))
            {
                return false;
            }
            else
            {
                tileAnimationData.animatedSprites = currentCombination.Frames;
                tileAnimationData.animationSpeed = UnityEngine.Random.Range(animationMinSpeed, animationMaxSpeed);
                tileAnimationData.animationStartTime = animationStartTime;
                return true;
            }
        }


        private bool TileValue(ITilemap tileMap, Vector3Int position)
        {
            TileBase tile = tileMap.GetTile(position);
            if (tile == null) return false;

            return tile == this || (connectableTiles != null && connectableTiles.Contains(tile));
        }
    }

    [Serializable]
    public class TileCombination
    {
        [SerializeField] private uint combinationId = 0;
        [SerializeField] private Sprite[] frames = default;

        public uint CombinationId => combinationId;
        public Sprite[] Frames => frames;
    }
}