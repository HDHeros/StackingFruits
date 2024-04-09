using Sirenix.OdinInspector;
using UnityEngine;

namespace Infrastructure.SceneManagement
{
    public class SceneRoot : MonoBehaviour
    {
        public SceneId Id => _sceneId;
        [ValidateInput(nameof(ValidateSceneName))]
        [SerializeField] private SceneId _sceneId;

        private bool ValidateSceneName() =>
            gameObject.scene.name == _sceneId.ToString();
    }
}