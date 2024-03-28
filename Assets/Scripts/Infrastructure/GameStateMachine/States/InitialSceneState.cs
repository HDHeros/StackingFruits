using Cysharp.Threading.Tasks;
using Infrastructure.SceneManagement;
using Infrastructure.SceneManagement.Roots;

namespace Infrastructure.GameStateMachine.States
{
    public class InitialSceneState : BaseGameState
    {
        public override void Enter()
        {
            base.Enter();
            Setup().Forget();
        }

        private async UniTaskVoid Setup()
        {
            if (Fields.SceneService.GetActiveSceneId() != SceneId.InitialScene)
                await Fields.SceneService.LoadSceneSingle<InitialSceneRoot>(SceneId.InitialScene);
            StateSwitcher.SwitchState<GameSceneState>();
        }
    }
}