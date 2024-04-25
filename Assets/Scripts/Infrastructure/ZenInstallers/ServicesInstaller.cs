using Gameplay.LevelsLogic;
using GameStructConfigs;
using HDH.Audio;
using HDH.Audio.Confgis;
using HDH.GoPool;
using HDH.Popups;
using HDH.Popups.Configs;
using HDH.UserData;
using HDH.UserData.Dto;
using Infrastructure.SceneManagement;
using Infrastructure.SoundsLogic;
using Infrastructure.Tutor;
using Sirenix.OdinInspector;
using UI;
using UI.Popups;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Infrastructure.ZenInstallers
{
    public class ServicesInstaller : MonoInstaller
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private UserDataConfig _userDataConfig;
        [SerializeField] private Volume _globalVolume;
        [SerializeField] private Hud _hudPrefab;
        [SerializeField, BoxGroup("Popups")] private PopupsParent _popupParentPrefab;
        [SerializeField, BoxGroup("Popups")] private PopupConfig[] _popups;

        public override void InstallBindings()
        {
            InstallInputService();
            InstallHud();
            InstallGlobalVolumeService();
            InstallGameConfig();
            InstallAudio();
            InstallPopups();
            InstallSceneService();
            InstallGoPool();
            InstallUserDataService();
            InstallLevelsService();
            InstallTutorInfoService();
        }

        private void InstallHud() =>
            Container
                .Bind<Hud>()
                .FromComponentInNewPrefab(_hudPrefab)
                .AsSingle();

        private void InstallGlobalVolumeService() =>
            Container
                .Bind<GlobalVolumeService>()
                .FromNew()
                .AsSingle()
                .WithArgumentsExplicit(new[] { new TypeValuePair(typeof(Volume), _globalVolume) });

        private void InstallAudio()
        {
            Container
                .Bind<AudioService>()
                .FromNew()
                .AsSingle()
                .WithArgumentsExplicit(new[]
                    { new TypeValuePair(typeof(AudioServiceConfig), _gameConfig.AudioServiceCfg) });

            Container
                .Bind<SoundsService>()
                .FromNew()
                .AsSingle()
                .WithArgumentsExplicit(new[]
                    { new TypeValuePair(typeof(SoundsContainer), _gameConfig.SoundsContainer) });
            
            Container
                .Bind<MusicPlayer>()
                .FromNew()
                .AsSingle()
                .WithArgumentsExplicit(new[] { new TypeValuePair(typeof(AudioConfig), _gameConfig.BackgroundMusic) })
                .NonLazy();
        }

        private void InstallPopups()
        {
            PopupsParent parent = Instantiate(_popupParentPrefab, Vector3.zero, Quaternion.identity, null);
            PopupsController popups = new PopupsController(new PopupControllerConfig
            {
                PopupsConfigs = _popups,
                ViewsFactory = new ZenPopupFactory(Container),
                PopupsParent = parent,
            });
            Container
                .Bind<PopupsController>()
                .FromInstance(popups)
                .AsSingle()
                .NonLazy();
        }

        private void InstallGameConfig()
        {
            Container
                .Bind<GameConfig>()
                .FromInstance(_gameConfig)
                .AsSingle();
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

        private void InstallUserDataService() =>
            Container
                .Bind<UserDataService>()
                .FromInstance(new UserDataService(_userDataConfig))
                .AsSingle();

        private void InstallLevelsService() =>
            Container
                .Bind<LevelsService>()
                .FromNew()
                .AsSingle();

        private void InstallTutorInfoService() =>
            Container
                .Bind<TutorInfoService>()
                .FromNew()
                .AsSingle()
                .WithArgumentsExplicit(new []{new TypeValuePair(typeof(TutorConfig), _gameConfig.TutorConfig)});
    }
}