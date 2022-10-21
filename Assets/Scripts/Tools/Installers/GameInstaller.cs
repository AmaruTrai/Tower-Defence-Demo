using Game;
using Tools.Liquidated;
using Zenject;

namespace Tools.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public LiquidatedObjectPool poolPrefab;

        public override void InstallBindings()
        {
            var pool = new LiquidatedObjectPool(Container);
            Container.Bind<LiquidatedObjectPool>().FromInstance(pool).AsSingle();
            Container.Bind<TowerSpawnPoint>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<SpawnPoint>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<Enemy>().FromComponentInParents();
        }
    }
}

