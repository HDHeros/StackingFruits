using HDH.GoPool;
using Infrastructure.SceneManagement;
using Zenject;

namespace Infrastructure.ZenInstallers
{
    public class ServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallInputService();
            InstallSceneService();
            InstallGoPool();
        }

        private void InstallSceneService() =>
            Container
                .Bind<SceneService>()
                .FromNew()
                .AsSingle();

        private void InstallInputService() =>
            Container
                .Bind<SimpleInput.InputService>()
                .FromNew()
                .AsSingle();

        private void InstallGoPool() =>
            Container
                .Bind<IGoPool>()
                .To<SimpleGoPool>()
                .FromInstance(new SimpleGoPool((prefab, parent) => 
                    Container.InstantiatePrefab(prefab, parent)))
                .AsSingle()
                .NonLazy();
    }
}