using System;
using System.Threading;
using Gameplay;
using Gameplay.CameraLogic;
using Gameplay.GameSceneLogic;
using Gameplay.LevelsLogic;
using GameStructConfigs;
using HDH.Fsm;
using HDH.GoPool;
using HDH.Popups;
using Infrastructure.SimpleInput;
using Infrastructure.SoundsLogic;
using Infrastructure.Tutor;
using Menu;
using UI;
using UnityEngine;

namespace Infrastructure.SceneManagement.Roots
{
    public class GameSceneRoot : SceneRoot
    {
        private CancellationTokenSource _ctSource;

        [SerializeField] private SharedFields _fields;
        private Fsm<GsBaseState, SharedFields> _fsm;
        
        public void OnEnter(InputService inputService, IGoPool pool, GameConfig gameConfig, LevelsService levelsService, 
            SoundsService sounds, PopupsController popups, Hud hud, TutorInfoService tutorInfo)
        {
            _ctSource = new CancellationTokenSource();
            _fields.Input = inputService;
            _fields.GameConfig = gameConfig;
            _fields.Pool = pool;
            _fields.LevelsService = levelsService;
            _fields.SoundService = sounds;
            _fields.Popups = popups;
            _fields.Hud = hud;
            _fields.TutorInfo = tutorInfo;
            _fields.GameView.Initialize(inputService, pool, sounds);
            
            _fsm = Fsm<GsBaseState, SharedFields>
                .Create(_fields)
                .AddState<GsHomeScreenState>()
                .AddState<GsSelectSectionState>()
                .AddState<GsSelectLevelState>()
                .AddState<GsGameInProgressState>()
                .AddState<GsTutorialState>()
                .Start();
        }

        public void OnExit()
        {
            _ctSource.Cancel();
        }

        [Serializable]
        public class SharedFields : IFsmSharedFields
        {
            public CameraController CameraController;
            public GameView GameView;
            public SectionPicker SectionPicker;
            public GameObject TapToStartLabel;
            [NonSerialized] public GameConfig GameConfig;
            [NonSerialized] public InputService Input;
            [NonSerialized] public IGoPool Pool;
            [NonSerialized] public SectionView PickedSection;
            [NonSerialized] public LevelPreview PickedLevel;
            [NonSerialized] public LevelsService LevelsService;
            [NonSerialized] public SoundsService SoundService;
            [NonSerialized] public PopupsController Popups;
            [NonSerialized] public Hud Hud;
            [NonSerialized] public TutorInfoService TutorInfo;
        }
    }
}