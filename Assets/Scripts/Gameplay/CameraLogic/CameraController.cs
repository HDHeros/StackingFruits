using Cinemachine;
using HDH.UnityExt.Extensions;
using UnityEngine;

namespace Gameplay.CameraLogic
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _homeScreenCamera;
        [SerializeField] private CinemachineVirtualCamera _selectSectionVCam;
        private CinemachineVirtualCamera _activeVCam;
        
        public void ActivateHomeScreenCamera() => 
            ActivateCamera(_homeScreenCamera);

        public void ActivateSelectSectionCamera() => 
            ActivateCamera(_selectSectionVCam);

        private void ActivateCamera(CinemachineVirtualCamera vCam)
        {
            if (_activeVCam.IsNotNull())
                _activeVCam.Priority = 0;
            _activeVCam = vCam;
            _activeVCam.Priority = 100;
        }
    }
}