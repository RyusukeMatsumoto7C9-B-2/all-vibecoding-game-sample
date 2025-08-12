using UnityEngine;
using VContainer;
using VContainer.Unity;
using MyGame.TilemapSystem;
using MyGame.TilemapSystem.Core;
using MyGame.Player;

namespace MyGame.DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private TilemapSystemController tilemapSystemController;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // TilemapSystemController自体を登録
            if (tilemapSystemController != null)
            {
                builder.RegisterInstance(tilemapSystemController);
                
                // ITilemapManagerインターフェースとしてTilemapManagerを登録
                builder.Register<ITilemapManager>(resolver => 
                {
                    var controller = resolver.Resolve<TilemapSystemController>();
                    return controller.Manager;
                }, Lifetime.Singleton);
            }
            else
            {
                Debug.LogError("[GameLifetimeScope] TilemapSystemControllerが設定されていません");
            }

            // PlayerMoveServiceを登録
            builder.Register<PlayerMoveService>(resolver =>
            {
                var tilemapManager = resolver.Resolve<ITilemapManager>();
                return new PlayerMoveService(tilemapManager);
            }, Lifetime.Singleton);

            // PlayerControllerにVContainer依存性注入を適用
            builder.RegisterComponentInHierarchy<PlayerController>();
        }
    }
}
