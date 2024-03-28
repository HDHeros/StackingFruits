using Cysharp.Threading.Tasks;
using Infrastructure.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;
using Zenject;

namespace YandexGamesIntegration
{
    [DefaultExecutionOrder(-1000)]
    public class YandexGameLauncher : MonoBehaviour
    {
        private void Start()
        {
            if (YandexGame.Instance == null)
            {
                SceneManager.LoadScene(SceneId.InitialScene.ToString());
                return;
            }
            Initialize().Forget();
        }

        private async UniTaskVoid Initialize()
        {
            await UniTask.WaitUntil(() => YandexGame.SDKEnabled);
            ProjectContext.Instance.EnsureIsInitialized();
        }
    }
}