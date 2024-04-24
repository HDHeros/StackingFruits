using Gameplay.LevelsLogic;
using GameStructConfigs;
using HDH.Fsm;
using HDH.GoPool;
using HDH.Popups;
using Infrastructure.GameStateMachine.States;
using Infrastructure.SceneManagement;
using Infrastructure.SimpleInput;
using Infrastructure.SoundsLogic;
using Infrastructure.Tutor;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure.GameStateMachine
{
    public class GameRoot : MonoBehaviour
    {
        private Fsm<BaseGameState, SharedFields> _fsm;
        
        [Inject]
        public void Inject(SceneService sceneService, InputService inputService, IGoPool pool, GameConfig gameConfig,
            LevelsService levelsService, SoundsService sounds, PopupsController popups, Hud hud,
            TutorInfoService tutorInfo)
        {
            var fields = new SharedFields
            {
                SceneService = sceneService,
                InputService = inputService,
                GoPool = pool,
                GameConfig = gameConfig,
                Levels = levelsService,
                SoundService = sounds,
                Popups = popups,
                Hud = hud,
                TutorInfo = tutorInfo
            };
            
            _fsm = Fsm<BaseGameState, SharedFields>
                .Create(fields)
                .AddState<InitialSceneState>()
                .AddState<GameSceneState>();
        }

        private void Start()
        {
            _fsm.Start();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDisable() => 
            _fsm.Stop();

        public class SharedFields : IFsmSharedFields
        {
            public SceneService SceneService;
            public InputService InputService;
            public IGoPool GoPool;
            public GameConfig GameConfig;
            public LevelsService Levels;
            public SoundsService SoundService;
            public PopupsController Popups;
            public Hud Hud;
            public TutorInfoService TutorInfo;
        }
    }
}