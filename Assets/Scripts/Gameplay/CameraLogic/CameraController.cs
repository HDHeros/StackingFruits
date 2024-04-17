using Cinemachine;
using HDH.UnityExt.Extensions;
using UnityEngine;

namespace Gameplay.CameraLogic
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CinemachineBrain _brain;
        [SerializeField] private CinemachineVirtualCamera _homeScreenCamera;
        [SerializeField] private CinemachineVirtualCamera _selectSectionVCam;
        [SerializeField] private CinemachineVirtualCamera _selectLevelVCam;
        [SerializeField] private CinemachineVirtualCamera _inGameVCam;
        [SerializeField] private CinemachineVirtualCamera _gameLoseCamera;
        private CinemachineVirtualCamera _activeVCam;
        public Camera Camera => _camera;
        public bool IsBlending => _brain.IsBlending; 

        public void ActivateHomeScreenCamera() => 
            ActivateCamera(_homeScreenCamera);

        public void ActivateSelectSectionCamera() => 
            ActivateCamera(_selectSectionVCam);

        public void ActivateSelectLevelCamera() => 
            ActivateCamera(_selectLevelVCam);

        public void ActivateInGameCamera() =>
            ActivateCamera(_inGameVCam);
        
        public void ActivateGameLoseCamera() =>
            ActivateCamera(_gameLoseCamera);

        private void ActivateCamera(CinemachineVirtualCamera vCam)
        {
            if (_activeVCam.IsNotNull())
                _activeVCam.Priority = 0;
            _activeVCam = vCam;
            _activeVCam.Priority = 100;
        }
    }
}