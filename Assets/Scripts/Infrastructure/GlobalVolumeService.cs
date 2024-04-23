using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Infrastructure
{
    public class GlobalVolumeService
    {
        private readonly Volume _volume;

        public GlobalVolumeService(Volume globalVolume)
        {
            _volume = globalVolume;
            SetActiveBlur(false);
        }

        public void SetActiveBlur(bool value)
        {
            if (_volume.profile.TryGet(out DepthOfField dof))
                dof.active = value;
        }
    }
}