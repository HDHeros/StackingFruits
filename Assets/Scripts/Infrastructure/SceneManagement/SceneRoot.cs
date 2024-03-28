using TriInspector;
using UnityEngine;

namespace Infrastructure.SceneManagement
{
    public class SceneRoot : MonoBehaviour
    {
        public SceneId Id => _sceneId;
        [ValidateInput(nameof(ValidateSceneName))]
        [SerializeField] private SceneId _sceneId;

        private TriValidationResult ValidateSceneName() => 
            gameObject.scene.name != _sceneId.ToString() 
                ? TriValidationResult.Error("Scene name and SceneId are not matching") 
                : TriValidationResult.Valid;
    }
}