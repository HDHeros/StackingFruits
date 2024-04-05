using Gameplay;
using HDH.GoPool;
using Infrastructure.SimpleInput;
using UnityEngine;

namespace Infrastructure.SceneManagement.Roots
{
    public class GameSceneRoot : SceneRoot
    {
        [SerializeField] private GameView _gameView;
        
        public void OnEnter(InputService inputService, IGoPool pool)
        {
            _gameView.Setup(inputService);
        }

        public void OnExit()
        {

        }
    }
}