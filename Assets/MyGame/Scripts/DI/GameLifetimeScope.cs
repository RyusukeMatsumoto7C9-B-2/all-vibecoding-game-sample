using UnityEngine;
using VContainer;
using VContainer.Unity;
using MyGame.TilemapSystem;

namespace MyGame.DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private TilemapSystemController _tilemapSystemController;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_tilemapSystemController).AsImplementedInterfaces();
        }
    }
}
