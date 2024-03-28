using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.SceneManagement
{
    public class SceneService
    {
        public SceneId GetActiveSceneId() => 
            GetActiveSceneRoot<SceneRoot>().Id;

        public async UniTask<T> LoadSceneSingle<T>(SceneId sceneId) where T : SceneRoot
        {
            AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneId.ToString(), LoadSceneMode.Single);
            while (loadingOperation.isDone == false)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            return GetActiveSceneRoot<T>();
        }

        private T GetActiveSceneRoot<T>() where T : SceneRoot
        {
            Scene activeScene = SceneManager.GetActiveScene();
            T sceneRoot = null;
            foreach (GameObject rootGo in activeScene.GetRootGameObjects())
            {
                if (rootGo.TryGetComponent(out sceneRoot) == false) continue;
                break;
            }

            if (sceneRoot == null)
                throw new AggregateException($"There is not SceneRoot of type '{nameof(T)}' on '{activeScene.name}'");
            
            return sceneRoot;
        }
    }
}