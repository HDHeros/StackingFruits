using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Infrastructure
{
    public class GlobalVolumeService
    {
        private readonly Volume _volume;
        private int _blurCounter;

        public GlobalVolumeService(Volume globalVolume)
        {
            _volume = globalVolume;
            SetActiveBlur(false);
        }

        public void SetActiveBlur(bool value)
        {
            _blurCounter = Mathf.Clamp(_blurCounter + (value ? 1 : -1), 0, int.MaxValue);
            if (_volume.profile.TryGet(out DepthOfField dof))
                dof.active = _blurCounter > 0;
        }
    }
}