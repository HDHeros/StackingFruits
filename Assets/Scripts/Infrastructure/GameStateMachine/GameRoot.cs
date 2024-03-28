using HDH.Fsm;
using HDH.GoPool;
using Infrastructure.GameStateMachine.States;
using Infrastructure.InputLogic;
using Infrastructure.SceneManagement;
using UnityEngine;
using Zenject;

namespace Infrastructure.GameStateMachine
{
    public class GameRoot : MonoBehaviour
    {
        private Fsm<BaseGameState, SharedFields> _fsm;
        
        [Inject]
        public void Inject(SceneService sceneService, InputService inputService, IGoPool pool)
        {
            var fields = new SharedFields
            {
                SceneService = sceneService,
                InputService = inputService,
                GoPool = pool,
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
        }
    }
}