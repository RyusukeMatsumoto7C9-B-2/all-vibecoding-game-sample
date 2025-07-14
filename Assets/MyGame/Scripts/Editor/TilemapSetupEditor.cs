using UnityEngine;
using UnityEditor;
using System.IO;

namespace MyGame.Editor
{
    public class TilemapSetupEditor : EditorWindow
    {
        [MenuItem("MyGame/Setup Tilemap System")]
        public static void ShowWindow()
        {
            GetWindow<TilemapSetupEditor>("Tilemap Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Tilemap System Setup", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Tile Sprites"))
            {
                CreateTileSprites();
            }
            
            if (GUILayout.Button("Create Tile Prefabs"))
            {
                CreateTilePrefabs();
            }
            
            if (GUILayout.Button("Setup Sample Scene"))
            {
                SetupSampleScene();
            }
        }
        
        private void CreateTileSprites()
        {
            // 既存のテクスチャパスを確認
            string rockTexturePath = "Assets/MyGame/Textures/Rock.png";
            string dirtTexturePath = "Assets/MyGame/Textures/Dirt.png";
            
            if (!File.Exists(rockTexturePath) || !File.Exists(dirtTexturePath))
            {
                // Unityの標準Square Spriteを作成
                CreateDefaultSquareSprites();
                return;
            }
            
            // 既存テクスチャからSpriteを作成
            SetupExistingTextures(rockTexturePath, dirtTexturePath);
        }
        
        private void CreateDefaultSquareSprites()
        {
            // Unity標準のSquare Spriteアセットを作成
            string spritesPath = "Assets/MyGame/Sprites";
            if (!Directory.Exists(spritesPath))
            {
                Directory.CreateDirectory(spritesPath);
                AssetDatabase.Refresh();
            }
            
            // Wall Sprite (グレー)
            Texture2D wallTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            wallTexture.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f, 1f)); // グレー
            wallTexture.Apply();
            
            byte[] wallBytes = wallTexture.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(spritesPath, "WallSprite.png"), wallBytes);
            
            // Ground Sprite (茶色)
            Texture2D groundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            groundTexture.SetPixel(0, 0, new Color(0.6f, 0.4f, 0.2f, 1f)); // 茶色
            groundTexture.Apply();
            
            byte[] groundBytes = groundTexture.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(spritesPath, "GroundSprite.png"), groundBytes);
            
            AssetDatabase.Refresh();
            
            // Spriteとして設定
            SetupSpriteImportSettings(Path.Combine(spritesPath, "WallSprite.png"));
            SetupSpriteImportSettings(Path.Combine(spritesPath, "GroundSprite.png"));
            
            Debug.Log("Wall and Ground sprites created successfully!");
        }
        
        private void SetupExistingTextures(string rockPath, string dirtPath)
        {
            SetupSpriteImportSettings(rockPath);
            SetupSpriteImportSettings(dirtPath);
            
            Debug.Log("Existing textures configured as sprites!");
        }
        
        private void SetupSpriteImportSettings(string texturePath)
        {
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texturePath);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 1f; // 1:1スケール
                importer.filterMode = FilterMode.Point; // ピクセルアート用
                importer.SaveAndReimport();
            }
        }
        
        private void CreateTilePrefabs()
        {
            string prefabsPath = "Assets/MyGame/Prefabs/Tiles";
            if (!Directory.Exists(prefabsPath))
            {
                Directory.CreateDirectory(prefabsPath);
                AssetDatabase.Refresh();
            }
            
            // Wall Tile Prefab
            GameObject wallTile = new GameObject("WallTile");
            SpriteRenderer wallRenderer = wallTile.AddComponent<SpriteRenderer>();
            
            // Rock.pngまたはWallSprite.pngを使用
            Sprite wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/MyGame/Textures/Rock.png");
            if (wallSprite == null)
                wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/MyGame/Sprites/WallSprite.png");
            
            wallRenderer.sprite = wallSprite;
            wallRenderer.color = Color.white;
            
            string wallPrefabPath = Path.Combine(prefabsPath, "WallTile.prefab");
            PrefabUtility.SaveAsPrefabAsset(wallTile, wallPrefabPath);
            DestroyImmediate(wallTile);
            
            // Ground Tile Prefab
            GameObject groundTile = new GameObject("GroundTile");
            SpriteRenderer groundRenderer = groundTile.AddComponent<SpriteRenderer>();
            
            // Dirt.pngまたはGroundSprite.pngを使用
            Sprite groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/MyGame/Textures/Dirt.png");
            if (groundSprite == null)
                groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/MyGame/Sprites/GroundSprite.png");
            
            groundRenderer.sprite = groundSprite;
            groundRenderer.color = Color.white;
            
            string groundPrefabPath = Path.Combine(prefabsPath, "GroundTile.prefab");
            PrefabUtility.SaveAsPrefabAsset(groundTile, groundPrefabPath);
            DestroyImmediate(groundTile);
            
            AssetDatabase.Refresh();
            Debug.Log("Tile prefabs created successfully!");
        }
        
        private void SetupSampleScene()
        {
            // Sampleシーンを開く
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/MyGame/Scenes/Sample.unity");
            
            // 既存のTilemapSystemオブジェクトを削除
            GameObject existingTilemapSystem = GameObject.Find("TilemapSystem");
            if (existingTilemapSystem != null)
            {
                DestroyImmediate(existingTilemapSystem);
                Debug.Log("Existing TilemapSystem deleted");
            }
            
            // Grid関連オブジェクトも削除
            GameObject existingGrid = GameObject.Find("Grid");
            if (existingGrid != null)
            {
                DestroyImmediate(existingGrid);
                Debug.Log("Existing Grid deleted");
            }
            
            // 新しいTilemapSystemTesterオブジェクトを作成
            GameObject tilemapTester = new GameObject("TilemapSystemTester");
            var testerComponent = tilemapTester.AddComponent<MyGame.TilemapSystem.TilemapSystemTester>();
            
            // Prefabを参照設定
            GameObject wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MyGame/Prefabs/Tiles/WallTile.prefab");
            GameObject groundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MyGame/Prefabs/Tiles/GroundTile.prefab");
            
            if (wallPrefab != null && groundPrefab != null)
            {
                // リフレクションを使用してprivateフィールドを設定
                var wallField = typeof(MyGame.TilemapSystem.TilemapSystemTester).GetField("wallTilePrefab", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var groundField = typeof(MyGame.TilemapSystem.TilemapSystemTester).GetField("groundTilePrefab", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                wallField?.SetValue(testerComponent, wallPrefab);
                groundField?.SetValue(testerComponent, groundPrefab);
                
                Debug.Log("TilemapSystemTester configured with prefabs!");
            }
            else
            {
                Debug.LogWarning("Could not find tile prefabs. Please create them first.");
            }
            
            // シーンを保存
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            
            Debug.Log("Sample scene setup completed!");
        }
    }
}