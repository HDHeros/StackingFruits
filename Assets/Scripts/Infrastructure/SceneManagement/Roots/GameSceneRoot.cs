﻿using System;
using System.Threading;
using Gameplay;
using Gameplay.CameraLogic;
using Gameplay.GameCore;
using Gameplay.GameSceneLogic;
using GameStructConfigs;
using HDH.Fsm;
using HDH.GoPool;
using Infrastructure.SimpleInput;
using UI;
using UnityEngine;

namespace Infrastructure.SceneManagement.Roots
{
    public class GameSceneRoot : SceneRoot
    {
        private CancellationTokenSource _ctSource;
        private LevelData<int> _levelData = new()
        {
            EmptyBlockValue = 0,
            FieldSize = new Vector2Int(4, 6),
            Blocks = new []
            {
                1,1,2,3,
                1,3,2,3,
                0,2,0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
            }
        };

        [SerializeField] private SharedFields _fields;
        private Fsm<GsBaseState, SharedFields> _fsm;
        public void OnEnter(InputService inputService, IGoPool pool, GameConfig gameConfig)
        {
            _ctSource = new CancellationTokenSource();
            _fields.Input = inputService;
            _fields.GameConfig = gameConfig;
            _fields.Pool = pool;
            _fields.GameView.Initialize(inputService, pool, gameConfig.BlocksContainer);
            
            _fsm = Fsm<GsBaseState, SharedFields>
                .Create(_fields)
                .AddState<GsHomeScreenState>()
                .AddState<GsSelectSectionState>()
                .AddState<GsSelectLevelState>()
                .AddState<GsGameInProgressState>()
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
            public Hud Hud;
            [NonSerialized] public GameConfig GameConfig;
            [NonSerialized] public InputService Input;
            [NonSerialized] public IGoPool Pool;
        }
    }
}