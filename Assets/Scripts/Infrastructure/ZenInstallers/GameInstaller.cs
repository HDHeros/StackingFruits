using Infrastructure.GameStateMachine;
using UnityEngine;
using Zenject;

namespace Infrastructure.ZenInstallers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameRoot _gameRootPrefab;
        public override void InstallBindings()
        {
            Container
                .Bind<GameRoot>()
                .FromComponentInNewPrefab(_gameRootPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}