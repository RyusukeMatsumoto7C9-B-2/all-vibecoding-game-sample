using UnityEngine;
using VContainer;
using VContainer.Unity;
using MyGame.TilemapSystem;
using MyGame.TilemapSystem.Core;

namespace MyGame.DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private TilemapSystemController _tilemapSystemController;
        [SerializeField] private GameObject _universalTilePrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // TilemapServiceをITilemapManagerとしてSingleton登録
            builder.Register<TilemapService>(Lifetime.Singleton)
                .WithParameter("parentTransform", _tilemapSystemController.transform)
                .WithParameter("universalTilePrefab", _universalTilePrefab)
                .AsImplementedInterfaces();
            
            // TilemapSystemControllerをコンポーネントとして登録
            builder.RegisterComponent(_tilemapSystemController);
        }
    }
}
