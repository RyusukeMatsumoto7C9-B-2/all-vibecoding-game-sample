using UnityEngine;
using UnityEngine.Tilemaps;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;
using System.Collections.Generic;

namespace MyGame.TilemapSystem
{
    public class TilemapSystemTester : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileBase wallTile;
        [SerializeField] private TileBase groundTile;
        [SerializeField] private int testLevel = 1;
        [SerializeField] private int testSeed = 12345;

        private TilemapGenerator _generator;
        private TilemapManager _manager;
        private SeedManager _seedManager;

        private void Start()
        {
            InitializeSystem();
            GenerateTestMap();
        }

        private void InitializeSystem()
        {
            _seedManager = new SeedManager(testSeed);
            _generator = new TilemapGenerator(_seedManager);

            var tileAssets = new Dictionary<TileType, TileBase>
            {
                { TileType.Wall, wallTile },
                { TileType.Ground, groundTile }
            };

            _manager = new TilemapManager(tilemap, tileAssets);
            _manager.OnMapGenerated += OnMapGenerated;
        }

        private void GenerateTestMap()
        {
            if (_generator == null)
            {
                Debug.LogError("TilemapGenerator is not initialized");
                return;
            }

            var mapData = _generator.GenerateMap(testLevel, testSeed);
            
            if (_manager != null)
            {
                _manager.PlaceTiles(mapData);
            }

            Debug.Log($"マップ生成完了: Level={mapData.Level}, Seed={mapData.Seed}, Size={mapData.Width}x{mapData.Height}");
        }

        private void OnMapGenerated(MapData mapData)
        {
            Debug.Log($"マップ配置完了: Level {mapData.Level}");
        }

        [ContextMenu("新しいマップを生成")]
        private void GenerateNewMap()
        {
            testLevel++;
            GenerateTestMap();
        }

        [ContextMenu("メモリ最適化テスト")]
        private void TestMemoryOptimization()
        {
            if (_manager != null)
            {
                _manager.OptimizeMemory(testLevel);
                Debug.Log($"メモリ最適化実行: Current Level {testLevel}");
            }
        }

        private void OnDestroy()
        {
            if (_manager != null)
            {
                _manager.OnMapGenerated -= OnMapGenerated;
            }
        }
    }
}